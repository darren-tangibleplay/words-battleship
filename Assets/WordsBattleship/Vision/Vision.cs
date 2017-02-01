using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public static class Vision {
        // PRAGMA MARK - Public Interface
        public static char[] AllNewLetters() {
            List<char> newLetters = null;

            // TODO (darren): hook up vision platform here

            #if UNITY_EDITOR
            foreach (char letter in kLettersToCheck) {
                if (Input.GetKeyDown(letter.ToString())) {
                    newLetters = newLetters ?? new List<char>();
                    newLetters.Add(letter);
                }
            }
            #endif

            return newLetters != null ? newLetters.ToArray() : kEmptyLetterArray;
        }


        // PRAGMA MARK - Internal
        private static readonly char[] kEmptyLetterArray = new char[0];

        private static readonly char[] kLettersToCheck = new char[] {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        };
    }
}