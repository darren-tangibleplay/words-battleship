using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class WordSelectState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        private WordSelectView wordSelectView_;

        protected sealed override void OnStateEntered() {
            if (GameSetup.CurrentPlayer == GamePlayer.None) {
                foreach (GamePlayer player in GamePlayerUtil.ValidPlayers) {
                    if (GameSetup.GetWordForPlayer(player) == null) {
                        GameSetup.CurrentPlayer = player;
                        break;
                    }
                }

                if (GameSetup.CurrentPlayer == GamePlayer.None) {
                    ExitWordSelect();
                    return;
                }
            }

            wordSelectView_ = ObjectPoolManager.CreateView<WordSelectView>(viewManager: GameSetupView.Instance.SubViewManager);
            wordSelectView_.Init(HandleNextTapped, HandleTimeout);
        }

        protected sealed override void OnStateExited() {
            if (wordSelectView_ != null) {
                ObjectPoolManager.Recycle(wordSelectView_);
                wordSelectView_ = null;
            }

            GameSetup.CurrentPlayer = GamePlayer.None;
        }

        private bool HasValidWord() {
            string word = GameSetup.GetWordForCurrentPlayer();

            if (GameSetup.Theme != null && !GameSetup.Theme.AllWordsSet.Contains(word)) {
                // TODO (darren): pop up message
                return false;
            }

            return true;
        }

        private void HandleNextTapped() {
            if (!HasValidWord()) {
                string word = GameSetup.GetWordForCurrentPlayer();
                PopupMessage.Make(word + " is not in the list of valid words!");
                return;
            }

            Exit();
        }

        private void HandleTimeout() {
            GameSetup.SetWordForCurrentPlayer(GameSetup.Theme.Words.Random());

            CoroutineWrapper.DoAfterDelay(1.3f, () => {
                Exit();
            });
        }

        private void Exit() {
            // exit current will go back to this state
            // which happens until all words are chosen
            StateMachine_.ExitCurrent();
        }

        private void ExitWordSelect() {
            StateMachine_.StartGame();
        }
    }
}