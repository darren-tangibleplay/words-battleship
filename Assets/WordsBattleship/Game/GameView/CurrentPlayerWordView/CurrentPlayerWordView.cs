using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class CurrentPlayerWordView : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            Game.OnCurrentPlayerChanged += RefreshCurrentPlayer;
            RefreshCurrentPlayer();
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            Game.OnCurrentPlayerChanged -= RefreshCurrentPlayer;
            gameObject.RecycleAllChildren();
        }


        // PRAGMA MARK - Internal
        private void RefreshCurrentPlayer() {
            gameObject.RecycleAllChildren();

            string word = Game.GetWordForCurrentPlayer();
            if (word == null) {
                return;
            }

            foreach (char c in word) {
                var currentPlayerLetter = ObjectPoolManager.Create<CurrentPlayerLetter>(parent: gameObject);
                currentPlayerLetter.Init(c);
            }
        }
    }
}