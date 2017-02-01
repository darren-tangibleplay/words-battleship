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
        public void Init(Action<Character> onCharacterSelected) {
            onCharacterSelected_ = onCharacterSelected;
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
        [SerializeField] private GameObject characterBubblesContainer_;

        private Action<Character> onCharacterSelected_;

        private void HandleCharacterSelected(Character character) {
            onCharacterSelected_.Invoke(character);
        }
    }
}