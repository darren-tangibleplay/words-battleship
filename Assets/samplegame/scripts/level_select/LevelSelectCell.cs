// using UnityEngine;
// using System.Collections;
// using System;
// using System.Collections.Generic;
//
// public class LevelSelectCell : MonoBehaviour {
//
//     public delegate void OnTapCallback(LevelSelectCell cell);
// 	public delegate bool CanTapCallback();
//
// 	[SerializeField]
//     private LevelSelectCellGraphics graphics_prefab_;
//     private LevelSelectCellGraphics graphics_;
//
// 	[SerializeField]
//     private string level_name_;
//     public string LevelName { get { return level_name_; } }
//
//     private List<LevelSelectCell> next_cells_ = new List<LevelSelectCell>();
//     public List<LevelSelectCell> NextCells { get { return next_cells_; } }
//
//     private OnTapCallback on_tap_;
// 	private CanTapCallback can_tap_;
//
// 	public void Init(OnTapCallback on_tap, CanTapCallback can_tap, Dictionary<string, LevelSelectCell> all_cells) {
//         on_tap_ = on_tap;
// 		can_tap_ = can_tap;
//
//         LevelDefinition level_definition = Game.LevelManager.GetLevelDefinition(level_name_);
//
//         graphics_ = GameObject.Instantiate(graphics_prefab_);
//         graphics_.Init(transform, OnTap, level_name_);
//
//         // Wiring all the next cells
//         foreach (string next_level in level_definition.NextLevels) {
//             if (!all_cells.ContainsKey(next_level)) {
//                 Debug.LogError("Level '" + next_level + "' does not have a matching LevelSelectCell");
//             }
//             next_cells_.Add(all_cells[next_level]);
//         }
//
// 		LevelSave level_display = Game.LevelManager.GetLevelDisplay(level_name_);
// 		bool available = level_display == null ? false : level_display.Available;
// 		bool is_solved = level_display == null ? false : level_display.Solved;
//
// 		if (!available) {
// 			graphics_.SetState(LevelSelectCellGraphics.State.LOCKED, false);
// 		} else if (!is_solved) {
// 			graphics_.SetState(LevelSelectCellGraphics.State.UNLOCKED, false);
// 		} else {
// 			graphics_.SetState(LevelSelectCellGraphics.State.SOLVED, false);
// 		}
// 	}
//
// 	public float AnimateLevelSolved() {
// 		return graphics_.SetState(LevelSelectCellGraphics.State.SOLVED, true);
//     }
//
//     public float AnimateLevelUnlocked(LevelSelectCell from) {
//         float dur = graphics_.SetState(LevelSelectCellGraphics.State.UNLOCKED, true);
// 		SampleGameSoundManager.instance.QueueSoundFile(SampleGameSoundManager.LEVEL_UNLOCK_NAME, dur / 2);
// 		return dur;
//     }
//
//     public float AnimateLevelLocked() {
//         return graphics_.SetState(LevelSelectCellGraphics.State.LOCKED, true);
//     }
//
// 	public float SetSelected(bool selected, bool animate) {
// 		return graphics_.SetSelected(selected, animate);
// 	}
//
// 	bool CanTap() {
// 		// don't allow levels to be selected while a menu is showing over them
// 		if (SampleGameUIManager.instance.input_blocked) {
// 			return false;
// 		}
// 		if (can_tap_ != null) {
// 			return can_tap_();
// 		}
// 		return true;
// 	}
//
// 	void OnTap() {
// 		if (!CanTap ()) {
// 			return;
// 		}
// 		LevelSave display_level = Game.LevelManager.GetLevelDisplay(level_name_);
// 		if (display_level != null && display_level.Available) {
// 			OnPlay ();
// 		} else {
// 			Debug.Log("LOCKED");
// 		}
// 	}
//
// 	void OnPlay() {
// 		SampleGameSoundManager.instance.OnButtonClick();
// 		on_tap_(this);
// 	}
// }
