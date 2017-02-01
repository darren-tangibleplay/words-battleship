using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;

namespace Tangible.WordsBattleship {
    public class ChooseCharacterState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        private ChooseCharacterView chooseCharacterView_;
        private GamePlayer target_;

        protected sealed override void OnStateEntered() {
            if (GameSetup.FirstPlayerCharacter == null) {
                target_ = GamePlayer.First;
            } else if (GameSetup.SecondPlayerCharacter == null) {
                target_ = GamePlayer.Second;
            } else {
                // we should probably skip this state if characters are already selected
                // although we probably want to make a "i already have a character
                // but want to change it intent"

                // for now we'll do nothing
            }

            GameSetupView.Instance.Show();
            chooseCharacterView_ = ObjectPoolManager.CreateView<ChooseCharacterView>(viewManager: GameSetupView.Instance.SubViewManager);
            chooseCharacterView_.Init(HandleCharacterSelected);
        }

        protected sealed override void OnStateExited() {
            if (chooseCharacterView_ != null) {
                ObjectPoolManager.Recycle(chooseCharacterView_);
                chooseCharacterView_ = null;
            }
        }

        private void HandleCharacterSelected(Character character) {
            switch (target_) {
                case GamePlayer.First:
                {
                    GameSetup.FirstPlayerCharacter = character;
                    break;
                }
                case GamePlayer.Second:
                default:
                {
                    GameSetup.SecondPlayerCharacter = character;
                    break;
                }
            }
        }
    }
}