using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class RenderTextureSource : MonoBehaviour {
        // PRAGMA MARK - Public Interface
        public void RegisterForRenderTexture(Action<RenderTexture> callback) {
            if (renderTexture_ != null) {
                callback.Invoke(renderTexture_);
            } else {
                callback_ = callback;
            }
        }


        // PRAGMA MARK - Internal
        [SerializeField] private RawImage rawImageTarget_;

        private Action<RenderTexture> callback_;
        private RenderTexture renderTexture_;

        void Start() {
            // CoroutineWrapper.DoAfterFrame(() =>
            {
                var worldCorners = new Vector3[4];
                var rectTransform = GetComponent<RectTransform>();

                rectTransform.GetWorldCorners(worldCorners);

                int xMax = worldCorners.Max(v => (int)v.x);
                int xMin = worldCorners.Min(v => (int)v.x);
                int yMax = worldCorners.Max(v => (int)v.y);
                int yMin = worldCorners.Min(v => (int)v.y);

                int width = xMax - xMin;
                int height = yMax - yMin;

                renderTexture_ = new RenderTexture(width, height, depth: 12);
                renderTexture_.Create();

                rawImageTarget_.texture = renderTexture_;

                if (callback_ != null) {
                    callback_.Invoke(renderTexture_);
                }
            }
            // );
        }
    }
}