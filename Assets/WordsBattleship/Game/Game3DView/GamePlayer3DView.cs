using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class GamePlayer3DView : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            var character = Game.GetCharacterForPlayer(targetPlayer_);
            if (character == null) {
                Debug.LogWarning("GamePlayer3DView: no character for " + targetPlayer_);
                return;
            }

            characterView_ = ObjectPoolManager.Create<Character3DView>(character.GamePrefabName, parent: characterContainer_);
            characterView_.Init(targetPlayer_);
            Game.OnCurrentPlayerChanged += RefreshCurrentPlayer;
            RefreshCurrentPlayer();
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            characterContainer_.RecycleAllChildren();
            Game.OnCurrentPlayerChanged -= RefreshCurrentPlayer;
        }

        // PRAGMA MARK - Internal
        [Header("Outlets")]
        [SerializeField] private GameObject characterContainer_;
        [SerializeField] private GameObject currentPlayerContainer_;

        [Header("Properties")]
        [SerializeField] private GamePlayer targetPlayer_;

        private Character3DView characterView_;

        private void RefreshCurrentPlayer() {
            currentPlayerContainer_.SetActive(Game.CurrentPlayer == targetPlayer_);
        }
    }
}