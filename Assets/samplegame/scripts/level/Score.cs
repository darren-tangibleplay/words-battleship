using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	private int display_value_;
    private int immediate_value_;

    private Vector3 completed_local_position_;
		
    [SerializeField]
    private Text text_;

    [SerializeField]
    private Color highscore_color_;

	private GoTweenFlow text_flow_;
    private bool completed_;
	private int highscore_ = 0;
	private bool highscore_triggered_ = false;

    public int DisplayValue {
        get { return display_value_; }
        set {
			int delta = value - display_value_;
			display_value_ = value;
			text_.text = display_value_.ToString();
			OnDisplayValueChanged(delta);
		}
    }

    public int ImmediateValue {
        get { return immediate_value_; }
        set { immediate_value_ = value;  }
    }
	
	public float Alpha {
		get { return text_.color.a; }
		set { text_.color = TweenHelper.FadedColor(text_.color, value); }
	}

	public Vector3 FloatyTextDestination {
        get { return completed_ ? transform.parent.TransformPoint(completed_local_position_) : transform.position; }
	}

	void Start() {
		FadeIn();
	}

	public void Init(int highscore) {
		highscore_ = highscore;
		DisplayValue = 0;
		ImmediateValue = 0;
	}

	void FadeIn() {
        if (text_flow_ != null) text_flow_.complete();
		text_flow_ = new GoTweenFlow();
		text_flow_.autoRemoveOnComplete = true;
		text_flow_.insert(0.0f, Go.to(this, 25f, new GoTweenConfig().floatProp("Alpha", 255).setEaseType(GoEaseType.CubicIn)));
		text_flow_.play();
	}

	private void OnDisplayValueChanged(int delta) {
		if (!highscore_triggered_ && highscore_ <= display_value_) {
			highscore_triggered_ = true;
			OnHighscoreReached();
		}
	}

    void OnHighscoreReached() {
        if (text_flow_ != null) text_flow_.complete();
        text_flow_ = new GoTweenFlow();
        text_flow_.autoRemoveOnComplete = true;
        text_flow_.insert(0.0f, Go.to(text_, 0.3f, new GoTweenConfig().colorProp("color", highscore_color_).setEaseType(GoEaseType.QuadOut).setIterations(12, GoLoopType.PingPong)));
        text_flow_.play();
    }
	
    public void OnLevelComplete(Vector3 completed_local_position, int completed_font_size) {
        completed_ = true;
        completed_local_position_ = completed_local_position;

        text_.alignment = TextAnchor.MiddleCenter;

		if (text_flow_ != null) text_flow_.complete();
		text_flow_ = new GoTweenFlow();
		text_flow_.autoRemoveOnComplete = true;
        text_flow_.insert(0.0f, Go.to(transform, 0.5f, new GoTweenConfig().localPosition(completed_local_position).setEaseType(GoEaseType.QuadInOut)));
        text_flow_.insert(0.0f, Go.to(text_, 0.5f, new GoTweenConfig().intProp("fontSize", completed_font_size).setEaseType(GoEaseType.QuadInOut)));
        text_flow_.play();
	}

    void OnDestroy() {
        if (text_flow_ != null) text_flow_.destroy();
    }
}
