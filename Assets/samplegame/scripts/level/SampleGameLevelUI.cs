// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.UI;
//
// public class SampleGameLevelUI : LevelUI {
//
// 	[SerializeField]
// 	private Button scoreCheatButton_;
// 	public Button scoreCheatButton { get { return scoreCheatButton_; } }
//
// 	[SerializeField]
// 	private Button winCheatButton_;
// 	public Button winCheatButton { get { return winCheatButton_; } }
//
//
// 	public override void Init(OnContinue on_continue, OnReplay on_replay) {
// 		base.Init(on_continue, on_replay);
//
// 		if (!definition_.UsesScore) {
// 			if (scoreCheatButton_ != null) {
// 				scoreCheatButton_.gameObject.SetActive (false);
// 			}
// 			if (winCheatButton_ != null) {
// 				winCheatButton_.gameObject.SetActive (false);
// 			}
// 		}
// 	}
//
// 	override public void OnLevelComplete(bool is_highscore) {
// 		if (scoreCheatButton_ != null) {
// 			scoreCheatButton_.gameObject.SetActive (false);
// 		}
// 		if (winCheatButton != null) {
// 			winCheatButton_.gameObject.SetActive (false);
// 		}
//
// 		base.OnLevelComplete (is_highscore);
//     }
// }
