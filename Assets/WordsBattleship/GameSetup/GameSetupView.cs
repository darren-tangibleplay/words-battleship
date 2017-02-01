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

        void Awake() {
            GameSetup.OnFirstPlayerCharacterChanged += Refresh;
            GameSetup.OnSecondPlayerCharacterChanged += Refresh;
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

            if (GameSetup.FirstPlayerCharacter != null) {
                firstCharacterImage_.sprite = GameSetup.FirstPlayerCharacter.MugShotSprite;
            } else {
                firstCharacterImage_.sprite = null;
            }

            if (GameSetup.SecondPlayerCharacter != null) {
                secondCharacterImage_.sprite = GameSetup.SecondPlayerCharacter.MugShotSprite;
            } else {
                secondCharacterImage_.sprite = null;
            }
        }
    }
}