using UnityEngine;
using System.Collections.Generic;

public class TrackingUtil {
	
	private const string TRACKING_LAST_STATE = "last_state";
	private const string TRACKING_PROP_LEVEL_NAME = "level_name";
	private const string TRACKING_PROP_LEVEL_IS_REPLAY = "is_replay"; // whether they used the replay button and end of level to play this
	private const string TRACKING_PROP_NUM_STARTED = "levels_started";
	private const string TRACKING_PROP_NUM_COMPLETED = "levels_completed";
	private const string TRACKING_PROP_NUM_ABANDONED = "levels_abandoned";
	private const string TRACKING_PROP_LEVEL_COMPLETED = "completed";
	private const string TRACKING_PROP_LEVEL_SCORE = "score";
	private const string TRACKING_PROP_LEVEL_PREVIOUS_HIGH_SCORE = "previous_high_score";
	private const string TRACKING_PROP_LEVEL_SOLVED_BEFORE = "solved_before";
	private const string TRACKING_PROP_SESSION_LEVEL_PLAYING = "level_playing";

		
	public static void OnSessionStart () {
#if ENABLE_PROTOTRACKING
		if (ProtoTracking.instance != null) {
			ProtoTracking.instance.TrackSessionStart();
		}
#endif
	}

	public static void OnSessionEnd(Dictionary<string, object> properties = null) {
#if ENABLE_PROTOTRACKING
		if (ProtoTracking.instance != null) {
			ProtoTracking.instance.TrackSessionEnd();
		}
#endif
	}

	public static void OnStartLevel(LevelDefinition levelDefinition, bool isReplay) {
#if ENABLE_PROTOTRACKING
		if (ProtoTracking.instance != null) {
			ProtoTracking.instance.TrackLevelStart(levelDefinition.Name);

			generated.SessionEvent session = ProtoTracking.instance.GetCurrentSessionEvent();
			if (session != null) {
				session.samplegame.level_playing = levelDefinition.Name;
			} else {
				Debug.LogWarning("[TrackingUtil] OnStartLevel attempted to access a session analytics event that is null!");
			}
		}
#endif
	}

	public static void OnEndLevel(LevelDefinition levelDefinition, bool completed, int score, int previousHighScore, bool solvedBefore) {
#if ENABLE_PROTOTRACKING
		if (ProtoTracking.instance != null) {
			generated.LevelEvent level = ProtoTracking.instance.GetCurrentLevelEvent();
			if (level != null) {
				level.samplegame.score = score;
				level.samplegame.previous_high_score = previousHighScore;
				level.samplegame.solved_before = solvedBefore;
				ProtoTracking.instance.TrackLevelEnd(completed);
			} else {
				Debug.LogWarning("[TrackingUtil] OnEndLevel attempted to access a level analytics event that is null!");
			}

			generated.SessionEvent session = ProtoTracking.instance.GetCurrentSessionEvent();
			if (session != null) {
				session.samplegame.level_playing = "none";
			} else {
				Debug.LogWarning("[TrackingUtil] OnEndLevel attempted to access a session analytics event that is null!");
			}
		}
#endif
	}

	public static void OnChoseReplay() {
#if ENABLE_PROTOTRACKING
		if (ProtoTracking.instance != null) {
			generated.UserProperties props = ProtoTracking.instance.GetUserPropertiesToIncrement();
			if (props != null) {
				props.samplegame.num_replay_presses = props.samplegame.num_replay_pressesSpecified ? props.samplegame.num_replay_presses + 1 : 1;
			} else {
				Debug.LogWarning("[TrackingUtil] OnChoseReplay attempted to access a user props analytics event that is null!");
			}
		}
#endif
	}

	public static void OnChoseContinue() {
#if ENABLE_PROTOTRACKING
		if (ProtoTracking.instance != null) {
			generated.UserProperties props = ProtoTracking.instance.GetUserPropertiesToIncrement();
			if (props != null) {
				props.samplegame.num_continue_presses = props.samplegame.num_continue_pressesSpecified ? props.samplegame.num_continue_presses + 1 : 1;
			} else {
				Debug.LogWarning("[TrackingUtil] OnChoseContinue attempted to access a user props analytics event that is null!");
			}
		}
#endif
	}

	public static void OnLevelRestart() {
#if ENABLE_PROTOTRACKING
		if (ProtoTracking.instance != null) {
			generated.UserProperties props = ProtoTracking.instance.GetUserPropertiesToIncrement();
			if (props != null) {
				props.samplegame.num_level_restarts = props.samplegame.num_level_restartsSpecified ? props.samplegame.num_level_restarts + 1 : 1;
			} else {
				Debug.LogWarning("[TrackingUtil] OnLevelRestart attempted to access a user props analytics event that is null!");
			}
		}
#endif
	}

	public static void OnTookMove(bool isFirst, double seconds) {
#if ENABLE_PROTOTRACKING
		if (ProtoTracking.instance != null) {
			generated.LevelEvent level = ProtoTracking.instance.GetCurrentLevelEvent();
			if (level != null) {
				level.samplegame.num_moves = level.samplegame.num_movesSpecified ? level.samplegame.num_moves + 1 : 1;
				if (isFirst) {
					level.samplegame.time_to_first_move_ms = (int)(seconds * 1000);
				}
			} else {
				Debug.LogWarning("[TrackingUtil] OnTookMove attempted to access a level analytics event that is null!");
			}

			generated.UserProperties props = ProtoTracking.instance.GetUserPropertiesToIncrement();
			if (props != null) {
				props.samplegame.total_num_moves = props.samplegame.total_num_movesSpecified ? props.samplegame.total_num_moves + 1 : 1;
			} else {
				Debug.LogWarning("[TrackingUtil] OnTookMove attempted to access a user props analytics event that is null!");
			}
		}
#endif
	}
}

