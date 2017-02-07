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
            HashSet<char> newLetters = new HashSet<char>(newLetters_);

            // NOTE (darren): we do this separately from the vision
            // because the vision won't get hit unless there is something on the board
            // therefore keyboard events are separate and treated differently
            #if UNITY_EDITOR
            foreach (char letter in ApplicationConstants.kLetters) {
                if (Input.GetKeyDown(letter.ToString())) {
                    newLetters.Add(letter);
                }
            }
            #endif

            return newLetters.ToArray();
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
    }
}