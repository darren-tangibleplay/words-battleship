using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeckProgramming : Deck {
    public Texture2D atlas;
	public float mmToScreen = 48f/25.4f;  //trying the same ratio as in Numbers: 48/25.4
	public OnScreenObject cardPrefab;

	private ProgrammingIdConfig idConfig;

	void Awake() {
		idConfig = gameObject.GetComponent<ProgrammingIdConfig> ();
		if (idConfig == null) {
			Debug.LogError ("ProgrammingIdConfig component must be set on DeckProgramming");
		}
	}

	void Start() {
		if (atlas.format != TextureFormat.RGB24) {
			Debug.LogError("Deck texture format must be RGB24");
		}
	}

	override public float GetWidthMillimeters(int id) {
		return idConfig.GetWidthMillimeters (id);
	}

	override public float GetHeightMillimeters(int id) {
		return idConfig.GetHeightMillimeters (id);
	}
		
	override public float GetMillimeterToScreen() {
		return mmToScreen;
	}
		
	override public int GetId(int index) {
		return idConfig.GetIdAtIndex (index);
	}
	
	override public int GetIndex(int id) {
		return idConfig.GetIndexForId (id);
	}

	override public OnScreenObject GetPrefab() {
		return cardPrefab;
	}
	
	override public Tangible.TangibleObject.Shape GetShape(int index) {
		return Tangible.TangibleObject.Shape.card;
	}

	void AssignTexture(Texture texture, int index, Renderer renderer) {
		int id = GetId (index);
        renderer.material.mainTexture = texture;
		renderer.material.mainTextureScale = new Vector2(idConfig.GetTextureWidthMillimeters(id), 
			idConfig.GetTextureHeightMillimeters(id));
		renderer.material.mainTextureOffset = new Vector2(idConfig.GetTextureXPosMillimeters(id), 
			idConfig.GetTextureYPosMillimeters(id));
	}

	override protected void AssignGraphicsToMeshRenderer(int index, MeshRenderer renderer) {
		AssignTexture(atlas, index, renderer);
    }

	override public IdConfig GetIdConfig() {
		return idConfig;
	}

	override public Tangible.Config.RecognitionMode GetRecognitionMode() {
		return Tangible.Config.RecognitionMode.STRAWBIES;
	}

	public string GetConfigJson() {
		return idConfig.GetConfigJson ();
	}

	override public Vector3[] GetEdgePointsMillimeters (int id) {
		return idConfig.GetEdgePoints (id);
	}

	override public Vector3 ComputeAttachPointMillimeters(int id, PairEdge edge) {
		return idConfig.ComputeAttachPointMillimeters (id, edge);
	}

	override public float GetTileZMod(int id) {
		return idConfig.GetTileZMod (id);
	}

	// in millimeters, making this pretty loose for programming because the positions coming from vision aren't
	// that precise
	public override float GetPositionErrorStrict() {
		return 15.0f;
	}

	public override float GetPositionErrorLoose() {
		return 20.0f;
	}
		
	public override bool ShouldCheckFlip(int id) {
		return idConfig.ShouldCheckFlip (id);
	}

	public override bool HasPressedState(int id) {
		return idConfig.HasPressedState (id);
	}

	public override bool IsPressedState(int id) {
		return idConfig.IsPressedState (id);
	}

	public override int GetIdForPressedState(int id) {
		return idConfig.GetIdForPressedState (id);
	}

	public override int GetIdFromTangibleId(int tangibleId) {
		return idConfig.GetIdFromTangibleId (tangibleId);
	}

	public override int GetDefaultRotationForId(int id) {
		return idConfig.GetRotationForId(id);
	}
		
	// TODO: for now just putting in a few combined ids to be able to test with the new combined ids when needed
	// should make this more integrated into the real editor flow so our normal testing is closer to what's happening
	// when on device

	#if TEST_COMBINEDIDS
	public override List<int> GetIdsForOnScreenCards() {
		List<int> ids = new List<int> ();
		ids.Add (212);
		ids.Add (335);
		ids.Add (600);
		ids.Add (500);
		ids.Add (760);
		ids.Add (750);
		return ids;
	}
	#endif



}
