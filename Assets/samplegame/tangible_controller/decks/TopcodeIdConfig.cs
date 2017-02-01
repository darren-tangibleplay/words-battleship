using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public class TopcodeIdConfig : MonoBehaviour, IdConfig {

	const float DEFAULT_SIZE = 25.4f;

	public class TopcodeInfo {
		public TopcodeInfo() {
			// Default values
			WidthMm = DEFAULT_SIZE;
			HeightMm = DEFAULT_SIZE;
			Button = false;
			BgColor = "ffffff";
			TextOverride = null;
		}

		public int Id { get; set; }
		public int Value { get; set; } 
		public int Count { get; set; } 
		public float WidthMm { get; set; } 
		public float HeightMm { get; set; }
		public bool Button { get; set; }
		public string BgColor { get; set; }
		public string TextOverride { get; set; }
		public float TopcodeXOffset { get; set; }
		public float TopcodeYOffset { get; set; }

		public TopcodeInfo DeepCopy() {
			TopcodeInfo copy = new TopcodeInfo ();
			copy.Id = this.Id;
			copy.Value = this.Value;
			copy.Count = this.Count;
			copy.WidthMm = this.WidthMm;
			copy.HeightMm = this.HeightMm;
			copy.Button = this.Button;
			copy.BgColor = this.BgColor;
			copy.TextOverride = this.TextOverride;
			copy.TopcodeXOffset = this.TopcodeXOffset;
			copy.TopcodeYOffset = this.TopcodeYOffset;
			return copy;
		}
	}

	public class Database {
		List<TopcodeInfo> ids_ = new List<TopcodeInfo>();
		public List<TopcodeInfo> Ids {
			get { return ids_; }
			set { ids_.Clear(); ids_.AddRange(value); }
		}
	}

	public class TopcodeConfig {
		public Database Database { get; set; }
	}

	[SerializeField]
	TextAsset config_json_file_;

	TopcodeConfig config_;
	Dictionary<int, TopcodeInfo> topcode_dict_ = new Dictionary<int, TopcodeInfo> ();
	// list of ids to use for mapping indexes
	List<int> ids_ = new List<int>();

	void Awake() {
		config_ = ReadFromString(config_json_file_.ToString()); 

		// make a dictionary of tiles for faster access than searching a list each time
		// could maybe access array by index, but less reliable in case order changes
		topcode_dict_.Clear();
		ids_.Clear ();
		foreach (TopcodeInfo topcode in config_.Database.Ids) {
			topcode_dict_.Add (topcode.Id, topcode);
			ids_.Add (topcode.Id);
		}
	}

	TopcodeConfig ReadFromString(string json) {
		TopcodeConfig config = null;
		try {
			TextReader tr = new StringReader(json);
			using (JsonReader reader = new JsonTextReader(tr)) {
				JsonSerializer serializer = new JsonSerializer();
				config = serializer.Deserialize<TopcodeConfig>(reader);
			}
			Debug.Log ("Successfully Loaded topcode vision config File");
		} catch {
			Debug.LogError("ERROR LOADING TOPCODE VISION CONFIG FILE");
		}

		return config;
	}
		
	public TopcodeInfo GetInfoForId(int id) {
		if (!topcode_dict_.ContainsKey (id)) {
			Debug.LogError ("topcode with id " + id + " not found in config database");
			return null;
		}
		return topcode_dict_ [id];
	}
		
	public int GetIdAtIndex(int index) {
		if (index < 0 || index >= ids_.Count) {
			return -1;
		}
		return ids_ [index];
	}

	public int GetIndexForId(int id) {
		int numIds = ids_.Count;
		for (int i = 0; i < numIds; i++) {
			if (ids_ [i] == id) {
				return i;
			}
		}
		return -1;
	}

	public int GetNumIds() {
		return ids_.Count;
	}
		
	public int GetValueForId(int id) {
		TopcodeInfo info = GetInfoForId (id );
		if (info != null) {
			return info.Value;
		}
		return 0;
	}
		
	public Color GetColorForId(int id) {
		TopcodeInfo info = GetInfoForId (id );
		if (info != null) {
			return StringHelper.HexStringToColor(info.BgColor);
		}
		return Color.white;
	}

	public string GetTextForId(int id) {
		TopcodeInfo info = GetInfoForId (id );
		if (info != null && !string.IsNullOrEmpty(info.TextOverride)) {
			return info.TextOverride;
		}
		return id.ToString ();
	}

	public float GetWidthMmForId(int id) {
		TopcodeInfo info = GetInfoForId (id );
		if (info != null) {
			return info.WidthMm;
		}
		return DEFAULT_SIZE;
	}

	public float GetHeightMmForId(int id) {
		TopcodeInfo info = GetInfoForId (id );
		if (info != null) {
			return info.HeightMm;
		}
		return DEFAULT_SIZE;
	}

	public bool IsButton(int id) {
		TopcodeInfo info = GetInfoForId (id );
		if (info != null) {
			return info.Button;
		}
		return false;
	}

	// using the first tile in the combination (which should be the action tile) as the determining
	// factor for how many of a given id we can have as that should be the biggest constraint and 
	// simplifies the handling of unique ids/creating of editor cards
	public int GetCountForId(int id) {
		TopcodeInfo info = GetInfoForId (id);
		if (info != null) {
			return info.Count;
		}
		return 0;
	}

	public int GetUniqueGroupForId(int id) {
		return 0;
	}
}
