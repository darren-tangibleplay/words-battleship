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

        [Space]
        public int MaxWordLength = 10;
    }
}
