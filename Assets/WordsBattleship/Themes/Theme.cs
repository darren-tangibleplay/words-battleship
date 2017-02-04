using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

using DTAnimatorStateMachine;
using Tangible.Shared;

namespace Tangible.WordsBattleship {
    [CreateAssetMenu(fileName="Theme", menuName="Theme")]
    public class Theme : ScriptableObject {
        // PRAGMA MARK - Public Interface
        public string Name {
            get { return name_; }
        }

        public string[] Words {
            get {
                if (parsedWords_ == null || parsedWords_.Length == 0) {
                    parsedWords_ = Regex.Split(wordsCSV_.text, @"\s*,\s*").Where(s => !string.IsNullOrEmpty(s)).Select(s => s.ToLower()).ToArray();
                }
                return parsedWords_;
            }
        }

        public HashSet<string> AllWordsSet {
            get {
                if (allWordsSet_ == null) {
                    allWordsSet_ = new HashSet<string>(Words);
                }
                return allWordsSet_;
            }
        }



        // PRAGMA MARK - Internal
        [SerializeField] private string name_;
        [SerializeField] private TextAsset wordsCSV_;

        [NonSerialized] private string[] parsedWords_ = null;
        [NonSerialized] private HashSet<string> allWordsSet_ = null;
    }
}