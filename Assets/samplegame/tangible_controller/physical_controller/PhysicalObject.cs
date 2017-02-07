using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalObject : MonoBehaviour {
    private TangibleObject tangible = null;
	public float visibleAlpha = 0.60f;
	public float hiddenAlpha = 0.30f;

	void Start() {
	}

	void OnDestroy() {
		MeshRenderer renderer = GetComponent<MeshRenderer>() as MeshRenderer;
		if (renderer == null) renderer = GetComponentInChildren<MeshRenderer>() as MeshRenderer;
		if (renderer != null) {
			Destroy(renderer.material);
		}
	}

	public int Id {
        get { return tangible != null ? tangible.Id : -1; }
    }
	/*
	public float LastVisible {
        get { return tangible != null ? tangible.lastVisible : float.NegativeInfinity; }
    }

	public bool Visible {
        get { return tangible != null ? tangible.visible : false; }
        set { if (tangible != null) tangible.visible = value; }
    }
*/
	public void Init(int _id, TangibleObject obj, bool _enabled) {
		if (tangible == null) tangible = new TangibleObject(_id);

		//tangible.lastVisible = UnityEngine.Time.timeSinceLevelLoad;
		gameObject.SetActive(_enabled);
	}

	public TangibleObject Persist() {
		if (tangible == null) return null;

		//tangible.visible = false;

		return tangible;
	}

	public TangibleObject UpdatePosition(float x, float y, float orientation/*, bool visible*/) {
		if (tangible == null) return null;

		tangible.location.x = x;
		tangible.location.y = y;
		tangible.location.orientation = orientation;
		//tangible.visible = visible;
		//if (visible) tangible.lastVisible = UnityEngine.Time.timeSinceLevelLoad;

		return tangible;
	}

	public void UpdateScreenPosition(Vector3 p, Quaternion r, bool visible) {
		transform.localPosition = p;
		transform.localRotation = r;
		if (tangible == null) return;
		MeshRenderer renderer = GetComponent<MeshRenderer>() as MeshRenderer;
		if (renderer == null) renderer = GetComponentInChildren<MeshRenderer>() as MeshRenderer;
		if (renderer != null) {
			renderer.material.SetFloat("_Alpha", visible ? visibleAlpha : hiddenAlpha);
		}
	}

	public bool IsCoveringAny(List<TangibleObject> tangibles) {
		float minDistSqr = Tangible.EventHelper.cardSize * Tangible.EventHelper.cardSize * 0.75f * 0.75f;
		foreach (TangibleObject t in tangibles) {
			float dx = t.location.x - tangible.location.x;
			float dy = t.location.y - tangible.location.y;
			if (dx * dx + dy * dy < minDistSqr) return true;
		}
		return false;
	}
}
