using System;
using System.Collections;
using System.Linq;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class GuessWordState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        protected sealed override void OnStateEntered() {
            MonoBehaviourHelper.OnUpdate += HandleUpdate;

            if (Game.IsPlayerAI(Game.CurrentPlayer)) {
                CoroutineWrapper.StartCoroutine(AIGuessLoop());
            }
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
            if (Game.IsPlayerAI(Game.CurrentPlayer)) {
                return;
            }

            foreach (char letter in Vision.AllNewLetters()) {
                if (Game.DidCurrentPlayerAlreadyGuessLetter(letter)) {
                    continue;
                }

                bool exited = HandleGuess(letter);
                if (exited) {
                    return;
                }
            }
        }

        // returns true if exited state machine
        private bool HandleGuess(char letter) {
            if (Game.DidCurrentPlayerAlreadyGuessLetter(letter)) {
                return false;
            }

            bool correct = Game.CurrentPlayerGuessedLetter(letter);
            if (!correct) {
                StateMachine_.ExitCurrent();
                return true;
            }

            if (Game.DidCurrentPlayerGuessAllLetters()) {
                StateMachine_.ExitWinner();
                return true;
            }

            return false;
        }

        private IEnumerator AIGuessLoop() {
            while (true) {
                yield return new WaitForSeconds(UnityEngine.Random.Range(2.0f, 3.0f));

                char[] nonGuessedLetters = ApplicationConstants.kLetters.Where(l => !Game.DidCurrentPlayerAlreadyGuessLetter(l)).ToArray();
                bool exited = HandleGuess(nonGuessedLetters.Random());
                if (exited) {
                    yield break;
                }
            }
        }
    }
}