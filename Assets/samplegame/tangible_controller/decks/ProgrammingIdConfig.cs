// ﻿using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System;
//
// using Newtonsoft.Json;
// using Newtonsoft.Json.Bson;
//
// public enum ProgrammingAction {
// 	NONE,
// 	WALK,
// 	JUMP,
// 	TOOL,
// 	MAGIC,
// 	REPEAT,
// 	IF,
// 	END_INACTIVE,
// 	END_ACTIVE,
// 	END_OCCLUDED
// }
//
// // order matters here since it is used to determine ids to use to
// // match what vision is sending
// public enum ProgrammingDirection {
// 	NONE,
// 	UP,
// 	DOWN,
// 	LEFT,
// 	RIGHT
// }
//
// public enum ProgrammingType {
// 	Action,
// 	Quantifier,
// 	Direction,
// 	Magic,
// 	If,
// 	Compile
// }
//
// public class ProgrammingIdConfig : MonoBehaviour, IdConfig {
//
// 	const float INCH_TO_MM = 25.4f;
// 	const float TOTAL_WIDTH_MM = 5.4f * INCH_TO_MM;
// 	const float TOTAL_HEIGHT_MM = 9.194f * INCH_TO_MM;
//
// 	public class TileInfo {
// 		public int Id { get; set; }
// 		public ProgrammingType Type { get; set; }
// 		public ProgrammingAction Action { get; set; }
// 		public ProgrammingDirection Direction { get; set; }
// 		public int Value { get; set; }
// 		public int Count { get; set; }
//
// 		// inches from left side
// 		public float NubXPos { get; set; }
//
// 		// x, y, width, height
// 		List<float> position_ = new List<float>();
// 		public List<float> Position {
// 			get { return position_; }
// 			set { position_.Clear(); position_.AddRange(value); }
// 		}
//
// 		// distances from center to edge: left, right, top, bottom
// 		List<float> edge_override_ = new List<float>();
// 		public List<float> EdgeOverride {
// 			get { return edge_override_; }
// 			set { edge_override_.Clear(); edge_override_.AddRange(value); }
// 		}
//
// 		public float GetTextureXPosMillimeters() {
// 			return position_ [0] * INCH_TO_MM;
// 		}
//
// 		public float GetTextureYPosMillimeters() {
// 			return position_ [1] * INCH_TO_MM;
// 		}
//
// 		public float GetWidthMillimeters() {
// 			return position_ [2] * INCH_TO_MM;
// 		}
//
// 		public float GetHeightMillimeters() {
// 			return position_ [3] * INCH_TO_MM;
// 		}
//
// 		public float GetEdgeLeftMm() {
// 			if (edge_override_.Count > 0) {
// 				return edge_override_ [0] * -1f * INCH_TO_MM;
// 			}
// 			return -0.5f * GetWidthMillimeters ();
// 		}
//
// 		public float GetEdgeRightMm() {
// 			if (edge_override_.Count > 1) {
// 				return edge_override_ [1] * INCH_TO_MM;
// 			}
// 			return 0.5f * GetWidthMillimeters ();
// 		}
//
// 		public float GetEdgeTopMm() {
// 			if (edge_override_.Count > 2) {
// 				return edge_override_ [2] * INCH_TO_MM;
// 			}
// 			return 0.5f * GetHeightMillimeters ();
// 		}
//
// 		public float GetEdgeBottomMm() {
// 			if (edge_override_.Count > 3) {
// 				return edge_override_ [3] * -1f * INCH_TO_MM;
// 			}
// 			return -0.5f * GetHeightMillimeters ();
// 		}
//
// 		// convert from position from left side to position from center
// 		public float GetNubXOffsetMm() {
// 			float xPosMm = NubXPos * INCH_TO_MM;
// 			return xPosMm - (0.5f * GetWidthMillimeters ());
// 		}
//
// 		public TileInfo DeepCopy() {
// 			TileInfo copy = new TileInfo ();
// 			copy.Id = this.Id;
// 			copy.Type = this.Type;
// 			copy.Action = this.Action;
// 			copy.Direction = this.Direction;
// 			copy.Value = this.Value;
// 			copy.Count = this.Count;
// 			copy.NubXPos = this.NubXPos;
// 			copy.Position = this.Position;
// 			copy.EdgeOverride = this.EdgeOverride;
// 			return copy;
// 		}
// 	}
//
// 	public class Database {
// 		public int count { get; set; }
// 		public int pixels_per_inch { get; set; }
// 		public bool individual_ids { get; set; }
// 		List<TileInfo> graphics_ = new List<TileInfo>();
// 		public List<TileInfo> graphics {
// 			get { return graphics_; }
// 			set { graphics_.Clear(); graphics_.AddRange(value); }
// 		}
// 	}
//
// 	public class StrawbiesConfig {
// 		public Database database { get; set; }
// 	}
//
// 	[SerializeField]
// 	TextAsset config_json_file_;
//
// 	StrawbiesConfig config_;
// 	Dictionary<int, TileInfo> tile_dict_ = new Dictionary<int, TileInfo> ();
// 	// list of ids to use for mapping indexes to ids, easier processing for handling combined ids
// 	List<int> ids_ = new List<int>();
//
// 	void Awake() {
// 		config_ = ReadFromString(config_json_file_.ToString());
//
// 		// make a dictionary of tiles for faster access than searching a list each time
// 		// could maybe access array by index, but less reliable in case order changes
// 		tile_dict_.Clear();
// 		ids_.Clear ();
// 		foreach (TileInfo graphic in config_.database.graphics) {
// 			// we only have one direction entry in the json because that's what vision is using,
// 			// making entries for each here to make it easier to interpret the ids into component info
// 			if (!config_.database.individual_ids && graphic.Type == ProgrammingType.Direction) {
// 				int increment = graphic.Id;
// 				graphic.Direction = ProgrammingDirection.UP;
// 				graphic.Id = increment * (int)ProgrammingDirection.UP;
// 				ids_.Add (graphic.Id);
// 				tile_dict_.Add (graphic.Id, graphic);
// 				TileInfo down = graphic.DeepCopy ();
// 				down.Direction = ProgrammingDirection.DOWN;
// 				down.Id = increment * (int)ProgrammingDirection.DOWN;
// 				ids_.Add (down.Id);
// 				tile_dict_.Add (down.Id, down);
// 				TileInfo left = graphic.DeepCopy ();
// 				left.Direction = ProgrammingDirection.LEFT;
// 				left.Id = increment * (int)ProgrammingDirection.LEFT;
// 				ids_.Add (left.Id);
// 				tile_dict_.Add (left.Id, left);
// 				TileInfo right = graphic.DeepCopy ();
// 				right.Direction = ProgrammingDirection.RIGHT;
// 				right.Id = increment * (int)ProgrammingDirection.RIGHT;
// 				ids_.Add (right.Id);
// 				tile_dict_.Add (right.Id, right);
// 			} else {
// 				ids_.Add (graphic.Id);
// 				tile_dict_.Add (graphic.Id, graphic);
// 			}
// 		}
// 	}
//
// 	StrawbiesConfig ReadFromString(string json) {
// 		StrawbiesConfig config = null;
// 		try {
// 			TextReader tr = new StringReader(json);
// 			using (JsonReader reader = new JsonTextReader(tr)) {
// 				JsonSerializer serializer = new JsonSerializer();
// 				config = serializer.Deserialize<StrawbiesConfig>(reader);
// 			}
// 			Debug.Log ("Successfully Loaded Strawbies vision config File");
// 		} catch {
// 			Debug.LogError("ERROR LOADING STRAWBIES VISION CONFIG FILE");
// 		}
//
// 		return config;
// 	}
//
// 	// always getting the first id, which should be the highest numbered/
// 	// farthest to the left in the group
// 	// could instead more explicitly search for an action tile but should be the same result
// 	TileInfo GetSingleInfoForId(int id) {
// 		List<TileInfo> infos = GetInfoForId (id);
// 		if (infos.Count >= 1) {
// 			return infos [0];
// 		}
// 		Debug.LogError ("trying to get info for invalid id: " + id);
// 		return null;
// 	}
//
// 	List<TileInfo> GetInfoForId(int id) {
// 		List<int> splitIds = GetIdsFromCombinedId (id);
// 		List<TileInfo> infos = new List<TileInfo> ();
// 		int numIds = splitIds.Count;
// 		for (int i = 0; i < numIds; i++) {
// 			int splitId = splitIds [i];
// 			if (!tile_dict_.ContainsKey (splitId)) {
// 				Debug.LogError ("tile with id " + splitId + " not found in config database");
// 			}
// 			infos.Add (tile_dict_ [splitId]);
// 		}
// 		return infos;
// 	}
//
// 	TileInfo GetInfoOfTypeForId(int id, ProgrammingType type) {
// 		List<TileInfo> infos = GetInfoForId (id);
// 		int numInfos = infos.Count;
// 		for (int i = 0; i < numInfos; i++) {
// 			// also checking for actions on info objects if action type is passed in because there are other types
// 			// like compile and magic that would be intended to fall into this category
// 			if (infos [i].Type == type || (type == ProgrammingType.Action && infos[i].Action != ProgrammingAction.NONE)) {
// 				return infos [i];
// 			}
// 		}
// 		return null;
// 	}
//
// 	public int GetIdAtIndex(int index) {
// 		if (index < 0 || index >= ids_.Count) {
// 			return -1;
// 		}
// 		return ids_ [index];
// 	}
//
// 	public int GetIndexForId(int id) {
// 		int singleId = GetSingleInfoForId (id).Id;
// 		int numIds = ids_.Count;
// 		for (int i = 0; i < numIds; i++) {
// 			if (ids_ [i] == singleId) {
// 				return i;
// 			}
// 		}
// 		return -1;
// 	}
//
// 	public int GetNumIds() {
// 		return ids_.Count;
// 	}
//
// 	// keep subtracting the highest id possible from the current total
// 	// until we find all ids that combine to get the desired value
// 	// exiting if we somehow find nothing in a pass even if we haven't
// 	// reached the correct value yet to be sure we can't have infinite loops
// 	List<int> GetIdsFromCombinedId(int combined) {
// 		List<int> splitIds = new List<int> ();
// 		if (combined == 0) {
// 			splitIds.Add (0);
// 			return splitIds;
// 		}
// 		int curTotal = combined;
// 		int curIdToAdd = 0;
// 		int numIds = ids_.Count;
// 		bool found = true;
// 		int idToCheck;
// 		while (curTotal > 0 && found) {
// 			curIdToAdd = 0;
// 			found = false;
// 			for (int i = 0; i < numIds; i++) {
// 				idToCheck = ids_ [i];
// 				if (idToCheck > curIdToAdd && idToCheck <= curTotal) {
// 					curIdToAdd = idToCheck;
// 				}
// 			}
// 			if (curIdToAdd > 0) {
// 				found = true;
// 				curTotal -= curIdToAdd;
// 				splitIds.Add (curIdToAdd);
// 			}
// 		}
// 		return splitIds;
// 	}
//
// 	public int GetMaxQuantifier() {
// 		int max = 0;
// 		foreach (KeyValuePair<int, TileInfo> kvp in tile_dict_) {
// 			if (kvp.Value.Type == ProgrammingType.Quantifier && kvp.Value.Value > max) {
// 				max = kvp.Value.Value;
// 			}
// 		}
// 		return max;
// 	}
//
// 	public int GetCountForAction(ProgrammingAction action) {
// 		int count = 0;
// 		foreach (KeyValuePair<int, TileInfo> kvp in tile_dict_) {
// 			if (kvp.Value.Action == action) {
// 				count += kvp.Value.Count;
// 			}
// 		}
// 		return count;
// 	}
//
// 	public int GetValueForId(int id) {
// 		TileInfo info = GetInfoOfTypeForId (id, ProgrammingType.Quantifier);
// 		if (info != null) {
// 			return info.Value;
// 		}
// 		return 0;
// 	}
//
// 	// easier in this case to go through all the infos than to try to call the function to get one of a specific
// 	// type because types like If and Compile that can have an action as well
// 	public ProgrammingAction GetActionForId(int id) {
// 		List<TileInfo> infos = GetInfoForId (id);
// 		int numInfos = infos.Count;
// 		for (int i = 0; i < numInfos; i++) {
// 			if (infos [i].Action != ProgrammingAction.NONE) {
// 				return infos [i].Action;
// 			}
// 		}
// 		return ProgrammingAction.NONE;
// 	}
//
// 	// always get the rotation for the first tile
// 	public int GetRotationForId(int id) {
// 		return GetRotationForDirection (GetSingleInfoForId (id).Direction);
// 	}
//
// 	public int GetIdFromTangibleId(int tangibleId) {
// 		return GetSingleInfoForId (tangibleId).Id;
// 	}
//
// 	public int GetRotationForDirection(ProgrammingDirection dir) {
// 		if (dir == ProgrammingDirection.DOWN) {
// 			return 180;
// 		}
// 		if (dir == ProgrammingDirection.LEFT) {
// 			return 90;
// 		}
// 		if (dir == ProgrammingDirection.RIGHT) {
// 			return 270;
// 		}
// 		return 0;
// 	}
//
// 	// if this is a direction tile, determine direction based on orientation (default is up)
// 	// otherwise return none
// 	public ProgrammingDirection GetDirectionForId(int id, float orientation = 0) {
// 		TileInfo info = GetInfoOfTypeForId (id, ProgrammingType.Direction);
// 		if (info == null) {
// 			return ProgrammingDirection.NONE;
// 		}
//
// 		// if we have a direction associated with this id, always return that direction
// 		// instead of using the orientation
// 		if (info.Direction != ProgrammingDirection.NONE) {
// 			return info.Direction;
// 		}
//
// 		ProgrammingDirection dir = ProgrammingDirection.UP;
//
// 		// force the orientation to be positive
// 		while (orientation < 0) {
// 			orientation += 360f;
// 		}
//
// 		// adjust direction based on orientation
// 		int directionInt = (Mathf.RoundToInt(orientation/90f) % 4) * 90;
// 		if (directionInt == GetRotationForDirection (ProgrammingDirection.DOWN)) {
// 			dir = ProgrammingDirection.DOWN;
// 		}
// 		if (directionInt == GetRotationForDirection (ProgrammingDirection.LEFT)) {
// 			dir = ProgrammingDirection.LEFT;
// 		}
// 		if (directionInt == GetRotationForDirection (ProgrammingDirection.RIGHT)) {
// 			dir = ProgrammingDirection.RIGHT;
// 		}
// 		return dir;
// 	}
//
// 	public Color GetColorForId(int id) {
// 		return Color.white;
// 	}
//
// 	// using the first tile in the combination (which should be the action tile) as the determining
// 	// factor for how many of a given id we can have as that should be the biggest constraint and
// 	// simplifies the handling of unique ids/creating of editor cards
// 	public int GetCountForId(int id) {
// 		TileInfo info = GetSingleInfoForId (id);
// 		return info.Count;
// 	}
//
// 	public int GetUniqueGroupForId(int id) {
// 		return 0;
// 	}
//
// 	public string GetConfigJson() {
// 		return config_json_file_.ToString ();
// 	}
//
// 	// these only really makes sense when called for an individual id and not a combination
// 	public float GetTextureXPosMillimeters(int id) {
// 		TileInfo info = GetSingleInfoForId (id);
// 		return info.GetTextureXPosMillimeters () / TOTAL_WIDTH_MM;
// 	}
//
// 	public float GetTextureYPosMillimeters(int id) {
// 		TileInfo info = GetSingleInfoForId (id);
// 		return 1.0f - ((info.GetTextureYPosMillimeters () / TOTAL_HEIGHT_MM) + GetTextureHeightMillimeters (id));
// 	}
//
// 	public float GetTextureWidthMillimeters(int id) {
// 		return GetWidthMillimeters (id) / TOTAL_WIDTH_MM;
// 	}
//
// 	public float GetTextureHeightMillimeters(int id) {
// 		return GetHeightMillimeters (id) / TOTAL_HEIGHT_MM;
// 	}
//
// 	public float GetWidthMillimeters(int id) {
// 		List<TileInfo> infos = GetInfoForId (id);
// 		return GetWidthThroughSubtile (id, infos.Count - 1);
// 	}
//
// 	float GetWidthThroughSubtile(int id, int subIndex) {
// 		List<TileInfo> infos = GetInfoForId (id);
// 		float width = infos [0].GetWidthMillimeters();
// 		for (int i = 1; i < subIndex; i++) {
// 			width -= 0.5f * infos [i - 1].GetWidthMillimeters () - ComputeAttachPointMillimeters(infos [i - 1].Id, PairEdge.RIGHT).x;
// 			width += 0.5f * infos [i].GetWidthMillimeters() - ComputeAttachPointMillimeters (infos [i].Id, PairEdge.LEFT).x;
// 		}
// 		return width;
// 	}
//
// 	Vector3 GetOffsetForSubtile(int id, int subId) {
// 		List<TileInfo> infos = GetInfoForId (id);
// 		int numTiles = infos.Count;
// 		int index = 0;
// 		for (int i = 0; i < numTiles; i++) {
// 			if (infos [i].Id == subId) {
// 				index = i;
// 				break;
// 			}
// 		}
// 		// gets the center position of the subtile relative to the center position of the overall tile
// 		float xOffset = GetWidthThroughSubtile (id, index) - 0.5f * GetWidthMillimeters (subId) - 0.5f * GetWidthMillimeters(id);
// 		return new Vector3(xOffset, 0, 0);
// 	}
//
// 	// height the max height of any component ids
// 	public float GetHeightMillimeters(int id) {
// 		float max = 0;
// 		List<TileInfo> infos = GetInfoForId (id);
// 		int numTiles = infos.Count;
// 		for (int i = 0; i < numTiles; i++) {
// 			max = Mathf.Max (max, infos [i].GetHeightMillimeters ());
// 		}
// 		return max;
// 	}
//
// 	public Vector3[] GetEdgePoints (int id) {
// 		Vector3[] points = new Vector3[4];
// 		List<TileInfo> infos = GetInfoForId (id);
// 		int numTiles = infos.Count;
// 		if (numTiles == 0) {
// 			Debug.LogError ("trying to get edge points for invalid id: " + id);
// 			return points;
// 		}
// 		TileInfo leftInfo = infos [0];
// 		TileInfo rightInfo = infos [numTiles - 1];
// 		Vector3 leftOffset = GetOffsetForSubtile(id, leftInfo.Id);
// 		Vector3 rightOffset = GetOffsetForSubtile (id, rightInfo.Id);
// 		float leftEdge = leftInfo.GetEdgeLeftMm () + leftOffset.x;
// 		float rightEdge = rightInfo.GetEdgeRightMm () + rightOffset.x;
// 		points[0] = new Vector3 (leftEdge, leftInfo.GetEdgeBottomMm(), 0); // bottom left
// 		points[1] = new Vector3(rightEdge, rightInfo.GetEdgeBottomMm(), 0); 	// bottom right
// 		points[2] = new Vector3 (rightEdge, rightInfo.GetEdgeTopMm(), 0); // top right
// 		points[3] = new Vector3(leftEdge, leftInfo.GetEdgeTopMm(), 0); // top left
// 		return points;
// 	}
//
// 	public Vector3 ComputeAttachPointMillimeters(int id, PairEdge edge) {
// 		TileInfo info = GetAttachTileForEdge (id, edge);
//
// 		// first get local attach point on the tile that would be connected with for the given edge
// 		Vector3 attachPoint = Vector3.zero;
// 		// if direction tile, always use center point
// 		if (info.Type != ProgrammingType.Direction) {
// 			// if left or right use center of edge
// 			if (edge == PairEdge.RIGHT) {
// 				attachPoint = new Vector3 (info.GetEdgeRightMm (), 0, 0);
// 			} else if (edge == PairEdge.LEFT) {
// 				attachPoint = new Vector3 (info.GetEdgeLeftMm (), 0, 0);
// 			}
// 			// if top or bottom use nub position along edge
// 			else if (edge == PairEdge.BOTTOM) {
// 				attachPoint = new Vector3 (info.GetNubXOffsetMm (), info.GetEdgeBottomMm (), 0);
// 			} else if (edge == PairEdge.TOP) {
// 				attachPoint = new Vector3 (info.GetNubXOffsetMm (), info.GetEdgeTopMm (), 0);
// 			}
// 		}
//
// 		return attachPoint + GetOffsetForSubtile(id, info.Id);
// 	}
//
// 	TileInfo GetAttachTileForEdge(int id, PairEdge edge) {
// 		List<TileInfo> infos = GetInfoForId (id);
// 		int numIds = infos.Count;
// 		TileInfo info = null;
// 		// always use the action tile when checking top and bottom edges
// 		if (edge == PairEdge.BOTTOM || edge == PairEdge.TOP) {
// 			info = GetInfoOfTypeForId (id, ProgrammingType.Action);
// 		} else if (edge == PairEdge.LEFT) {
// 			info = infos [0];
// 		} else if (edge == PairEdge.RIGHT) {
// 			info = infos [numIds - 1];
// 		}
//
// 		// fall back on main tile if nothing found
// 		if (info == null) {
// 			info = GetSingleInfoForId (id);
// 		}
// 		return info;
// 	}
//
// 	public ProgrammingType GetTileTypeAtEdge(int id, PairEdge edge) {
// 		TileInfo info = GetAttachTileForEdge (id, edge);
// 		return info.Type;
// 	}
//
// 	// only handling direction separately if it's an individual tile not when combined with an action
// 	public float GetTileZMod(int id) {
// 		TileInfo info = GetSingleInfoForId (id);
// 		if (info.Type == ProgrammingType.Direction) {
// 			return -1;
// 		}
// 		return 0;
// 	}
//
// 	// check if repeat tile is upside down because the icon looks the same flipped
// 	public bool ShouldCheckFlip(int id) {
// 		TileInfo info = GetInfoOfTypeForId (id, ProgrammingType.Action);
// 		if (info != null && info.Action == ProgrammingAction.REPEAT) {
// 			return true;
// 		}
// 		return false;
// 	}
//
// 	public bool HasPressedState(int id) {
// 		TileInfo info = GetInfoOfTypeForId(id, ProgrammingType.Action);
// 		return (info != null && info.Action == ProgrammingAction.END_INACTIVE);
// 	}
//
// 	public bool IsPressedState(int id) {
// 		TileInfo info = GetInfoOfTypeForId(id, ProgrammingType.Action);
// 		return (info != null && info.Action == ProgrammingAction.END_ACTIVE);
// 	}
//
// 	public int GetIdForPressedState(int id) {
// 		TileInfo info = GetInfoOfTypeForId(id, ProgrammingType.Action);
// 		if (info != null) {
// 			foreach (KeyValuePair<int, TileInfo> kvp in tile_dict_) {
// 				if (info.Action == ProgrammingAction.END_INACTIVE && kvp.Value.Action == ProgrammingAction.END_ACTIVE) {
// 					return kvp.Key;
// 				}
// 			}
// 		}
// 		return -1;
// 	}
// }
