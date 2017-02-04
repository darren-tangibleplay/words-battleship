using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class PlayerLetter : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Public Interface
        public void Init(GamePlayer player, char letter) {
            player_ = player;
            letter_ = letter;

            letterBubble_.Init(letter_);

            Refresh();
        }


        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            Game.OnAnyPlayerGuessedLetter += Refresh;

            letterBubble_ = ObjectPoolManager.Create<LetterBubble>(parent: gameObject);
            canvasGroup_ = letterBubble_.GetRequiredComponent<CanvasGroup>();
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            Game.OnAnyPlayerGuessedLetter -= Refresh;

            if (letterBubble_ != null) {
                ObjectPoolManager.Recycle(letterBubble_);
                letterBubble_ = null;
            }

            if (canvasGroup_ != null) {
                canvasGroup_.alpha = 1.0f;
                canvasGroup_ = null;
            }
        }


        // PRAGMA MARK - Internal
        [Header("Outlets")]
        private GamePlayer player_;
        private char letter_;

        private LetterBubble letterBubble_;
        private CanvasGroup canvasGroup_;

        private void Refresh() {
            if (letterBubble_ == null) {
                return;
            }

            if (Game.DidPlayerAlreadyGuessLetter(player_, letter_)) {
                canvasGroup_.alpha = 0.5f;
            } else {
                canvasGroup_.alpha = 1.0f;
            }
        }
    }
}