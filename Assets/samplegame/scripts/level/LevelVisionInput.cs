// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using Tangible;
//
// public class LevelVisionInput : ClusterProcessor {
//
//     [SerializeField]
//     private TableHint table_hint_prefab_;
//     private TableHint table_hint_;
//
// 	Level.Hint hint_;
// 	bool paused_;
// 	private bool enabled_ = true;
//
// 	private float confidence_begin_ = 0.0f;
// 	private float confidence_count_ = 0;
//
// 	const int CONFIDENCE_COUNT_THRESHOLD = 5; // Vision frame count
// 	const float CONFIDENCE_DURATION_THRESHOLD = 1.0f; // Seconds
//
// 	private Dictionary<int, bool> buttons_active_ = new Dictionary<int, bool>();
// 	private List<int> button_ids_ = new List<int>();
//
// 	protected enum State {
// 		NONE = 0,
// 		PLAY = 1,
// 		ANIMATE_TURN,
// 	}
//
// 	private State state_ = State.NONE;
//
//     protected List<List<ClusterHelper.Card>> last_clusters_ ;
//
//
//     public bool Enable {
//         get { return enabled_; }
//         set { if (enabled_ != value) ResetConfidenceCheck(); enabled_ = value; }
//     }
//
// 	protected void ResetConfidenceCheck() {
// 		confidence_begin_ = UnityEngine.Time.time;
// 		confidence_count_ = 1;
// 	}
//
// 	protected void IncrementConfidence() {
// 		confidence_count_++;
// 	}
//
// 	void UpdateEffects(List<List<ClusterHelper.Card>> clusters) {
// 		foreach (List<ClusterHelper.Card> cluster in clusters) {
//
// 			foreach (ClusterHelper.Card card in cluster) {
// 				if (card.newcomer) {
// 					Debug.Log("new card: " + card.id + " unique id " + card.unique_id);
// 				}
//
// 				if (card.newly_connected) {
// 					Debug.Log("new card connected id " + card.id + " unique id " + card.unique_id);
// 				}
// 			}
//         }
//     }
//
//     public float ComputeConfidence() {
//         float now = UnityEngine.Time.time;
//         float duration = (now - confidence_begin_) / CONFIDENCE_DURATION_THRESHOLD;
//         float delta = (float) confidence_count_ / (float) CONFIDENCE_COUNT_THRESHOLD;
//         return Mathf.Min(1.0f, Mathf.Min(delta, duration));
//     }
//
// 	virtual protected void UpdateCurrentState(List<List<ClusterHelper.Card>> clusters) {
//
// 	}
//
// 	virtual protected void UpdateCurrentInputDisplay(List<List<ClusterHelper.Card>> clusters) {
//
//     }
//
// 	virtual protected void HandleButtonPress(ClusterHelper.Card card) {
// 		Debug.Log ("button pressed " + card.id);
// 	}
//
// 	private bool SameClusters(List<List<ClusterHelper.Card>> ca, List<List<ClusterHelper.Card>> cb) {
//         if (ca == null || cb == null) return false;
//         if (ca.Count != cb.Count) return false;
// 		ClusterHelper.Card card1;
// 		ClusterHelper.Card card2;
//         for (int i=0; i < ca.Count; i++) {
//             if (ca[i].Count != cb[i].Count) return false;
//             for (int j=0; j < ca[i].Count; j++) {
// 				card1 = ca [i] [j];
// 				card2 = cb [i] [j];
// 				if (card1.id != card2.id || card1.x_mm != card2.x_mm || card1.y_mm != card2.y_mm
// 					|| card1.orientation != card2.orientation) {
// 					return false;
// 				}
//             }
//         }
//
//         return true;
//     }
//
//     // This function is called on vision platform events, at most once a frame.
// 	public override void UpdateClusters(List<List<ClusterHelper.Card>> clusters) {
//         if (enabled_ == false) return;
//
// 		List<List<ClusterHelper.Card>> originalClusters = clusters;
//
// 		// do this before check for same clusters so that if a tile is added in the
// 		// wrong position the hint display will update the animation (it would still
// 		// have an empty tile in the equation so the clusters match)
// 		// need to pass in the original vision input as well to look for tiles that are
// 		// there in the wrong position
// 		UpdateHintDisplay (clusters, originalClusters);
//
//         if (SameClusters(last_clusters_, clusters)) {
// 			IncrementConfidence ();
//             return;
//         }
//
// 		// check for simple buttons, which you cover up to press it. look for any buttons that were
// 		// present in the previous frame and missing now
// 		if (state_ == State.PLAY) {
// 			int numButtons = button_ids_.Count;
// 			List<ClusterHelper.Card> previousButtons;
// 			for (int i = 0; i < numButtons; i++) {
// 				previousButtons = CardsWithId (last_clusters_, button_ids_ [i]);
// 				int numPrev = previousButtons.Count;
// 				for (int j = 0; j < numPrev; j++) {
// 					if (!ContainsUniqueId (clusters, previousButtons [j].unique_id)) {
// 						HandleButtonPress (previousButtons [j]);
// 					}
// 				}
// 			}
// 		}
//
// 		UpdateCurrentState(clusters);
// 		UpdateEffects(clusters);
// 		UpdateCurrentInputDisplay (clusters);
//
// 		last_clusters_ = clusters;
//     }
//
//     public void LevelStart(Level level) {
//         SetState(State.PLAY);
//
// 		buttons_active_.Clear ();
// 		button_ids_ = Game.Deck.GetButtonIds ();
// 		int numButtons = button_ids_.Count;
// 		for (int i = 0; i < numButtons; i++) {
// 			buttons_active_.Add (button_ids_ [i], false);
// 		}
//     }
//
//     public void LevelEnd() {
//         SetState(State.NONE);
// 		HideHint ();
//     }
//
//     virtual protected void Start() {
//         if (state_ == State.NONE) gameObject.SetActive(false);
//
// 		table_hint_ = GameObject.Instantiate(table_hint_prefab_) as TableHint;
// 		table_hint_.transform.SetParent (Game.Level.transform, true);
//     }
//
// 	// Update is called once per frame
// 	void Update() {
//         if (state_ == State.PLAY) {
// 			CheckForTurn();
//         }
//
// 		UpdateHint ();
// 	}
//
// 	virtual protected void CheckForTurn() {
//
// 	}
//
// 	void UpdateHintDisplay(List<List<ClusterHelper.Card>> clusters, List<List<ClusterHelper.Card>> visionClusters) {
// 		if (hint_ == null || hint_.pieces.Count != 2) {
// 			return;
// 		}
//
// 		if (!paused_) {
// 			table_hint_.ShowTableHint (hint_.pieces);
// 		}
// 	}
//
// 	void UpdateHint() {
// 		if (state_ == State.NONE) {
// 			return;
// 		}
// 		bool validHint = false;
// 		if (Game.Level.ShouldShowHint ()) {
// 			hint_ = Game.Level.GetHint();
// 			if (hint_ != null && hint_.pieces.Count == 2) {
// 				validHint = true;
//
// 				if (!paused_) {
// 					table_hint_.ShowTableHint(hint_.pieces);
// 				}
//
// 			}
// 		}
// 		if (!validHint) {
// 			HideHint();
// 		}
// 	}
//
// 	void HideHint() {
// 		table_hint_.HideHint();
// 		hint_ = null;
// 	}
//
// 	public void SetPause(bool pause) {
//         if (pause != paused_) {
//             ResetConfidenceCheck();
//         }
//
// 		paused_ = pause;
//
//         if (pause && table_hint_ != null) {
// 			table_hint_.HideHint();
// 		}
// 	}
//
// 	virtual protected void CompleteAllAnimations() {
//
// 	}
//
// 	virtual protected void AnimateTurn() {
// 		CompleteAllAnimations();
//
// 	}
//
// 	virtual protected void AnimateTurnComplete() {
// 		hint_ = null;
//
// 		Game.Level.OnTurnComplete();
//
// 		if (state_ == State.ANIMATE_TURN) {
// 			SetState (State.PLAY);
// 		}
// 	}
//
//     // STATE TRANSITIONS
//
//     protected void SetState(State new_state) {
//         State old_state = state_;
//         if (old_state == new_state) return;
//         //Debug.Log("Vision Input Display State " + old_state  + " -> " + new_state);
//
//         switch (old_state) {
//             case State.NONE: OnExitNone(new_state); break;
//             case State.PLAY: OnExitPlay(new_state); break;
// 			case State.ANIMATE_TURN: OnExitAnimateTurn(new_state); break;
//         }
//
//         state_ = new_state;
//
//         switch (new_state) {
//             case State.NONE: OnEnterNone(old_state); break;
//             case State.PLAY: OnEnterPlay(old_state); break;
// 			case State.ANIMATE_TURN: OnEnterAnimateTurn(old_state); break;
//         }
//     }
//
//     private void OnExitNone(State next_state) {
//         gameObject.SetActive(true);
//     }
//
//     private void OnExitPlay(State next_state) {
//         last_clusters_ = null;
//     }
//
// 	private void OnExitAnimateTurn(State next_state) {
// 	}
//
//     private void OnEnterNone(State previous_state) {
//         gameObject.SetActive(false);
//     }
//
//     private void OnEnterPlay(State previous_state) {
// 		CompleteAllAnimations();
//         last_clusters_ = null;
//     }
//
// 	private void OnEnterAnimateTurn(State previous_state) {
// 		Game.Level.OnTakenTurn ();
// 		AnimateTurn();
// 	}
//
// 	void OnDestroy() {
// 		if (table_hint_ != null) {
// 			Destroy (table_hint_.gameObject);
// 			table_hint_ = null;
// 		}
// 		CompleteAllAnimations ();
// 	}
// }
