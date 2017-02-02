using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class ActionScroller : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Public Interface
        public void Scroll(string prefabName, float relativeScrollSpeed, HorizontalDirection direction, Action<GameObject> initializationCallback) {
            if (actionScroller3D_ == null) {
                Debug.LogWarning("ActionScroller - cannot scroll without 3D!");
                return;
            }

            actionScroller3D_.Scroll(prefabName, baseScrollSpeed_ * relativeScrollSpeed, direction, initializationCallback);
        }

        public void SetFlipped(bool flipped) {
            GetComponent<RectTransform>().localScale = new Vector3(flipped ? -1.0f : 1.0f, 1.0f, 1.0f);
        }


        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            actionScroller3D_ = ObjectPoolManager.Create<ActionScroller3D>();
            actionScroller3D_.Init(renderTexture_);
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            if (actionScroller3D_ != null) {
                ObjectPoolManager.Recycle(actionScroller3D_);
                actionScroller3D_ = null;
            }
        }


        // PRAGMA MARK - Internal
        [SerializeField] private RenderTexture renderTexture_;
        [SerializeField] private float baseScrollSpeed_;

        private ActionScroller3D actionScroller3D_;
    }
}