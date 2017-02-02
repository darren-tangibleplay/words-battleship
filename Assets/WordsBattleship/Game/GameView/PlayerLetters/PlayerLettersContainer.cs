using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class PlayerLettersContainer : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            foreach (char letter in ApplicationConstants.kLetters) {
                var playerLetter = ObjectPoolManager.Create<PlayerLetter>(parent: gameObject);
                playerLetter.Init(player_, letter);
            }
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            gameObject.RecycleAllChildren();
        }


        // PRAGMA MARK - Internal
        [SerializeField] private GamePlayer player_;
    }
}