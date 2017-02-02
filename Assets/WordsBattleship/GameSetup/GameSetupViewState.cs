using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;

namespace Tangible.WordsBattleship {
    public class GameSetupViewState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        protected sealed override void OnStateEntered() {
            GameSetupView.Instance.Show();
        }

        protected sealed override void OnStateExited() {
            GameSetupView.Instance.Hide();
        }
    }
}