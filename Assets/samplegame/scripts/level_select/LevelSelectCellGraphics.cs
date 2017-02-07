// using UnityEngine;
// using System.Collections;
// using UnityEngine.UI;
//
// public class LevelSelectCellGraphics : MonoBehaviour {
//
// 	public enum State {
// 		LOCKED,
// 		UNLOCKED,
// 		SOLVED
// 	};
//
//     [SerializeField]
//     Text level_name_text_;
//
//     [SerializeField]
//     Text highscore_text_;
//
//     [SerializeField]
//     Image lock_;
//
//     [SerializeField]
//     Image solved_;
//
// 	[SerializeField]
// 	Image bg_;
//
// 	[SerializeField]
//     Color locked_color_ = new Color(1, 1, 1, 0.25f);
//
//     [SerializeField]
//     Color active_color_ = new Color(1, 1, 1, 1);
//
//     private bool selected_;
//     public bool Selected {
//         get { return selected_; }
//     }
//
// 	State state_ = State.LOCKED;
//
// 	Vector3 initialScale_;
//     SharedButton.OnButtonEvent on_click_;
// 	LevelDefinition level_info_;
// 	GoTweenFlow highlightFlow_;
//
// 	public void Awake() {
// 		initialScale_ = transform.localScale;
// 	}
//
//     public void Init(Transform parent, SharedButton.OnButtonEvent on_button_event, string level_name) {
// 		// saving and calling when invisible UI button is clicked instead of using the shared button
// 		// OnMouseUpAsButton so that clicks will be blocked by UI buttons on top of it (notably the settings
// 		// button).
//         on_click_ = on_button_event;
//
// 		transform.SetParent (parent, false);
//
//         level_info_ = Game.LevelManager.GetLevelDefinition(level_name);
//
// 		UpdateDisplay ();
//     }
//
// 	public float SetSelected(bool selected, bool animate) {
// 		selected_ = selected;
// 		UpdateSelected ();
//
// 		if (animate) {
// 			// TODO: animate to the selected state and return the actual duration of the animation
// 			// for now just using a dummy value to test the flow
// 			return 0.2f;
// 		} else {
// 			return 0;
// 		}
// 	}
//
// 	public float SetState(State state, bool animate) {
// 		if (state_ == state) {
// 			return 0;
// 		}
//
// 		state_ = state;
// 		UpdateDisplay ();
//
// 		if (animate) {
// 			// TODO: animate between states and return the actual duration of the animation
// 			// for now just using a dummy value to test the flow
// 			return 0.2f;
// 		} else {
// 			return 0;
// 		}
// 	}
//
// 	public void UpdateDisplay() {
// 		LevelSave levelSave = Game.DisplayData.FindLevelData (level_info_.Name);
//
// 		if (state_ == State.LOCKED) {
// 			// show locked
// 			lock_.gameObject.SetActive(true);
// 			solved_.gameObject.SetActive(false);
// 			bg_.color = locked_color_;
// 			level_name_text_.gameObject.SetActive(false);
// 			highscore_text_.gameObject.SetActive(false);
// 		} else {
// 			lock_.gameObject.SetActive(false);
// 			bg_.color = active_color_;
// 			level_name_text_.text = level_info_.GetDisplayName();
// 			level_name_text_.gameObject.SetActive(true);
// 			int highscore = 0;
// 			if (levelSave != null) {
// 				highscore = levelSave.Highscore;
// 			}
// 			highscore_text_.text = string.Format(Language.Get("BEST_SCORE"), highscore);
//
// 			solved_.gameObject.SetActive(state_ == State.SOLVED && level_info_.UsesScore);
// 			highscore_text_.gameObject.SetActive(level_info_.UsesScore);
// 		}
// 	}
//
// 	void ShowHighlight() {
// 		if (highlightFlow_ != null) {
// 			return;
// 		}
// 		GoTweenCollectionConfig config = new GoTweenCollectionConfig();
// 		config.setIterations(100, GoLoopType.RestartFromBeginning);
// 		highlightFlow_ = new GoTweenFlow(config);
// 		float scaleUpDur = 1.0f;
// 		float scaleDownDur = 0.75f;
// 		float pauseDur = 1.0f;
// 		highlightFlow_.insert(0, new GoTween(transform, scaleUpDur, new GoTweenConfig().scale (initialScale_ * 1.5f).setEaseType(GoEaseType.QuadOut)));
// 		highlightFlow_.insert(scaleUpDur, new GoTween(transform, scaleDownDur, new GoTweenConfig().scale (initialScale_).setEaseType(GoEaseType.QuadInOut)));
// 		highlightFlow_.insert(scaleUpDur + scaleDownDur, new GoTween(this, pauseDur, new GoTweenConfig()));
// 		highlightFlow_.play();
// 	}
//
// 	void HideHighlight() {
// 		if (highlightFlow_ != null) {
// 			highlightFlow_.destroy();
// 			highlightFlow_ = null;
// 		}
// 		transform.localScale = initialScale_;
// 	}
//
//     void OnDestroy() {
// 		HideHighlight ();
//     }
//
//     void UpdateSelected() {
// 		if (selected_) {
// 			ShowHighlight ();
// 		} else {
// 			HideHighlight();
// 		}
//     }
//
// 	public void OnButtonClick() {
// 		if (on_click_ != null) {
// 			on_click_();
// 		}
// 	}
// }
