// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.UI;
//
// public class LevelUI : LevelComplete {
//
//     [SerializeField]
//     private Text level_name_;
//
//     [SerializeField]
//     private Score in_game_score_;
//
//     [SerializeField]
//     private Text in_game_best_;
//
//     [SerializeField]
//     private Transform complete_score_parent_;
//
//     [SerializeField]
// 	private Text complete_score_label_;
//
// 	[SerializeField]
// 	private Text complete_score_;
//
//     [SerializeField]
//     private Text complete_best_;
//
//     private GoTweenFlow buttons_flow_;
// 	protected LevelDefinition definition_;
//
//     public Score Score {
//         get { return in_game_score_; }
//     }
//
//     public override void Init(OnContinue on_continue, OnReplay on_replay) {
//         base.Init(on_continue, on_replay);
//
// 		definition_ = Game.Level.Definition;
//         level_name_.text = definition_.GetDisplayName();
//
// 		if (complete_score_label_ != null) {
// 			complete_score_label_.text = Language.Get ("SCORE_LABEL");
// 		}
// 		if (complete_score_ != null) {
// 			complete_score_.text = "0";
// 		}
//
// 		if (replay_ != null) {
// 			replay_.gameObject.SetActive (false);
// 		}
// 		if (continue_ != null) {
// 			continue_.gameObject.SetActive (false);
// 		}
// 		if (complete_score_parent_ != null) {
// 			complete_score_parent_.gameObject.SetActive (false);
// 		}
//
// 		if (in_game_best_ != null) {
// 			int highscore = Game.Level.Save.Highscore;
// 			in_game_best_.text = string.Format (Language.Get ("BEST_SCORE"), highscore);
// 			in_game_best_.gameObject.SetActive (highscore > 0);
// 		}
//
// 		if (!definition_.UsesScore) {
// 			if (in_game_best_ != null) {
// 				in_game_best_.gameObject.SetActive (false);
// 			}
// 			if (in_game_score_ != null) {
// 				in_game_score_.gameObject.SetActive (false);
// 			}
// 		}
//
//     }
//
// 	virtual public void OnLevelComplete(bool is_highscore) {
// 		if (complete_score_parent_ != null && definition_.UsesScore) {
// 			complete_score_parent_.gameObject.SetActive (true);
// 		}
// 		if (complete_score_ != null) {
// 			complete_score_.gameObject.SetActive (false);
// 		}
//
// 		if (in_game_score_ != null && definition_.UsesScore) {
// 			in_game_score_.transform.SetParent (complete_score_parent_);
// 			in_game_score_.OnLevelComplete (complete_score_.transform.localPosition, complete_score_.fontSize);
// 		}
//
// 		if (in_game_best_ != null) {
// 			in_game_best_.gameObject.SetActive (false);
// 		}
//
// 		if (complete_best_ != null && definition_.UsesScore) {
// 			complete_best_.gameObject.SetActive (true);
// 			complete_best_.text = string.Format (Language.Get ("BEST_SCORE"), Game.Level.Save.Highscore);
// 		}
//
// 		if (level_name_ != null) {
// 			level_name_.gameObject.SetActive (true);
// 		}
//
// 		if (is_highscore && complete_best_ != null && definition_.UsesScore) {
// 			complete_best_.text = Language.Get("SCORE_LABEL_HIGH_SCORE");
// 		}
//
// 		DisplayButtons ();
//     }
//
// 	public override void DisplayButtons() {
// 		if (replay_ == null || continue_ == null) {
// 			return;
// 		}
// 		replay_.gameObject.SetActive (true);
//         continue_.gameObject.SetActive(true);
//
// 		SampleGameSoundManager.instance.QueueSoundFile(SampleGameSoundManager.LEVEL_COMPLETE_BUTTONS_NAME, 1.0f);
//
//         buttons_flow_ = new GoTweenFlow();
//         buttons_flow_.autoRemoveOnComplete = true;
//         buttons_flow_.insert(0.0f, Go.to(replay_.transform, 2f, new GoTweenConfig().position(new Vector3(-500, replay_.transform.position.y, replay_.transform.position.z)).setIsFrom().setEaseType(GoEaseType.BounceOut)));
//         buttons_flow_.insert(0.0f, Go.to(continue_.transform, 2f, new GoTweenConfig().position(new Vector3(500, continue_.transform.position.y, continue_.transform.position.z)).setIsFrom().setEaseType(GoEaseType.BounceOut)));
// 		buttons_flow_.play();
//     }
// }
