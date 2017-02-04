using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;
using TMPro;

namespace Tangible.WordsBattleship {
    public class WordSelectView : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - Public Interface
        public void Init(Action onNextTapped, Action onTimeout) {
            onNextTapped_ = onNextTapped;
            onTimeout_ = onTimeout;

            text_.text = string.Format(instructionText_, (GameSetup.CurrentPlayer == GamePlayer.First) ? "1" : "2");

            endTime_ = Time.time + ApplicationConstants.Instance.WordSelectTimeLimit;
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
        [SerializeField] private TMP_Text timerText_;
        [SerializeField] private Text text_;
        [SerializeField] private string instructionText_ = "Set a word for PLAYER {0} to guess!";

        [Space]
        [SerializeField] private Button nextButton_;
        [SerializeField] private Button deleteButton_;

        private Action onNextTapped_;
        private Action onTimeout_;

        private float endTime_;

        void Awake() {
            nextButton_.onClick.AddListener(HandleNextTapped);
            deleteButton_.onClick.AddListener(HandleDeleteTapped);
        }

        void OnDestroy() {
            nextButton_.onClick.RemoveListener(HandleNextTapped);
            deleteButton_.onClick.RemoveListener(HandleDeleteTapped);
        }

        private void HandleUpdate() {
            float now = Time.time;
            float timeLeft = endTime_ - now;

            timerText_.text = (Math.Max((int)timeLeft, 0)).ToString();
            timerText_.color = (timeLeft <= 10.0f) ? Color.red : Color.green;

            if (timeLeft <= 0.0f) {
                if (onTimeout_ != null) {
                    onTimeout_.Invoke();
                    onTimeout_ = null;
                }
                return;
            }

            string word = GameSetup.GetWordForCurrentPlayer();
            foreach (char letter in Vision.AllNewLetters()) {
                // append letter to current player word
                word = word ?? "";
                word += letter.ToString();
            }

            if (word == null) {
                return;
            }

            if (word.Length > ApplicationConstants.Instance.MaxWordLength) {
                word = word.Substring(0, ApplicationConstants.Instance.MaxWordLength);
                PopupMessage.Make("Can't add any more letters!");
            }

            GameSetup.SetWordForCurrentPlayer(word);
        }

        private void HandleNextTapped() {
            onNextTapped_.Invoke();
        }

        private void HandleDeleteTapped() {
            string word = GameSetup.GetWordForCurrentPlayer();
            if (string.IsNullOrEmpty(word)) {
                return;
            }

            word = word.WithLastCharacterRemoved();

            GameSetup.SetWordForCurrentPlayer(word);
        }
    }
}