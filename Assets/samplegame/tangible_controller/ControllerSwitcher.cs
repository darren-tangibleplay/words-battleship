using UnityEngine;
using System.Collections;

public class ControllerSwitcher : MonoBehaviour {

	void Start() {
	}

	void OnDestroy() {
		DestroyImmediate(GetComponent<Renderer>().material);
	}

	void OnMouseUpAsButton() {
		GetComponent<Renderer>().material.SetFloat("_Alpha", 0.25f);
		StartCoroutine(WaitAndEnable());
    }
	
	public IEnumerator WaitAndEnable() {
		yield return new WaitForFixedUpdate();
		SelectorController selector = transform.GetComponent<SelectorController>();
		if (!selector.ToggleSource()) Debug.LogWarning("Could not toggle the input source");
		
		yield return new WaitForSeconds(0.25f);
		GetComponent<Renderer>().material.SetFloat("_Alpha", 1.0f);
    }
}
