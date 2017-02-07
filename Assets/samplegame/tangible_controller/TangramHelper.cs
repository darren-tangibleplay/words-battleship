// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
//
// namespace Tangible {
//
// public class TangramHelper {
//
// 	public static readonly Vector2[] big_triangle_points = {
// 		new Vector2(0, 0),
// 		new Vector2(0, Mathf.Sqrt(2.0f) / 2.0f),
// 		new Vector2(Mathf.Sqrt(2.0f) / 2.0f, 0)
// 	};
//
// 	public static readonly Vector2[] big_triangle_snap_points = {
// 		new Vector2(0, 0),
// 		new Vector2(0, 1*Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(0, 2*Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(0, 3*Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(0, 4*Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(1*Mathf.Sqrt(2.0f) / 8.0f, 3*Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(2*Mathf.Sqrt(2.0f) / 8.0f, 2*Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(3*Mathf.Sqrt(2.0f) / 8.0f, 1*Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(4*Mathf.Sqrt(2.0f) / 8.0f, 0),
// 		new Vector2(3*Mathf.Sqrt(2.0f) / 8.0f, 0),
// 		new Vector2(2*Mathf.Sqrt(2.0f) / 8.0f, 0),
// 		new Vector2(1*Mathf.Sqrt(2.0f) / 8.0f, 0)
// 	};
//
// 	public static readonly Vector2[] big_triangle_1_uvs = {
// 		new Vector2(0.5f, 0.5f),
// 		new Vector2(0.0f, 0.0f),
// 		new Vector2(0.0f, 1.0f)
// 	};
//
// 	public static readonly Vector2[] big_triangle_2_uvs = {
// 		new Vector2(0.5f, 0.5f),
// 		new Vector2(0.0f, 1.0f),
// 		new Vector2(1.0f, 1.0f)
// 	};
//
// 	public static readonly Vector2[] medium_triangle_points = {
// 		new Vector2(0, 0),
// 		new Vector2(0, 0.5f),
// 		new Vector2(0.5f, 0)
// 	};
//
// 	public static readonly Vector2[] medium_triangle_snap_points = {
// 		new Vector2(0, 0),
// 		new Vector2(0, 0.25f),
// 		new Vector2(0, 0.5f),
// 		new Vector2(0.25f, 0.25f),
// 		new Vector2(0.5f, 0),
// 		new Vector2(0.25f, 0)
// 	};
//
// 	public static readonly Vector2[] medium_triangle_uvs = {
// 		new Vector2(1.0f, 0.0f),
// 		new Vector2(0.5f, 0.0f),
// 		new Vector2(1.0f, 0.5f)
// 	};
//
// 	public static readonly Vector2[] small_triangle_points = {
// 		new Vector2(0, 0),
// 		new Vector2(0, Mathf.Sqrt(2.0f) / 4.0f),
// 		new Vector2(Mathf.Sqrt(2.0f) / 4.0f, 0)
// 	};
//
// 	public static readonly Vector2[] small_triangle_snap_points = {
// 		new Vector2(0, 0),
// 		new Vector2(0, Mathf.Sqrt(2.0f) / 4.0f),
// 		new Vector2(Mathf.Sqrt(2.0f) / 8.0f, Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(Mathf.Sqrt(2.0f) / 4.0f, 0)
// 	};
//
// 	public static readonly Vector2[] small_triangle_1_uvs = {
// 		new Vector2(0.25f, 0.25f),
// 		new Vector2(0.5f, 0),
// 		new Vector2(0, 0)
// 	};
//
// 	public static readonly Vector2[] small_triangle_2_uvs = {
// 		new Vector2(0.5f, 0.5f),
// 		new Vector2(0.75f, 0.75f),
// 		new Vector2(0.75f, 0.25f)
// 	};
//
// 	public static readonly Vector2[] square_points = {
// 		new Vector2(-Mathf.Sqrt(2.0f) / 8.0f, -Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(-Mathf.Sqrt(2.0f) / 8.0f, Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(Mathf.Sqrt(2.0f) / 8.0f, Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(Mathf.Sqrt(2.0f) / 8.0f, -Mathf.Sqrt(2.0f) / 8.0f),
// 	};
//
// 	public static readonly Vector2[] square_snap_points = {
// 		new Vector2(-Mathf.Sqrt(2.0f) / 8.0f, -Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(-Mathf.Sqrt(2.0f) / 8.0f, 0),
// 		new Vector2(-Mathf.Sqrt(2.0f) / 8.0f, Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(0, Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(Mathf.Sqrt(2.0f) / 8.0f, Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(Mathf.Sqrt(2.0f) / 8.0f, 0),
// 		new Vector2(Mathf.Sqrt(2.0f) / 8.0f, -Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(0, -Mathf.Sqrt(2.0f) / 8.0f),
// 	};
//
// 	public static readonly Vector2[] square_uvs = {
// 		new Vector2(0.25f, 0.25f),
// 		new Vector2(0.5f, 0.5f),
// 		new Vector2(0.75f, 0.25f),
// 		new Vector2(0.5f, 0.0f),
// 	};
//
// 	public static readonly Vector2[] parallelogram_front_points = {
// 		new Vector2(-Mathf.Sqrt(2.0f) / 4.0f, -Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(0, Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(Mathf.Sqrt(2.0f) / 4.0f, Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(0, -Mathf.Sqrt(2.0f) / 8.0f),
// 	};
//
// 	public static readonly Vector2[] parallelogram_front_snap_points = {
// 		new Vector2(-Mathf.Sqrt(2.0f) / 4.0f, -Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(-Mathf.Sqrt(2.0f) / 8.0f, 0),
// 		new Vector2(0, Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(Mathf.Sqrt(2.0f) / 4.0f, Mathf.Sqrt(2.0f) / 8.0f),
// 		new Vector2(Mathf.Sqrt(2.0f) / 8.0f, 0),
// 		new Vector2(0, -Mathf.Sqrt(2.0f) / 8.0f),
// 	};
//
// 	public static readonly Vector2[] parallelogram_front_uvs = {
// 		new Vector2(0.75f, 0.25f),
// 		new Vector2(0.75f, 0.75f),
// 		new Vector2(1.0f, 1.0f),
// 		new Vector2(1.0f, 0.5f),
// 	};
//
// 	public static readonly Vector2[] parallelogram_back_points = {
// 		new Vector2(-3.0f / 8.0f, -1.0f / 8.0f),
// 		new Vector2(-1.0f / 8.0f, 1.0f / 8.0f),
// 		new Vector2(3.0f / 8.0f, 1.0f / 8.0f),
// 		new Vector2(1.0f / 8.0f, -1.0f / 8.0f),
// 	};
//
// 	public static readonly Vector2[] parallelogram_back_snap_points = {
// 		new Vector2(-3.0f / 8.0f, -1.0f / 8.0f),
// 		new Vector2(-1.0f / 8.0f, 1.0f / 8.0f),
// 		new Vector2(1.0f / 8.0f, 1.0f / 8.0f),
// 		new Vector2(3.0f / 8.0f, 1.0f / 8.0f),
// 		new Vector2(1.0f / 8.0f, -1.0f / 8.0f),
// 		new Vector2(-1.0f / 8.0f, -1.0f / 8.0f),
// 	};
//
// 	public static readonly Vector2[] parallelogram_back_uvs = {
// 		new Vector2(0.75f, 0.25f),
// 		new Vector2(1.0f, 0.5f),
// 		new Vector2(1.0f, 1.0f),
// 		new Vector2(0.75f, 0.75f),
// 	};
//
// 	public static readonly int[] triangle_index = {
// 		0, 1, 2
// 	};
//
// 	public static readonly int[] quad_index = {
// 		0, 1, 2,
// 		0, 2, 3
// 	};
//
// 	public static readonly int[] triangle_outline_index = {
// 		0, 2, 3,
// 		0, 3, 1,
// 		2, 4, 5,
// 		2, 5, 3,
// 		4, 0, 1,
// 		4, 1, 5,
// 	};
//
// 	public static readonly int[] quad_outline_index = {/*
// 		0, 3, 2,
// 		0, 1, 3,
// 		2, 5, 4,
// 		2, 3, 5,
// 		4, 7, 6,
// 		4, 5, 7,
// 		6, 1, 0,
// 		6, 7, 1,
// 			*/
// 		0, 2, 3,
// 		0, 3, 1,
// 		2, 4, 5,
// 		2, 5, 3,
// 		4, 6, 7,
// 		4, 7, 5,
// 		6, 0, 1,
// 		6, 1, 7,
// 	};
//
// 	public static Vector2[] GetPoints(TangibleObject.Shape shape) {
// 		switch(shape) {
// 			case TangibleObject.Shape.tangram_big_triangle: return big_triangle_points;
// 			case TangibleObject.Shape.tangram_medium_triangle: return medium_triangle_points;
// 			case TangibleObject.Shape.tangram_small_triangle: return small_triangle_points;
// 			case TangibleObject.Shape.tangram_square: return square_points;
// 			case TangibleObject.Shape.tangram_parallelogram_front: return parallelogram_front_points;
// 			case TangibleObject.Shape.tangram_parallelogram_back: return parallelogram_back_points;
// 			default: return null;
// 		}
// 	}
//
// 	public static Vector3[] GetPoints3d(TangibleObject.Shape shape) {
// 		Vector2[] points2d = GetPoints(shape);
// 		Vector3[] points3d = new Vector3[points2d.Length];
// 		for (int i=0 ;i<points2d.Length; i++) {
// 			points3d[i] = points2d[i];
// 		}
// 		return points3d;
// 	}
//
// 	public static Vector2[] GetUVs(int rank) {
// 		switch(rank) {
// 			case 0: return big_triangle_1_uvs;
// 			case 1: return big_triangle_2_uvs;
// 			case 2: return medium_triangle_uvs;
// 			case 3: return small_triangle_1_uvs;
// 			case 4: return small_triangle_2_uvs;
// 			case 5: return square_uvs;
// 			case 6: return parallelogram_front_uvs;
// 			case 7: return parallelogram_back_uvs;
// 			default: return big_triangle_1_uvs;
// 		}
// 	}
//
// 	public static Vector2[] GetInnerPoints(Vector2[] points, float thickness) {
// 		Vector2[] innerLines = new Vector2[points.Length * 2];
// 		Vector2[] innerPoints = new Vector2[points.Length];
//
// 		for (int i = 0; i < points.Length; i++) {
// 			Vector2 p0 = points[i];
// 			Vector2 p1 = points[(i + 1) % points.Length];
// 			Vector3 l = new Vector3(p1.x - p0.x, p1.y - p0.y, 0);
// 			Vector3 dd = Vector3.Cross(l, Vector3.forward);
// 			Vector2 d = dd;
// 			d.Normalize();
// 			innerLines[2 * i] = p0 + d * thickness;
// 			innerLines[2 * i + 1] = p1 + d * thickness;
// 		}
//
// 		for (int i = 0; i < points.Length; i++) {
// 			Vector2 p1 = innerLines[(2 * i) % innerLines.Length];
// 			Vector2 p2 = innerLines[(2 * i + 1) % innerLines.Length];
// 			Vector2 p3 = innerLines[(2 * (i + 1)) % innerLines.Length];
// 			Vector2 p4 = innerLines[(2 * (i + 1) + 1) % innerLines.Length];
// 			Vector2 p21 = p1 - p2;
// 			Vector2 p34 = p3 - p4;
// 			float a = p1.x * p2.y - p1.y * p2.x;
// 			float b = p3.x * p4.y - p3.y * p4.x;
// 			float d = p21.x * p34.y - p21.y * p34.x;
// 			Vector2 p = p2;
// 			if (Mathf.Abs(d) > 0.0001f ) p = new Vector2((a * p34.x - b * p21.x) / d, (a * p34.y - b * p21.y) / d);
// 			innerPoints[(i + 1) % points.Length] = p;
// 		}
// 		return innerPoints;
// 	}
//
// 	public static Vector2[] GetSnapPoints(TangibleObject.Shape shape) {
// 		switch(shape) {
// 			case TangibleObject.Shape.tangram_big_triangle: return big_triangle_snap_points;
// 			case TangibleObject.Shape.tangram_medium_triangle: return medium_triangle_snap_points;
// 			case TangibleObject.Shape.tangram_small_triangle: return small_triangle_snap_points;
// 			case TangibleObject.Shape.tangram_square: return square_snap_points;
// 			case TangibleObject.Shape.tangram_parallelogram_front: return parallelogram_front_snap_points;
// 			case TangibleObject.Shape.tangram_parallelogram_back: return parallelogram_back_snap_points;
// 			default: return null;
// 		}
// 	}
//
// 	public static TangibleObject.Shape GetShape(int rank) {
// 		switch(rank) {
// 			case 0: return TangibleObject.Shape.tangram_big_triangle;
// 			case 1: return TangibleObject.Shape.tangram_big_triangle;
// 			case 2: return TangibleObject.Shape.tangram_medium_triangle;
// 			case 3: return TangibleObject.Shape.tangram_small_triangle;
// 			case 4: return TangibleObject.Shape.tangram_small_triangle;
// 			case 5: return TangibleObject.Shape.tangram_square;
// 			case 6: return TangibleObject.Shape.tangram_parallelogram_front;
// 			case 7: return TangibleObject.Shape.tangram_parallelogram_back;
// 			default: return TangibleObject.Shape.tangram_big_triangle;
// 		}
// 	}
//
// 	public static Vector2 GetCenter(TangibleObject.Shape shape, Vector2 offset, float orientation) {
// 		Vector2 center = Vector2.zero;
// 		Vector2[] points = TangramHelper.GetPoints(shape);
// 		Quaternion q = Quaternion.Euler (0, 0, orientation);
// 		for (int i = 0; i < points.Length; i++) {
// 			center += points[i];
// 		}
// 		center /= points.Length;
// 		center = q * center;
// 		return center + offset;
// 	}
//
// 	public static bool Connected(bool precise, TangibleObject.Shape s1, Vector2 t1, float o1,TangibleObject.Shape s2, Vector2 t2, float o2, ref Vector2 offset, ref float orientation, ref Vector2 at) {
// 		Vector2[] points1 = TangramHelper.GetPoints(s1);
// 		Vector2[] refpoints2 = TangramHelper.GetPoints(s2);
// 		Vector2[] points2 = new Vector2[refpoints2.Length];
//
// 		// Transform shape 2 to be in shape 1 space
//         float o = GeometryHelper.NormalizeAngle(o2 - o1, s2);
// 		//float o = o2 - (o1 - rotation);
// 		Vector2 t = Quaternion.Euler(0, 0, -o1) * (t2 - t1);
// 		//Vector2 t = Quaternion.Euler(0, 0, -(o1 + rotation)) * (t2 - t1);
// 		Quaternion q = Quaternion.Euler (0, 0, o);
// 		for (int i = 0; i < refpoints2.Length; i++) {
// 			points2[i] = q * refpoints2[i];
// 			points2[i] += t;
// 		}
//
// 		offset = t;
// 		orientation = o;
//
// 		float slop = precise ? 0.025f : 0.15f;
//
// 		// If any shape 2 points is touching shape 1 segments
// 		for (int i = 0; i < points1.Length; i++) {
// 			Vector2 a = points1[i];
// 			Vector2 b = points1[(i + 1) % (points1.Length)];
// 			for (int j = 0; j < points2.Length; j++) {
// 				if (GeometryHelper.PointToSegmentSqr(points2[j], a, b) < slop * slop) {
// 					at = Quaternion.Euler (0, 0, o1) * points2[j];
// 					at += t1;
// 					return true;
// 				}
// 			}
// 		}
//
// 		// If any shape 1 points is touching shape 2 segments
// 		for (int i = 0; i < points2.Length; i++) {
// 			Vector2 a = points2[i];
// 			Vector2 b = points2[(i + 1) % (points2.Length)];
// 			for (int j = 0; j < points1.Length; j++) {
//                     if (GeometryHelper.PointToSegmentSqr(points1[j], a, b) < slop * slop) {
// 					at = Quaternion.Euler (0, 0, o1) * points1[j];
// 					at += t1;
// 					return true;
// 				}
// 			}
// 		}
//
// 		return false;
// 	}
// }
//
// }
//
