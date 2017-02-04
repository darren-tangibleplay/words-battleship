using DTAnimatorStateMachine;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Tangible.WordsBattleship {
    public enum GamePlayer {
        None = 0,
        First = 1,
        Second = 2,
    }

    public static class GamePlayerUtil {
        // PRAGMA MARK - Static Public Interface
        public static GamePlayer[] AllPlayers {
            get; private set;
        }

        public static GamePlayer[] ValidPlayers {
            get; private set;
        }

        public static GamePlayer NextPlayer(GamePlayer player) {
            int index = Array.IndexOf(GamePlayerUtil.ValidPlayers, player);
            int newIndex = GamePlayerUtil.ValidPlayers.WrapIndex(index + 1);

            return GamePlayerUtil.ValidPlayers[newIndex];
        }

        static GamePlayerUtil() {
            AllPlayers = Enum.GetValues(typeof(GamePlayer)).ESelect(e => (GamePlayer)e).OrderBy(e => e).ToArray();
            ValidPlayers = AllPlayers.Where(e => e != GamePlayer.None).ToArray();
        }
    }
}