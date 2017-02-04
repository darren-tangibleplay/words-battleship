using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;
using TMPro;

namespace Tangible.WordsBattleship {
    public class PopupMessage : MonoBehaviour, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Static Public Interface
        private static PopupMessage popupMessage_;
        public static void Make(string text) {
            if (popupMessage_ != null) {
                ObjectPoolManager.Recycle(popupMessage_);
                popupMessage_ = null;
            }

            popupMessage_ = ObjectPoolManager.CreateView<PopupMessage>();
            popupMessage_.Init(text);
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            canvasGroup_.alpha = 1.0f;
        }


        // PRAGMA MARK - Internal
        [SerializeField] private TMP_Text text_;

        [Space]
        [SerializeField] private float duration_;

        [Space]
        [SerializeField] private float fadeDuration_;
        [SerializeField] private AnimationCurve fadeCurve_;

        private ActionScroller3D actionScroller3D_;
        private CanvasGroup canvasGroup_;

        void Awake() {
            canvasGroup_ = GetComponent<CanvasGroup>();
        }

        private void Init(string text) {
            text_.text = text;

            CoroutineWrapper.DoAfterDelay(duration_ - fadeDuration_, () => {
                CoroutineWrapper.StartCoroutine(AnimateOut());
            });
        }

        private IEnumerator AnimateOut() {
            for (float timePassed = 0.0f; timePassed <= fadeDuration_; timePassed += Time.deltaTime) {
                float alpha = fadeCurve_.Evaluate(timePassed / fadeDuration_);
                canvasGroup_.alpha = alpha;
                yield return null;
            }

            canvasGroup_.alpha = 0.0f;
            ObjectPoolManager.Recycle(this.gameObject);

            popupMessage_ = null;
        }
    }
}