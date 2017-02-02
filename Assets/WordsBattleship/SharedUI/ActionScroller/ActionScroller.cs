using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class ActionScroller : MonoBehaviour, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Public Interface
        public void Scroll(string prefabName, float relativeScrollSpeed, HorizontalDirection direction) {
            var scrollContainer = ObjectPoolManager.Create<ScrollContainer>(parent: gameObject);
            scrollContainer.Init(prefabName, baseScrollSpeed_ * relativeScrollSpeed, direction);
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            gameObject.RecycleAllChildren();
        }


        // PRAGMA MARK - Internal
        [SerializeField] private float baseScrollSpeed_;
    }
}