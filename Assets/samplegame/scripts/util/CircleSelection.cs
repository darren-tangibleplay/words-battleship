using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircleSelection : MonoBehaviour {
    
    [SerializeField]
    float duration_ = 1.0f;
    
    [SerializeField]
    float end_radius_multiplier_ = 1.5f;
    
    [SerializeField]
    float thickness_ = 6.0f;
    
    private float start_radius_;
    private float end_radius_;
    
    List<CircleAA> circles_ = new List<CircleAA>();

    private float alpha_ = 1.0f;
    public float Alpha {
        get { return alpha_; }
        set { alpha_ = value; }
    }

    private GoTweenFlow flow_;
    
    public void Init(Transform t, float radius, float duration) {
        
        circles_ = new List<CircleAA>(GetComponentsInChildren<CircleAA>());

        Vector3 initial_local_position = transform.localPosition;
        
        transform.SetParent(t);
        transform.localPosition = initial_local_position;
        transform.localScale = Vector3.one;
        
        start_radius_ = radius + 0.25f;
        end_radius_ = end_radius_multiplier_ * start_radius_;

        alpha_ = 0.0f;
        for (int i = 0; i < circles_.Count; i++) {
            circles_[i].AlphaMultiplier = alpha_;
        }

        AnimateIn(duration);
    }
    
    void Update() {
        for (int i = 0; i < circles_.Count; i++) {
            float t = Time.time / duration_ + ((float) i ) / circles_.Count;
            t -= Mathf.Floor(t); 
            float outer_radius = Mathf.Lerp(start_radius_, end_radius_, t);
            circles_[i].InnerRadius = Mathf.Max(start_radius_, outer_radius - thickness_);
            circles_[i].OuterRadius = outer_radius;
            circles_[i].AlphaMultiplier = (1.0f - t) * alpha_;
        }
    }

    public void AnimateIn(float duration) {
        if (flow_ != null) flow_.destroy();
        flow_ = new GoTweenFlow();
        flow_.autoRemoveOnComplete = true;
        flow_.insert(0.0f, new GoTween(this, duration, new GoTweenConfig().floatProp("Alpha", 1.0f).setEaseType(GoEaseType.QuadIn)));
        flow_.play();
    }

    public void AnimateOut(float duration, bool self_destroy = true) {
        if (flow_ != null) flow_.destroy();
        flow_ = new GoTweenFlow();
        flow_.autoRemoveOnComplete = true;
        flow_.insert(0.0f, new GoTween(this, duration, new GoTweenConfig().floatProp("Alpha", 0.0f).setEaseType(GoEaseType.QuadOut)));
        if (self_destroy) flow_.setOnCompleteHandler(thisTween => SelfDestroy());
        flow_.play();
    }
    
    void SelfDestroy() {
        GameObject.Destroy(gameObject);
    }

    void OnDestroy() {
        if (flow_ != null) flow_.destroy();
    }
}