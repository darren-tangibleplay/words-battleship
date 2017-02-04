using System;
using System.Collections;
using UnityEngine;

using DTAnimatorStateMachine;
using DTObjectPoolManager;
using Tangible.Shared;
using TMPro;

namespace Tangible.WordsBattleship {
    public class ThemeSelectState : DTStateMachineBehaviour<ApplicationStateMachine> {
        // PRAGMA MARK - Internal
        private ActionScroller actionScroller_;

        protected sealed override void OnStateEntered() {
            if (!ApplicationConstants.Instance.EnableThemes) {
                StateMachine_.ExitCurrent();
                return;
            }

            GameSetup.Theme = ApplicationConstants.Instance.AllThemes.Random();

            // set AI words based on theme
            foreach (GamePlayer player in GamePlayerUtil.ValidPlayers) {
                if (GameSetup.IsPlayerAI(player)) {
                    GameSetup.SetWordForPlayer(GamePlayerUtil.NextPlayer(player), GameSetup.Theme.Words.Random());
                }
            }

            actionScroller_ = ObjectPoolManager.CreateView<ActionScroller>();
            actionScroller_.Scroll("ScrollThemeTitle", 0.3f, HorizontalDirection.Right, (GameObject prefab) => {
                // stub - do nothing
            });
            actionScroller_.Scroll("ScrollThemeName", 0.5f, HorizontalDirection.Left, (GameObject prefab) => {
                prefab.GetComponentInChildren<TMP_Text>().text = GameSetup.Theme.Name;
            });

            CoroutineWrapper.DoAfterDelay(2f, () => {
                StateMachine_.ExitCurrent();
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