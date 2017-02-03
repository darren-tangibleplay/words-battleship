using System;
using System.Collections;
using System.Linq;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class GuessWordState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Public Interface
        public float TimeLeft {
            get { return _endTime - Time.time; }
        }


        // PRAGMA MARK - Internal
        private float _endTime;
        private GuessWordView guessWordView_;

        protected sealed override void OnStateEntered() {
            MonoBehaviourHelper.OnUpdate += HandleUpdate;

            if (Game.IsPlayerAI(Game.CurrentPlayer)) {
                CoroutineWrapper.StartCoroutine(AIGuessLoop());
            }

            RefreshEndTime();

            guessWordView_ = ObjectPoolManager.CreateView<GuessWordView>(viewManager: GameView.Instance.SubViewManager);
            guessWordView_.Init(this);
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

            if (guessWordView_ != null) {
                ObjectPoolManager.Recycle(guessWordView_);
                guessWordView_ = null;
            }
        }

        private void HandleUpdate() {
            if (TimeLeft <= 0.0f) {
                StateMachine_.ExitCurrent();
                return;
            }

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

            RefreshEndTime();

            if (Game.DidCurrentPlayerGuessAllLetters()) {
                StateMachine_.ExitWinner();
                return true;
            }

            return false;
        }

        private IEnumerator AIGuessLoop() {
            while (true) {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.8f, 1.5f));

                char[] nonGuessedLetters = ApplicationConstants.kLetters.Where(l => !Game.DidCurrentPlayerAlreadyGuessLetter(l)).ToArray();
                bool exited = HandleGuess(nonGuessedLetters.Random());
                if (exited) {
                    yield break;
                }
            }
        }

        private void RefreshEndTime() {
            _endTime = Time.time + ApplicationConstants.Instance.GuessWordTimeLimit;
        }
    }
}