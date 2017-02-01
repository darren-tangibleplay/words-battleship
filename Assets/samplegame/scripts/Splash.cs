using UnityEngine;
using System.Collections;

public class Splash : MonoBehaviour {
    
    public delegate void OnComplete(ExitType exit_type);
    
    public enum ExitType {
        SKIP,
        ANIMATION,
    };

    GoTweenFlow intro_flow_;
        
    private OnComplete on_complete_;
    
    public void Init(OnComplete on_complete) {
        on_complete_ = on_complete;
    }
    
    void OnDestroy() {
        if (intro_flow_ != null) intro_flow_.destroy();
    }

    void Start () {
        intro_flow_ = new GoTweenFlow();
        intro_flow_.autoRemoveOnComplete = true;

        // Dummy animation that does nothing
        intro_flow_.insert(0, new GoTween(transform, 0.1f, new GoTweenConfig().position(Vector3.zero)));
		
        intro_flow_.setOnCompleteHandler(thisTween => OnAnimationComplete());
        intro_flow_.play();
    }
        
    void OnAnimationComplete() {
        if (on_complete_ != null) on_complete_(ExitType.ANIMATION);
    }
    
    void OnSkip() {
        if (on_complete_ != null) on_complete_(ExitType.SKIP);
    }
    
    public void AnimateOut() {
        on_complete_ = null;

        if (intro_flow_ != null) {
            intro_flow_.complete();
            intro_flow_ = null;
        }

		OnAllDone();
    }
    
    void OnAllDone() {
        Destroy(gameObject);
    }
}