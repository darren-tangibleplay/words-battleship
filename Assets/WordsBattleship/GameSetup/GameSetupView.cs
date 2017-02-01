using System;
using System.Collections;
using UnityEngine;

using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class GameSetupView : Singleton<GameSetupView> {
        // PRAGMA MARK - Public Interface
        public ViewManager SubViewManager {
            get { return subViewManager_; }
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }


        // PRAGMA MARK - Internal
        [SerializeField] private ViewManager subViewManager_;

        // NOTE (darren): we use Start instead of Awake here
        // because we want to let this class register as a singleton
        // this is kinda hacky.. should just change singleton to grab disabled MonoBehaviours
        void Start() {
            Hide();
        }
    }
}