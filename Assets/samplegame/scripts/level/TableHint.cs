using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TableHint : MonoBehaviour {

	[SerializeField]
	bool is_top_;

	Vector3 target_left_pos_ = new Vector3(-.3f, 0f, 0.0f);
	Vector3 target_right_pos_ = new Vector3(.3f, 0f, 0.0f);

	Vector3 start_left_pos_ = new Vector3(-1.2f, -0.5f, 0.0f);
	Vector3 start_right_pos_ = new Vector3(1.2f, -0.5f, 0.0f);
	
    Vector3 end_left_offset_ = new Vector3(-1.0f, -0.5f, -1.0f);
    Vector3 end_right_offset_ = new Vector3(1.0f, -0.5f, -1.0f);

	Vector3 hand_left_offset_ = new Vector3 (-0.1f, -0.1f, -2f);
	Vector3 hand_right_offset_ = new Vector3 (-0.1f, -0.1f, -2f);

	Vector3 hand_left_offset_table_ = new Vector3 (-0.1f, 0f, -2f);
	Vector3 hand_right_offset_table_ = new Vector3 (-0.1f, 0f, -2f);

	GoTweenFlow play_hint_flow_;
    GoTweenFlow table_show_flow_;
    GoTweenFlow table_hide_flow_;
    
	Vector3 table_position_show_bottom_ = new Vector3(0.0f, -550.0f, 0.0f);
	Vector3 table_position_hide_bottom_ =  new Vector3(0.0f, -1000.0f, 0.0f);

	Vector3 table_position_show_top_ = new Vector3(0.0f, 520.0f, 0.0f);
	Vector3 table_position_hide_top_ = new Vector3(0.0f, 1000.0f, 0.0f);

	Vector3 table_position_show { get { return is_top_ ? table_position_show_top_ : table_position_show_bottom_; }}
	Vector3 table_position_hide { get { return is_top_ ? table_position_hide_top_ : table_position_hide_bottom_; }}

    bool playing_ = false;
        
	[SerializeField]
	SpriteRenderer tile_prefab_;

	SpriteRenderer left_tile_;
	SpriteRenderer right_tile_;

	[SerializeField]
	Transform right_parent_;

    [SerializeField]
    Transform left_parent_;
    
	[SerializeField]
	Transform table_graphics_;

	[SerializeField]
	Transform left_hand_prefab_;

	[SerializeField]
	Transform right_hand_prefab_;
	
	List<Sprite> display_order_;
	bool is_left_correct_;
	bool is_right_correct_;
	bool already_on_table_;
    
    void Awake() {
        table_graphics_.transform.position = table_position_hide;
    }
    
    void Start() {
        table_graphics_.transform.position = table_position_hide;
        
        foreach (Circle c in table_graphics_.GetComponentsInChildren<Circle>()) {
            c.AlphaMultiplier = 0.0f;
        }
    }

	public void ShowTableHint(List<Sprite> displayOrder, bool isLeftCorrect = false, bool isRightCorrect = false, bool alreadyOnTable = false) {
		display_order_ = displayOrder;
		is_left_correct_ = isLeftCorrect;
		is_right_correct_ = isRightCorrect;
		already_on_table_ = alreadyOnTable;

		if (!playing_) {
			PlayHintInternal();
		}
	}

	public void PlayAnimation() {
		CleanHands();

		play_hint_flow_ = new GoTweenFlow();
		play_hint_flow_.autoRemoveOnComplete = true;
	
		if (display_order_ != null && display_order_.Count > 0) {
			Vector3 targetPos = target_left_pos_;
			Vector3 startPos = start_left_pos_;
			if (is_left_correct_) {
				startPos = targetPos;
			}
			if (already_on_table_) {
				startPos = target_left_pos_;
			}
			PlayHandAnimation (display_order_ [0], left_parent_, startPos, targetPos, end_left_offset_, 
			                   already_on_table_ ? hand_left_offset_table_ : hand_left_offset_, left_hand_prefab_, 0.3f);
		}
		if (display_order_ != null && display_order_.Count > 0) {
			Vector3 targetPos = target_right_pos_;
			Vector3 startPos = start_right_pos_;
			if (is_right_correct_) {
				startPos = targetPos;
			}
			if (already_on_table_) {
				startPos = target_right_pos_;
			}
			PlayHandAnimation (display_order_ [1], right_parent_, startPos, targetPos, end_right_offset_, 
			                   already_on_table_ ? hand_right_offset_table_ : hand_right_offset_, right_hand_prefab_, 0.0f);
		}

		play_hint_flow_.insert(0, new GoTween(this, 4.8f, new GoTweenConfig().onComplete(thisTween => PlayAnimation())));
		play_hint_flow_.play();
	}

	void OnDestroy() {
        CleanAll();
		if (table_show_flow_ != null) {
			table_show_flow_.destroy();
		}
		if (table_hide_flow_ != null) {
			table_hide_flow_.destroy();
		}
	}
    
    private void PlayHandAnimation(Sprite sprite, Transform parent, Vector3 start, Vector3 target, Vector3 end_offset, Vector3 hand_offset, Transform hand_prefab, float delay) {
		SpriteRenderer tile = GameObject.Instantiate(tile_prefab_) as SpriteRenderer;
		tile.sprite = sprite;
		tile.transform.parent = parent;
        parent.transform.localPosition = start;
		tile.transform.localPosition = Vector3.zero;
         
		if (start != target) {
            play_hint_flow_.insert(0.0f + delay, new GoTween (parent, 2.0f, new GoTweenConfig().localPosition(target, false).setEaseType(GoEaseType.CubicOut)));
        
			Transform hand = Instantiate(hand_prefab) as Transform;
			hand.parent = parent;
			hand.localPosition = hand_offset;
                
            play_hint_flow_.insert(2.2f + delay, new GoTween (hand.transform, 1.5f, new GoTweenConfig().localPosition(end_offset, true).setEaseType(GoEaseType.CubicInOut))); 
		}

		SetSortingLayer setLayer = GetComponent<SetSortingLayer>();
		if (setLayer != null) {
			setLayer.RefreshSortingLayer();
		}
    }

	private void PlayHintInternal() {
		playing_ = true;

		if (table_hide_flow_ != null) {
			table_hide_flow_.destroy();
			table_hide_flow_ = null;
		}
		
		if (play_hint_flow_ != null) {
			play_hint_flow_.destroy();
			play_hint_flow_ = null;
		}

		if (table_graphics_.transform.localPosition == table_position_show) {
			PlayAnimation();
		} else {
			table_show_flow_ = new GoTweenFlow();
			table_show_flow_.autoRemoveOnComplete = true;
			table_show_flow_.insert(0.0f, new GoTween(table_graphics_.transform, 1.0f, new GoTweenConfig().localPosition(table_position_show).setEaseType(GoEaseType.CubicOut).onComplete(thisTween => PlayAnimation())));
			foreach (Circle c in table_graphics_.GetComponentsInChildren<Circle>()) {
				table_show_flow_.insert(0.0f, new GoTween(c, 1.0f, new GoTweenConfig().floatProp("AlphaMultiplier", 1.0f).setEaseType(GoEaseType.CubicIn)));
			}
			table_show_flow_.play();
		}
	}

	public void HideHint() {
		if (!playing_) {
			return;
		}
        playing_ = false;

		if (table_show_flow_ != null) {
            table_show_flow_.destroy();
            table_show_flow_ = null;
        }
        
		table_hide_flow_ = new GoTweenFlow();
        table_hide_flow_.autoRemoveOnComplete = true;
        table_hide_flow_.insert(0.0f, new GoTween(table_graphics_.transform, 0.75f, new GoTweenConfig().localPosition(table_position_hide).setEaseType(GoEaseType.QuadIn).onComplete(thisTween => CleanAll())));
        foreach (Circle c in table_graphics_.GetComponentsInChildren<Circle>()) {
            table_hide_flow_.insert(0.0f, new GoTween(c, 0.75f, new GoTweenConfig().floatProp("AlphaMultiplier", 0.0f).setEaseType(GoEaseType.QuadIn)));
        }
        table_hide_flow_.play();
  	}
    
    private void CleanAll() {
        CleanHands();
    }
    
    private void CleanHands() {
        if (play_hint_flow_ != null) {
            play_hint_flow_.destroy();
            play_hint_flow_ = null;
        }
        
		if (left_parent_ != null) {
			for (int i = 0; i < left_parent_.childCount; i++) {
				Destroy(left_parent_.GetChild(i).gameObject);
			}
		}

		if (right_parent_ != null) {
			for (int i = 0; i < right_parent_.childCount; i++) {
				Destroy(right_parent_.GetChild(i).gameObject);
			}
		}
    }
}
