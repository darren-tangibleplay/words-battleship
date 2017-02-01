using UnityEngine;
using System.Collections;

public class SpriteFadeIn : MonoBehaviour {

    [SerializeField]
    public float duration_ = 1.0f;

    [SerializeField]
    private GoEaseType ease_type_ = GoEaseType.Linear;

    private GoTweenFlow flow_;

	void Start () {
        SpriteRenderer sprite_renderer = GetComponent<SpriteRenderer>();
        if (sprite_renderer == null) {
            Debug.LogError(name + " does not have a SpriteRenderer component but have a SpriteFadeIn component. Abort.");
            return;
        }
        flow_ = new GoTweenFlow();
        flow_.autoRemoveOnComplete = true;
        flow_.insert(0.0f, Go.to(sprite_renderer, duration_, new GoTweenConfig().colorProp("color", TweenHelper.FadedColor(sprite_renderer.color, 0)).setIsFrom().setEaseType(ease_type_)));
        flow_.setOnCompleteHandler(thisTween => GameObject.Destroy(this)); // Only remove the component
        flow_.play();
	}
	
    void OnDestroy() {
        if (flow_ != null) flow_.destroy();
    }
}
