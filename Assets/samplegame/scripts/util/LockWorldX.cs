using UnityEngine;
using System.Collections;

public class LockWorldX : MonoBehaviour {

    [SerializeField]
    float world_x_ = 0;

	void Update() {
        Vector3 p = transform.position;
        p.x = world_x_;
        transform.position = p;
	}
}
