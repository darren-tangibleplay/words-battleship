using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class GamePlayerWordView : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Public Interface
        public void Init(GamePlayer player) {
            useCurrentPlayer_ = false;
            player_ = player;
            Refresh();
        }

        public void Init(bool useCurrentPlayer) {
            useCurrentPlayer_ = useCurrentPlayer;
            Refresh();
        }


        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            Game.OnCurrentPlayerChanged += Refresh;
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            Game.OnCurrentPlayerChanged -= Refresh;
            gameObject.RecycleAllChildren();
        }


        // PRAGMA MARK - Internal
        [SerializeField] private bool useCurrentPlayer_;
        [SerializeField, ReadOnly] private GamePlayer player_;

        private void Refresh() {
            gameObject.RecycleAllChildren();

            GamePlayer player = player_;
            if (useCurrentPlayer_) {
                player = Game.CurrentPlayer;
            }

            string word = Game.GetWordForPlayer(player);
            if (word == null) {
                return;
            }

            foreach (char c in word) {
                var playerLetter = ObjectPoolManager.Create<GamePlayerLetter>(parent: gameObject);
                playerLetter.Init(c, player);
            }
        }
    }
}