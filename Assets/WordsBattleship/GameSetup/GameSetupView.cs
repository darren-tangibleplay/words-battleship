using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class GameSetupView : Singleton<GameSetupView> {
        // PRAGMA MARK - Public Interface
        public ViewManager SubViewManager {
            get { return subViewManager_; }
        }

        public void Show() {
            Refresh(force: true);
            RefreshCurrentPlayer(force: true);
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }


        // PRAGMA MARK - Internal
        [Header("Outlets")]
        [SerializeField] private ViewManager subViewManager_;

        // NOTE (darren): this top view should be a 3d play area
        // this is currently placeholder
        // TODO (darren): remove placeholder
        [Space]
        [SerializeField] private Image firstCharacterImage_;
        [SerializeField] private Image secondCharacterImage_;

        [Space]
        [SerializeField] private GameObject firstPlayerSelectedContainer_;
        [SerializeField] private GameObject secondPlayerSelectedContainer_;

        void Awake() {
            GameSetup.OnCharacterChanged += Refresh;
            GameSetup.OnCurrentPlayerChanged += RefreshCurrentPlayer;
        }

        // NOTE (darren): we use Start instead of Awake here
        // because we want to let this class register as a singleton
        // this is kinda hacky.. should just change singleton to grab disabled MonoBehaviours
        void Start() {
            Hide();
        }

        private void Refresh(bool force = false) {
            // don't refresh if not active
            if (!force && !gameObject.activeSelf) {
                return;
            }

            Character firstPlayerCharacter = GameSetup.GetCharacterForPlayer(GamePlayer.First);
            Character secondPlayerCharacter = GameSetup.GetCharacterForPlayer(GamePlayer.Second);

            if (firstPlayerCharacter != null) {
                firstCharacterImage_.sprite = firstPlayerCharacter.MugShotSprite;
            } else {
                firstCharacterImage_.sprite = null;
            }

            if (secondPlayerCharacter != null) {
                secondCharacterImage_.sprite = secondPlayerCharacter.MugShotSprite;
            } else {
                secondCharacterImage_.sprite = null;
            }
        }

        private void RefreshCurrentPlayer(bool force = false) {
            // don't refresh if not active
            if (!force && !gameObject.activeSelf) {
                return;
            }

            firstPlayerSelectedContainer_.SetActive(GameSetup.CurrentPlayer == GamePlayer.First);
            secondPlayerSelectedContainer_.SetActive(GameSetup.CurrentPlayer == GamePlayer.Second);
        }
    }
}