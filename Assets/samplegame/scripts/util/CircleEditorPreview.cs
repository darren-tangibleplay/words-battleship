using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CircleEditorPreview : MonoBehaviour {

#if UNITY_EDITOR
    void OnDrawGizmos() {
        CircleAA circle = GetComponent<CircleAA>();
        if (circle != null) {
            circle.EditorForceUpdateGeometry();
        }
    }
#endif
}
