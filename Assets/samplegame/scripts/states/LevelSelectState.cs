// ﻿using UnityEngine;
// using System;
//
// public class LevelSelectState : GameState {
// 	public const string STATE_NAME = "level_select";
//
// 	GameObject level_select_scroll_panel_;
// 	LevelSelectScreen level_select_screen_;
//
// 	public LevelSelectState(Action complete, string level_name) : base(STATE_NAME, false) {
// 		level_select_scroll_panel_ = GameObject.Instantiate(Resources.Load("prefabs/LevelSelectScrollPanel")) as GameObject;
// 		level_select_scroll_panel_.GetComponent<RectTransform>().SetParent(SampleGameUIManager.instance.gameRoot, false);
//
// 		level_select_screen_ = level_select_scroll_panel_.transform.Find("ScrollContent/LevelSelectScreen").GetComponent<LevelSelectScreen>();
// 		level_select_screen_.Init(complete, level_name);
//
// 		level_select_scroll_panel_.transform.SetSiblingIndex(0);
// 	}
//
// 	override public void OnPush() {
// 		SampleGameSoundManager.instance.SelectAndPlayBackgroundMusic (STATE_NAME);
// 		SampleGameSoundManager.instance.PreloadSoundEffects(STATE_NAME);
//
// 		base.OnPush();
// 	}
//
// 	override public void OnPop() {
// 		base.OnPop();
// 		level_select_screen_.Cleanup();
// 		GameObject.Destroy(level_select_scroll_panel_);
// 	}
//
// 	override public void OnEnter() {
// 		base.OnEnter();
// 	}
//
// 	override public void OnExit() {
// 		base.OnExit();
// 	}
//
//     public void DebugSolve() {
// 		level_select_screen_.DebugSolve();
//     }
//
// }
