using UnityEngine;

public interface Deck {
	// The number of indices in the deck
	int GetCount();
	float GetSizeMillimeter();
	bool ContainsId(int id);
	void AssignGraphics(int index, MeshRenderer renderer);
	OnScreenObject GetPrefab();
}
