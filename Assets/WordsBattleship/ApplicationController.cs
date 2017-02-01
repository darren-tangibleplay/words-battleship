using System;
using Tangible.Shared;
using UnityEngine;

namespace Tangible.WordsBattleship {
    [RequireComponent(typeof(ApplicationStateMachine))]
    public class ApplicationController : Singleton<ApplicationController> {
        // PRAGMA MARK - Public Interface
        public void Init() {
            Debug.LogWarning("hi");
            stateMachine_.Init();
        }

        public void Cleanup() {
            stateMachine_.Cleanup();
        }


        // PRAGMA MARK - Internal
        private ApplicationStateMachine stateMachine_;

        void Awake() {
            stateMachine_ = this.GetRequiredComponent<ApplicationStateMachine>();
        }
    }
}
