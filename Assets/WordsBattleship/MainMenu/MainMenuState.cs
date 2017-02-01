using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;

namespace Tangible.WordsBattleship {
    public class MainMenuState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        private MainMenu mainMenu_;

        protected sealed override void OnInitialized() {
        }

        protected sealed override void OnStateEntered() {
            mainMenu_ = ObjectPoolManager.CreateView<MainMenu>();
            mainMenu_.Init(StartGameSetup);
        }

        protected sealed override void OnStateExited() {
            if (mainMenu_ != null) {
                ObjectPoolManager.Recycle(mainMenu_);
                mainMenu_ = null;
            }
        }

        protected sealed override void OnStateUpdated() {
        }

        private void StartGameSetup() {
            StateMachine_.StartGameSetup();
        }
    }
}