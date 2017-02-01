using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DTObjectPoolManager.Internal {
    public static class SingletonLoader {
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeSingletons() {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())) {
                if (type.IsAbstract) {
                    continue;
                }

                var baseType = type.BaseType;
                if (baseType == null) {
                    continue;
                }

                if (!baseType.IsGenericType) {
                    continue;
                }

                if (baseType.GetGenericTypeDefinition() != typeof(Singleton<>)) {
                    continue;
                }

                var methodInfo = baseType.GetMethod("CacheSingletonIfNecessary", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Static);
                if (methodInfo == null) {
                    continue;
                }

                methodInfo.Invoke(null, null);
            }
        }
    }

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _instance;

        private static object _lock = new object();

        public static T Instance {
            get {
                CacheSingletonIfNecessary();
                return Singleton<T>._instance;
            }
        }

        private static void CacheSingletonIfNecessary() {
            lock (Singleton<T>._lock) {
                if (Singleton<T>._instance == null) {
                    var behaviours = UnityEngine.Object.FindObjectsOfType<T>();
                    if (behaviours.Length >= 1) {
                        Singleton<T>._instance = behaviours[0];

                        if (behaviours.Length > 2) {
                            Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");
                        }
                    }

                    if (Singleton<T>._instance == null && Application.isPlaying) {
                        GameObject singleton = new GameObject();
                        Singleton<T>._instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) "+ typeof(T).ToString();

                        GameObject.DontDestroyOnLoad(singleton);
                    }
                }
            }
        }
    }
}