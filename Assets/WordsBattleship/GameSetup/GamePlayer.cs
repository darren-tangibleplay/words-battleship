using DTAnimatorStateMachine;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Tangible.WordsBattleship {
    public enum GamePlayer {
        None,
        First,
        Second,
    }

    public static class GamePlayerUtil {
        // PRAGMA MARK - Static Public Interface
        public static GamePlayer[] AllPlayers {
            get; private set;
        }

        public static GamePlayer[] ValidPlayers {
            get; private set;
        }


        static GamePlayerUtil() {
            AllPlayers = Enum.GetValues(typeof(GamePlayer)).ESelect(e => (GamePlayer)e).ToArray();
            ValidPlayers = AllPlayers.Where(e => e != GamePlayer.None).ToArray();
        }
    }
}