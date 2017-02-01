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
            text_.text = letter.ToString();
            // animator_.SetTrigger("Show");
        }

        public void Hide() {
            ObjectPoolManager.Recycle(this);
            // animator_.SetTrigger("Hide");
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            // animator_.SetTrigger("Reset");
        }


        // PRAGMA MARK - Internal
        [SerializeField] private Text text_;

        // private Animator animator_;

        void Awake() {
            // animator_ = this.GetRequiredComponent<Animator>();
        }
    }
}