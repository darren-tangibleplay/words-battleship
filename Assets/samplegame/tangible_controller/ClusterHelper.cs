using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tangible {

// Self sufficient class
public class ClusterHelper {

	public class Cluster : List<TangibleObject> {
		public long signature;
		public bool matched = true;
		public Cluster(List<TangibleObject> reference) {
			AddRange(reference);
			UpdateSignature();
		}
		public void UpdateSignature() {
			signature = EventHelper.ComputeSignature(this);
		}
	}

	public List<Cluster> clusters = new List<Cluster>();

	private static int CompareCount(List<TangibleObject> a, List<TangibleObject> b) {
		return a.Count.CompareTo(b.Count);
	}

	private static int CompareId(TangibleObject a, TangibleObject b) {
		return a.Id.CompareTo(b.Id);
	}

	private int Ones32(int x) {
        x -= ((x >> 1) & 0x55555555);
        x = (((x >> 2) & 0x33333333) + (x & 0x33333333));
        x = (((x >> 4) + x) & 0x0f0f0f0f);
        x += (x >> 8);
        x += (x >> 16);
        return(x & 0x0000003f);
	}

	private int Ones64(long x) {
        return Ones32((int) (x & 0xFFffFFff)) + Ones32((int) (x >> 32));
	}

	public bool UpdateCards(TangibleObject[] objects) {
		bool clusterChange = false;
		List<TangibleObject> candidates = new List<TangibleObject>(objects);
		candidates.Sort(CompareId);

		// remove card from clusters
		foreach (Cluster cluster in clusters) {
			if (cluster.RemoveAll(delegate(TangibleObject o) { return !candidates.Contains(o); }) > 0) {
				clusterChange = true;
			}
			cluster.UpdateSignature();
			cluster.matched = false;
		}
		// remove empty clusters
		if (clusters.RemoveAll(delegate(Cluster c) { return c.Count == 0; }) > 0) {
			clusterChange = true;
		}
		// New clusters get matched to old ones
		List<List<TangibleObject>> newClusters = EventHelper.Clusters(objects);
		for (int i = newClusters.Count - 1; i >= 0; i--) {
			long newSignature = EventHelper.ComputeSignature(newClusters[i]);
			//Debug.Log("dealing with cluster " + EventHelper.ToString(newClusters[i]) + " signature " + newSignature.ToString("x8"));
			int bestScore = -1;
			int bestIndex = -1;
			bool same = false;
			for (int j = 0; j < clusters.Count; j++) {
				if (clusters[j].matched) continue;
				if (newSignature == clusters[j].signature) {
					bestScore = 64;
					bestIndex = j;
					same = true;
					//Debug.Log(" -> same");
					break;
				}
				long mask = newSignature & clusters[j].signature;
				int score = Ones64(mask);
				//Debug.Log("mask " + mask.ToString("x8") + " -> " + score);
				if (score > bestScore) {
					bestScore = score;
					bestIndex = j;
				}
			}
			if (bestScore > 0 && bestIndex >= 0) {
				clusters[bestIndex].matched = true;
				// Add the missing cards
				if (!same) {
					clusters[bestIndex].RemoveAll(delegate(TangibleObject o) { return !newClusters[i].Contains(o); });
					foreach(TangibleObject t in newClusters[i]) {
						if (!clusters[bestIndex].Contains(t)) {
							clusters[bestIndex].Add(t);
						}
					}
					clusterChange = true;
				}
			} else {
				// Create the cluster
				clusters.Add(new Cluster(newClusters[i]));
				clusterChange = true;
			}
		}
		// remove un-matched clusters
		if (clusters.RemoveAll(delegate(Cluster c) { return !c.matched; }) > 0) {
			clusterChange = true;
		}

		clusters.Sort(CompareCount);
		//Debug.Log("ClusterHelper " + this.ToString() + "  from  " + EventHelper.ToString(newClusters));
		return clusterChange;
	}

	public override string ToString() {
		string s = "";
		foreach (Cluster c in clusters) {
			s += EventHelper.ToString(c) + " ";
		}
		return s;
	}
}

}