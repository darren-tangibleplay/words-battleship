using System.Collections;
using System.Collections.Generic;
using Tangible;
using UnityEngine;

public class OnScreenObject : MonoBehaviour {
    private int id;
	public delegate void OnMotionEnd(OnScreenObject obj);

	private Vector3 originalPosition;
	private float originalAngle;

	public Vector3 OriginalPosition {
		get { return originalPosition; }
	}

	public float OriginalOrientation {
		get { return OriginalOrientation; }
	}

	private Vector3 downPosition;
	private Vector3 downOffset;
	private float downAngle;
	private float referenceAngle;
	private float currentAngle;
	private float screenToMillimeter;
	private bool onMouseUp = false;
	private bool toOriginal = false;
	public OnMotionEnd onMotionEnd;

	public Vector2[] snapPolygon;

	private OnScreenController onScreenController;
    private TangibleObject tangible = null;

    bool IsActive(Vector3 pos) {
        return Mathf.Abs(pos.x) < onScreenController.areaWidth / 2
            && Mathf.Abs(pos.y) < onScreenController.areaHeight / 2;
    }

    //----------------
    // MonoBehaviour
    //----------------

    void Start() {
    }

	public void Init(OnScreenController _controller, int _id, float cardSize, float _screenToMillimeter) {
		name = _id.ToString();
		id = _id;
		transform.parent = _controller.transform;
		transform.localScale = new Vector3(cardSize, cardSize, 1.0f);
        onScreenController = _controller;
		screenToMillimeter = _screenToMillimeter;
		originalPosition = transform.position;
		originalAngle = currentAngle;
	}

	public void AnimateToOriginal() {
		toOriginal = true;
	}

	void OnMouseDown() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        downPosition = ray.GetPoint(-ray.origin.z / ray.direction.z);
        downPosition.z = transform.position.z;
		downOffset = transform.position - downPosition;
		downAngle = currentAngle;
	}

	public void MoveBy(Vector2 offset) {
		if (offset.sqrMagnitude == 0) return;
		transform.position = transform.position + new Vector3(offset.x, offset.y, 0);

		if (SetTangibleLocation(tangible, transform.position, currentAngle)) {
            onScreenController.OnObjectUpdate(tangible);
		}
	}

	void Update() {
		bool motion = false;
		// Angle smooth snapping
		float angle = toOriginal ? originalAngle : Mathf.Round(currentAngle / 15.0f) * 15.0f;
		Vector3 pos = toOriginal ? originalPosition : transform.position;
		if (angle != currentAngle || pos != transform.position) {
			if (Mathf.Abs(angle - currentAngle) > 0.1f) {
				angle = currentAngle * 0.9f + 0.1f * angle;
			}
			transform.localRotation = Quaternion.Euler(0, 0, angle);
			currentAngle = angle;

			if (Vector3.Distance(pos, transform.position) > 1.0f) {
				pos = transform.position * 0.95f + 0.05f * pos;
			}

			transform.position = pos;

			OnMoving(pos, angle);

			motion = true;
		}

		if (toOriginal) {
			toOriginal = originalAngle != currentAngle || transform.position != originalPosition;
		}

		if(onMouseUp && !motion) {
			onMouseUp = false;
			if (onMotionEnd != null) onMotionEnd(this);
		}
	}

	void OnMouseUp() {
		onMouseUp = true;
	}

    void OnMouseDrag() {
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyUp(KeyCode.Space)) OnMouseDown();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mousePosition = ray.GetPoint(-ray.origin.z / ray.direction.z);
        mousePosition.z = transform.position.z;
		Vector3 delta = mousePosition - downPosition;
		Vector3 pos = transform.position;
		float angle = transform.localRotation.eulerAngles.z;

		if (Input.GetKey(KeyCode.Space)) {
			angle = downAngle + delta.x;
			transform.localRotation = Quaternion.Euler(0, 0, angle);
			currentAngle = angle;
		} else {
	        pos = downPosition + delta + downOffset;
			transform.position = pos;
			currentAngle = angle;
		}
		OnMoving(pos, angle);
    }

	private void OnMoving(Vector3 pos, float angle) {
		if (IsActive(pos)) {
            if (tangible == null) {
                tangible = new TangibleObject(id);
				//tangible.visible = true;
				SetTangibleLocation(tangible, pos, angle);
                onScreenController.OnObjectEnter(tangible);
            } else if (SetTangibleLocation(tangible, pos, angle)) {
                onScreenController.OnObjectUpdate(tangible);
            }
        } else if (tangible != null) {
            onScreenController.OnObjectExit(tangible);
			transform.localRotation = Quaternion.Euler(0, 0, angle);
			tangible = null;
        }
	}

	private bool SetTangibleLocation(TangibleObject tangible, Vector3 p, float orientation) {
		if (tangible == null) return false;
		if (tangible.location.x == p.x * screenToMillimeter && tangible.location.y == p.y * screenToMillimeter && tangible.location.orientation == orientation) return false;
		tangible.location.x = p.x * screenToMillimeter;
        tangible.location.y = p.y * screenToMillimeter;
		tangible.location.orientation = orientation;
		return true;
	}
}
