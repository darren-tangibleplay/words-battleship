using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DTObjectPoolManager;
using DTViewManager;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    public static class StringExtensions {
        // PRAGMA MARK - Public Interface
        public static string WithLastCharacterRemoved(this string s) {
            return s.Substring(0, s.Length - 1);
        }

        public static bool ContainsIndex(this string s, int index) {
            if (s == null) {
                return false;
            }

            return index >= 0 && index < s.Length;
        }
    }
}