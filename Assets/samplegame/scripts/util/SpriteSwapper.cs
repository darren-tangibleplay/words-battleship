using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpriteSwapper : MonoBehaviour {
	[SerializeField]
	Sprite[] _spriteOptions;

	[SerializeField]
	string[] _soundOptions;

	[SerializeField]
	SpriteRenderer _renderer;


	[SerializeField]
	Button _btn;

	int _curIndex;

	void Awake() {
		_curIndex = 0;
		_renderer.sprite = _spriteOptions [_curIndex];
		_btn.onClick.AddListener (ToggleSprite);
	}
		
	void ToggleSprite () {
		_curIndex = (_curIndex + 1) % _spriteOptions.Length;
		_renderer.sprite = _spriteOptions [_curIndex];

		if (_soundOptions.Length > _curIndex && _soundOptions [_curIndex] != "") {
			SampleGameSoundManager.instance.PlaySoundFile (_soundOptions [_curIndex]);
		}
	}
}

