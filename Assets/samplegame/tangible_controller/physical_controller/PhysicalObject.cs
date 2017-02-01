using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicalObject : MonoBehaviour {
    private Tangible.TangibleObject tangible_ = null;
	public float visible_alpha = 0.60f;
	public float hidden_alpha = 0.30f;

	void Start() {
	}

	void OnDestroy() {
		MeshRenderer renderer = GetComponent<MeshRenderer>() as MeshRenderer;
		if (renderer == null) renderer = GetComponentInChildren<MeshRenderer>() as MeshRenderer;
		if (renderer != null) {
			DestroyImmediate(renderer.material);
		}
	}

	public int UniqueId {
        get { return tangible_ != null ? tangible_.unique_id : -1; }
    }

	public Tangible.TangibleObject.Shape Shape {
        get { return tangible_ != null ? tangible_.shape : Tangible.TangibleObject.Shape.card; }
    }

	public float LastVisible {
        get { return tangible_ != null ? tangible_.lastVisible : float.NegativeInfinity; }
    }

	public bool Visible {
        get { return tangible_ != null ? tangible_.visible : false; }
        set { if (tangible_ != null) tangible_.visible = value; }
    }

    public void Init(Tangible.TangibleObject tangible, bool _enabled) {
        tangible_ = tangible;
		gameObject.SetActive(_enabled);
	}
			
	public void UpdateScreenPosition(Vector3 p, Quaternion r, bool visible) {
		transform.localPosition = p;
		transform.localRotation = r;
		MeshRenderer renderer = GetComponent<MeshRenderer>() as MeshRenderer;
		if (renderer == null) renderer = GetComponentInChildren<MeshRenderer>() as MeshRenderer;
		if (renderer != null) {
			renderer.material.SetFloat("_Alpha", visible ? visible_alpha : hidden_alpha);
		}
	}
}
