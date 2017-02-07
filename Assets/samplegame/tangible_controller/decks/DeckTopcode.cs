// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
//
// public class DeckTopcode : Deck {
// 	[SerializeField]
// 	float _mmToScreen;
//
// 	[SerializeField]
// 	OnScreenObject _cardPrefab;
//
// 	[SerializeField]
// 	Sprite _defaultImage;
//
//
// 	// not a great way of setting these, but don't want to put this in resources folder to get loaded all the time
// 	[SerializeField]
// 	List<int> _overrideIds = new List<int> ();
//
// 	[SerializeField]
// 	List<Sprite> _overrideImages = new List<Sprite>();
//
// 	Dictionary<int, Sprite> _imageMap = new Dictionary<int, Sprite>();
//
// 	private TopcodeIdConfig idConfig;
//
// 	virtual protected void Awake() {
// 		idConfig = gameObject.GetComponent<TopcodeIdConfig> ();
// 		if (idConfig == null) {
// 			Debug.LogError ("IdConfig component must be set on DeckTopcode");
// 		}
//
// 		int numOverrides = _overrideIds.Count;
// 		for (int i = 0; i < numOverrides; i++) {
// 			_imageMap [_overrideIds[i]] = _overrideImages[i];
// 		}
// 	}
//
// 	override public float GetWidthMillimeters(int id) {
// 		return idConfig.GetWidthMmForId (id);
// 	}
//
// 	override public float GetHeightMillimeters(int id) {
// 		return idConfig.GetHeightMmForId (id);
// 	}
//
// 	// if we're overriding the image, assume the image is at the size we want already and
// 	// keep the on screen card at the default scale
// 	public override float GetCardDisplayScaleX(int id) {
// 		if (_imageMap.ContainsKey (id)) {
// 			return _cardPrefab.transform.localScale.x;
// 		}
// 		return GetWidthScreen (id);
// 	}
//
// 	public override float GetCardDisplayScaleY(int id) {
// 		if (_imageMap.ContainsKey (id)) {
// 			return _cardPrefab.transform.localScale.y;
// 		}
// 		return GetHeightScreen (id);
// 	}
//
// 	override public float GetMillimeterToScreen() {
// 		return _mmToScreen;
// 	}
//
// 	override public OnScreenObject GetPrefab() {
// 		return _cardPrefab;
// 	}
//
// 	override public TangibleObject.Shape GetShape(int index) {
// 		return TangibleObject.Shape.card;
// 	}
//
// 	override public void AssignGraphics(int index, GameObject obj) {
// 		int id = GetId (index);
// 		TextMesh text = obj.GetComponentInChildren<TextMesh>();
// 		SpriteRenderer renderer = obj.GetComponentInChildren<SpriteRenderer> ();
// 		if (_imageMap.ContainsKey (id)) {
// 			text.text = "";
// 			renderer.sprite = _imageMap [id];
// 		} else {
// 			text.text = idConfig.GetTextForId(id);
// 			renderer.sprite = _defaultImage;
// 		}
// 		renderer.color = idConfig.GetColorForId (id);
//     }
//
// 	public override bool IsButton(int id) {
// 		return idConfig.IsButton (id);
// 	}
//
// 	public override int GetId(int index) {
// 		return idConfig.GetIdAtIndex (index);
// 	}
//
// 	override public int GetIndex(int id) {
// 		return idConfig.GetIndexForId (id);
// 	}
//
// 	override public IdConfig GetIdConfig() {
// 		return idConfig;
// 	}
//
// 	override public Tangible.Config.RecognitionMode GetRecognitionMode() {
// 		return Tangible.Config.RecognitionMode.TOPCODE;
// 	}
//
// 	public override Vector2 GetOffsetFromVisionPos(int id) {
// 		TopcodeIdConfig.TopcodeInfo info = idConfig.GetInfoForId (id);
// 		if (info != null) {
// 			return new Vector2 (-1f * info.TopcodeXOffset, -1f * info.TopcodeYOffset);
// 		}
// 		return Vector2.zero;
// 	}
// }
