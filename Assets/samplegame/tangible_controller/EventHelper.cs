using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tangible {

public class EventHelper {

	public const float cardSize = 31.75f; // millimeter

	private static float positionError = 5.0f; // millimeter
	private static float angleError = 10.0f; // degree

	private static int CompareCount(List<TangibleObject> a, List<TangibleObject> b) {
		return a.Count.CompareTo(b.Count);
	}

	private static int CompareId(TangibleObject a, TangibleObject b) {
		return a.Id.CompareTo(b.Id);
	}

	public static float NormalizeOrientation(float orientation) {
		float angle = (orientation + 360.0f) % 90;
		if (angle > 45.0f) angle -= 90.0f;
		return angle;
	}

	public static long ComputeSignature(List<TangibleObject> cluster) {
		long signature = 0;
		foreach(TangibleObject t in cluster) {
			if (t.Id < 0) continue;
			if (t.Id > TangibleObject.CARDS_PER_ALPHABET * TangibleObject.NUM_ALPHABETS) {
				Debug.LogError("id above 63 are not supported by this implementation");
				continue;
			}
			signature = signature | (((long) 1) << t.Id);
		}
		return signature;
	}

	public static List<List<TangibleObject>> Clusters(TangibleObject[] objects) {
		List<List<TangibleObject>> clusters = new List<List<TangibleObject>>();

		List<TangibleObject> candidates = new List<TangibleObject>(objects);
		candidates.Sort(CompareId);
		while (candidates.Count > 0) {
			List<TangibleObject> cluster = new List<TangibleObject>(2);
			cluster.Add(candidates[0]);
			candidates.RemoveAt(0);
			clusters.Add(cluster);

			// Try to find all the pair from this seed
			int lastClusterCount = 0;
			while(lastClusterCount != cluster.Count) {
				lastClusterCount = cluster.Count;
				bool found = false;
				for (int i = 0; i < cluster.Count && !found; i++) {
					for (int j = 0; j < candidates.Count && !found; j++) {
						if (IsPair(cluster[i], candidates[j])) {
							cluster.Add(candidates[j]);
							candidates.RemoveAt(j);
							found = true;
						}
					}
				}
			}
		}
		SortClusters(clusters);
		return clusters;
	}

	private static void SortClusters(List<List<TangibleObject>> clusters) {
		clusters.Sort(CompareCount);
	}

	public static List<TangibleObject> Copy(List<VisionEventItem> objects) {
		List<TangibleObject> keep = new List<TangibleObject>();
		foreach(VisionEventItem item in objects) {
			keep.Add(new TangibleObject(item));
		}

		return keep;
	}

	public static List<TangibleObject> FilterBorder(List<VisionEventItem> objects) {
		const float interestRadiusSqr = 100 * 100;

		// keep the ones next to the center and the ones next to them
		List<TangibleObject> candidates = new List<TangibleObject>();
		List<TangibleObject> keep = new List<TangibleObject>();
		foreach (VisionEventItem t in objects) {
			float x = t.pt.x;
			float y = t.pt.y > 0 ? 0 : t.pt.y;
			if (x * x + y * y < interestRadiusSqr) {
				keep.Add(new TangibleObject(t));
			} else {
				candidates.Add(new TangibleObject(t));
			}
		}

		int lastKeepCount = 0;
		while(lastKeepCount != keep.Count) {
			lastKeepCount = keep.Count;
			bool found = false;
			for (int i = 0; i < keep.Count && !found; i++) {
				for (int j = 0; j < candidates.Count && !found; j++) {
					if (IsClose(keep[i], candidates[j])) {
						keep.Add(candidates[j]);
						candidates.RemoveAt(j);
						found = true;
					}
				}
			}
		}

		return keep;
	}

	public static bool IsClose(TangibleObject o1, TangibleObject o2) {
		float dx = o1.location.x - o2.location.x;
		float dy = o1.location.y - o2.location.y;
		float distSqr = dx*dx + dy*dy;
		return distSqr < cardSize * cardSize * 4;
	}

	public static bool IsPair(TangibleObject o1, TangibleObject o2) {
		//if (o1 == null || o2 == null) return false;
		//if (o1.id == o2.id) return false;

		float dx = o1.location.x - o2.location.x;
		float dy = o1.location.y - o2.location.y;
		float distSqr = dx*dx + dy*dy;

		float minDist = cardSize - positionError;
		if (distSqr < minDist * minDist) {
			//Debug.Log("min dst " + Mathf.Sqrt(distSqr) + " < " + minDist);
			return false;
		}

		float maxdist = cardSize + positionError;
		if (distSqr > maxdist * maxdist) {
			//Debug.Log("max dst " + Mathf.Sqrt(distSqr) + " > " + maxdist);
			return false;
		}

		float a1 = NormalizeOrientation(o1.location.orientation);
		float a2 = NormalizeOrientation(o2.location.orientation);
        if (Mathf.Abs(a1 - a2) > angleError) {
			//Debug.Log("angle a1 " + a1 + "  a2 " + a2);
			return false;
		}

		float a12 = (a1 + a2) / 2.0f;
		float a = Mathf.Acos(dx / Mathf.Sqrt(distSqr)) * Mathf.Sign(-dy) * 180 / Mathf.PI;
		float aN = NormalizeOrientation(a);
		if (Mathf.Abs(a12 - aN) > 2.0f * angleError) {
			//Debug.Log("alignement a12 " + a12 + "  a " + a);
			return false;
		}

 		return true;
	}

	public static string ToString(List<List<TangibleObject>> clusters) {
		string s = "";
		foreach (List<TangibleObject> c in clusters) {
			s += ToString(c) + " ";
		}
		return s;
	}

	public static string ToString(List<TangibleObject> cluster) {
		string s = "[";
		foreach (TangibleObject t in cluster) {
			s += t.Id + " ";
		}
		s += "]";
		return s;
	}

}

}
