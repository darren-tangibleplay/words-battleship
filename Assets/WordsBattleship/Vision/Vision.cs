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
    public static class Vision {
        // PRAGMA MARK - Public Interface
        public static char[] AllNewLetters() {
            return newLetters_.ToArray();
        }

        public static void HandleLettersFromVisionEvent(IList<char> letters) {
            newLetters_.Clear();

            foreach (char letter in letters) {
                lettersFoundThisFrameBuffer_.Add(char.ToLower(letter));
            }

            foreach (char letter in lettersFoundThisFrameBuffer_) {
                if (!lettersFoundThisFrame_.Contains(letter)) {
                    newLetters_.Add(letter);
                }
            }

            lettersFoundThisFrame_.Clear();
            foreach (char letter in lettersFoundThisFrameBuffer_) {
                lettersFoundThisFrame_.Add(letter);
            }

            // temporary buffer doesn't need to store data
            lettersFoundThisFrameBuffer_.Clear();
        }


        // PRAGMA MARK - Internal
        private static readonly char[] kEmptyLetterArray = new char[0];

        private static readonly HashSet<char> lettersFoundThisFrame_ = new HashSet<char>();
        private static readonly HashSet<char> newLetters_ = new HashSet<char>();

        // temporary buffer
        private static readonly HashSet<char> lettersFoundThisFrameBuffer_ = new HashSet<char>();

        [RuntimeInitializeOnLoadMethod]
        private static void RegisterUpdate() {
            MonoBehaviourHelper.OnUpdate += HandleUpdate;
        }

        private static void HandleUpdate() {
            // Handle Unity editor keys in same way as vision events
            #if UNITY_EDITOR
            foreach (char letter in ApplicationConstants.kLetters) {
                if (Input.GetKey(letter.ToString())) {
                    lettersFoundThisFrameBuffer_.Add(letter);
                }
            }
            #endif
        }
    }
}