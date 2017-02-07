using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCard : MonoBehaviour, Deck {
	public static Dictionary<string, int> TEAMNAMES = new Dictionary<string, int>() {
		{ "blue", 0 }, { "red", 1 }
	};

	public static Dictionary<int, string> TEAMCOLORS = new Dictionary<int, string>() {
		{ 0, "blue" }, { 1, "red" }
	};

	public int widthSubdivision;
    public int heightSubdivision;
    public Texture2D atlas;
	public OnScreenObject cardPrefab;

	private int cardCount;

	private Dictionary<int, Dictionary<char, int>> teamLetterToIndex;

	void Awake() {
		if (atlas.format != TextureFormat.RGB24) {
			Debug.LogError("Deck texture format must be RGBA32");
		}

		teamLetterToIndex = new Dictionary<int, Dictionary<char, int>>();
		foreach(string team in TEAMNAMES.Keys) {
			int player = TEAMNAMES[team];
			Dictionary<char, int> lettersToIndices = new Dictionary<char, int>();

			for(char letter = 'A'; letter <= 'Z'; ++letter) {
				lettersToIndices.Add(letter, TangibleObject.ToIndex(letter, player));
				cardCount++;
			}

			for(char letter = 'a'; letter <= 'z'; ++letter) {
				lettersToIndices.Add(letter, TangibleObject.ToIndex(letter, player));
				cardCount++;
			}

			teamLetterToIndex.Add(TEAMNAMES[team], lettersToIndices);
		}
	}

	// ----------------------------- interface Deck

	public bool Contains(TangibleObject obj) {
		return TEAMNAMES.ContainsKey(obj.Color) && teamLetterToIndex[TEAMNAMES[obj.Color]].ContainsKey(obj.letter);
	}

	public int GetCount() {
		return cardCount;
	}

	public float GetSizeMillimeter() {
		return Tangible.EventHelper.cardSize;
	}

	public OnScreenObject GetPrefab() {
		return cardPrefab;
	}

	public void AssignGraphics(int index, MeshRenderer renderer) {
		AssignTexture(atlas, widthSubdivision, heightSubdivision, index, renderer);
	}

	public bool ContainsId(int id) {
		char letter = TangibleObject.LetterFromId(id);
		int player = TangibleObject.PlayerFromId(id);
		return teamLetterToIndex.ContainsKey(player) && teamLetterToIndex[player].ContainsKey(letter);
	}

	// -------------------------------

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
}
