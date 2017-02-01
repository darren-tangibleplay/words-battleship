using System;
using Tangible.Shared;
using UnityEngine;

namespace Tangible.WordsBattleship {
    [RequireComponent(typeof(Animator))]
    public class ApplicationController : Singleton<ApplicationController> {
        // PRAGMA MARK - Public Interface
        public void Start() {
            _animator.SetTrigger("Reset");
        }

        public void Stop() {
        }


        // PRAGMA MARK - Internal
        private Animator _animator;

        void Awake() {
            _animator = GetRequiredComponent<Animator>();
        }
    }
}
