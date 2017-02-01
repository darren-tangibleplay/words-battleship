using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public class WordSelectLetterBubbles : MonoBehaviour, IRecycleSetupSubscriber, IRecycleCleanupSubscriber {
        // PRAGMA MARK - IRecycleSetupSubscriber Implementation
        public void OnRecycleSetup() {
            GameSetup.OnWordChanged += Refresh;
            Refresh();
        }


        // PRAGMA MARK - IRecycleCleanupSubscriber Implementation
        public void OnRecycleCleanup() {
            GameSetup.OnWordChanged -= Refresh;
            letterBubblesContainer_.RecycleAllChildren();
            showingWord_ = "";
            letterBubbles_.Clear();
        }


        // PRAGMA MARK - Internal
        [SerializeField] private GameObject placeholderContainer_;
        [SerializeField] private GameObject letterBubblesContainer_;

        private string showingWord_;
        private Dictionary<int, LetterBubble> letterBubbles_ = new Dictionary<int, LetterBubble>();

        private void Refresh() {
            string word = GameSetup.GetWordForPlayer(GameSetup.WordSelectTarget);
            if (word == showingWord_) {
                return;
            }

            placeholderContainer_.SetActive(string.IsNullOrEmpty(word));
            for (int i = 0; i < word.Length; i++) {
                char letter = word[i];

                if (showingWord_ != null && showingWord_.ContainsIndex(i)) {
                    char oldLetter = showingWord_[i];
                    if (oldLetter == letter) {
                        continue;
                    } else {
                        // empty the showing word so all other characters will be added
                        showingWord_ = "";
                        foreach (var kvp in letterBubbles_) {
                            int index = kvp.Key;
                            if (index < i) {
                                continue;
                            }

                            var letterBubble = kvp.Value;
                            letterBubble.Hide();
                        }
                    }
                }

                // NOTE (darren): this is separate because showingWord_ is modified in
                // the above statement
                if (showingWord_ == null || !showingWord_.ContainsIndex(i)) {
                    // create letter bubble
                    var letterBubble = ObjectPoolManager.Create<LetterBubble>(parent: letterBubblesContainer_);
                    letterBubble.Init(letter);

                    letterBubbles_[i] = letterBubble;
                }
            }

            // remove all letter bubbles not in word.Length
            var keysToRemove = new List<int>();
            foreach (var kvp in letterBubbles_) {
                int index = kvp.Key;
                if (index < word.Length) {
                    continue;
                }

                var letterBubble = kvp.Value;
                letterBubble.Hide();

                keysToRemove.Add(index);
            }

            foreach (int key in keysToRemove) {
                letterBubbles_.Remove(key);
            }

            showingWord_ = word;
        }
    }
}