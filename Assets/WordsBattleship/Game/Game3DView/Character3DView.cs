using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class Character3DView : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        private const int kMaxSweat = 10;

        // PRAGMA MARK - Public Interface
        public void Init(GamePlayer player) {
            player_ = player;
            Refresh();
        }


        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            Game.OnAnyPlayerGuessedLetter += RefreshSweat;
            sweatParticleSystem_.transform.position = sweatContainer_.transform.position;
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            Game.OnAnyPlayerGuessedLetter -= RefreshSweat;
        }


        // PRAGMA MARK - Internal
        [Header("Outlets")]
        [SerializeField] private GameObject sweatContainer_;

        [Header("Properties")]
        [SerializeField, ReadOnly] private GamePlayer player_;

        private ParticleSystem sweatParticleSystem_;

        void Awake() {
            sweatParticleSystem_ = ObjectPoolManager.Create<ParticleSystem>("SweatParticleSystem");
        }

        void OnDisable() {
            SetSweatRate(0.0f);
        }

        private void Refresh() {
            RefreshSweat();
        }

        private void RefreshSweat() {
            // NOTE (darren): we use previous player because they are guessing to win!
            GamePlayer previousPlayer = GamePlayerUtil.PreviousPlayer(player_);
            string word = Game.GetWordForPlayer(previousPlayer);
            if (string.IsNullOrEmpty(word)) {
                SetSweatRate(0.0f);
                return;
            }

            float wordFinishedAmount = 0.0f;
            foreach (char c in word) {
                if (Game.DidPlayerAlreadyGuessLetter(previousPlayer, c)) {
                    wordFinishedAmount += 1.0f / word.Length;
                }
            }

            float sweat = Mathf.Lerp(0, kMaxSweat, wordFinishedAmount);

            SetSweatRate(sweat);
        }

        private void SetSweatRate(float rate) {
            sweatParticleSystem_.SetRate(new ParticleSystem.MinMaxCurve(rate));
        }
    }
}