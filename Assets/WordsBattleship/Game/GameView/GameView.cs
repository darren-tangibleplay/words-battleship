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
            Recycle3DView();
            game3DView_ = ObjectPoolManager.Create<Game3DView>();
            game3DView_.Init(renderTexture_);
        }

        public void Hide() {
            Recycle3DView();

            ObjectPoolManager.Recycle(gameObject);
            instance_ = null;
        }


        // PRAGMA MARK - Internal
        [Header("Outlets")]
        [SerializeField] private ViewManager subViewManager_;
        [SerializeField] private RenderTexture renderTexture_;

        private Game3DView game3DView_;

        private void Recycle3DView() {
            if (game3DView_ != null) {
                ObjectPoolManager.Recycle(game3DView_);
                game3DView_ = null;
            }
        }
    }
}