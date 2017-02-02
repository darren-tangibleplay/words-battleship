using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;

namespace Tangible.WordsBattleship {
    public class GameViewState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        protected sealed override void OnStateEntered() {
            GameView.Instance.Show();
        }

        protected sealed override void OnStateExited() {
            GameView.Instance.Hide();
        }
    }
}