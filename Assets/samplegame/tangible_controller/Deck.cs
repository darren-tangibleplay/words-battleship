using UnityEngine;
using System.Collections.Generic;

public enum PairEdge {
	NONE,
	TOP,
	BOTTOM,
	LEFT,
	RIGHT
}

public abstract class Deck : MonoBehaviour {
	abstract public Tangible.TangibleObject.Shape GetShape(int index);
	abstract public float GetWidthMillimeters(int id);
	abstract public float GetHeightMillimeters(int id);
	abstract public float GetMillimeterToScreen();
	abstract public OnScreenObject GetPrefab();
	abstract public IdConfig GetIdConfig();
	abstract public Tangible.Config.RecognitionMode GetRecognitionMode();

	public int GetCount() {
		return GetIdConfig().GetNumIds ();
	}

	public bool ContainsId(int id) {
		int index = GetIndex(id);
		return index >= 0 && index < GetCount();
	}

	public Color GetColor (int index) {
		int id = GetId (index);
		return GetIdConfig().GetColorForId (id);
	}

	public int GetCount(int index) {
		int id = GetId (index);
		return GetIdConfig().GetCountForId (id);
	}

	public int GetUniqueGroup(int index) {
		int id = GetId (index);
		return GetIdConfig().GetUniqueGroupForId(id);
	}

	// by default ids are consecutive numbers starting at 0 and ids and indices are the same
	// can override if it's different for a deck
	public virtual int GetId(int index) {
		return index;
	}

	public virtual int GetIndex(int id) {
		return id;
	}

	protected virtual void AssignGraphicsToMeshRenderer(int index, MeshRenderer renderer) {
	}

	public virtual void AssignGraphics(int index, GameObject obj) {
		MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
		AssignGraphicsToMeshRenderer (index, renderer);
	}

	// convenience functions, should not need to be overridden but could be if the deck has data
	// stored in a way that gives better performance than converting each time
	public virtual float GetWidthScreen(int id) {
		return GetWidthMillimeters (id) * GetMillimeterToScreen ();
	}

	public virtual float GetHeightScreen(int id) {
		return GetHeightMillimeters (id) * GetMillimeterToScreen ();
	}

	public virtual float GetScreenToMillimeter() {
		return 1f / GetMillimeterToScreen ();
	}

	public virtual float GetCardDisplayScaleX(int id) {
		return GetWidthScreen (id);
	}

	public virtual float GetCardDisplayScaleY(int id) {
		return GetHeightScreen (id);
	}

	// in most cases these are just standard 4 corners based on the width/height, but adding the ability to override
	// for cases of non-standard shapes
	public virtual Vector3[] GetEdgePointsMillimeters (int id) {
		float width = GetWidthMillimeters (id);
		float height = GetHeightMillimeters (id);
		Vector3[] points = new Vector3[4];
		points[0] = new Vector3 (-0.5f * width, -0.5f * height, 0); // bottom left
		points[1] = new Vector3(0.5f * width, -0.5f * height, 0); 	// bottom right
		points[2] = new Vector3 (0.5f * width, 0.5f * height, 0); // top right
		points[3] = new Vector3(-0.5f * width, 0.5f * height, 0); // top left
		return points;
	}

	Dictionary<int, Dictionary<PairEdge, Vector3>> _cachedAttachPoints = new Dictionary<int, Dictionary<PairEdge, Vector3>> ();
	public Vector3 GetAttachPointMillimeters(int id, PairEdge edge) {
		Dictionary<PairEdge, Vector3> edgeDict = null;
		_cachedAttachPoints.TryGetValue (id, out edgeDict);
		if (edgeDict == null) {
			edgeDict = new Dictionary<PairEdge, Vector3> ();
			_cachedAttachPoints [id] = edgeDict;
		}
		if (!edgeDict.ContainsKey(edge)) {	
			edgeDict [edge] = ComputeAttachPointMillimeters (id, edge);
		}
		return edgeDict [edge];
	}

