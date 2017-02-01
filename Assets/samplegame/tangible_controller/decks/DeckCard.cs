using UnityEngine;
using System.Collections;

public class DeckCard : Deck {
    public int widthSubdivision;
    public int heightSubdivision;
    public Texture2D atlas;
	public float sizeScreen;
	private float sizeMm = 25.4f;
	public Tangible.Config.RecognitionMode recognitionMode;
	public OnScreenObject cardPrefab;

	private IdConfig idConfig;

	virtual protected void Awake() {
		idConfig = gameObject.GetComponent<IdConfig> ();
		if (idConfig == null) {
			Debug.LogError ("IdConfig component must be set on DeckCard");
		}
	}

	void Start() {
		if (atlas.format != TextureFormat.RGB24) {
			Debug.LogError("Deck texture format must be RGB24");
		}
		if (recognitionMode != Tangible.Config.RecognitionMode.LETTER_TILES && recognitionMode != Tangible.Config.RecognitionMode.MATH_MANIPULATIVES &&
			recognitionMode != Tangible.Config.RecognitionMode.IMAGECARD) {
			Debug.LogError ("Unsupported recognition mode set for DeckCard " + recognitionMode);
		}
	}

	override public float GetWidthMillimeters(int id) {
		return sizeMm;
	}

	override public float GetHeightMillimeters(int id) {
		return sizeMm;
	}

	override public float GetMillimeterToScreen() {
		return sizeScreen /sizeMm;
	}

	override public OnScreenObject GetPrefab() {
		return cardPrefab;
	}
	
	override public Tangible.TangibleObject.Shape GetShape(int index) {
		return Tangible.TangibleObject.Shape.card;
	}

	// The texture is assumed to have the first card at the top left,
	// the second just below, etc...
	public static void AssignTexture(Texture texture, int subX, int subY, int index, Renderer renderer) {
		int i = 0;
		int j = 0;
		float stepX = 0;
		float stepY = 0;
		if (index >= 0) {
			stepX = 1.0f / subX;
        	stepY = 1.0f / subY;
        	i = index % subX;
			j = index / subX;
		}
		
        renderer.material.mainTexture = texture;
        renderer.material.mainTextureScale = new Vector2(stepX, stepY);
        renderer.material.mainTextureOffset = new Vector2(i * stepX, 1.0f - ((j + 1) * stepY));
	}

	override protected void AssignGraphicsToMeshRenderer(int index, MeshRenderer renderer) {
		AssignTexture(atlas, widthSubdivision, heightSubdivision, index, renderer);
    }

	override public IdConfig GetIdConfig() {
		return idConfig;
	}

	override public Tangible.Config.RecognitionMode GetRecognitionMode() {
		return recognitionMode;
	}
}
