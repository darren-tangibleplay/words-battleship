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
        public void Init(string prefabName, float scrollSpeed, HorizontalDirection direction) {
            scrollSpeed_ = scrollSpeed;
            direction_ = direction;
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            transform_.anchoredPosition = Vector2.zero;
            gameObject.RecycleAllChildren();
        }


        // PRAGMA MARK - Internal
        [SerializeField, ReadOnly] private float scrollSpeed_;
        [SerializeField, ReadOnly] private HorizontalDirection direction_;

        private RectTransform transform_;

        void Awake() {
            transform_ = this.GetRequiredComponent<RectTransform>();
        }

        void Update() {
            float moveDelta = scrollSpeed_ * Time.deltaTime;
            moveDelta *= (direction_ == HorizontalDirection.Right) ? 1.0f : -1.0f;

            transform_.anchoredPosition = transform_.anchoredPosition.SetX(transform_.anchoredPosition.x + moveDelta);
        }
    }
}