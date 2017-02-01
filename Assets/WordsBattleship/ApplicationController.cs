using System;
using Tangible.Shared;
using UnityEngine;

namespace Tangible.WordsBattleship {
    [RequireComponent(typeof(ApplicationStateMachine))]
    public class ApplicationController : Singleton<ApplicationController> {
        // PRAGMA MARK - Public Interface
        public void Init() {
            stateMachine_.Init();
        }

        public void Cleanup() {
            stateMachine_.Cleanup();
        }


        // PRAGMA MARK - Internal
        [SerializeField] private ApplicationConstants constants_;

        private ApplicationStateMachine stateMachine_;

        void Awake() {
            if (constants_ == null) {
                Debug.LogError("No ApplicationConstants in ApplicationController - can't start game!");
                return;
            }

            ApplicationConstants.Instance = constants_;
            stateMachine_ = this.GetRequiredComponent<ApplicationStateMachine>();
        }
    }
}
