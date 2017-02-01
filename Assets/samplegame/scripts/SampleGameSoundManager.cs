using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;

public class SampleGameSoundManager : BaseSoundManager {

	public static SampleGameSoundManager instance = null;

	// sounds needed always
	public static readonly string BUTTON_CLICK_NAME = "button_click_01";
	public static readonly string ACHIEVEMENT_NAME = "achievement_made";

	// level sounds
	public static readonly string LEVEL_COMPLETE_BUTTONS_NAME = "button_replay_continue";
	public static readonly string SCORE_INCREASING_NAME = "score_increase_04";

	// menu sounds
	public static readonly string LEVEL_UNLOCK_NAME = "level_unlocked";

	// music
	static readonly string MUSIC_MENU_NAME = "mus_main_menu";
	static readonly string MUSIC_GAME_NAME = "mus_ambient_loop_forest";

	// background music, loaded during initialization for now, they're set to streaming and we have a lot of references
	// to the clips themselves at the moment that we'd have to make more resilient if we wanted to load dynamically
	AudioClip music_menu;
	AudioClip music_game;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}
	}

	override protected void LoadData() {
		base.LoadData ();

		SetMusicMute(!Game.enableBackgroundMusic);

		music_menu = (AudioClip)Resources.Load (asset_loader_.sound_base_path + MUSIC_MENU_NAME, typeof(AudioClip));
		music_game = (AudioClip)Resources.Load (asset_loader_.sound_base_path + MUSIC_GAME_NAME, typeof(AudioClip));
	}



	// cache any files specific to the level or mode you're in
	// right now passing in state name and level name to use to determine what to preload
	// could change to pass in game specific enum
	public void PreloadSoundEffects(string stateName, string levelName = null) {
		if (stateName == PlayState.STATE_NAME) {
			CacheFile(LEVEL_COMPLETE_BUTTONS_NAME);
			CacheFile(SCORE_INCREASING_NAME);
		}
		if (stateName == LevelSelectState.STATE_NAME) {
			CacheFile(LEVEL_UNLOCK_NAME);
		}

		// cache always
		CacheFile(BUTTON_CLICK_NAME);
		CacheFile(ACHIEVEMENT_NAME);

	}

	public void SelectAndPlayBackgroundMusic(string stateName, string levelName = null) {
		AudioClip clip = music_menu;
		if (stateName == PlayState.STATE_NAME) {
			clip = music_game;
		}
		PlayBackgroundMusic (clip);
	}

	public void OnButtonClick() {
		PlaySoundFile(BUTTON_CLICK_NAME);
	}
}
