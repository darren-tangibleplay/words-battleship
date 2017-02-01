using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MenuScroll : UIBehaviour, IEndDragHandler, IBeginDragHandler {

    public delegate void ScrollListener(Vector2 offset_unit);

    [SerializeField]
    private ScrollRect scroll_rect_;

    [SerializeField]
    private RectTransform content_rect_;

    private Vector2 normalized_to_unit_ = Vector2.one;
    private Vector2 unit_to_normalized_ = Vector2.one;

    List<ScrollListener> listeners_ = new List<ScrollListener>();

    GoTweenFlow snap_flow_;

    public Vector2 ScrollRange {
        get { return normalized_to_unit_; }
    }

    protected override void Awake() {
        base.Awake();

        normalized_to_unit_ = content_rect_.offsetMax - content_rect_.offsetMin;
        unit_to_normalized_.x = 1.0f / normalized_to_unit_.x;
        unit_to_normalized_.y = 1.0f / normalized_to_unit_.y;
    }

    public void Listen(ScrollListener listener) {
        listeners_.Add(listener);
    }

    public void Unlisten(ScrollListener listener) {
        listeners_.Remove(listener);
    }

    public Vector2 OffsetUnit {
        get { 
            float x = (1.0f - scroll_rect_.horizontalNormalizedPosition) * normalized_to_unit_.x;
            float y = (1.0f - scroll_rect_.verticalNormalizedPosition) * normalized_to_unit_.y; 
            return new Vector2(x, y);
        }
        set { 
            Vector2 normalized_offset;
            normalized_offset.x = 1.0f - (value.x * unit_to_normalized_.x); 
            normalized_offset.y = 1.0f - (value.y * unit_to_normalized_.y); 

			if (!float.IsInfinity(normalized_offset.x) && !float.IsInfinity(normalized_offset.y)) {
				scroll_rect_.normalizedPosition = normalized_offset;
			}
            OnScroll(value); // This is needed as the scroll rect is disabled during animations
        }
    }

    public Vector2 WorldToOffset(Vector3 p) {
        Vector2 offsetUnit = new Vector2(p.x, p.y) - OffsetUnit; // Account for already applied scrolling
        offsetUnit.x = -Mathf.Clamp(offsetUnit.x, -normalized_to_unit_.x, 0);
        offsetUnit.y = -Mathf.Clamp(offsetUnit.y, -normalized_to_unit_.y, 0);
        return offsetUnit;
    }

    // Wired to the scroll rect in the Editor 
    public void OnScrollChanged(Vector2 normalized_offset) {
        Vector2 offset_unit;
        offset_unit.x = (1.0f - normalized_offset.x) * normalized_to_unit_.x;
        offset_unit.y = (1.0f - normalized_offset.y) * normalized_to_unit_.y;
        OnScroll(offset_unit);
    }

    private void OnScroll(Vector2 offset_unit) {
        foreach (ScrollListener listener in listeners_) {
            listener(offset_unit);
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (snap_flow_ != null) snap_flow_.destroy();
    }
    
    public void OnBeginDrag(PointerEventData eventData) {
        if (snap_flow_ != null) {
            snap_flow_.complete();
            snap_flow_ = null;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        //Debug.Log("OnEndDrag offset: " + OffsetUnit);

        // Snap to the Main menu or the first set of puzzles
        if (OffsetUnit.y < 384) {
            AnimateOffsetTo(new Vector2(OffsetUnit.x, 0), 0.5f);
        } else if (OffsetUnit.y < 768) {
            AnimateOffsetTo(new Vector2(OffsetUnit.x, 768), 0.5f);
        }
    }

    public void AnimateOffsetTo(Vector2 target_offset_unit, float duration) {
        scroll_rect_.enabled = false;
        
        if (snap_flow_ != null) snap_flow_.destroy();
        
        snap_flow_ = new GoTweenFlow();
        snap_flow_.autoRemoveOnComplete = true;
        snap_flow_.insert(0.0f, Go.to(this, duration, new GoTweenConfig().vector2Prop("OffsetUnit", target_offset_unit).setEaseType(GoEaseType.CubicOut)));
        snap_flow_.setOnCompleteHandler(thisTween => OnAnimationComplete());
        snap_flow_.play();
    }
    
    void OnAnimationComplete() {
        scroll_rect_.enabled = true;
    }
}
