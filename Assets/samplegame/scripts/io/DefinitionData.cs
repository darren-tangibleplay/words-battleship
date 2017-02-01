using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public class LevelDefinition {
	public LevelDefinition() {
		// Default values
		Name = "<please_name_me>";
	}
	
	public string Name { get; set; } 
	public string LevelPrefab { get; set; }
	public bool UsesScore { get; set; }
	public bool StartUnlocked { get; set; }
	public bool InitialSelection { get; set; }

	List<string> next_levels_ = new List<string>();
	public List<string> NextLevels {
		get { return next_levels_; }
		set { next_levels_.Clear(); next_levels_.AddRange(value); }
	}

	public string GetDisplayName() {
		return Language.Get (Name);
	}
}

public class DefinitionData {

	// Data
	//------------

	List<LevelDefinition> levels_ = new List<LevelDefinition>();
	public List<LevelDefinition> Levels {
		get { return levels_; }
		set { levels_.Clear(); levels_.AddRange(value); }
	}
	
	public LevelDefinition Get(string level_name) {
		return Levels.Find(d => d.Name == level_name);
	}

    // Helper
    //------------
	 
    public static DefinitionData ReadFromString(string json) {
        DefinitionData definition_data = null;
        try {
            TextReader tr = new StringReader(json);
            using (JsonReader reader = new JsonTextReader(tr)) {
                JsonSerializer serializer = new JsonSerializer();
                definition_data = serializer.Deserialize<DefinitionData>(reader);
            }
            Debug.Log ("Successfully Loaded Level File");
        } catch {
            Debug.LogError("ERROR LOADING LEVELS");
        }

        if (definition_data == null) {
            definition_data = GenerateDummyDefinitionData();
        }
        
        #if UNITY_EDITOR
        // Re-save the data for up-to-date syntax
        definition_data.DevEdit();
        definition_data.WriteToFile();
        #endif

        return definition_data;
    }
    
    static DefinitionData GenerateDummyDefinitionData() {        
        DefinitionData definition_data = new DefinitionData();
        
        LevelDefinition level_definition = new LevelDefinition();
        
        definition_data.Levels.Add(level_definition);
        definition_data.Levels.Add(level_definition);

        return definition_data;
    }
    
    // Do any batch processing on levels here:
    void DevEdit() {
        for(int i = 0; i < Levels.Count; ++i) {
            LevelDefinition level_definition = Levels[i];
            DevEditOnLevel(level_definition);
        }
    }
    
    void DevEditOnLevel(LevelDefinition level_definition) {

    }

	private static string GetPath() {
		return Path.Combine(Application.persistentDataPath, "levels.json");
	}
	
	void WriteToFile() {
        string path = GetPath();
        using(FileStream fs = new FileStream(path, FileMode.Create)) {
            TextWriter tw = new StreamWriter(fs);
            using (JsonWriter writer = new JsonTextWriter(tw)) {
                writer.Formatting = Formatting.Indented;
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, this);
            }
            
            fs.Close();
            Debug.Log ("SAVED definition data json to: " + path);
        }
    }

}
