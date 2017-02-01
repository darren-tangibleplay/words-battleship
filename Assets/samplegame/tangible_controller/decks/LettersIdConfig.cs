using UnityEngine;
using System.Collections;

public class LettersIdConfig : MonoBehaviour, IdConfig {

	public int GetNumIds() {
		return 52;
	}

	public int GetValueForId(int id) {
		return id % 26 + 1;
	}

	public Color GetColorForId(int id) {
		return Color.white;
	}

	public int GetCountForId(int id) {
		return 1;
	}

	public int GetUniqueGroupForId(int id) {
		return 0;
	}
}
