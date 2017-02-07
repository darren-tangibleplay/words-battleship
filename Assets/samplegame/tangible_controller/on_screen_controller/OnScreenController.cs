using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class OnScreenController : MonoBehaviour, Controller {

    // Config
    public float areaWidth { get { return Screen.width; } }
    public float areaHeight { get { return Screen.height; } }
    public float cardSizeScreen;
	public Transform margins;

	public float scaleFactor = 0.4f;

    // Controller
    private TangibleObject.EventDelegate processEvent = null;
    private float fovFactor = 1.0f;
    private bool dispatchRequested = false;
    private List<TangibleObject> objects = new List<TangibleObject>();
	private Deck deck;
	private float originalCameraFOV;
	private float originalOrthographicSize;
	private float screenToMillimeter;
	private float sizeScreen;
	private bool mute = false;

    //----------
    // Methods
    //----------

	public void Init(Deck _deck) {
		deck = _deck;
		sizeScreen = cardSizeScreen;
		float marginScreen = cardSizeScreen;
		fovFactor = 1.0f + Mathf.Max(marginScreen / areaWidth, marginScreen / areaHeight) * 1.25f;
		screenToMillimeter = deck.GetSizeMillimeter() / sizeScreen;
	}

	private void Activate() {
		originalCameraFOV = Camera.main.fieldOfView;
		originalOrthographicSize = Camera.main.orthographicSize;
		if (Camera.main.orthographic) {
			Camera.main.orthographicSize *= fovFactor * 1.14f;
		} else {
	        Camera.main.fieldOfView *= fovFactor;
		}

		// margins.localScale = new Vector3(areaWidth, areaHeight, 1.0f);

        CreateOnScreenObjects();
	}

	private void Deactivate() {
		DeleteCards();

		Camera.main.fieldOfView = originalCameraFOV;
		Camera.main.orthographicSize = originalOrthographicSize;
	}

    public void OnObjectEnter(TangibleObject card) {
        objects.Add(card);
        dispatchRequested = true;
    }

    public void OnObjectUpdate(TangibleObject card) {
        dispatchRequested = true;
    }

    public void OnObjectExit(TangibleObject card) {
        objects.Remove(card);
        dispatchRequested = true;
    }

	public void AnimateToOriginal() {
		OnScreenObject[] objs = gameObject.GetComponentsInChildren<OnScreenObject>();
		foreach (OnScreenObject obj in objs) {
			obj.AnimateToOriginal();
		}
	}

	public void OnMotionEnd(OnScreenObject last) {
		List<Vector2[]> polygons = new List<Vector2[]>();
		OnScreenObject[] objs = gameObject.GetComponentsInChildren<OnScreenObject>();
		foreach (OnScreenObject obj in objs) {
			if (obj == last) continue;
			polygons.Add(obj.snapPolygon);
		}
		Vector2 offset = Tangible.SnapHelper.SnapOffset(last.snapPolygon, polygons);
		last.MoveBy(offset);
	}

    private void DispatchEvent() {
        if (processEvent == null) {
			Debug.LogError("NO processEvent set");
            return;
		}
        dispatchRequested = false;
		if (mute) return;
		VisionEventInput e = new VisionEventInput(objects);

        processEvent(e);
    }

	void CreateOnScreenObjects() {
		if (deck == null) Debug.LogError("A Deck must be specified on the Controller");
		float margin = (Mathf.Min(areaWidth, areaHeight) * scaleFactor) * (fovFactor - 1.0f) / 1.1f;
        int horizontalCount = (int)(areaWidth / margin) + 1;
        int verticalCount = (int)(areaHeight / margin) + 1;
        Vector3 p = margins.localPosition;
        for (int k = 0; k < deck.GetCount(); k++) {
            // Create a card at the right position
            if (k < horizontalCount) {
                p.x = (k + 0.5f) * margin - areaWidth / 2.0f;
                p.y = (areaHeight + margin) / 2.0f;
            } else if (k < horizontalCount + verticalCount) {
                p.x = (areaWidth + margin) / 2.0f;
                p.y = areaHeight / 2.0f - (k - horizontalCount + 0.5f) * margin;
            } else if (k < 2 * horizontalCount + verticalCount) {
                p.x = areaWidth / 2.0f - (k - horizontalCount - verticalCount + 0.5f) * margin;
                p.y = -(areaHeight + margin) / 2.0f;
            } else if (k < 2 * horizontalCount + 2 * verticalCount) {
                p.x = -(areaWidth + margin) / 2.0f;
                p.y = -areaHeight / 2.0f + (k - 2 * horizontalCount - verticalCount + 0.5f) * margin;
            }

            OnScreenObject card = Instantiate(deck.GetPrefab(), p, Quaternion.identity) as OnScreenObject;
			card.Init(this, TangibleObject.IndexToId(k), sizeScreen * scaleFactor, screenToMillimeter);
			card.onMotionEnd = OnMotionEnd;

            // Set the card texture and uv rectangle
			MeshRenderer cardRenderer = card.GetComponentInChildren<MeshRenderer>();
			deck.AssignGraphics(k, cardRenderer);
        }
    }

	void DeleteCards() {
		OnScreenObject[] objs = gameObject.GetComponentsInChildren<OnScreenObject>();
		foreach (OnScreenObject obj in objs) {
			GameObject.Destroy(obj.gameObject);
		}
	}

    //----------------
    // MonoBehaviour
    //----------------

	void Controller.Mute(bool _mute) {
		mute = _mute;
	}

    void Update() {
        if (dispatchRequested) {
            DispatchEvent();
        }
		if (Input.touchCount > 0) {
//			Debug.Log("touchCount " + Input.touchCount);
		}

        if (Input.GetKeyDown(KeyCode.Backspace)) {
            AnimateToOriginal();
        }
    }

    //----------------------
    // Controller
    //----------------------

    bool Controller.Register(TangibleObject.EventDelegate _processEvent) {
        processEvent = _processEvent;
		Activate();
		return true;
    }

    void Controller.Unregister() {
        processEvent = null;
		Deactivate();
    }
}
