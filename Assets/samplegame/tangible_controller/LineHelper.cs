using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineHelper : MonoBehaviour {
    
    private Mesh mesh_;

    [SerializeField]
    bool flat_ = true;
    
    class Point {
        public Vector3 p;
        public Color c;
        public float u;
        public float thickness;
        public Point(Vector3 _p, Color _c, float _u, float _thickness) {
            p = _p;
            c = _c;
            u = _u;
            thickness = _thickness;
        }
    }
    
    List<Vector3> debug_control_points_ = new List<Vector3>();
    List<Point> points_ = new List<Point>();
    List<int> line_lengths_ = new List<int>();
    int tri_count = 0;
    int line_length_ = 0;

	// doing simple pooling of points here for now since it's a big source of allocation/garbage collection
	// should probably move towards a more generic pooling solution
	Stack<Point> point_pool_ = new Stack<Point>();

	Point GetPoint(Vector3 p, Color c, float u, float thickness) {
		Point newPoint;
		if (point_pool_.Count > 0) {
			newPoint = point_pool_.Pop();
			newPoint.p = p;
			newPoint.c = c;
			newPoint.u = u;
			newPoint.thickness = thickness;
		} else {
			newPoint = new Point(p, c, u, thickness);
		}
		return newPoint;
	}

	void OnPointRemoved(Point pointRemoved) {
		point_pool_.Push(pointRemoved);
	}

    void OnDestroy() {
        DestroyImmediate(GetComponent<Renderer>().material);
    }
    
    void Start() {
        mesh_ = new Mesh();
        mesh_.MarkDynamic();
        
        MeshFilter mesh_filter = gameObject.GetComponent<MeshFilter>();
        if (mesh_filter == null) {
            mesh_filter = gameObject.AddComponent<MeshFilter>();
        }
        mesh_filter.mesh = mesh_;
        
        if (gameObject.GetComponent<Renderer>() == null) {
            Debug.LogError("Mesh renderer expected on " + name + ". Abort.");
            return;
        }
        
        UpdateGeometry();        
    }
    
    public void Clear() {
        debug_control_points_.Clear();
		for (int i = 0; i < points_.Count; i++) {
			OnPointRemoved(points_[i]);
		}
        points_.Clear();
        line_lengths_.Clear();
        tri_count = 0;

        UpdateGeometry();
    }
    
    public void DrawLine(Vector3 p1, Vector3 p2, Color c, float thickness = 1.0f) {
        Add(p1, p2, c, c, thickness, thickness);
    }

    public void DrawCircle(Vector3 p, float radius, Color c, float thickness = 1.0f) {
        Vector3 p1 = p + new Vector3(radius, 0, 0);
        for (int i = 1; i <= 16; i++) {
            float angle = i * 2.0f * Mathf.PI / 16.0f;
            Vector3 p2 = p + new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
            Add(p1, p2, c, c, thickness, thickness);
            p1 = p2;
        }
    }

    public void DrawCircle(Vector3 p, float radius, float anle_from, float angle_to, Color c, float thickness = 1.0f) {
        float radian = anle_from * Mathf.PI / 180.0f;
        Vector3 p1 = p + new Vector3(radius * Mathf.Cos(radian), radius * Mathf.Sin(radian), 0);
        for (int i = 1; i <= 16; i++) {
            radian = Mathf.Lerp(anle_from, angle_to, i / 16.0f) * Mathf.PI / 180.0f;
            Vector3 p2 = p + new Vector3(radius * Mathf.Cos(radian), radius * Mathf.Sin(radian), 0);
            Add(p1, p2, c, c, thickness, thickness);
            p1 = p2;
        }
    }

    public void DrawDashedCircle(Vector3 p, float radius, Color c, int segments, float thickness = 1.0f) {
        for (int i = 1; i < segments * 2; i += 2) {
            float a1 = (i-1) * Mathf.PI / segments;
            float a2 = i * Mathf.PI / segments;
            Vector3 p1 = p + new Vector3(radius * Mathf.Cos(a1), radius * Mathf.Sin(a1), 0);
            Vector3 p2 = p + new Vector3(radius * Mathf.Cos(a2), radius * Mathf.Sin(a2), 0);
            Add(p1, p2, c, c, thickness, thickness);
        }
    }

    public float ComputeLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        Vector3 previous = p0;
        float l = 0;
        for (int i = 1; i <= 20; i++) {
            float t = (float) i / ((float) 20);
            Vector3 m0 = Vector3.Lerp(p0, p1, t);
            Vector3 m1 = Vector3.Lerp(p1, p2, t);
            Vector3 m2 = Vector3.Lerp(p2, p3, t);
            Vector3 n0 = Vector3.Lerp(m0, m1, t);
            Vector3 n1 = Vector3.Lerp(m1, m2, t);
            Vector3 current = Vector3.Lerp(n0, n1, t);
            l += Vector3.Distance(current, previous);
            previous = current;
        }
        return l;
    }

    void CheckNaN(Vector3 p) {
        if (float.IsNaN(p.x) || float.IsNaN(p.y) || float.IsNaN(p.z)) {
            Debug.LogError("NaN Vector");
        }
    }

    public Vector3 DrawCurvedLine(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color c0, Color c3, int segments, float thickness = 1.0f, float progression = 1.0f, float u_length = 1.0f) {
        Vector3 p = p0;
        if (progression < 0.0001f) return p;
        segments = Mathf.Max(1, Mathf.CeilToInt(segments * progression + 0.1f));
        for (int i = 0; i <= segments; i++) {
            float t = progression * ((float) i) / ((float) segments);
            Vector3 m0 = Vector3.Lerp(p0, p1, t);
            Vector3 m1 = Vector3.Lerp(p1, p2, t);
            Vector3 m2 = Vector3.Lerp(p2, p3, t);
            Vector3 n0 = Vector3.Lerp(m0, m1, t);
            Vector3 n1 = Vector3.Lerp(m1, m2, t);
            p = Vector3.Lerp(n0, n1, t);
            Add(p, Color.Lerp(c0, c3, t), t * u_length, thickness, i == segments);
        }

        #if UNITY_EDITOR
        debug_control_points_.Add(p0);
        debug_control_points_.Add(p1);
        
        debug_control_points_.Add(p1);
        debug_control_points_.Add(p2);
        
        debug_control_points_.Add(p2);
        debug_control_points_.Add(p3);
        #endif

        return p;
    }

    public Vector3 DrawCurvedLine(Vector3 p0, Vector3 p1, Vector3 p2, Color c0, Color c3, int segments, float thickness = 1.0f, float progression = 1.0f, float u_length = 1.0f) {
        Vector3 p = p0;
        if (progression < 0.0001f) return p;
        segments = Mathf.Max(1, Mathf.CeilToInt(segments * progression + 0.1f));
        for (int i = 1; i <= segments; i++) {
            float t = progression * ((float) i) / ((float) segments);
            Vector3 n0 = Vector3.Lerp(p0, p1, t);
            Vector3 n1 = Vector3.Lerp(p1, p2, t);
            p = Vector3.Lerp(n0, n1, t);
            Add(p, Color.Lerp(c0, c3, t), t * u_length, thickness, i == segments);
        }

        #if UNITY_EDITOR
        debug_control_points_.Add(p0);
        debug_control_points_.Add(p1);
        
        debug_control_points_.Add(p1);
        debug_control_points_.Add(p2);
        #endif

        return p;
    }

	public void DrawDashedLine(Vector3 p1, Vector3 p2, Color c, float dashLength, float spaceLength, float thickness = 1.0f) {
		AddDashed(p1, p2, c, dashLength, spaceLength, thickness);
	}
    
    public void Commit() {
        UpdateGeometry();
    }
    
    void Add(Vector3 p1, Vector3 p2, Color c1, Color c2, float thickness1, float thickness2) {
#if UNITY_EDITOR
        CheckNaN(p1);
        CheckNaN(p2);
#endif
        points_.Add(GetPoint(p1, c1, 0, thickness1));        
        points_.Add(GetPoint(p2, c2, 1, thickness2));  
        line_lengths_.Add(2);
        tri_count += 2;

        #if UNITY_EDITOR
        debug_control_points_.Add(p1);
        debug_control_points_.Add(p2);
        #endif
    }

    public void Add(Vector3 p, Color c, float u, float thickness, bool close) {
#if UNITY_EDITOR
        CheckNaN(p);
#endif
        points_.Add(GetPoint(p, c, u, thickness));        
        line_length_++;
        if (close) {
            line_lengths_.Add(line_length_);
            line_length_ = 0;
        } else {
            tri_count += 2;
        }

        #if UNITY_EDITOR
        if (points_.Count > 2) {
            debug_control_points_.Add(points_[points_.Count - 2].p);
            debug_control_points_.Add(p);
        }
        #endif
    }

	void AddDashed(Vector3 p1, Vector3 p2, Color c, float dashLength, float spaceLength, float thickness) {
		float totalLength = Vector3.Distance (p1, p2);
		Vector3 dir = p2 - p1;
		dir.Normalize();
		float curLength = 0f;
		Vector3 nextPoint = p1;

		// add segments for dashes until there isn't space left for a full dash
		while ((curLength + dashLength) < totalLength) {
            Add(nextPoint, nextPoint + dashLength * dir, c, c, thickness, thickness);
			nextPoint = nextPoint + (dashLength + spaceLength) * dir;
			curLength = curLength + dashLength + spaceLength;
		}

		// add any fractional dash at the end needed to fill up the rest of the length
		if (curLength < totalLength) {
            Add(nextPoint, nextPoint + (totalLength - curLength) * dir, c, c, thickness, thickness);
		}    
	}
    
	Vector3 previous_point = new Vector3(0,0,0);
	Vector3 previous_previous = new Vector3(0,0,0);
	Vector3 previous_n = new Vector3(0,0,0);
	Vector3 dir = new Vector3(0,0,0);
	Vector3 n = new Vector3(0,0,0);
	Vector3 forward = Vector3.forward;
	Vector3 up = Vector3.up;
	Vector3 min_point = new Vector3(0,0,0);
	Vector3 max_point = new Vector3(0,0,0);

	Vector3[] pts = null;
	Color32[] cls = null;
	Vector2[] uvs = null;
	int[] tris = null;
	int lastPointsCount = -1;
	int lastTriCount = -1;
	Vector2 emptyVector2 = Vector2.zero;
	Vector3 emptyVector3 = Vector3.zero;
	Color32 emptyColor = new Color32(0x00, 0x00, 0x00, 0x00);
	

    void UpdateGeometry() {
        if (mesh_ == null) return;
        mesh_.Clear();

		int numPoints = points_.Count;
		int lineCount = line_lengths_.Count;
        
        if (numPoints < 2 || tri_count == 0 || lineCount == 0) return;

		// to reduce the amount of garbage collection from creating new arrays every frame,
		// keeping it if it is big enough to fit the current number of points and filling any
		// extra at the end with zeros
		if (pts == null || lastPointsCount < numPoints) {
			pts = new Vector3[2 * numPoints];
			cls = new Color32[2 * numPoints];
			uvs = new Vector2[2 * numPoints];
			lastPointsCount = numPoints;
		}

		for (int i = numPoints * 2; i < lastPointsCount * 2; i++) {
			pts[i] = emptyVector3;
			cls[i] = emptyColor;
			uvs[i] = emptyVector2;
		}

		if (tris == null || lastTriCount < tri_count) {
			tris = new int[3 * tri_count];
			lastTriCount = tri_count;
		}

		for (int i = tri_count * 3; i < lastTriCount * 3; i++) {
			tris[i] = 0;
		}

        int start_index = 0;
        int tri_index = 0;
		Color c;
		Point point1;
		Point point2;
		Point curPoint;
        for (int line = 0; line < lineCount; line++) {
            int line_length = line_lengths_[line];
            if (line_length >= 2) {
				point1 = points_[start_index];
				point2 = points_[start_index + 1];
				previous_point.x = 2 * point1.p.x - point2.p.x;
				previous_point.y = 2 * point1.p.y - point2.p.y;
				previous_point.z = 2 * point1.p.z - point2.p.z;
				previous_previous.x = 2 * previous_point.x - point1.p.x + forward.x;
				previous_previous.y = 2 * previous_point.y - point1.p.y + forward.y;
				previous_previous.z = 2 * previous_point.z - point1.p.z + forward.z;
				previous_n.x = up.x;
				previous_n.y = up.y;
				previous_n.z = up.z;
                for (int i = start_index; i < start_index + line_length; i++) {
					curPoint = points_[i];
                    dir.x = previous_point.x - curPoint.p.x;
					dir.y = previous_point.y - curPoint.p.y;
					dir.z = previous_point.z - curPoint.p.z;
					c = curPoint.c;
					if (flat_) {
                        dir.z = 0;
                        if (dir.sqrMagnitude > 0.0001f) dir.Normalize();
						n.x = -dir.y;
						n.y = dir.x;
						n.z = 0;
                    } else {
                        if (dir.sqrMagnitude > 0.0001f) dir.Normalize();
                        Vector3 previous_dir = previous_previous - previous_point;
                        n = Vector3.Cross(previous_dir, dir);
                        if (n.sqrMagnitude > 0.0001f) n.Normalize();
                        else n = previous_n;
                        n = Vector3.Lerp(n, previous_n, 0.8f);
                        c.a *= Mathf.Pow(Mathf.Clamp01(Mathf.Abs(Vector3.Cross(n, dir).z)), 2.0f);
                    }
                    n *= 2.0f;
					Color32 c32 = new Color32((byte) Mathf.RoundToInt(c.r * 255), (byte) Mathf.RoundToInt(c.g * 255), (byte) Mathf.RoundToInt(c.b *255), (byte) Mathf.RoundToInt(c.a * 255));;

					pts[2 * i + 0] = curPoint.p + curPoint.thickness * n;
					cls[2 * i + 0] = c32;
					uvs[2 * i + 0] = new Vector2(curPoint.u, 0);
					
					pts[2 * i + 1] = curPoint.p - curPoint.thickness * n;
					cls[2 * i + 1] = c32;
					uvs[2 * i + 1] = new Vector2(curPoint.u, 1);
					
					previous_n = n * 0.5f;
                    previous_previous = previous_point;
					previous_point = curPoint.p;
				}
				
				for (int i = start_index; i < (start_index + line_length - 1); i++) {
                    tris[tri_index + 0] = 2 * i + 1;
                    tris[tri_index + 1] = 2 * i + 2;
                    tris[tri_index + 2] = 2 * i + 0;
                    
                    tris[tri_index + 3] = 2 * i + 1;
                    tris[tri_index + 4] = 2 * i + 3;
                    tris[tri_index + 5] = 2 * i + 2;

                    tri_index += 6;
                }

                start_index += line_length;
            }
        }

		min_point.x = points_[0].p.x;
		min_point.y = points_[0].p.y;
		min_point.z = points_[0].p.z;
		max_point.x = points_[0].p.x;
		max_point.y = points_[0].p.y;
		max_point.z = points_[0].p.z;
        foreach (Vector3 p in pts) {
			min_point.x = Mathf.Min(p.x, min_point.x);
			min_point.y = Mathf.Min(p.y, min_point.y);
			min_point.z = Mathf.Min(p.z, min_point.z);
			max_point.x = Mathf.Max(p.x, max_point.x);
			max_point.y = Mathf.Max(p.y, max_point.y);
			max_point.z = Mathf.Max(p.z, max_point.z);
        }
        
        mesh_.vertices = pts;
        mesh_.uv = uvs;
        mesh_.colors32 = cls;
        mesh_.bounds = new Bounds((min_point + max_point) * 0.5f, max_point - min_point);
        mesh_.triangles = tris;
    }

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        if (gameObject != UnityEditor.Selection.activeGameObject) return;
        if (mesh_ == null) return;

        Gizmos.color = Color.blue;

        Vector3 p0 = new Vector3(mesh_.bounds.extents.x, mesh_.bounds.extents.y, 0) + mesh_.bounds.center;
        Vector3 p1 = new Vector3(-mesh_.bounds.extents.x, mesh_.bounds.extents.y, 0) + mesh_.bounds.center;
        Vector3 p2 = new Vector3(-mesh_.bounds.extents.x, -mesh_.bounds.extents.y, 0) + mesh_.bounds.center;
        Vector3 p3 = new Vector3(mesh_.bounds.extents.x, -mesh_.bounds.extents.y, 0) + mesh_.bounds.center;
        p0 = transform.localToWorldMatrix.MultiplyPoint3x4(p0);
        p1 = transform.localToWorldMatrix.MultiplyPoint3x4(p1);
        p2 = transform.localToWorldMatrix.MultiplyPoint3x4(p2);
        p3 = transform.localToWorldMatrix.MultiplyPoint3x4(p3);
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);

        Gizmos.color = Color.cyan;

        for (int i = 0; i < debug_control_points_.Count; i+=2) {
            p0 = transform.localToWorldMatrix.MultiplyPoint3x4(debug_control_points_[i]);
            p1 = transform.localToWorldMatrix.MultiplyPoint3x4(debug_control_points_[i+1]);
            Gizmos.DrawLine(p0, p1);
        }
    }
    #endif
}
