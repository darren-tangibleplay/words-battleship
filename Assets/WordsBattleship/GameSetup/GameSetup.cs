using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DTAnimatorStateMachine;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public static class GameSetup {
        // PRAGMA MARK - Static Public Interface
        public static event Action OnFirstPlayerCharacterChanged = delegate {};
        public static event Action OnSecondPlayerCharacterChanged = delegate {};

        public static event Action OnWordChanged = delegate {};

        public static Character FirstPlayerCharacter {
            get { return firstPlayerCharacter_; }
            set {
                if (firstPlayerCharacter_ == value) {
                    return;
                }

                firstPlayerCharacter_ = value;
                OnFirstPlayerCharacterChanged.Invoke();
            }
        }

        public static Character SecondPlayerCharacter {
            get { return secondPlayerCharacter_; }
            set {
                if (secondPlayerCharacter_ == value) {
                    return;
                }

                secondPlayerCharacter_ = value;
                OnSecondPlayerCharacterChanged.Invoke();
            }
        }

        public static string GetWordForPlayer(GamePlayer player) {
            return playerWords_.GetValueOrDefault(player, defaultValue: null);
        }

        public static void SetWordForPlayer(GamePlayer player, string word) {
            string oldWord = GetWordForPlayer(player);
            if (oldWord == word) {
                return;
            }

            playerWords_[player] = word;
            OnWordChanged.Invoke();
        }

        public static GamePlayer CharacterSelectTarget = GamePlayer.None;
        public static GamePlayer WordSelectTarget = GamePlayer.None;


        // PRAGMA MARK - Static Internal
        private static Character firstPlayerCharacter_;
        private static Character secondPlayerCharacter_;

        private static Dictionary<GamePlayer, string> playerWords_ = new Dictionary<GamePlayer, string>();
    }
}