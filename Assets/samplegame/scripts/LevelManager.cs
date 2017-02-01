using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	[SerializeField]
	TextAsset levels_json_file_;

    public string LevelNameToStart { get; set; }
    public SampleGameLevel Level { get { return current_level_; } }
	
    DefinitionData definition_data_;
    public DefinitionData DefinitionData { get { return definition_data_; } }

	SampleGameLevel current_level_ = null;

    public LevelDefinition GetLevelDefinition(string level_name) {
        return definition_data_.Get(level_name);
    }

    public LevelSave GetLevelSave(string level_name) {
        return Game.SaveData.FindLevelData(level_name);
    }

    public LevelSave GetLevelDisplay(string level_name) {
        return Game.DisplayData.FindLevelData(level_name);
    }

    public string UnlockNextLevel(string level_name, bool select) {
		LevelDefinition level_definition = definition_data_.Get(level_name);
		if (level_definition == null)
			return null;

		string last_next_level_name = null;
		foreach (string next_level_name in level_definition.NextLevels) {
			if (UnlockLevel(next_level_name, level_name, true, last_next_level_name == null && select)) {
				last_next_level_name = next_level_name;
			}
		}

		return last_next_level_name;
	}
	
	public bool UnlockLevel(string level_name, string from_level_name, bool add_transition, bool select) {
        LevelSave level_save = Game.SaveData.FindOrCreateLevelData(level_name);
        LevelSave level_display = Game.DisplayData.FindOrCreateLevelData(level_name);

        if (!level_save.Available) {
            if (add_transition) {
            Game.TransitionManager.Add(new TransitionManager.Transition(
                    delegate () { 
						level_display.Available = true; 
						if (select) {
							Game.DisplayData.LevelSelected = level_name; 
						}
					},
                    TransitionManager.Moment.LEVEL_UNLOCKED, 
                    level_name, null, from_level_name));
            } else {
                level_display.Available = true;
            }

            level_save.Available = true;
            if (select) Game.SaveData.LevelSelected = level_name;
            return true;
        }

        return false;
	}

	public LevelDefinition GetInitialCell() {
		int numLevels = definition_data_.Levels.Count;
		for (int i = 0; i < numLevels; i++) {
			if (definition_data_.Levels [i].InitialSelection) {
				return definition_data_.Levels [i];
			}
		}
		return null;
	}

	public void UnlockInitialCells() {
		int numLevels = definition_data_.Levels.Count;
		LevelDefinition definition;
		for (int i = 0; i < numLevels; i++) {
			definition = definition_data_.Levels [i];
			if (definition.StartUnlocked) {
				bool shouldSelect = definition.InitialSelection && string.IsNullOrEmpty(Game.SaveData.LevelSelected);
				UnlockLevel (definition.Name, null, false, shouldSelect);
			}
		}
	}

	void Awake() {
        definition_data_ = DefinitionData.ReadFromString(levels_json_file_.ToString()); 
	}
    		
	public void ExitGame() {
		if (current_level_ != null) {
			ClearCurrentLevel();
		}
    }

	public void SetPauseCurrentLevel(bool pause) {
		if (current_level_ != null) {
			current_level_.SetPause (pause);
		}
	}

	public void HideDialogsForCurrentLevel() {
		if (current_level_ != null) {
			current_level_.HideDialogs ();
		}
	}

    public void SetScore(string level_name, int highscore) {
        LevelSave level_save = Game.SaveData.FindOrCreateLevelData(level_name);
        LevelSave level_display = Game.DisplayData.FindOrCreateLevelData(level_name);

        // Solved
        if (!level_save.Solved) {
            Game.TransitionManager.Add(new TransitionManager.Transition(
                delegate () { level_display.Solved = true; },
            TransitionManager.Moment.LEVEL_SOLVED, 
            level_name));

            level_save.Solved = true;
        } 

        // Highscore
        if (level_save.Highscore < highscore) {
            level_display.Highscore = highscore;
            level_save.Highscore = highscore;
        }
    }

	public void UpdateAchievementGoals() {
		AchievementManager.instance.SetGoal ("completeall", DefinitionData.Levels.Count);
	}

	public bool SolvedAllLevelsInGame() {
		List<LevelDefinition> levels = DefinitionData.Levels;
		foreach (LevelDefinition definition in levels) {
			LevelSave save = Game.SaveData.FindLevelData(definition.Name);
			if (save == null || !save.Solved) {
				return false;
			}
		}
		return true;
	}
	
	public void SetAchievementProgressFromSaveData() {
		List<LevelSave> levels = Game.SaveData.Levels;

		int numLevels = levels.Count;
		int numSolved = 0;
		for (int i = 0; i<numLevels; i++) {
			if (levels[i].Solved) {
				numSolved++;
			}
		}
		if (numSolved > 0) {
			AchievementManager.instance.SetAchievement ("levels_solved", numSolved);
		}
	}
		
	public void StartLevel(bool isReplay) {
		if (!isReplay) {
			SampleGameSoundManager.instance.PreloadSoundEffects(PlayState.STATE_NAME, LevelNameToStart);
		}
		LoadLevel (LevelNameToStart, isReplay);
	}

	public void LoadLevel(string level_name, bool isReplay) {
        ClearCurrentLevel();

        LevelDefinition level_definition = definition_data_.Get(level_name);
        if (level_definition == null) {
            Debug.Log("Level definition data for level '" + level_name + "' could not be found.");
            level_definition = new LevelDefinition();
        }

        LevelSave level_save = Game.SaveData.FindOrCreateLevelData(level_name);

		current_level_ = (Instantiate (Resources.Load ("prefabs/" + level_definition.LevelPrefab)) as GameObject).GetComponent<SampleGameLevel> ();
        current_level_.Init(level_definition, level_save);

		TrackingUtil.OnStartLevel (level_definition, isReplay);    
	}
	
	public void ClearCurrentLevel() {
		if (current_level_ != null) {
            current_level_.Cleanup();
            GameObject.Destroy(current_level_.gameObject);
        }
        Game.singleton.OnLevelUnloaded();
		current_level_ = null;
	}
}
