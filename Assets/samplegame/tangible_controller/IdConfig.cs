using UnityEngine;

public interface IdConfig {
	int GetNumIds();
	int GetValueForId(int id);
	Color GetColorForId(int id);
	int GetCountForId(int id);
	int GetUniqueGroupForId(int id);
}