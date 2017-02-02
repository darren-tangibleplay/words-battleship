using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class GameView : MonoBehaviour {
        // PRAGMA MARK - Public Static Interface
        private static GameView instance_;
        public static GameView Instance {
            get {
                if (instance_ == null) {
                    instance_ = ObjectPoolManager.CreateView<GameView>();
                }
                return instance_;
            }
        }


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
            Game.OnCurrentPlayerChanged += RefreshCurrentPlayer;
        }

        private void Refresh(bool force = false) {
            // don't refresh if not active
            if (!force && !gameObject.activeSelf) {
                return;
            }

            Character firstPlayerCharacter = Game.GetCharacterForPlayer(GamePlayer.First);
            Character secondPlayerCharacter = Game.GetCharacterForPlayer(GamePlayer.Second);

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

            firstPlayerSelectedContainer_.SetActive(Game.CurrentPlayer == GamePlayer.First);
            secondPlayerSelectedContainer_.SetActive(Game.CurrentPlayer == GamePlayer.Second);
        }
    }
}