using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class LetterBubble : MonoBehaviour, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Public Interface
        public void Init(char letter) {
            if (letter == ' ') {
                canvasGroup_.alpha = 0.1f;
                text_.text = "";
            } else {
                canvasGroup_.alpha = 1.0f;
                text_.text = letter.ToString();
            }
        }

        public void Hide() {
            ObjectPoolManager.Recycle(this);
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
        }


        // PRAGMA MARK - Internal
        [SerializeField] private Text text_;

        private CanvasGroup canvasGroup_;

        void Awake() {
            canvasGroup_ = this.GetRequiredComponent<CanvasGroup>();
        }
    }
}