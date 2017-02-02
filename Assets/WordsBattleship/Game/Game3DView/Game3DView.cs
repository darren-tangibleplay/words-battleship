using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class Game3DView : MonoBehaviour {
        // PRAGMA MARK - Public Interface
        public void Init(RenderTexture renderTexture_) {
            camera_.targetTexture = renderTexture_;
        }


        // PRAGMA MARK - Internal
        [SerializeField] private Camera camera_;
    }
}