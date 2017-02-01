using UnityEngine;
using System;

namespace Tangible.Game {
    // this is a wrapper state that connects sample game code to
    // outside code (no dependencies between the two)
    public class DependencyState : GameState {
        public const string kStateName = "dependency";

        public DependencyState() : base(kStateName, usesVision: false) {
        }

        override public void OnPush() {
            base.OnPush();

            Tangible.WordsBattleship.ApplicationController.Instance.Init();
        }

        override public void OnPop() {
            base.OnPop();

            Tangible.WordsBattleship.ApplicationController.Instance.Cleanup();
        }

        override public void OnEnter() {
            base.OnEnter();
        }

        override public void OnExit() {
            base.OnExit();
        }
    }
}
