using DTAnimatorStateMachine;
using System;
using System.Collections;
using UnityEngine;

namespace Tangible.WordsBattleship {
    public class MainMenu : MonoBehaviour {
        // PRAGMA MARK - Public Interface
        public void Init(Action onComplete) {
            onComplete_ = onComplete;
        }

        public void PlayHumanVsAI() {
            GameSetup.SetPlayerAsAI(GamePlayer.Second);

            onComplete_.Invoke();
        }

        public void PlayHumanVsHuman() {
            onComplete_.Invoke();
        }

        // PRAGMA MARK - Internal
        private Action onComplete_;
    }
}