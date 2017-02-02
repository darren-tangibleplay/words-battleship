using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class GuessWordState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        protected sealed override void OnStateEntered() {
            MonoBehaviourHelper.OnUpdate += HandleUpdate;
        }

        protected sealed override void OnStateExited() {
            MonoBehaviourHelper.OnUpdate -= HandleUpdate;

            if (Game.DidCurrentPlayerGuessAllLetters()) {
                Game.CurrentPlayer = GamePlayer.None;
            } else {
                int index = Array.IndexOf(GamePlayerUtil.ValidPlayers, Game.CurrentPlayer);
                int newIndex = GamePlayerUtil.ValidPlayers.WrapIndex(index + 1);

                Game.CurrentPlayer = GamePlayerUtil.ValidPlayers[newIndex];
            }
        }

        private void HandleUpdate() {
            foreach (char letter in Vision.AllNewLetters()) {
                if (Game.DidCurrentPlayerAlreadyGuessLetter(letter)) {
                    continue;
                }

                bool correct = Game.CurrentPlayerGuessedLetter(letter);
                if (!correct) {
                    StateMachine_.ExitCurrent();
                    return;
                }

                if (Game.DidCurrentPlayerGuessAllLetters()) {
                    StateMachine_.ExitWinner();
                    return;
                }
            }
        }
    }
}