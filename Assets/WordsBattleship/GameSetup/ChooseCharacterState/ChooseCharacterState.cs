using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;

namespace Tangible.WordsBattleship {
    public class ChooseCharacterState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        private ChooseCharacterView chooseCharacterView_;

        protected sealed override void OnStateEntered() {
            if (GameSetup.ChooseCharacterTarget == GamePlayer.None) {
                if (GameSetup.FirstPlayerCharacter == null) {
                    GameSetup.ChooseCharacterTarget = GamePlayer.First;
                } else if (GameSetup.SecondPlayerCharacter == null) {
                    GameSetup.ChooseCharacterTarget = GamePlayer.Second;
                } else {
                    ExitCharacterSelect();
                    return;
                }
            }

            GameSetupView.Instance.Show();
            chooseCharacterView_ = ObjectPoolManager.CreateView<ChooseCharacterView>(viewManager: GameSetupView.Instance.SubViewManager);
            chooseCharacterView_.Init(HandleCharacterSelected, HandleNextTapped);
        }

        protected sealed override void OnStateExited() {
            if (chooseCharacterView_ != null) {
                ObjectPoolManager.Recycle(chooseCharacterView_);
                chooseCharacterView_ = null;
            }

            GameSetup.ChooseCharacterTarget = GamePlayer.None;
        }

        private void HandleCharacterSelected(Character character) {
            switch (GameSetup.ChooseCharacterTarget) {
                case GamePlayer.First:
                {
                    GameSetup.FirstPlayerCharacter = character;
                    break;
                }
                case GamePlayer.Second:
                {
                    GameSetup.SecondPlayerCharacter = character;
                    break;
                }
                default:
                {
                    Debug.LogError("Invalid target for HandleCharacterSelected()!");
                    GameSetup.SecondPlayerCharacter = character;
                    break;
                }
            }
        }

        private bool HasCharacterSelected() {
            switch (GameSetup.ChooseCharacterTarget) {
                case GamePlayer.First:
                {
                    return GameSetup.FirstPlayerCharacter != null;
                }
                case GamePlayer.Second:
                {
                    return GameSetup.SecondPlayerCharacter != null;
                }
                default:
                {
                    Debug.LogError("Invalid target for HasCharacterSelected()!");
                    return GameSetup.SecondPlayerCharacter != null;
                }
            }
        }

        private void HandleNextTapped() {
            if (!HasCharacterSelected()) {
                Debug.LogWarning("Cannot leave state until character has been selected!");
                return;
            }

            Exit();
        }

        private void Exit() {
            // exit current will go back to this state
            // which happens until all characters are chosen
            StateMachine_.ExitCurrent();
        }

        private void ExitCharacterSelect() {
            StateMachine_.ExitCharacterSelect();
        }
    }
}