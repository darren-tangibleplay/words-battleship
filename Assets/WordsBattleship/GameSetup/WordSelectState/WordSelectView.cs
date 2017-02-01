using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class WordSelectView : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Public Interface
        public void Init(Action onNextTapped) {
            onNextTapped_ = onNextTapped;
        }


        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            MonoBehaviourHelper.OnUpdate += HandleUpdate;
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            MonoBehaviourHelper.OnUpdate -= HandleUpdate;
        }


        // PRAGMA MARK - Internal
        [SerializeField] private Button nextButton_;
        [SerializeField] private Button deleteButton_;

        private Action onNextTapped_;

        void Awake() {
            nextButton_.onClick.AddListener(HandleNextTapped);
            deleteButton_.onClick.AddListener(HandleDeleteTapped);
        }

        void OnDestroy() {
            nextButton_.onClick.RemoveListener(HandleNextTapped);
            deleteButton_.onClick.RemoveListener(HandleDeleteTapped);
        }

        private void HandleUpdate() {
            string word = GameSetup.GetWordForPlayer(GameSetup.WordSelectTarget);
            foreach (char letter in Vision.AllNewLetters()) {
                // append letter to current player word
                word = word ?? "";
                word += letter.ToString();
            }

            if (word == null) {
                return;
            }

            if (word.Length > ApplicationConstants.Instance.MaxWordLength) {
                word = word.Substring(ApplicationConstants.Instance.MaxWordLength);
            }

            GameSetup.SetWordForPlayer(GameSetup.WordSelectTarget, word);
        }

        private void HandleNextTapped() {
            onNextTapped_.Invoke();
        }

        private void HandleDeleteTapped() {
            string word = GameSetup.GetWordForPlayer(GameSetup.WordSelectTarget);
            if (string.IsNullOrEmpty(word)) {
                return;
            }

            word = word.WithLastCharacterRemoved();

            GameSetup.SetWordForPlayer(GameSetup.WordSelectTarget, word);
        }
    }
}