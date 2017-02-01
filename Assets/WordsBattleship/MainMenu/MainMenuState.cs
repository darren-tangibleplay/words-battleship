using DTAnimatorStateMachine;
using System;
using System.Collections;
using UnityEngine;

namespace Tangible.WordsBattleship {
    public class MainMenuState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        protected sealed override void OnInitialized() {
            Debug.LogWarning("OnInitialized");
        }

        protected sealed override void OnStateEntered() {
            Debug.LogWarning("OnStateEntered");
        }

        protected sealed override void OnStateExited() {
            Debug.LogWarning("OnStateExited");
        }

        protected sealed override void OnStateUpdated() {
        }
    }
}