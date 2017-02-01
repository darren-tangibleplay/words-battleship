using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;

namespace Tangible.WordsBattleship {
    public class ChooseCharacterState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        private ChooseCharacterView chooseCharacterView_;

        protected sealed override void OnInitialized() {
        }

        protected sealed override void OnStateEntered() {
            GameSetupView.Instance.Show();
            chooseCharacterView_ = ObjectPoolManager.CreateView<ChooseCharacterView>(viewManager: GameSetupView.Instance.SubViewManager);
        }

        protected sealed override void OnStateExited() {
            if (chooseCharacterView_ != null) {
                ObjectPoolManager.Recycle(chooseCharacterView_);
                chooseCharacterView_ = null;
            }
        }

        protected sealed override void OnStateUpdated() {
        }
    }
}