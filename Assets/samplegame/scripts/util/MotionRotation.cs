using UnityEngine;
using System.Collections;

public class MotionRotation : MonoBehaviour {

    Vector3 last_position_;

	void Start() {
        last_position_ = transform.position;
	}
	
	void Update() {
        Vector3 position = transform.position;
        Vector3 delta = transform.position - last_position_;
        delta.z = 0;
        if (Vector3.SqrMagnitude(delta) < 0.00001f) return;

        delta.Normalize();

        transform.rotation = Quaternion.FromToRotation(Vector3.right, delta);

        last_position_ = position;
	}
}
