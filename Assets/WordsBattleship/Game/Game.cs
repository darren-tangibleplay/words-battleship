using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DTAnimatorStateMachine;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public static class Game {
        // PRAGMA MARK - Static Public Interface
        public static event Action OnCurrentPlayerChanged = delegate {};
        public static event Action OnPlayerGuessedLetter = delegate {};

        public static bool PopulateFromSetup() {
            if (!IsGameSetupValid()) {
                return false;
            }

            playerCharacters_.Clear();
            playerWords_.Clear();

            foreach (GamePlayer player in GamePlayerUtil.ValidPlayers) {
                playerCharacters_[player] = GameSetup.GetCharacterForPlayer(player);
                playerWords_[player] = GameSetup.GetWordForPlayer(player);
            }

            foreach (HashSet<char> guessedLetters in guessedLettersMap_.Values) {
                guessedLetters.Clear();
            }

            currentPlayer_ = GamePlayer.First;

            return true;
        }

        public static Character GetCharacterForPlayer(GamePlayer player) {
            return playerCharacters_.GetValueOrDefault(player, defaultValue: null);
        }

        public static Character GetCharacterForCurrentPlayer() {
            return GetCharacterForPlayer(CurrentPlayer);
        }

        public static string GetWordForPlayer(GamePlayer player) {
            return playerWords_.GetValueOrDefault(player, defaultValue: null);
        }

        public static string GetWordForCurrentPlayer() {
            return GetWordForPlayer(CurrentPlayer);
        }

        public static bool DidCurrentPlayerAlreadyGuessLetter(char letter) {
            return DidPlayerAlreadyGuessLetter(CurrentPlayer, letter);
        }

        public static bool DidPlayerAlreadyGuessLetter(GamePlayer player, char letter) {
            HashSet<char> guessedLetters = guessedLettersMap_.GetAndCreateIfNotFound(player);
            return guessedLetters.Contains(letter);
        }

        public static bool CurrentPlayerGuessedLetter(char letter) {
            return PlayerGuessedLetter(CurrentPlayer, letter);
        }

        public static bool PlayerGuessedLetter(GamePlayer player, char letter) {
            HashSet<char> guessedLetters = guessedLettersMap_.GetAndCreateIfNotFound(player);
            bool successful = guessedLetters.Add(letter);
            if (!successful) {
                Debug.LogWarning("Can't guess letter that player has already guessed!");
                return false;
            }

            OnPlayerGuessedLetter.Invoke();
            return GetWordForPlayer(player).Contains(letter.ToString());
        }

        public static bool DidCurrentPlayerGuessAllLetters() {
            string word = GetWordForPlayer(CurrentPlayer);
            foreach (char c in word) {
                if (!DidCurrentPlayerAlreadyGuessLetter(c)) {
                    return false;
                }
            }

            return true;
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
        private static Dictionary<GamePlayer, HashSet<char>> guessedLettersMap_ = new Dictionary<GamePlayer, HashSet<char>>();

        private static Dictionary<GamePlayer, Character> playerCharacters_ = new Dictionary<GamePlayer, Character>();
        private static Dictionary<GamePlayer, string> playerWords_ = new Dictionary<GamePlayer, string>();

        private static GamePlayer currentPlayer_;

        private static bool IsGameSetupValid() {
            foreach (GamePlayer player in GamePlayerUtil.ValidPlayers) {
                if (GameSetup.GetCharacterForPlayer(player) == null) {
                    return false;
                }

                if (string.IsNullOrEmpty(GameSetup.GetWordForPlayer(player))) {
                    return false;
                }
            }

            return true;
        }
    }
}