using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloatyText : MonoBehaviour {
	const int FLOATY_TEXT_OFFSET = 25;

    public enum Behavior {
        FLOAT,
        FOLLOW,
    }
	
	private int float_distance_ = 50;

    private Text text_;
	private GoTweenFlow text_flow_;
    private Transform anchor_;
	private Vector3 spawn_pos_;
	private static Vector3 override_pos_;

    public float Alpha {
        get { return text_.color.a; }
        set { text_.color = TweenHelper.FadedColor(text_.color, value); }
    }

	public static FloatyText Create(Transform anchor, string msg, FloatyText.Behavior behavior = FloatyText.Behavior.FLOAT, Vector3 override_pos = default(Vector3), bool init = true, string prefabOverride = null) {
		string path = "prefabs/FloatyText";
		if (prefabOverride != null) {
			path = prefabOverride;
		}
		FloatyText text = (Instantiate(Resources.Load(path)) as GameObject).GetComponent<FloatyText>();

		override_pos_ = override_pos;

		if (init) text.Init(anchor, msg, behavior, override_pos_); 
        return text;
    }

    Vector3 ClampToScreen(Vector3 pos) {
		Text text = GetComponentInChildren<Text> ();
        float text_width = Mathf.Min(text.preferredWidth, text.rectTransform.sizeDelta.x) * 0.5f;
		float text_height = text.preferredHeight;
        Vector3 camera_center = Camera.main.transform.position;
        float camera_width = 768f * Game.CameraZoomMultiplier * 0.5f;
		float camera_height = 1024f * Game.CameraZoomMultiplier * 0.5f;
		pos.x = Mathf.Clamp(pos.x, camera_center.x - camera_width + text_width, camera_center.x + camera_width - text_width);
		pos.y = Mathf.Clamp(pos.y, camera_center.y - camera_height + text_height, camera_center.y + camera_height - text_height);
		return pos;
    }

	public void Init(Transform anchor, string message, Behavior behavior, Vector3 override_pos = default(Vector3)) {
		text_ = GetComponentInChildren<Text>();

		if (message != null) {
			text_.horizontalOverflow = HorizontalWrapMode.Overflow;
			string[] words = message.Split ();
			float minWidth = 0;
			foreach (string word in words) {
				text_.text = word;
				float width = text_.preferredWidth;
				if (width > minWidth) {
					minWidth = width;
				}
			}

			Vector2 rectSize = text_.rectTransform.sizeDelta;
			if (rectSize.x < minWidth) {
				rectSize.x = minWidth;
				text_.rectTransform.sizeDelta = rectSize;
			}
			text_.horizontalOverflow = HorizontalWrapMode.Wrap;
		}

        text_.text = message;

        gameObject.name = "FloatyText (" + message + ")";

        if (override_pos != default(Vector3)) override_pos_ = override_pos;

		if (anchor == null || override_pos_ != Vector3.zero) {
			spawn_pos_ = override_pos_;
		} else {
			spawn_pos_ = anchor.position;
		}

		transform.position = ClampToScreen(new Vector3(spawn_pos_.x, spawn_pos_.y + FLOATY_TEXT_OFFSET, 0));
        transform.SetParent(Game.Level.LevelUIRoot);

        if (behavior == Behavior.FOLLOW) {
            anchor_ = anchor;
        } else {
            FloatAndFade();
        }
    }
	
	void FloatAndFade() {
        if (text_flow_ != null) text_flow_.destroy();
		text_flow_ = new GoTweenFlow();
		text_flow_.autoRemoveOnComplete = true;
		text_flow_.insert(0.0f, Go.to(transform, 4f, new GoTweenConfig().position(new Vector3(transform.position.x, transform.position.y + float_distance_, transform.position.z)).setEaseType(GoEaseType.QuadOut)));
        text_flow_.insert(0.0f, Go.to(this, 4f, new GoTweenConfig().floatProp("Alpha", 0).setEaseType(GoEaseType.CubicIn)));
        text_flow_.setOnCompleteHandler(thisTween => StartCoroutine(SelfDestroy()));
		text_flow_.play();
	}

	public void FadeAndDestroy() {
		if (text_flow_ != null) text_flow_.destroy();
		text_flow_ = new GoTweenFlow();
		text_flow_.autoRemoveOnComplete = true;
		text_flow_.insert(0.0f, Go.to(this, 1.5f, new GoTweenConfig().floatProp("Alpha", 0).setEaseType(GoEaseType.CubicIn)));
		text_flow_.setOnCompleteHandler(thisTween => StartCoroutine(SelfDestroy()));
		text_flow_.play();
	}

    public void Unfollow() {
        anchor_ = null;
    }

	public IEnumerator SelfDestroy() {
        yield return new WaitForEndOfFrame();
		if (this != null) GameObject.Destroy(this.gameObject);
	}

	void OnDestroy() {
		if (text_flow_ != null) text_flow_.destroy();
	}

    void Update() {
        if (anchor_ != null) {
			if (override_pos_ != Vector3.zero) {
				transform.position = ClampToScreen(new Vector3(spawn_pos_.x, spawn_pos_.y + FLOATY_TEXT_OFFSET, 0));
			} else {
				transform.position = ClampToScreen(new Vector3(anchor_.position.x, anchor_.position.y + FLOATY_TEXT_OFFSET, 0));
			}
        }
    }
}