using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public class LevelSave {
	public LevelSave(string level_name) {
		Name = level_name;
		Available = false;
		Solved = false;
		Highscore = 0;
	}
	public string Name { get; set; }
	// Per level data
	public bool Available { get; set; }
	public bool Solved { get; set; }
	public int Highscore { get; set; }

	public void MergeLevelData(LevelSave mergeLevel) {
		if (mergeLevel == null) {
			return;
		}
		if (mergeLevel.Available) {
			Available = true;
		}
		if (mergeLevel.Solved) {
			Solved = true;
		}
		if (mergeLevel.Highscore > Highscore) {
			Highscore = mergeLevel.Highscore;
		}
	}
	
	public LevelSave DeepCopy() {
		LevelSave level = new LevelSave(Name);
		level.Available = Available;
		level.Solved = Solved;
		level.Highscore = Highscore;

		return level;
	}
	
	public string ToString<T>(IEnumerable<T> enumerable) {
		string str = "[";
		foreach (T s in enumerable) {
			str += s.ToString() + " ";
		}
		return str + "]";
	}
	
	public bool DeepEquals(LevelSave level, bool expect_equal) {
		if (Name != level.Name) {
			if (expect_equal) Debug.LogError("Level.Equals: Name: " + Name + " <> " + level.Name);
			return false;
		}
		if (Available != level.Available) {
			if (expect_equal) Debug.LogError("Level.Equals: Available: " + Available + " <> " + level.Available);
			return false;
		}
		if (Solved != level.Solved) {
			if (expect_equal) Debug.LogError("Level.Equals: Solved: " + Solved + " <> " + level.Solved);
			return false;
		}
		if (Highscore != level.Highscore) {
			if (expect_equal) Debug.LogError("Level.Equals: Highscore: " + Highscore + " <> " + level.Highscore);
			return false;
		}
		return true;
	}
};

public class SaveData {

    const int CURRENT_VERSION = 1; // Remember to update SaveData.UpgradeVersion(..)

    // Data
    //------
    
    public SaveData() {
        Version = CURRENT_VERSION;
		creation_timestamp = DateTime.UtcNow.Ticks;
    }

    public int Version { get; set; }
    public string LevelSelected { get; set; }
    public long last_saved_timestamp { get; set; }
	public long creation_timestamp { get; set; }
	public long reset_timestamp { get; set; }
    
    List<LevelSave> levels_ = new List<LevelSave>();
    public List<LevelSave> Levels {
        get { return levels_; }
        set { levels_.Clear(); levels_.AddRange(value); }
    }
	
    public SaveData DeepCopy() {
        SaveData save_data = new SaveData();
        save_data.Version = Version;
        save_data.LevelSelected = LevelSelected;
		save_data.last_saved_timestamp = last_saved_timestamp;
		save_data.creation_timestamp = creation_timestamp;
		save_data.reset_timestamp = reset_timestamp;
        Levels.ForEach(delegate (LevelSave level) { save_data.Levels.Add(level.DeepCopy()); });
        return save_data;
    }
    
    public bool DeepEquals(SaveData save_data, bool expect_equal) {
        if (Version != save_data.Version) {
            if (expect_equal) Debug.LogError("SaveData.Equals: Version: " + Version + " <> " + save_data.Version);
            return false;
        }
        if (LevelSelected != save_data.LevelSelected) {
            // Does not matter for this one
        }
        if (Levels.Count != save_data.Levels.Count) {
            if (expect_equal) Debug.LogError("SaveData.Equals: Levels.Count: " + Levels.Count + " <> " + save_data.Levels.Count);
            return false;
        }

        for (int i = 0; i < Levels.Count; i++) {
            if (!Levels[i].DeepEquals(save_data.Levels[i], expect_equal)) {
                return false;
            }
        }
        return true;
    }


    // Helpers
    //---------

    public LevelSave FindOrCreateLevelData(string level_name) {
        LevelSave level = FindLevelData(level_name);
        if (level == null) {
            level = new LevelSave(level_name);
            levels_.Add(level);
        }
        return level;
    }
    
    public LevelSave FindLevelData(string level_name) {
        return levels_.Find(d => d.Name == level_name);
    }
	
	public static SaveData UpgradeVersion(SaveData save_data) {
		if (save_data.Version != CURRENT_VERSION) {
            string notes = "";

            // Check the old version and current version and make any changes to the data here specifically
            // needed to convert the player's data to be valid without losing their progress

            Debug.Log("[SaveData] Upgraded save version " + save_data.Version + " -> version " + CURRENT_VERSION + "\n" + notes);
            save_data.Version = CURRENT_VERSION;
		}
		return save_data;
	}

