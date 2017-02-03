using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class GameSetupView : MonoBehaviour {
        // PRAGMA MARK - Public Static Interface
        private static GameSetupView instance_;
        public static GameSetupView Instance {
            get {
                if (instance_ == null) {
                    instance_ = ObjectPoolManager.CreateView<GameSetupView>();
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
            GameSetup.OnCurrentPlayerChanged += Refresh;
        }

        private void Refresh(bool force = false) {
            // don't refresh if not active
            if (!force && !gameObject.activeSelf) {
                return;
            }

            Character firstPlayerCharacter = GameSetup.GetCharacterForPlayer(GamePlayer.First);
            Character secondPlayerCharacter = GameSetup.GetCharacterForPlayer(GamePlayer.Second);

            if (firstPlayerCharacter != null) {
                if (GameSetup.CurrentPlayer == GamePlayer.First) {
                    firstCharacterImage_.sprite = firstPlayerCharacter.HappySprite;
                } else {
                    firstCharacterImage_.sprite = firstPlayerCharacter.NeutralSprite;
                }
            } else {
                firstCharacterImage_.sprite = null;
            }

            if (secondPlayerCharacter != null) {
                if (GameSetup.CurrentPlayer == GamePlayer.Second) {
                    secondCharacterImage_.sprite = secondPlayerCharacter.HappySprite;
                } else {
                    secondCharacterImage_.sprite = secondPlayerCharacter.NeutralSprite;
                }
            } else {
                secondCharacterImage_.sprite = null;
            }

            firstPlayerSelectedContainer_.SetActive(GameSetup.CurrentPlayer == GamePlayer.First);
            secondPlayerSelectedContainer_.SetActive(GameSetup.CurrentPlayer == GamePlayer.Second);
        }
    }
}