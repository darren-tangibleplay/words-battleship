using System;
using Tangible.Shared;
using UnityEngine;

namespace Tangible.WordsBattleship {
    [CreateAssetMenu(fileName="ApplicationConstants", menuName="ApplicationConstants")]
    public class ApplicationConstants : ScriptableObject {
        public static ApplicationConstants Instance;

        // PRAGMA MARK - Public Interface
        public Character[] AllCharacters;
    }
}
