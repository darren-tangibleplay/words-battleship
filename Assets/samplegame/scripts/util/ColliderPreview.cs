using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ColliderPreview : MonoBehaviour {

    [SerializeField]
    float default_radius_ = -1;

    void OnDrawGizmos() {
        Collider2D collider = GetComponent<Collider2D>();
        Gizmos.color = Color.yellow;

        if (collider is BoxCollider2D) {
            BoxCollider2D c = collider as BoxCollider2D;
            Vector3 p0 = new Vector3(c.size.x / 2, c.size.y / 2, 0);
            Vector3 p1 = new Vector3(-c.size.x / 2, c.size.y / 2, 0);
            Vector3 p2 = new Vector3(-c.size.x / 2, -c.size.y / 2, 0);
            Vector3 p3 = new Vector3(c.size.x / 2, -c.size.y / 2, 0);
            p0 = transform.localToWorldMatrix.MultiplyPoint3x4(p0);
            p1 = transform.localToWorldMatrix.MultiplyPoint3x4(p1);
            p2 = transform.localToWorldMatrix.MultiplyPoint3x4(p2);
            p3 = transform.localToWorldMatrix.MultiplyPoint3x4(p3);
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p0);
        } else if (collider is CircleCollider2D) {
            CircleCollider2D c = collider as CircleCollider2D;
            float radius = c.radius;
            for (int i=0; i<24; i++) {
                float angle0 = 2 * Mathf.PI * i / 24.0f;
                float angle1 = 2 * Mathf.PI * (i + 1) / 24.0f;
                Vector3 p0 = new Vector3(Mathf.Cos(angle0), Mathf.Sin(angle0), 0);
                Vector3 p1 = new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0);
                p0 = transform.localToWorldMatrix.MultiplyPoint3x4(p0 * radius);
                p1 = transform.localToWorldMatrix.MultiplyPoint3x4(p1 * radius);
                Gizmos.DrawLine(p0, p1);
            }
        } else if (default_radius_ > 0) {
			Color color = Color.yellow;
			color.a = 0.5f;
			Gizmos.color = color; 
            float radius = default_radius_;
            for (int i=0; i<24; i++) {
                float angle0 = 2 * Mathf.PI * i / 24.0f;
                float angle1 = 2 * Mathf.PI * (i + 1) / 24.0f;
                Vector3 p0 = new Vector3(Mathf.Cos(angle0), Mathf.Sin(angle0), 0);
                Vector3 p1 = new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0);
                p0 = transform.localToWorldMatrix.MultiplyPoint3x4(p0 * radius);
                p1 = transform.localToWorldMatrix.MultiplyPoint3x4(p1 * radius);
                Gizmos.DrawLine(p0, p1);
            }
        }
    }
}
