using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;

namespace Tangible.WordsBattleship {
    public class CharacterSelectState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        private CharacterSelectView characterSelectView_;

        protected sealed override void OnStateEntered() {
            if (GameSetup.CurrentPlayer == GamePlayer.None) {
                foreach (GamePlayer player in GamePlayerUtil.ValidPlayers) {
                    if (GameSetup.GetCharacterForPlayer(player) == null) {
                        GameSetup.CurrentPlayer = player;
                        break;
                    }
                }

                if (GameSetup.CurrentPlayer == GamePlayer.None) {
                    ExitCharacterSelect();
                    return;
                }
            }

            characterSelectView_ = ObjectPoolManager.CreateView<CharacterSelectView>(viewManager: GameSetupView.Instance.SubViewManager);
            characterSelectView_.Init(HandleCharacterSelected, HandleNextTapped);
        }

        protected sealed override void OnStateExited() {
            if (characterSelectView_ != null) {
                ObjectPoolManager.Recycle(characterSelectView_);
                characterSelectView_ = null;
            }

            GameSetup.CurrentPlayer = GamePlayer.None;
        }

        private void HandleCharacterSelected(Character character) {
            GameSetup.SetCharacterForCurrentPlayer(character);
        }

        private bool HasCharacterSelected() {
            return GameSetup.GetCharacterForCurrentPlayer() != null;
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