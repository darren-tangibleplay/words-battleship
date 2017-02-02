using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DTAnimatorStateMachine;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public static class GameSetup {
        // PRAGMA MARK - Static Public Interface
        public static event Action OnCharacterChanged = delegate {};
        public static event Action OnWordChanged = delegate {};

        public static event Action OnCurrentPlayerChanged = delegate {};

        public static void Clear() {
            playerCharacters_.Clear();
            playerWords_.Clear();

            currentPlayer_ = GamePlayer.None;
        }

        public static Character GetCharacterForPlayer(GamePlayer player) {
            return playerCharacters_.GetValueOrDefault(player, defaultValue: null);
        }

        public static void SetCharacterForPlayer(GamePlayer player, Character character) {
            Character oldCharacter = GetCharacterForPlayer(player);
            if (oldCharacter == character) {
                return;
            }

            playerCharacters_[player] = character;
            OnCharacterChanged.Invoke();
        }

        public static Character GetCharacterForCurrentPlayer() {
            return GetCharacterForPlayer(CurrentPlayer);
        }

        public static void SetCharacterForCurrentPlayer(Character character) {
            SetCharacterForPlayer(CurrentPlayer, character);
        }

        public static string GetWordForPlayer(GamePlayer player) {
            return playerWords_.GetValueOrDefault(player, defaultValue: null);
        }

        public static void SetWordForPlayer(GamePlayer player, string word) {
            string oldWord = GetWordForPlayer(player);
            if (oldWord == word) {
                return;
            }

            playerWords_[player] = word.ToLower();
            OnWordChanged.Invoke();
        }

        public static string GetWordForCurrentPlayer() {
            return GetWordForPlayer(CurrentPlayer);
        }

        public static void SetWordForCurrentPlayer(string word) {
            SetWordForPlayer(CurrentPlayer, word);
        }

        public static GamePlayer CurrentPlayer {
            get { return currentPlayer_; }
            set {
                if (currentPlayer_ == value) {
                    return;
                }

                currentPlayer_ = value;
                OnCurrentPlayerChanged.Invoke();
            }
        }


        // PRAGMA MARK - Static Internal
        private static Dictionary<GamePlayer, Character> playerCharacters_ = new Dictionary<GamePlayer, Character>();
        private static Dictionary<GamePlayer, string> playerWords_ = new Dictionary<GamePlayer, string>();

        private static GamePlayer currentPlayer_;
    }
}