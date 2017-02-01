using DTAnimatorStateMachine;
using System;
using System.Collections;
using UnityEngine;

namespace Tangible.WordsBattleship {
    public enum GamePlayer {
        None,
        First,
        Second,
    }

    public static class GameSetup {
        // PRAGMA MARK - Static Public Interface
        public static event Action OnFirstPlayerCharacterChanged = delegate {};
        public static event Action OnSecondPlayerCharacterChanged = delegate {};

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

        public static GamePlayer ChooseCharacterTarget = GamePlayer.None;


        // PRAGMA MARK - Static Internal
        private static Character firstPlayerCharacter_;
        private static Character secondPlayerCharacter_;
    }
}