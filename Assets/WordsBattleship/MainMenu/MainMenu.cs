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
            // TODO (darren): remove testing code should only set second player
            GameSetup.SetCharacterForPlayer(GamePlayer.First, ApplicationConstants.Instance.AllCharacters.Random());
            GameSetup.SetWordForPlayer(GamePlayer.First, "apple");

            GameSetup.SetCharacterForPlayer(GamePlayer.Second, ApplicationConstants.Instance.AllCharacters.Random());
            GameSetup.SetWordForPlayer(GamePlayer.Second, "banana");

            onComplete_.Invoke();
        }

        public void PlayHumanVsHuman() {
            onComplete_.Invoke();
        }

        // PRAGMA MARK - Internal
        private Action onComplete_;
    }
}