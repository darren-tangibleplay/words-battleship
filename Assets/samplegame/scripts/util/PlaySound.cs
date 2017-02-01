using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

	public void PlaySoundFile(string fileName) {
		SampleGameSoundManager.instance.PlaySoundFile (fileName);
	}
}
