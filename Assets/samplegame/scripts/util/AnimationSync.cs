using UnityEngine;
using System.Collections;
using System;

public class AnimationSync : MonoBehaviour {
	[SerializeField]
	private float _animationTime = 0f;

	[SerializeField]
	private string _animationName = "";

	[SerializeField]
	private Animator _anim;

	void Start() {
		_anim.enabled = false;
	}

	void Update() {
		if (_anim.enabled) {
			return;
		}
		long elapsedSeconds = DateTime.UtcNow.Second * 60 + (long)(DateTime.UtcNow.Millisecond / 16.66667);
//		Debug.Log ("seconds: " + elapsedSeconds.ToString ());
		if (elapsedSeconds % _animationTime == 0) {
			if (_animationName != "") {
				_anim.enabled = true;
				_anim.Play (_animationName);
			}
		}
	}
}
