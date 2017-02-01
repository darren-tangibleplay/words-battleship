#if ENABLE_PROTOTRACKING

using UnityEngine;
using generated;
using System.IO;
using System.Collections.Generic;

public class SampleGameProtoTracking : ProtoTracking {

	protected override void PopulatePlayerState(PlayerState playerState) {
		playerState.samplegame = new generated.SampleGamePlayerState ();

		SaveData saveData = Game.SaveData;
		if (saveData != null) {
			List<LevelSave> levels = Game.SaveData.Levels;
			int numLevels = levels.Count;
			int numSolved = 0;
			int numAvailable = 0;
			for (int i = 0; i < numLevels; i++) {
				if (levels [i].Solved) {
					numSolved++;
				}
				if (levels [i].Available) {
					numAvailable++;
				}
			}

			playerState.samplegame.unique_levels_unlocked = numAvailable;
			playerState.samplegame.unique_levels_solved = numSolved;
		}
	}

	protected override void InitializeGameSpecificUserProperties(UserProperties properties, bool isIncrement) {
		properties.samplegame = new SampleGameUserProperties ();
	}
		
	protected override void InitializeGameSpecificSessionEvent(SessionEvent sessionEvent) {
		sessionEvent.samplegame = new SampleGameSessionEvent ();
	}
		
	protected override void InitializeGameSpecificLevelEvent(LevelEvent levelEvent) {
		levelEvent.samplegame = new SampleGameLevelEvent ();
	}

	protected override void SetMetadataSource() {
		_curSource = EventMetadata.Source.GAME_SAMPLEGAME;
	}

	protected override string GetCurrentGameState() {
		string stateName = "none";
		if (Game.StateMachine.currentState != null) {
			stateName = Game.StateMachine.currentState.name; 
		}
		return stateName;
	}

	protected override byte[] SerializeEventData(EventBatch batch) {
		using (MemoryStream ms = new MemoryStream()) {
			SampleGameAnalyticsEventSerializer serializer = new SampleGameAnalyticsEventSerializer();
			serializer.Serialize(ms, batch);
			return ms.ToArray();
		}
	}
}

#endif