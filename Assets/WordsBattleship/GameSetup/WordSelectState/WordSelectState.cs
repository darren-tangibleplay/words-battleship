using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;

namespace Tangible.WordsBattleship {
    public class WordSelectState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        private WordSelectView wordSelectView_;

        protected sealed override void OnStateEntered() {
            if (GameSetup.WordSelectTarget == GamePlayer.None) {
                foreach (GamePlayer player in GamePlayerUtil.ValidPlayers) {
                    if (GameSetup.GetWordForPlayer(player) == null) {
                        GameSetup.WordSelectTarget = player;
                    }
                }

                if (GameSetup.WordSelectTarget == GamePlayer.None) {
                    ExitWordSelect();
                    return;
                }
            }

            GameSetupView.Instance.Show();
            wordSelectView_ = ObjectPoolManager.CreateView<WordSelectView>(viewManager: GameSetupView.Instance.SubViewManager);
            wordSelectView_.Init(HandleNextTapped);
        }

        protected sealed override void OnStateExited() {
            if (wordSelectView_ != null) {
                ObjectPoolManager.Recycle(wordSelectView_);
                wordSelectView_ = null;
            }

            GameSetup.WordSelectTarget = GamePlayer.None;
        }

        private bool HasValidWord() {
            // TODO (darren): add spell check
            // TODO (darren): add themes (mandatory word choice?)
            return true;
        }

        private void HandleNextTapped() {
            if (!HasValidWord()) {
                return;
            }

            Exit();
        }

        private void Exit() {
            // exit current will go back to this state
            // which happens until all words are chosen
            StateMachine_.ExitCurrent();
        }

        private void ExitWordSelect() {
            StateMachine_.ExitWordSelect();
        }
    }
}