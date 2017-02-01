using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTObjectPoolManager {
    public partial class ObjectPoolManager : Singleton<ObjectPoolManager> {
        // PRAGMA MARK - Static
        public static event Action<GameObject> OnGameObjectCreated = delegate {};

        public static T Create<T>(string prefabName = null, GameObject parent = null, bool worldPositionStays = false) where T : MonoBehaviour {
            if (prefabName == null) {
                prefabName = typeof(T).Name;
            }

            GameObject instantiatedPrefab =  ObjectPoolManager.Create(prefabName, parent, worldPositionStays);
            return instantiatedPrefab.GetRequiredComponent<T>();
        }

        public static GameObject Create(string prefabName, GameObject parent = null, bool worldPositionStays = false) {
            return ObjectPoolManager.Instance.CreateInternal(prefabName, parent, worldPositionStays);
        }

        public static void Recycle(MonoBehaviour usedObject, bool worldPositionStays = false) {
            ObjectPoolManager.Recycle(usedObject.gameObject, worldPositionStays);
        }

        public static void Recycle(GameObject usedObject, bool worldPositionStays = false) {
            ObjectPoolManager.Instance.RecycleInternal(usedObject, worldPositionStays);
        }


        // PRAGMA MARK - Internal
        private HashSet<GameObject> _objectsBeingCleanedUp = new HashSet<GameObject>();
        private Dictionary<string, Stack<GameObject>> _objectPools = new Dictionary<string, Stack<GameObject>>();

        private GameObject CreateInternal(string prefabName, GameObject parent = null, bool worldPositionStays = false) {
            prefabName = prefabName.ToLower();

            GameObject instantiatedPrefab = this.GetGameObjectForPrefabName(prefabName);

            if (parent != null) {
                instantiatedPrefab.transform.SetParent(parent.transform, worldPositionStays);
            }

            RecyclablePrefab recycleData = instantiatedPrefab.GetOrAddComponent<RecyclablePrefab>();
            recycleData.Setup();

            return instantiatedPrefab;
        }

        private void RecycleInternal(GameObject usedObject, bool worldPositionStays = false) {
            if (usedObject == null) {
                Debug.LogWarning("Recycle: called on null object!");
                return;
            }

            RecyclablePrefab recycleData = usedObject.GetComponent<RecyclablePrefab>();
            if (recycleData == null) {
                Debug.LogWarning("Recycle: usedObject - (" + usedObject + ") does not have RecyclablePrefab script!");
                // Because the recycle lifecycle wasn't set up properly, just destroy this object instead of recycling
                GameObject.Destroy(usedObject);
                return;
            }

            if (this._objectsBeingCleanedUp.Contains(usedObject)) {
                return;
            }

            this._objectsBeingCleanedUp.Add(usedObject);
            recycleData.Cleanup();
            usedObject.transform.SetParent(this.transform, worldPositionStays);
            this.DoAfterFrame(() => {
                this.DoAfterFrame(() => {
                    usedObject.SetActive(false);

                    Stack<GameObject> recycledObjects = this.ObjectPoolForPrefabName(recycleData.prefabName);
                    recycledObjects.Push(usedObject);

                    this._objectsBeingCleanedUp.Remove(usedObject);
                });
            });
        }

        private Stack<GameObject> ObjectPoolForPrefabName(string prefabName) {
            return this._objectPools.GetAndCreateIfNotFound(prefabName);
        }

        private GameObject GetGameObjectForPrefabName(string prefabName) {
            Stack<GameObject> recycledObjects = this.ObjectPoolForPrefabName(prefabName);

            // try to find a recycled object that is usable
            while (recycledObjects.Count > 0) {
                GameObject recycledObj = recycledObjects.Pop();
                if (this._objectsBeingCleanedUp.Contains(recycledObj)) {
                    Debug.LogError("ObjectPoolManager - instantiating object that is being recycled (did you forget to clear references to recycled objects?)");
                }

                if (recycledObj != null) {
                    if (!this.ValidateRecycledObject(recycledObj, prefabName)) {
                        return null;
                    }

                    recycledObj.SetActive(true);
                    return recycledObj;
                }
            }

            // if no recycled object is found, instantiate one
            GameObject prefab = PrefabList.PrefabForName(prefabName);
            if (prefab == null) {
                return null;
            }

            GameObject instantiatedPrefab = GameObject.Instantiate(prefab);

            RecyclablePrefab recycleData = instantiatedPrefab.GetOrAddComponent<RecyclablePrefab>();
            recycleData.prefabName = prefabName;

            ObjectPoolManager.OnGameObjectCreated.Invoke(instantiatedPrefab);
            return instantiatedPrefab;
        }

        private bool ValidateRecycledObject(GameObject recycledObject, string prefabName) {
            if (recycledObject.activeSelf) {
                Debug.LogError("GetGameObjectForPrefabName: recycled object: (" + recycledObject + ") is still active, is someone else using it?");
                return false;
            }

            RecyclablePrefab recycleData = recycledObject.GetComponent<RecyclablePrefab>();
            if (recycleData == null) {
                Debug.LogError("GetGameObjectForPrefabName: recycled object: (" + recycledObject + ") doesn't have a recyclable prefab script!");
                return false;
            }

            if (recycleData.prefabName != prefabName) {
                Debug.LogError("GetGameObjectForPrefabName: recycled object: (" + recycledObject + ") doesn't match prefab name: " + prefabName + "!");
                return false;
            }

            return true;
        }
    }
}