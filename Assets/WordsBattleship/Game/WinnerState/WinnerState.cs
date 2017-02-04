using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTAnimatorStateMachine;
using DTObjectPoolManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class WinnerState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        private ActionScroller actionScroller_;

        protected sealed override void OnStateEntered() {
            GamePlayer winner = Game.GetWinner();
            if (winner == GamePlayer.None) {
                Debug.LogWarning("WinnerState - no winner! Going to MainMenu");
                StateMachine_.GoToMainMenu();
                return;
            }

            Game.CurrentPlayer = winner;

            CoroutineWrapper.DoAfterDelay(1.2f, () => {
                Character winnerCharacter = Game.GetCharacterForPlayer(winner);
                Character loserCharacter = Game.GetCharacterForPlayer(GamePlayerUtil.ValidPlayers.FirstOrDefault(p => p != winner));

                actionScroller_ = ObjectPoolManager.CreateView<ActionScroller>();
                actionScroller_.Scroll("ScrollWinnerCharacter", 0.5f, HorizontalDirection.Right, (GameObject prefab) => {
                    prefab.GetComponent<SpriteRenderer>().sprite = winnerCharacter.HappySprite;
                });
                actionScroller_.Scroll("ScrollLoserCharacter", 0.7f, HorizontalDirection.Left, (GameObject prefab) => {
                    prefab.GetComponent<SpriteRenderer>().sprite = loserCharacter.SadSprite;
                });

                actionScroller_.SetFlipped(winner != GamePlayer.First);

                CoroutineWrapper.DoAfterDelay(3f, () => {
                    StateMachine_.GoToMainMenu();
                });
            });
        }

        protected sealed override void OnStateExited() {
            if (actionScroller_ != null) {
                ObjectPoolManager.Recycle(actionScroller_);
                actionScroller_ = null;
            }
        }
    }
}