// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System;
// using UnityEngine.UI;
//
// public class LevelSelectScreen : MonoBehaviour {
//
// 	[SerializeField]
// 	MenuScroll menu_scroll_;
//
// 	string current_level_name_selection_;
//
//     Action on_tap_;
//     bool running_transition_;
// 	bool running_tap_;
//
//     Vector3 initial_camera_position_;
//     float initial_camera_size_;
//     GoTweenFlow zoom_flow_;
//
// 	const float ZOOM_FACTOR = 1.8f;
//
//     const float SLOW_TIMESCALE = 0.25f;
//     const float NORMAL_TIMESCALE = 1.0f;
//     float timescale_ = NORMAL_TIMESCALE;
//     public float Timescale {
//         get { return timescale_; }
//         set { timescale_ = value; PropagateTimescale(); }
//     }
//
//     Dictionary<string, LevelSelectCell> cells_ = new Dictionary<string, LevelSelectCell>();
//
//     public void Init(Action on_tap, string focus_level_name) {
//         on_tap_ = on_tap;
//
// 		LevelSelectCell[] cells = transform.GetComponentsInChildren<LevelSelectCell>();
// 		foreach(LevelSelectCell cell in cells) {
// 			cells_[cell.LevelName] = cell;
// 		}
//
//         // Keep it a separate loop
// 		foreach(LevelSelectCell cell in cells) {
//             cell.Init(OnTap, CanTapCell, cells_);
// 		}
//
//         Select(focus_level_name);
// 	}
//
// 	void UnlockInitialCell(LevelSelectCell cell, bool select) {
// 		Game.LevelManager.UnlockLevel(cell.LevelName, null, false, select);
// 	}
//
//     void OnTap(LevelSelectCell cell) {
//         if (running_tap_) return;
//         Game.LevelManager.LevelNameToStart = cell.LevelName;
//         StartCoroutine(PlayTap(cell));
//     }
//
// 	bool CanTapCell() {
// 		if (running_tap_) {
// 			return false;
// 		}
// 		return true;
// 	}
//
//     IEnumerator PlayTap(LevelSelectCell cell) {
//         running_tap_ = true;
// 		string levelName = cell.LevelName;
//         TransitionManager.Transition transition = new TransitionManager.Transition(() => on_tap_(), TransitionManager.Moment.LEVEL_SELECTED, levelName);
//         float duration = LaunchTransition(transition, false);
//         yield return new WaitForSeconds(duration);
//         transition.OnComplete();
//         running_tap_ = false;
//     }
//
// 	// Use this for initialization
// 	void Start () {
//         initial_camera_position_ = Camera.main.transform.position;
//         initial_camera_size_ = Camera.main.orthographicSize;
//     }
//
//     void Update() {
//         if (!running_transition_ && Game.TransitionManager.HasNext()) {
//             StartCoroutine(PlayTransitions());
//         }
//     }
//
//     void OnDestroy() {
//         Cleanup();
//     }
//
//     public void Cleanup() {
//         if (zoom_flow_ != null) zoom_flow_.destroy();
// 		if (Camera.main != null) {
// 			Camera.main.transform.position = initial_camera_position_;
// 			Camera.main.orthographicSize = initial_camera_size_;
// 		}
//     }
//
//     IEnumerator PlayTransitions() {
//         if (!running_transition_) {
//             running_transition_ = true;
//
//             TransitionManager.Transition transition = Game.TransitionManager.PopNext();
//             if (transition != null) {
//                 while (transition != null) {
//                     float duration = LaunchTransition(transition, true);
//                     yield return new WaitForSeconds(duration);
//                     transition.OnComplete(); // Commits the related save_data updates to display_data
//
//                     transition = Game.TransitionManager.PopNext();
//                 }
//                 Game.singleton.CheckDisplayDataConsistency();
//
//                 Select(Game.DisplayData.LevelSelected);
//             }
//
//             yield return new WaitForEndOfFrame();
//             running_transition_ = false;
//         }
//     }
//
//     public float LaunchTransition(TransitionManager.Transition transition, bool zoom) {
//         LevelSelectCell cell = cells_[transition.level_name];
//         float duration = 0;
//         switch (transition.moment) {
//             case TransitionManager.Moment.LEVEL_SOLVED:
//                 duration = cell.AnimateLevelSolved();
//                 break;
//             case TransitionManager.Moment.LEVEL_UNLOCKED:
//                 duration = cell.AnimateLevelUnlocked(transition.from_level_name == null ? null : cells_[transition.from_level_name]);
//                 break;
// 			case TransitionManager.Moment.LEVEL_SELECTED:
// 				duration = cell.SetSelected(true, true);
// 				break;
// 		}
// 		//Debug.Log("Running transition: " + transition.ToString() + " [" + duration + " seconds]");
//         if (zoom) ZoomOn(cell, duration);
//         return duration;
//     }
//
//     public void ZoomOn(LevelSelectCell cell, float duration) {
//         if (duration <= 0) return;
//         float zoom_duration = Mathf.Min(duration / 2, 0.5f);
//
//         Vector2 offset = menu_scroll_.WorldToOffset(cell.transform.position);
//         Vector2 scroll_start_offset = offset;
//         scroll_start_offset.y = Mathf.Clamp(menu_scroll_.OffsetUnit.y, scroll_start_offset.y - 768, scroll_start_offset.y + 768);
//         menu_scroll_.OffsetUnit = scroll_start_offset;
//         menu_scroll_.AnimateOffsetTo(offset, zoom_duration);
//
//         Vector3 pos = cell.transform.position;
//
//         float camera_size_zoom_in = initial_camera_size_ / ZOOM_FACTOR;
//         float camera_x_min = 768.0f * (1.0f - (1.0f / ZOOM_FACTOR)) / 2.0f;
//         //float camera_y_min = 1024.0f * (1.0f - (1.0f / Constants.CAMERA_ZOOM_FACTOR)) / 2.0f;
//
//         Vector3 camera_pos_zoom_in = initial_camera_position_;
//         if (pos.x < 768.0f * 0.5f) {
//             camera_pos_zoom_in.x = Mathf.Max(pos.x, -camera_x_min);
//         } else if (pos.x > (768.0f * 0.5f + menu_scroll_.ScrollRange.x)) {
//             camera_pos_zoom_in.x = Mathf.Min(pos.x - menu_scroll_.ScrollRange.x, camera_x_min);
//         }
//
//         if (zoom_flow_ != null) zoom_flow_.destroy();
//         zoom_flow_ = new GoTweenFlow();
//         zoom_flow_.autoRemoveOnComplete = true;
//         // Zoom in
//         zoom_flow_.insert(0.0f, Go.to(Camera.main, zoom_duration, new GoTweenConfig().floatProp("orthographicSize", camera_size_zoom_in).setEaseType(GoEaseType.CubicInOut)));
//         zoom_flow_.insert(0.0f, Go.to(Camera.main.transform, zoom_duration, new GoTweenConfig().position(camera_pos_zoom_in).setEaseType(GoEaseType.CubicInOut)));
//         zoom_flow_.insert(0.0f, Go.to(this, zoom_duration, new GoTweenConfig().floatProp("Timescale", SLOW_TIMESCALE).setEaseType(GoEaseType.CubicInOut)));
//         // Zoom out
//         zoom_flow_.insert(duration, Go.to(Camera.main, zoom_duration * 2, new GoTweenConfig().floatProp("orthographicSize", initial_camera_size_).setEaseType(GoEaseType.CubicInOut)));
//         zoom_flow_.insert(duration, Go.to(Camera.main.transform, zoom_duration * 2, new GoTweenConfig().position(initial_camera_position_).setEaseType(GoEaseType.CubicInOut)));
//         zoom_flow_.insert(duration, Go.to(this, zoom_duration * 2, new GoTweenConfig().floatProp("Timescale", NORMAL_TIMESCALE).setEaseType(GoEaseType.CubicInOut)));
//         zoom_flow_.play();
//     }
//
//     void PropagateTimescale() {
//
//     }
//
//     public void DebugSolve() {
// 		LevelDefinition initialCell = Game.LevelManager.GetInitialCell ();
// 		if (initialCell != null) {
// 			LevelSelectCell last_unlock = RecursiveUnlock (initialCell.Name, null);
// 			if (last_unlock != null) {
// 				Select (last_unlock.LevelName);
// 			}
// 		}
//     }
//
//     public void FocusOn(string level_name, bool long_scroll = false) {
//         Vector2 offset = new Vector2(512, 0);
//         if (level_name != null && cells_.ContainsKey(level_name)) {
//             Vector3 world = cells_[level_name].transform.position;
//             offset = menu_scroll_.WorldToOffset(world);
//             offset.y = Math.Max(offset.y, 768.0f);
//         }
//
//         Vector2 scroll_start_offset = offset;
//         float range = long_scroll ? 768 : 384;
//         scroll_start_offset.y = Mathf.Clamp(menu_scroll_.OffsetUnit.y, scroll_start_offset.y - range, scroll_start_offset.y + range);
//
//         menu_scroll_.OffsetUnit = scroll_start_offset;
//         menu_scroll_.AnimateOffsetTo(offset, long_scroll ? 0.75f : 0.5f);
//     }
//
//     public void SelectDefaultLevel() {
//         Select(Game.SaveData.LevelSelected);
//     }
//
//     public void Select(string level_name, bool focus = true) {
//         if (focus) FocusOn(level_name, true);
//
// 		IEnumerable<LevelSelectCell> cells = cells_.Values;
// 		foreach(LevelSelectCell cell in cells) {
// 			cell.SetSelected(level_name == cell.LevelName, false);
// 		}
//         if (level_name != null && cells_.ContainsKey(level_name) && current_level_name_selection_ != level_name) {
//             current_level_name_selection_ = level_name;
//
//             Game.SaveData.LevelSelected = level_name;
// 			Game.DisplayData.LevelSelected = level_name;
// 			SampleGamePlayerState.stateDirty = true;
//         }
//     }
//
//     private LevelSelectCell RecursiveUnlock(string level_name_to_unlock, string from_level_name) {
//         if (Game.LevelManager.UnlockLevel(level_name_to_unlock, from_level_name, true, false)) {
//             return cells_[level_name_to_unlock];
//         }
//
//         LevelSelectCell last = null;
//         foreach (string next_level_name in Game.LevelManager.GetLevelDefinition(level_name_to_unlock).NextLevels) {
//             LevelSelectCell cell = RecursiveUnlock(next_level_name, level_name_to_unlock);
//             if (cell != null) last = cell;
//         }
//         return last;
//     }
// }
