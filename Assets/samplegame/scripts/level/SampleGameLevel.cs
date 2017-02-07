// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System;
// using UnityEngine.UI;
// using Tangible;
//
// public class SampleGameLevel : Level {
//
// 	SampleGameLevelUI sample_game_ui_;
//
// 	override protected void Setup(float delay) {
// 		base.Setup (delay);
//
// 		sample_game_ui_ = level_ui_ as SampleGameLevelUI;
// 		sample_game_ui_.scoreCheatButton.onClick.AddListener(AddToScoreCheat);
// 		sample_game_ui_.winCheatButton.onClick.AddListener(WinCheat);
// 	}
//
// 	override protected Hint GetNewHint() {
// 		// showing random sprites as a hint for sample game
// 		List<Sprite> pieces = new List<Sprite> ();
// 		pieces.Add (SampleGameAssetLoader.instance.LoadImageAtPath("cards/numbers_card_" + UnityEngine.Random.Range(1, 10)));
// 		pieces.Add (SampleGameAssetLoader.instance.LoadImageAtPath("cards/numbers_card_" + UnityEngine.Random.Range(1, 10)));
// 		return new Hint (pieces);
// 	}
//
// 	void AddToScoreCheat() {
// 		CreateChildScore (null, UnityEngine.Random.Range (1, 1000));
// 		OnTakenTurn ();
// 	}
//
// 	void WinCheat() {
// 		DebugSolve ();
// 	}
// }
