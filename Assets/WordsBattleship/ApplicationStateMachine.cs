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
        }

        public void Cleanup() {
        }


        // PRAGMA MARK - Internal
        private Animator animator_;

        void Awake() {
            animator_ = this.GetRequiredComponent<Animator>();
            this.ConfigureAllStateBehaviours(animator_);
        }
    }
}
