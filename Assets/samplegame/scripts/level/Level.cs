// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Tangible;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class Level : MonoBehaviour {
// 	private const float TIME_TO_HINT = 3f;
// 	private const float EXTRA_TIME_PER_HINT_SEEN = 3f;
// 	private const float MULTIPLIER_PER_HINT_SEEN = 1.1f;
//
// 	public class Hint {
// 		public readonly List<Sprite> pieces = new List<Sprite>();
//
// 		public Hint(IEnumerable<Sprite> _pieces) {
// 			pieces.AddRange(_pieces);
// 		}
// 	}
//
// 	private float initial_camera_size_;
// 	private Vector3 initial_camera_pos_;
// 	private float initial_time_scale_;
// 	private float initial_fixed_delta_time_;
// 	private bool level_ended_ = false;
// 	private int pause_count_ = 0;
//
//     private LevelDefinition definition_;
//     public LevelDefinition Definition {
//         get { return definition_; }
//     }
//
//     private LevelSave save_;
//     public LevelSave Save {
//         get { return save_; }
//     }
//
//     public string LevelName {
//         get { return definition_.Name; }
//     }
//
// 	public LevelVisionInput VisionDisplay {
// 		get { return vision_display_; }
// 	}
//
// 	[SerializeField]
// 	private GameObject ui_prefab_;
//
// 	private DateTime last_move_time_;
// 	private int num_hints_;
// 	private int num_moves_;
// 	public bool have_taken_turn {
// 		get { return num_moves_ > 0; }
// 	}
//
// 	public bool level_ended {
// 		get { return level_ended_; }
// 	}
//
// 	public Transform LevelUIRoot { get; private set; }
// 	public Transform LevelHigherUIRoot { get; private set; }
//
// 	protected LevelUI level_ui_;
// 	private Score score_;
// 	private List<GoTweenFlow> child_score_flows_ = new List<GoTweenFlow> ();
// 	private GoTweenFlow score_increment_flow_;
// 	private AudioSource score_increment_sound_;
// 	private Hint hint_;
//
//     public void Init(LevelDefinition definition, LevelSave save) {
//         definition_ = definition;
//         save_ = save;
//     }
//
// 	void Start() {
//         Debug.LogError("vision_display_prefab_: " + vision_display_prefab_);
// 		vision_display_ = GameObject.Instantiate(vision_display_prefab_);
// 		vision_display_.transform.parent = transform;
// 		vision_display_.transform.localPosition = new Vector3(0, 300, 0);
//
// 		initial_camera_size_ = Camera.main.orthographicSize;
// 		initial_camera_pos_ = Camera.main.transform.position;
//
// 		initial_time_scale_ = Time.timeScale;
// 		initial_fixed_delta_time_ = Time.fixedDeltaTime;
//
// 		Setup(0);
// 	}
//
// 	public void OnTakenTurn() {
// 		DateTime curTime = DateTime.UtcNow;
// 		double duration = (curTime - last_move_time_).TotalSeconds;
// 		last_move_time_ = curTime;
// 		TrackingUtil.OnTookMove(num_moves_ == 0, duration);
// 		num_moves_++;
// 		hint_ = null;
// 	}
//
// 	public void OnTurnComplete() {
// 	}
//
// 	public bool ShouldShowHint() {
// 		if (level_ended_) {
// 			return false;
// 		}
//
// 		float timeTilMoveHint = TIME_TO_HINT + num_hints_ * EXTRA_TIME_PER_HINT_SEEN;
// 		for (int i=0; i<num_hints_; i++) {
// 			timeTilMoveHint *= MULTIPLIER_PER_HINT_SEEN;
// 		}
// 		DateTime curTime = DateTime.UtcNow;
// 		double lastMoveDuration = (curTime - last_move_time_).TotalSeconds;
// 		if (lastMoveDuration > timeTilMoveHint) {
// 			return true;
// 		}
// 		return false;
// 	}
//
// 	public Hint GetHint() {
// 		if (hint_ != null && hint_.pieces.Count > 0) {
// 			return hint_;
// 		}
//
// 		hint_ = GetNewHint ();
//
// 		// we're showing a new hint every time we calculate valid hint values and
// 		// didn't already have stored the hint we were currently showing
// 		if (hint_ != null) {
// 			num_hints_++;
// 		}
//
// 		return hint_;
// 	}
//
// 	// override to provide game specific hint
// 	virtual protected Hint GetNewHint() {
// 		return null;
// 	}
//
// 	bool IsPaused() {
// 		return pause_count_ > 0;
// 	}
//
// 	public void SetPause(bool pause, bool force = false) {
// 		if (level_ended_) return;
// 		if (pause) {
// 			pause_count_++;
// 		} else {
// 			pause_count_--;
// 		}
//
// 		bool paused = IsPaused ();
//
// 		if (VisionDisplay != null) {
// 			VisionDisplay.SetPause(paused);
// 		}
//
// 		SetPauseOnGameElements (paused);
//
// 		if (score_increment_flow_ != null && score_increment_flow_.state != GoTweenState.Complete &&
// 		    score_increment_flow_.state != GoTweenState.Destroyed) {
// 			if (paused) {
// 				score_increment_flow_.pause();
// 				if (score_increment_sound_ != null) {
// 					score_increment_sound_.Pause();
// 				}
// 			} else {
// 				if (score_increment_sound_ != null) {
// 					score_increment_sound_.UnPause();
// 				}
// 				score_increment_flow_.play();
// 			}
// 		}
// 	}
//
// 	virtual protected void SetPauseOnGameElements(bool paused) {
// 	}
//
// 	void Update() {
//
// 	}
//
//     void OnDestroy() {
// 		CleanupFlows ();
// 		CleanupUIObjects ();
//     }
//
// 	virtual protected void Setup(float delay) {
// 		CleanupUIObjects ();
// 		GameObject uiRootObj = new GameObject ();
// 		uiRootObj.name = "LevelUIRoot";
// 		LevelUIRoot = uiRootObj.transform;
// 		LevelUIRoot.SetParent (SampleGameUIManager.instance.gameRoot, false);
// 		uiRootObj = new GameObject ();
// 		uiRootObj.name = "LevelHigherUIRoot";
// 		LevelHigherUIRoot = uiRootObj.transform;
// 		LevelHigherUIRoot.SetParent (SampleGameUIManager.instance.higherGameRoot, false);
//
// 		level_ui_ = (Instantiate(ui_prefab_) as GameObject).GetComponent<LevelUI>();
// 		level_ui_.transform.SetParent (LevelUIRoot, false);
// 		level_ui_.Init(Exit, Replay);
// 		score_ = level_ui_.Score;
// 		score_.Init (save_.Highscore);
//
// 		level_ended_ = false;
// 		num_moves_ = 0;
// 		num_hints_ = 0;
// 		pause_count_ = 0;
// 		vision_display_.Enable = true;
//
// 		SampleGameSoundManager.instance.SelectAndPlayBackgroundMusic (PlayState.STATE_NAME, LevelName);
//
// 		StartLevel (delay);
// 	}
//
// 	virtual protected void StartLevel(float delay) {
// 		string welcomeText = string.Format (Language.Get ("LEVEL_ENTER"), definition_.GetDisplayName());
// 		DialogInfo info = new DialogInfo("prefabs/WelcomeDialog", Language.Get("TITLE_WELCOME"), welcomeText, 3.0f, true, () => StartCoroutine(BuildLevel (delay)), null);
// 		SampleGameUIManager.instance.ShowDialog(info);
// 	}
//
//     public void DebugSolve() {
// 		CelebrateEndLevel ();
//     }
//
// 	private IEnumerator BuildLevel(float delay) {
// 		yield return new WaitForSeconds (delay);
//
// 		last_move_time_ = DateTime.UtcNow;
// 		vision_display_.LevelStart(this);
// 	}
//
// 	private void CelebrateEndLevel() {
// 		if (level_ended_) return;
//
// 		level_ended_ = true;
//
// 		StartCoroutine(CelebrateEndLevelInternal());
// 	}
//
//
//     private IEnumerator CelebrateEndLevelInternal() {
//
// 		vision_display_.LevelEnd();
//
// 		// do any end level animations here instead of this placeholder delay to simulate the flow
// 		yield return new WaitForSeconds(0.1f);
//
// 		ShowLevelComplete ();
//     }
//
// 	int ComputeScore() {
// 		// if anything gets computed based on state at end of game it would be done here
// 		return score_.ImmediateValue;
// 	}
//
// 	virtual protected void ShowLevelComplete() {
// 		int score = ComputeScore();
// 		int previousHighScore = save_.Highscore;
// 		bool solvedBefore = save_.Solved;
// 		bool is_high_score = score > previousHighScore;
//
// 		Game.LevelManager.SetScore(LevelName, score);
// 		SampleGamePlayerState.stateDirty = true;
//
// 		Game.LevelManager.UnlockNextLevel(LevelName, true);
// 		TrackingUtil.OnEndLevel (definition_, true, score, previousHighScore, solvedBefore);
//
// 		Game.LevelManager.SetAchievementProgressFromSaveData();
//
// 		if (score > 0) {
// 			AchievementManager.instance.SetAchievement("high_score", score);
// 		}
//
// 		if (level_ui_ != null) {
// 			level_ui_.OnLevelComplete (is_high_score);
// 		}
// 	}
//
//
// 	// make sure everything created for the level and parented to the UI root
// 	// gets cleaned up
// 	private void CleanupUIObjects() {
// 		if (LevelUIRoot != null) {
// 			Destroy (LevelUIRoot.gameObject);
// 			LevelUIRoot = null;
// 		}
// 		if (LevelHigherUIRoot != null) {
// 			Destroy (LevelHigherUIRoot.gameObject);
// 			LevelHigherUIRoot = null;
// 		}
// 	}
//
// 	virtual protected void CleanupFlows() {
// 		// clean up any go tween flows here
// 		foreach (GoTweenFlow flow in child_score_flows_) {
// 			if (flow != null) {
// 				flow.destroy();
// 			}
// 		}
// 		child_score_flows_.Clear ();
// 	}
//
// 	public void HideDialogs() {
// 		// hide any dialog types here that could have been shown during the level
// 	}
//
// 	public void Cleanup() {
// 		CleanupFlows ();
//
// 		if (!level_ended_) {
// 			TrackingUtil.OnEndLevel (definition_, false, 0, save_.Highscore, save_.Solved);
// 		}
//
//         level_ended_ = true;
//
// 		CleanupUIObjects ();
//
// 		HideDialogs ();
//
// 		// make sure all looping sounds are stopped so nothing sticks around if it's stop call
// 		// didn't happen
// 		SampleGameSoundManager.instance.ClearLoopingSounds ();
//
//         // Repair anything that is global and may have changed
//         Camera.main.orthographicSize = initial_camera_size_;
//         Camera.main.transform.position = initial_camera_pos_;
//         Time.timeScale = initial_time_scale_;
//         Time.fixedDeltaTime = initial_fixed_delta_time_;
//     }
//
//     private void Exit() {
// 		TrackingUtil.OnChoseContinue ();
//         Cleanup();
//
//         Game.singleton.GoToMainMenuState(LevelName);
//     }
//
//     public void Replay() {
// 		TrackingUtil.OnChoseReplay ();
// 		HandleRestart ();
//     }
//
// 	void HandleRestart() {
// 		Game.LevelManager.LevelNameToStart = LevelName;
// 		Game.LevelManager.StartLevel(true);
// 		Game.singleton.OnLevelLoaded();
// 	}
//
// 	public void Restart() {
// 		TrackingUtil.OnLevelRestart ();
// 		HandleRestart ();
// 	}
//
// 	private void HandleScoreIncrement(int points) {
// 		if (score_increment_flow_ != null) {
// 			score_increment_flow_.destroy();
// 		}
//
// 		score_increment_flow_ = new GoTweenFlow();
// 		score_increment_flow_.autoRemoveOnComplete = true;
//
// 		if (score_increment_sound_ == null) {
// 			SampleGameSoundManager.instance.RemoveFadesForClipWithName(SampleGameSoundManager.SCORE_INCREASING_NAME);
// 			score_increment_sound_ = SampleGameSoundManager.instance.StartLoopingFile(SampleGameSoundManager.SCORE_INCREASING_NAME);
// 			if (IsPaused()) {
// 				score_increment_sound_.Pause();
// 			}
// 		}
//
// 		int score = score_.ImmediateValue; // need a local variable for closure
// 		score_increment_flow_.insert(0, Go.to(score_.transform, 0.25f, new GoTweenConfig().scale(1.25f).setEaseType(GoEaseType.QuadOut)));
// 		score_increment_flow_.insert(0, Go.to(score_, 0.8f, new GoTweenConfig().intProp("DisplayValue", score)));
// 		score_increment_flow_.insert(1.0f, Go.to(score_.transform, 0.75f, new GoTweenConfig().scale(1.0f).setEaseType(GoEaseType.BounceOut)));
//
// 		score_increment_flow_.insert(0, new GoTween(this, 0.75f, new GoTweenConfig().onComplete(tween => OnScoreIncreased ())));
// 		if (!IsPaused ()) {
// 			score_increment_flow_.play ();
// 		}
// 	}
//
// 	void OnScoreIncreased() {
// 		SampleGameSoundManager.instance.StopLooping (score_increment_sound_, true);
// 		score_increment_sound_ = null;
// 	}
//
// 	private void CreateTextDisplayBeginning(Transform anchor, string t, FloatyText floaty_text, FloatyText.Behavior behavior = FloatyText.Behavior.FOLLOW) {
// 		floaty_text.Init(anchor, t, behavior);
// 	}
//
// 	private void OnAnimateToScoreDisplayFinished(FloatyText float_text) {
// 		if (float_text != null) float_text.SelfDestroy();
// 	}
//
// 	// used for scoring objects during play
// 	public void CreateChildScore(Transform source_obj, int value, Vector3 override_pos = default(Vector3)) {
// 		if (score_ == null) {
// 			return;
// 		}
//
// 		score_.ImmediateValue += value;
//
// 		FloatyText points_text = FloatyText.Create(source_obj, "", FloatyText.Behavior.FOLLOW, override_pos, false);
// 		GoTweenFlow flow = new GoTweenFlow();
// 		flow.autoRemoveOnComplete = true;
//
// 		float delay = 0.001f;
// 		flow.insert(0, new GoTween(this, delay, new GoTweenConfig().onComplete(thisTween => CreateTextDisplayBeginning(source_obj, value.ToString(), points_text))));
//
// 		// go to score display
// 		flow.insert(delay, Go.to(points_text, 0.5f, new GoTweenConfig().floatProp("Alpha", 1.0f).setEaseType(GoEaseType.QuadOut)));
// 		flow.insert(delay, Go.to(points_text, 0.5f, new GoTweenConfig().onComplete(thisTween => points_text.Unfollow())));
// 		flow.insert(delay + 0.5f, Go.to(points_text.transform, 0.5f, new GoTweenConfig().position(score_.FloatyTextDestination).setEaseType(GoEaseType.QuintIn)));
//
// 		flow.insert(delay + 0.75f, Go.to(points_text, 0.25f, new GoTweenConfig().floatProp("Alpha", 0.0f).
// 		                                 setEaseType(GoEaseType.QuintIn).onComplete(thisTween => OnAnimateToScoreDisplayFinished(points_text))));
//
// 		delay += 0.85f;
//
// 		flow.insert(0, new GoTween(this, delay, new GoTweenConfig().onComplete(thisTween => HandleScoreIncrement(value))));
//
// 		child_score_flows_.Add (flow);
// 		flow.setOnCompleteHandler(tween => OnCreateChildScoreCompleted(flow, points_text));
// 		flow.play();
// 	}
//
// 	private void OnCreateChildScoreCompleted(GoTweenFlow flow, FloatyText points_text) {
// 		child_score_flows_.Remove (flow);
// 		Destroy(points_text.gameObject);
// 	}
//
// 	protected void Log(string msg) {
// #if LOG_LEVEL
// 		Debug.Log(msg);
// #endif
// 	}
// }
