using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OverlayColor : MonoBehaviour {

	private Color overlay_;

	void Start() {
		if (!GetComponent<SpriteRenderer>().material.HasProperty("_Overlay")) {
			Debug.LogError("Object '" + name + "' does not have an _Overlay propertie on its material.");
		} else {
			overlay_ = Overlay;
		}
	}
    
	public Color Overlay { 
		get { 
			overlay_ = GetComponent<SpriteRenderer>().material.GetColor("_Overlay");
			return overlay_; 
		} 
		set { GetComponent<SpriteRenderer>().material.SetColor("_Overlay", value);  }
	}
}
