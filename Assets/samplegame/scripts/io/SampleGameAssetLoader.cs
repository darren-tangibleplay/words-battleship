using UnityEngine;

public class SampleGameAssetLoader : GameAssetLoader {

	public static SampleGameAssetLoader instance { private set; get; }
	
	private void Awake() {
		instance = this;
	}
	
	private void OnDestroy() {
		if (instance == this) {
			instance = null;
		}
	}

	public SampleGameAssetLoader ()
	{
	}
}



