using DTAnimatorStateMachine;
using System;
using Tangible.Shared;
using UnityEngine;

namespace Tangible.WordsBattleship {
    [RequireComponent(typeof(Animator))]
    public class ApplicationStateMachine : MonoBehaviour {
        // PRAGMA MARK - Public Interface
        public void Init() {
            animator_.SetTrigger("Reset");

            this.DoAfterFrame(() => {
                if (ApplicationConstants.Instance.InitialState != ApplicationState.MainMenu) {
                    animator_.CrossFadeInFixedTime(ApplicationConstants.Instance.InitialStateName, 0.0f);
                }
            });
        }

        public void Cleanup() {
        }

        public void ExitCurrent() {
            animator_.SetTrigger("ExitCurrent");
        }

        public void ExitCharacterSelect() {
            animator_.SetTrigger("ExitCharacterSelect");
        }

        public void StartGameSetup() {
            animator_.SetTrigger("StartGameSetup");
        }

        public void StartGame() {
            animator_.SetTrigger("StartGame");
        }


        // PRAGMA MARK - Internal
        private Animator animator_;

        void Awake() {
            animator_ = this.GetRequiredComponent<Animator>();
            this.ConfigureAllStateBehaviours(animator_);
        }
    }
}
