using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tangible;

public class OnScreenObject : MonoBehaviour {
    private int id_;
    private int unique_id_;
	private Tangible.TangibleObject.Shape shape_;
	private Deck deck_;
	private bool has_pressed_state_;
	private int pressed_id_;
	private int pressed_unique_id_;
	private bool is_pressed_;
	private bool is_simple_button_;

	public delegate void OnMotionEnd(OnScreenObject obj);
	
	private Vector3 originalPosition;
	private float originalAngle;
	
	public Vector3 OriginalPosition {
		get { return originalPosition; }
	}

	public float OriginalOrientation {
		get { return originalAngle; }
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
    private Tangible.TangibleObject tangible = null;

    bool IsActive(Vector3 pos) {
        return Mathf.Abs(pos.x) < onScreenController.AreaWidth / 2
            && Mathf.Abs(pos.y) < onScreenController.AreaHeight / 2 
			&& (!is_simple_button_ || !is_pressed_);
    }

    //----------------
    // MonoBehaviour
    //----------------

    void Start() {
    }

	public void Init(OnScreenController _controller, int _id, int _unique_id, Tangible.TangibleObject.Shape _shape, float width, float height, Deck deck, float initialAngle) {
		name = _shape + " " + _id;
		id_ = _id;
        unique_id_ = _unique_id;
		shape_ = _shape;
		transform.parent = _controller.transform;
		transform.localScale = new Vector3(width, height, 1.0f);
        onScreenController = _controller;
		deck_ = deck;
		screenToMillimeter = deck_.GetScreenToMillimeter ();
		originalPosition = transform.position;
		currentAngle = initialAngle;
		originalAngle = currentAngle;
		transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
		UpdateSnapPolygon();
		SetGraphics ();
	}

	void SetGraphics() {
		// Set the card texture and uv rectangle
		int curId = (is_pressed_ && has_pressed_state_) ? pressed_id_ : id_;
		int index = deck_.GetIndex (curId);
		deck_.AssignGraphics(index, this.gameObject);

		if (is_simple_button_) {
			Renderer renderer = GetComponentInChildren<Renderer> ();
			if (renderer != null) {
				renderer.enabled = !is_pressed_;
			}
		}
	}

	public void AddPressedId(int _pressedId, int _pressedUniqueId) {
		has_pressed_state_ = true;
		pressed_id_ = _pressedId;
		pressed_unique_id_ = _pressedUniqueId;
	}

	public void AddSimpleButton() { 
		is_simple_button_ = true;
	}

	void SetPressed(bool pressed) {
		if ((has_pressed_state_ || is_simple_button_) && pressed != is_pressed_) {
			is_pressed_ = pressed;
			SetGraphics ();
			OnMoving (transform.position, currentAngle);
		}
	}
	
	public void UpdateSnapPolygon() {
		Vector2[] points = TangramHelper.GetSnapPoints(shape_);
		if (points == null) return;
		if (snapPolygon == null || snapPolygon.Length != points.Length) snapPolygon = new Vector2[points.Length];
		for (int i = 0; i < points.Length; i++) {
			Vector3 p = transform.TransformPoint(points[i].x, points[i].y, 0);
			snapPolygon[i].x = p.x;
			snapPolygon[i].y = p.y;
		}
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

		if (Input.GetKey (KeyCode.P)) {
			SetPressed (true);
		}
	}
	
	public void MoveBy(Vector2 offset) {
		if (offset.sqrMagnitude == 0) return;
		transform.position = transform.position + new Vector3(offset.x, offset.y, 0);
		UpdateSnapPolygon();
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
			UpdateSnapPolygon();
			if (onMotionEnd != null) onMotionEnd(this);
		}
	}
	
	void OnMouseUp() {

		bool adjustAngle = false;

		if (Input.GetKey (KeyCode.S)) {
			currentAngle = 180f;
			adjustAngle = true;
		} else if (Input.GetKey (KeyCode.W)) {
			currentAngle = 0f;
			adjustAngle = true;
		} else if (Input.GetKey (KeyCode.A)) {
			currentAngle = 90f;
			adjustAngle = true;
		} else if (Input.GetKey (KeyCode.D)) {
			currentAngle = 270f;
			adjustAngle = true;
		} else if (Input.GetKey(KeyCode.R)) {
			currentAngle += 90f;
			adjustAngle = true;
		}

		if (adjustAngle) {
			transform.localRotation = Quaternion.Euler (0, 0, currentAngle);
			OnMoving (transform.position, currentAngle);
			UpdateSnapPolygon ();
		}

		onMouseUp = true;

		SetPressed (false);
	}
		
    void OnMouseDrag() {
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyDown(KeyCode.P)) OnMouseDown();
		
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
        
		UpdateSnapPolygon();
    }

	private void OnMoving(Vector3 pos, float angle) {
		if (IsActive(pos)) {
			int curId = is_pressed_ ? pressed_id_ : id_;
			int curUniqueId = is_pressed_ ? pressed_unique_id_ : unique_id_;
			if (tangible == null || curId != tangible.id) {
				if (tangible != null) {
					onScreenController.OnObjectExit (tangible);
				}
				tangible = new Tangible.TangibleObject(curId, curUniqueId, shape_);
				tangible.visible = true;
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
	
	private bool SetTangibleLocation(Tangible.TangibleObject tangible, Vector3 p, float orientation) {
		if (tangible == null) return false;
		if (tangible.location.X == p.x * screenToMillimeter && tangible.location.Y == p.y * screenToMillimeter && tangible.location.Orientation == orientation) return false;
		tangible.location.X = p.x * screenToMillimeter;
        tangible.location.Y = p.y * screenToMillimeter;
		tangible.location.Orientation = orientation;
		return true;
	}
}
