using DTAnimatorStateMachine;
using System;
using System.Collections;
using UnityEngine;

namespace Tangible.WordsBattleship {
    public static class GameSetup {
        // PRAGMA MARK - Static Public Interface
        public static event Action OnFirstPlayerCharacterChanged;
        public static event Action OnSecondPlayerCharacterChanged;

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


        // PRAGMA MARK - Static Internal
        private static Character firstPlayerCharacter_;
        private static Character secondPlayerCharacter_;
    }
}