	// by default connection point is at the center of the edge
	public virtual Vector3 ComputeAttachPointMillimeters(int id, PairEdge edge) {
		switch (edge) {
			case PairEdge.BOTTOM: 
				return new Vector3 (0, -0.5f * GetHeightMillimeters (id), 0);
			case PairEdge.TOP:
				return new Vector3 (0, 0.5f * GetHeightMillimeters (id), 0);
			case PairEdge.LEFT: 
				return new Vector3 (-0.5f * GetWidthMillimeters (id), 0, 0);
			case PairEdge.RIGHT:
				return new Vector3 (0.5f * GetWidthMillimeters (id), 0, 0);
					
		}
		return Vector3.zero;
	}
		
	public virtual PairEdge GetOppositeEdge(PairEdge edge) {
		switch (edge) {
			case PairEdge.LEFT:
				return PairEdge.RIGHT;
			case PairEdge.RIGHT:
				return PairEdge.LEFT;
			case PairEdge.BOTTOM:
				return PairEdge.TOP;
			case PairEdge.TOP:
				return PairEdge.BOTTOM;
			default:
				return PairEdge.NONE;
		}
	}
		
	public virtual PairEdge GetEdge(HashSet<int> indexes) {
		// if matching more than 2 different points it's not on one clear edge
		if (indexes.Count > 2) {
			return PairEdge.NONE;
		}
		if (indexes.Contains(0) && indexes.Contains(1)) {
			return PairEdge.BOTTOM;
		}
		if (indexes.Contains(1) && indexes.Contains(2)) {
			return PairEdge.RIGHT;
		}
		if (indexes.Contains(2) && indexes.Contains(3)) {
			return PairEdge.TOP;
		}
		if (indexes.Contains(3) && indexes.Contains(0)) {
			return PairEdge.LEFT;
		}
		return PairEdge.NONE;
	}

	// can override to make some on screen tiles higher than others for better easier rotation
	public virtual float GetTileZMod(int id) {
		return 0;
	}
		
	// in millimeters
	public virtual float GetPositionErrorStrict() {
		return 5.0f;
	}

	public virtual float GetPositionErrorLoose() {
		return 8.0f;
	}

	// ratio of cardsize
	public virtual float GetVerticalErrorStrict() {
		return 0.5f;
	}

	public virtual float GetVerticalErrorLoose() {
		return 0.65f;
	}

	// whether or not we should check to see if a tile was flipped because vision might not be able to properly 
	// detect the orientation if it looks the same upside down
	public virtual bool ShouldCheckFlip(int id) {
		return false;
	}

	// functions used for determining if multiple ids should be combined into the same on screen card that turns
	// into a different id when pressed
	public virtual bool HasPressedState(int id) {
		return false;
	}

	public virtual bool IsPressedState(int id) {
		return false;
	}

	public virtual bool IsButton(int id) {
		return false;
	}

	public List<int> GetButtonIds() {
		List<int> buttonIds = new List<int> ();
		int numCards = GetCount ();
		for (int i = 0; i < numCards; i++) {
			int id = GetId (i);
			if (IsButton (id)) {
				buttonIds.Add (id);
			}
		}
		return buttonIds;
	}

	public virtual int GetIdForPressedState(int id) {
		return -1;
	}

	// get the id to use for calculating unique ids from the one passed in from vision
	// usually this is the same but will need to override for cases when vision passes combined data back
	public virtual int GetIdFromTangibleId(int tangibleId) {
		return tangibleId;
	}

	public virtual int GetDefaultRotationForId(int id) {
		return 0;
	}

	// by default just using all the ids, but sometimes we want to customize a bit
	public virtual List<int> GetIdsForOnScreenCards() {
		List<int> ids = new List<int> ();
		int numCards = GetCount ();
		for (int i = 0; i < numCards; i++) {
			ids.Add (GetId (i));
		}
		return ids;
	}

	public virtual Vector2 GetOffsetFromVisionPos(int id) {
		return Vector2.zero;
	}

	public virtual string GetStackKey(int id) {
		return id.ToString();
	}
		
	public List<int> GetIdsForStackKey(string stackKey) {
		List<int> ids = new List<int> ();
		int numCards = GetCount ();
		int id;
		for (int i = 0; i < numCards; i++) {
			id = GetId (i);
			if (GetStackKey (id) == stackKey) {
				ids.Add (id);
			}
		}
		return ids;
	}
}