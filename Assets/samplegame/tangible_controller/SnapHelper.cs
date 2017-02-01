using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tangible {
	
public class SnapHelper {
		
	const float snapDistSqr = 16.0f * 16.0f;
		
	public static Vector2 ClosestPointOnSegment(Vector2 p, Vector2 s0, Vector2 s1) {
		Vector2 s01 = s1 - s0;
		Vector2 s0p = p - s0;
	  	float lSqr = Vector2.Dot(s01, s01);
	  	if (lSqr < 0.00001f) return s0;
		float t = Vector2.Dot(s0p, s01) / lSqr;
	  	if (t <= 0) return s0;
	  	if (t >= 1) return s1;
		return s0 + t * s01;
	}

	public static Vector2 SnapOffset(Vector2[] polygon, List<Vector2[]> polygons) {
		// Snap to points
		List<Vector2> points = new List<Vector2>();
		foreach (Vector2[] v in polygons) {
			points.AddRange(v);
		}
		float closestDistSqr = snapDistSqr;
		Vector2 closestShape = Vector2.zero;
		Vector2 closestReference = Vector2.zero;
		for (int i = 0; i < polygon.Length; i++) {
			Vector2 p0 = polygon[i];
			foreach(Vector2 p1 in points) {
				float distSqr = (p1 - p0).sqrMagnitude;
				if (distSqr < closestDistSqr) {
					closestDistSqr = distSqr;
					closestShape = p0;
					closestReference = p1;		
				}
			}
		}
		if (closestDistSqr < snapDistSqr) return closestReference - closestShape;
			
		// Snap to reference lines
		for (int i = 0; i < polygon.Length; i++) {
			Vector2 p0 = polygon[i];
			foreach (Vector2[] v in polygons) {
				for (int j = 0; j < v.Length; j++) {
					Vector2 s0 = v[j];
					Vector2 s1 = v[(j+1) % v.Length];
					Vector2 p1 = ClosestPointOnSegment(p0, s0, s1);
					float distSqr = (p1 - p0).sqrMagnitude;
					if (distSqr < closestDistSqr) {
						closestDistSqr = distSqr;
						closestShape = p0;
						closestReference = p1;		
					}	
				}
			}
		}
		if (closestDistSqr < snapDistSqr) return closestReference - closestShape;
			
		// Snap to shape lines
		foreach(Vector2 p1 in points) {
			for (int i = 0; i < polygon.Length; i++) {
				Vector2 s0 = polygon[i];
				Vector2 s1 = polygon[(i+1) % polygon.Length];
				Vector2 p0 = ClosestPointOnSegment(p1, s0, s1);
				float distSqr = (p1 - p0).sqrMagnitude;
				if (distSqr < closestDistSqr) {
					closestDistSqr = distSqr;
					closestShape = p0;
					closestReference = p1;		
				}	
			}
		}
		if (closestDistSqr < snapDistSqr) return closestReference - closestShape;
			
		return Vector2.zero;
	}
}
		
}

