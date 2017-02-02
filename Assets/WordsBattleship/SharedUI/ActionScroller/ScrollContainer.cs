using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class ScrollContainer : MonoBehaviour, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Public Interface
        public void Init(string prefabName, float scrollSpeed, HorizontalDirection direction, Action<GameObject> initializationCallback) {
            scrollSpeed_ = scrollSpeed;
            direction_ = direction;

            GameObject createdGameObject = ObjectPoolManager.Create(prefabName, parent: container_);
            initializationCallback.Invoke(createdGameObject);
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            container_.RecycleAllChildren();
            container_.transform.localPosition = Vector3.zero;
        }


        // PRAGMA MARK - Internal
        [SerializeField] private GameObject container_;

        [SerializeField, ReadOnly] private float scrollSpeed_;
        [SerializeField, ReadOnly] private HorizontalDirection direction_;

        void Update() {
            var containerTransform = container_.transform;

            float moveDelta = scrollSpeed_ * Time.deltaTime;
            moveDelta *= (direction_ == HorizontalDirection.Right) ? 1.0f : -1.0f;

            containerTransform.localPosition = containerTransform.localPosition.SetX(containerTransform.localPosition.x + moveDelta);
        }
    }
}