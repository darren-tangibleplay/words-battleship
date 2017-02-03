using System;
using Tangible.Shared;
using UnityEngine;

namespace Tangible.WordsBattleship {
    public enum ApplicationState {
        MainMenu,
        CharacterSelect,
        WordSelect,
    }

    [CreateAssetMenu(fileName="ApplicationConstants", menuName="ApplicationConstants")]
    public class ApplicationConstants : ScriptableObject {
        public static ApplicationConstants Instance;

        public static readonly string[] kRandomWords = new string[] { "apple", "apricot", "avocado", "banana", "blackberry", "blackcurrant", "blueberry", "boysenberry", "cherry", "coconut", "fig", "grape", "grapefruit", "kiwifruit", "lemon", "lime", "lychee", "mandarin", "mango", "melon", "nectarine", "orange", "papaya", "passion fruit", "peach", "pear", "pineapple", "plum", "pomegranate", "quince", "raspberry", "strawberry", "watermelon" };

        public static readonly char[] kLetters = new char[] {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        };


        // PRAGMA MARK - Public Interface
        [Header("Debug")]
        public ApplicationState InitialState = ApplicationState.MainMenu;
        public string InitialStateName {
            get {
                switch (InitialState) {
                    case ApplicationState.MainMenu:
                        return "Main Menu";
                    case ApplicationState.CharacterSelect:
                        return "Character Select";
                    case ApplicationState.WordSelect:
                        return "Word Select";
                    default:
                        return "Default";
                }
            }
        }

        [Header("Properties")]
        public Character[] AllCharacters;
        public Theme[] AllThemes;

        [Space]
        public int MaxWordLength = 10;
    }
}
