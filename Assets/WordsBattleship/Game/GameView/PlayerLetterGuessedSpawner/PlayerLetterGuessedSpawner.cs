using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class PlayerLetterGuessedSpawner : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            Game.OnPlayerGuessedLetter += HandlePlayerGuessedLetter;
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            Game.OnPlayerGuessedLetter -= HandlePlayerGuessedLetter;
        }


        // PRAGMA MARK - Internal
        private void HandlePlayerGuessedLetter(GamePlayer player, char letter) {
            var letterGuessed_ = ObjectPoolManager.CreateView<PlayerLetterGuessed>();
            letterGuessed_.Init(player, letter);
        }
    }
}