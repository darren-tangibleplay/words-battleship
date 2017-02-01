using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class CharacterBubble : MonoBehaviour {
        // PRAGMA MARK - Public Interface
        public void Init(Character character, Action<Character> onTapped) {
            character_ = character;

            mugShotImage_.sprite = character.MugShotSprite;
            nameText_.text = character.Name;

            onTapped_ = onTapped;
        }


        // PRAGMA MARK - Internal
        [SerializeField] private Button button_;
        [SerializeField] private Image mugShotImage_;
        [SerializeField] private Text nameText_;

        private Character character_;
        private Action<Character> onTapped_;

        void Awake() {
            button_.onClick.AddListener(HandleTapped);
        }

        void OnDestroy() {
            button_.onClick.RemoveListener(HandleTapped);
        }

        private void HandleTapped() {
            onTapped_.Invoke(character_);
        }
    }
}