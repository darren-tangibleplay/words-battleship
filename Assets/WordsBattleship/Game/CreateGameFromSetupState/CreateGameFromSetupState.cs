using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;

namespace Tangible.WordsBattleship {
    public class CreateGameFromSetupState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        protected sealed override void OnStateEntered() {
            bool successful = Game.PopulateFromSetup();
            if (successful) {
                StateMachine_.ExitCurrent();
            } else {
                Debug.LogWarning("GameSetup is not valid - going back through GameSetup flow!");
                StateMachine_.StartGameSetup();
            }
        }

        protected sealed override void OnStateExited() {
        }
    }
}