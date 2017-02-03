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

            Refresh();
        }


        // PRAGMA MARK - Internal
        [SerializeField] private Button button_;
        [SerializeField] private Image mugShotImage_;
        [SerializeField] private Text nameText_;

        private Character character_;
        private Action<Character> onTapped_;

        void Awake() {
            button_.onClick.AddListener(HandleTapped);
            GameSetup.OnCharacterChanged += Refresh;
        }

        void OnDestroy() {
            button_.onClick.RemoveListener(HandleTapped);
            GameSetup.OnCharacterChanged -= Refresh;
        }

        private void HandleTapped() {
            onTapped_.Invoke(character_);
        }

        private void Refresh() {
            bool characterAlreadySelected = false;

            foreach (GamePlayer player in GamePlayerUtil.ValidPlayers) {
                var character = GameSetup.GetCharacterForPlayer(player);
                if (character == null) {
                    continue;
                }

                if (character == character_) {
                    characterAlreadySelected = true;
                    break;
                }
            }

            button_.interactable = !characterAlreadySelected;
        }
    }
}