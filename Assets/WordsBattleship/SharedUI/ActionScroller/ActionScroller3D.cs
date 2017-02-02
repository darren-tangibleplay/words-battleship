using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class ActionScroller3D : MonoBehaviour, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Public Interface
        public void Init(RenderTexture renderTexture) {
            camera_.targetTexture = renderTexture;
        }

        public void Scroll(string prefabName, float scrollSpeed, HorizontalDirection direction, Action<GameObject> initializationCallback) {
            var scrollContainer = ObjectPoolManager.Create<ScrollContainer>(parent: container_);
            scrollContainer.Init(prefabName, scrollSpeed, direction, initializationCallback);
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            container_.RecycleAllChildren();
        }


        // PRAGMA MARK - Internal
        [SerializeField] private GameObject container_;
        [SerializeField] private Camera camera_;
    }
}