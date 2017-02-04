using System;
using System.Collections;
using System.Linq;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class GuessWordDelayState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        [SerializeField] private float delay_ = 1.5f;

        protected sealed override void OnStateEntered() {
            CoroutineWrapper.DoAfterDelay(delay_, () => {
                if (Game.DidCurrentPlayerGuessAllLetters()) {
                    Game.CurrentPlayer = GamePlayer.None;
                } else {
                    Game.CurrentPlayer = GamePlayerUtil.NextPlayer(Game.CurrentPlayer);
                }

                StateMachine_.ExitCurrent();
            });
        }
    }
}