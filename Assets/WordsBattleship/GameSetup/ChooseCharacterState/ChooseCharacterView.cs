using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class ChooseCharacterView : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Public Interface
        public void Init(Action<Character> onCharacterSelected, Action onNextTapped) {
            onCharacterSelected_ = onCharacterSelected;
            onNextTapped_ = onNextTapped;

            switch (GameSetup.ChooseCharacterTarget) {
                case GamePlayer.First:
                {
                    targetText_.text = "Player 1";
                    break;
                }
                case GamePlayer.Second:
                {
                    targetText_.text = "Player 2";
                    break;
                }
                default:
                {
                    targetText_.text = "INVALID PLAYER TARGET";
                    break;
                }
            }
        }


        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            foreach (var character in ApplicationConstants.Instance.AllCharacters) {
                var characterBubble = ObjectPoolManager.Create<CharacterBubble>(parent: characterBubblesContainer_);
                characterBubble.Init(character, HandleCharacterSelected);
            }
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            characterBubblesContainer_.RecycleAllChildren();
        }


        // PRAGMA MARK - Internal
        [SerializeField] private Text targetText_;
        [SerializeField] private Button nextButton_;
        [SerializeField] private GameObject characterBubblesContainer_;

        private Action<Character> onCharacterSelected_;
        private Action onNextTapped_;

        void Awake() {
            nextButton_.onClick.AddListener(HandleNextTapped);
        }

        void OnDestroy() {
            nextButton_.onClick.RemoveListener(HandleNextTapped);
        }

        private void HandleCharacterSelected(Character character) {
            onCharacterSelected_.Invoke(character);
        }

        private void HandleNextTapped() {
            onNextTapped_.Invoke();
        }
    }
}