	public static SaveData MergeSaveData(SaveData baseData, SaveData mergingData) {
		// if one has a reset timestamp greater than the last saved timestamp of the other, return the 
		// reset version so that old data doesn't get merged back in after a reset
		// uses unreliable client timestamps so not perfect but seems accurate enough to be worth doing 
		if (baseData.reset_timestamp > mergingData.last_saved_timestamp) {
			return baseData;
		}
		if (mergingData.reset_timestamp > baseData.last_saved_timestamp) {
			return mergingData;
		}
		Profiler.BeginSample("Merge Data");

		// start with base data
		SaveData mergedData = baseData.DeepCopy();

		//Take most recent
		if (mergingData.last_saved_timestamp > baseData.last_saved_timestamp) {
			mergedData.LevelSelected = mergingData.LevelSelected;
			mergedData.last_saved_timestamp = mergingData.last_saved_timestamp;
		} 

		// take oldest creation timestamp
		if (mergingData.creation_timestamp < baseData.creation_timestamp) {
			mergedData.creation_timestamp = mergingData.creation_timestamp;
		}

		// go through each level to merge in, add it if it doesn't exist otherwise
		// merge the level data
		mergingData.Levels.ForEach(delegate (LevelSave level) {
			LevelSave baseLevel = mergedData.FindLevelData(level.Name);
			if (baseLevel == null) {
				mergedData.levels_.Add(level);
			} else {
				baseLevel.MergeLevelData(level);
			}
		});
	
		Profiler.EndSample();
	
		return mergedData;
	}
}

public class SampleGamePlayerState : Persistence <SaveData> {
	public static bool stateDirty = false;
	
    private SaveData save_data_;
    public SaveData SaveData { get { return save_data_; } }

	public void Reset(string profileId) {
		long creationTimestamp = save_data_.creation_timestamp;
		save_data_ = new SaveData ();
		save_data_.reset_timestamp = DateTime.UtcNow.Ticks;
		save_data_.creation_timestamp = creationTimestamp;
		stateDirty = true;
		CheckSave(profileId);
	}
	
	public void Init() {
		base.Init(SectionId.samplegame);
	}

	const string migrationKey = "player_state_migration";
	
	protected override SaveData HandleNoData(string playerId) {
		SaveData data = null;
		if (!PlayerPrefs.HasKey(migrationKey)) {
			data = LoadFromFile(null);
			PlayerPrefs.SetString(migrationKey, playerId);
		}
		
		if (data == null) {
			data = new SaveData();
		}
		
		return data;
	}
	
	// update data that was added by conflict
	protected override bool HandleConflict (string playerId, string key, ref object current, object incoming, bool isDefault = false) {
		Debug.LogWarning("[SampleGamePlayerState] got a conflicting player state: " + playerId);
		
		if (key == "persistence") {
			Profiler.BeginSample("SampleGame state Conflict");
			Profiler.BeginSample("Serialize Data");
			SaveData incomingState = (incoming as Newtonsoft.Json.Linq.JObject).ToObject<SaveData>();
			SaveData currentState = (current as Newtonsoft.Json.Linq.JObject).ToObject<SaveData>();
			Profiler.EndSample();
			if(isDefault) {
				//force the merge to ignore reset timestamps when copying defaults, since we never want to compare the default datas resets to our accounts resets
				incomingState.reset_timestamp = currentState.reset_timestamp;
				incomingState.last_saved_timestamp = currentState.last_saved_timestamp;
			}
			
			SaveData merged = SaveData.MergeSaveData(currentState, incomingState);
			Profiler.BeginSample("Deserialize Data");
			current = Newtonsoft.Json.Linq.JObject.FromObject(merged);
			Profiler.EndSample();
			Profiler.EndSample();
			return true;
		} else {
			return base.HandleConflict(playerId, key, ref current, incoming, isDefault);
		}
	}
	
	public void CheckSave(string playerId) {
		if (stateDirty) {
			save_data_.last_saved_timestamp = DateTime.UtcNow.Ticks;
			Save (save_data_, playerId);
			stateDirty = false;
		}
	}

	public void Load(string playerId, Action<string> onComplete) {
		Load(playerId, (SaveData state) => {
			save_data_ = SaveData.UpgradeVersion(state);
			onComplete(playerId);
		});
	}

	
	// old code to save/load player data using a local file.  keeping this around so that we can load the
	// existing data to transfer over when switching to start using accounts
	
	// ======================================================== OLD

	private static string GetPath() {
		return Path.Combine(Application.persistentDataPath, "save.json");
	}

	public SaveData LoadFromFile(string profileId) {
		SaveData loaded = null;
		if (Game.saveLoad) {
			string path = GetPath ();
			if (File.Exists (path)) {
				using (FileStream fs = new FileStream(path, FileMode.Open)) {
					try {
						TextReader tr = new StreamReader (fs);
						using (JsonReader reader = new JsonTextReader(tr)) {						
							JsonSerializer serializer = new JsonSerializer ();
							loaded = serializer.Deserialize<SaveData> (reader);
						}
						Debug.Log ("LOADED... json");
					} catch (Exception e) {
						Dictionary<string, object> props = new Dictionary<string, object>();
						props["exception"] = e.ToString();
						Debug.LogError ("ERROR... loading json");
					}				
					fs.Close ();
				}
			}
		}
		return loaded;
	}

	public void SaveToFile(string profileId) {
		if (!Game.saveLoad || save_data_ == null) {
			return;
		}
		
		save_data_.last_saved_timestamp = DateTime.UtcNow.Ticks;
		
		if (!string.IsNullOrEmpty (profileId)) {
			CheckSave (profileId);
		} else {
			string path = GetPath ();
			using (FileStream fs = new FileStream(path, FileMode.Create)) {
				TextWriter tw = new StreamWriter (fs);
				using (JsonWriter writer = new JsonTextWriter(tw)) {
					writer.Formatting = Formatting.Indented;
					JsonSerializer serializer = new JsonSerializer ();
					serializer.Serialize (writer, save_data_);
				}
			
				fs.Close ();
                Debug.Log ("[SampleGamePlayerState] Save file stored to: " + path);
			}
		}
	}
}
