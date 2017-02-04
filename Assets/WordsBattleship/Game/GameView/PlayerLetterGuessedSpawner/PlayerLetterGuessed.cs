using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    [RequireComponent(typeof(Animator))]
    public class PlayerLetterGuessed : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Public Interface
        public void Init(GamePlayer player, char letter) {
            string word = Game.GetWordForPlayer(player);

            bool correctGuess = word.Contains(letter.ToString());
            animator_.SetTrigger("Start");
            animator_.SetBool("CorrectGuess", correctGuess);

            letterBubble_.Init(letter);
        }

        public void OnAnimationComplete() {
            ObjectPoolManager.Recycle(gameObject);
        }


        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            letterBubble_ = ObjectPoolManager.Create<LetterBubble>(parent: container_);
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            if (letterBubble_ != null) {
                ObjectPoolManager.Recycle(letterBubble_);
                letterBubble_ = null;
            }

            animator_.SetTrigger("Reset");
        }


        // PRAGMA MARK - Internal
        [SerializeField] private GameObject container_;

        private LetterBubble letterBubble_;
        private Animator animator_;

        void Awake() {
            animator_ = this.GetRequiredComponent<Animator>();
        }
    }
}