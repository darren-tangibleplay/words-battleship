using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tangible {

    public class ClusterHelper {

		public enum SortType {
			HORIZONTAL,
			VERTICAL,
			ROWS,
			NONE
		}

		public class Card {
			public readonly int value;
			public readonly int unique_id;
			public readonly int id;
			public readonly bool newcomer;
			public readonly bool newly_connected;
			public readonly bool event_enabled;
			public readonly bool is_negative;
			public readonly float x_mm;
			public readonly float y_mm;
			public readonly float orientation;
			
			public Card(int _value, int _unique_id, int _id, bool _newcomer, bool _newly_connected, bool _event_enabled, Location _location) {
				is_negative = _value < 0;
				value = Mathf.Abs(_value);
				unique_id = _unique_id;
				id = _id;
				newcomer = _newcomer;
				newly_connected = _newly_connected;
				event_enabled = _event_enabled;
				x_mm = _location.X;
				y_mm = _location.Y;
				orientation = _location.Orientation;
			}
			
			public override string ToString() {
				return "u_id:" + unique_id.ToString() + "  v:" + value.ToString() + "  newcomer:" + newcomer.ToString() + "  newly_connected:" + newly_connected + "  event_enabled:" + event_enabled;
			}
		}

        public class Cluster : List<TangibleObject> {
            public int id;
			public HashSet<int> uniqueIds = new HashSet<int>();
    		public bool matched = true;
            public List<int> newcomers = new List<int>();
			public List<int> last_ordering = new List<int>();

    		public Cluster(int _id, List<TangibleObject> reference) {
                id = _id;
    			AddRange(reference);
    			UpdateUniqueIds();
    		}

            public void Reset() {
                matched = false;
            }

            public void UpdateNewComers(Cluster reference) {
                newcomers.Clear();

                foreach (TangibleObject tangible in this) {
                    TangibleObject reference_tangible = null;

                    if (reference != null) {
                        reference_tangible = reference.Find(delegate(TangibleObject obj) { return obj.unique_id == tangible.unique_id; });
                    }

                    if (reference_tangible == null) {
                        newcomers.Add(tangible.unique_id);
                    }
                }
            }

    		public void UpdateUniqueIds() {
				uniqueIds.Clear ();
				foreach(TangibleObject t in this) {
					if (!uniqueIds.Contains (t.unique_id)) {
						uniqueIds.Add (t.unique_id);
					}
				}
    		}

			// getting a score of how well the clusters match by seeing how many unique ids they have in common
			HashSet<int> intersectSet = new HashSet<int> ();
			public int MatchValue(Cluster checkCluster) {
				intersectSet.Clear ();
				intersectSet.UnionWith (uniqueIds);
				intersectSet.IntersectWith (checkCluster.uniqueIds);
				return intersectSet.Count;
			}

			public bool IsMatch(Cluster checkCluster) {
				return uniqueIds.SetEquals (checkCluster.uniqueIds);
			}

            public void UpdateLastOrdering () {
				last_ordering.Clear ();
                foreach (TangibleObject tangible in this) {
                    last_ordering.Add(tangible.unique_id);
                }
            }

            public float AverageLocationX() {
                float x = 0;
                foreach (TangibleObject tangible in this) {
                    x += tangible.location.X;
                }
                return (Count > 0) ? x / Count : 0;
            }

			public float AverageLocationY() {
				float y = 0;
				foreach (TangibleObject tangible in this) {
					y += tangible.location.Y;
				}
				return (Count > 0) ? y / Count : 0;
			}
    	}
			
        const float hysteresisSingleDir = 10.0f; // if the element are less than 10mm appart then use the last sorting.
		const float hysteresisMultipleDir = 15.0f;
		const float row_tolerance = 10.0f; // if the elements are within a certain tolerance distance treat them as being in the same row
	
		public class Sorter : IComparer<Cluster>, IComparer<TangibleObject> {

            private readonly List<int> last_ordering_;
			private readonly SortType sort_type_;
			private readonly bool use_hysteresis_;

			public Sorter(SortType sortType, bool useHysteresis, List<int> last_ordering) {
				sort_type_ = sortType;
				use_hysteresis_ = useHysteresis;
				last_ordering_ = last_ordering;
            }

			bool CheckHysteresis(float x1, float x2, float y1, float y2) {
				if (!use_hysteresis_ || sort_type_ == SortType.NONE) { 
					return false;
				}
				if (sort_type_ == SortType.HORIZONTAL) {
					return Mathf.Abs (x1 - x2) < hysteresisSingleDir;
				}
				if (sort_type_ == SortType.VERTICAL) {
					return Mathf.Abs (y1 - y2) < hysteresisSingleDir;
				}

				return (Mathf.Abs (x1 - x2) + Mathf.Abs (y1 - y2)) < hysteresisMultipleDir;
			}
				
			int CompareWithSortType(float x1, float x2, float y1, float y2) {
				if (sort_type_ == SortType.HORIZONTAL) {
					return x1.CompareTo (x2);
				}

				// flip comparison for y values so that objects closer to the ipad come first
				if (sort_type_ == SortType.VERTICAL) {
					return y2.CompareTo (y1);
				}

				if (sort_type_ == SortType.ROWS) {
					float distance_y = Mathf.Abs(y1 - y2);
					// if in the same row compare by x value
					if (distance_y < row_tolerance) {
						return x1.CompareTo (x2);
					}
					// sort by row when possible, flipping comparison so that because higher Y values
					// are at the top of the screen
					return y2.CompareTo(y1);
				}

				// by default/with no sorting specified, just return in the order it came in
				return -1;
			}

			private int CompareByPosition (float x1, float x2, float y1, float y2, int id1, int id2) {
				bool hysteresisCheck = CheckHysteresis (x1, x2, y1, y2);
				if (last_ordering_ != null && last_ordering_.Count > 0 && hysteresisCheck) {
					int index_a = last_ordering_.IndexOf(id1);
					int index_b = last_ordering_.IndexOf(id2);
					if (index_a >= 0 && index_b >= 0) {
						return index_a.CompareTo(index_b);
					}
				}
				return CompareWithSortType(x1, x2, y1, y2); 
			}
										 
            public int Compare(Cluster a, Cluster b) {
				return CompareByPosition(a.AverageLocationX(), b.AverageLocationX(), a.AverageLocationY(), b.AverageLocationY(), a.id, b.id);	
            }

			public int Compare(TangibleObject a, TangibleObject b) {
				return CompareByPosition(a.location.X, b.location.X, a.location.Y, b.location.Y, a.unique_id, b.unique_id);						 
       		}
		}

        private static int id_counter = 0;
    	public List<Cluster> clusters = new List<Cluster>();

        // Needed for Hysteresis sorting
		private List<int> last_cluster_ordering = new List<int>();

        // Needed for Hysteresis pairing (value = EventHelper.PairKey(a,b))
        private HashSet<int> last_pairs_ = new HashSet<int>();

		private bool IsAlreadyMatchedPair(TangibleObject a, TangibleObject b, bool unused, Deck deck) {
            int key = EventHelper.PairKey(a,b);
            return last_pairs_.Contains(key);
        }

		private void ResetPairs(TangibleObject[] objects, EventHelper.IsPairFunc is_pair, Deck deck) {
            HashSet<int> new_pairs_ = new HashSet<int>();
			if (is_pair != null && is_pair != EventHelper.IsPairNever) {
				for (int i = 0; i < objects.Length; i++) {
					TangibleObject a = objects [i];

					for (int j = i + 1; j < objects.Length; j++) {
						TangibleObject b = objects [j];

						int key = EventHelper.PairKey (a, b);
						if ((last_pairs_.Contains (key) && is_pair (a, b, false, deck)) || is_pair (a, b, true, deck)) {
							new_pairs_.Add (key);
						}
					}
				}
			}
            last_pairs_ = new_pairs_;
        }
			
		private List<List<TangibleObject>> Clusters(TangibleObject[] objects, EventHelper.IsPairFunc is_pair, Deck deck) {
			List<List<TangibleObject>> clusters = new List<List<TangibleObject>>();
			List<TangibleObject> candidates = new List<TangibleObject>(objects);

			foreach (TangibleObject candidate in candidates) {
				List<List<TangibleObject>> inside = new List<List<TangibleObject>>();
				foreach (List<TangibleObject> cluster in clusters) {
					foreach (TangibleObject tangible in cluster) {
						if (is_pair(candidate, tangible, true, deck)) {
							inside.Add(cluster);
							break;
						}
					}
				}
				if (inside.Count == 0) {
					List<TangibleObject> new_cluster = new List<TangibleObject>();
					new_cluster.Add(candidate);
					clusters.Add(new_cluster);
				} else {
					inside[0].Add(candidate);
					for (int i = 1; i < inside.Count; i++) {
						inside[0].AddRange(inside[i]);
						inside[i].Clear();
					}
				}
			}
			clusters.RemoveAll(l => l.Count == 0);

			return clusters;
		}

		public void UpdateCards(TangibleObject[] objects, EventHelper.IsPairFunc is_pair, SortType sort_type, bool useHysteresis, Deck deck) {
            ResetPairs(objects, is_pair, deck);

    		// remove card from clusters
    		foreach (Cluster cluster in clusters) {
                cluster.Reset();
    		}

    		// New clusters get matched to old ones
            List<List<TangibleObject>> new_clustered_list = Clusters(objects, IsAlreadyMatchedPair, deck);
            List<Cluster> new_clusters = new List<Cluster>();
            foreach (List<TangibleObject> cluster_as_list in new_clustered_list) {
                Cluster cluster = new Cluster(-1, cluster_as_list);
                new_clusters.Add(cluster);
            }

            // Do bigger cluster first
            new_clusters.Sort(delegate(Cluster a, Cluster b) { return b.Count.CompareTo(a.Count); });

            // Find the best id to inherit from
            foreach (Cluster new_cluster in new_clusters) {
                // Find the best old cluster and steal its id
                int bestScore = 0;
                Cluster best = null;
				Cluster compareCluster;
                for (int j = 0; j < clusters.Count; j++) {
					compareCluster = clusters [j];
                    if (compareCluster.matched) continue;
					if (new_cluster.IsMatch(compareCluster)) {
						best = compareCluster;
                        break;
                    }
					int score = new_cluster.MatchValue (compareCluster);
                    if (score > bestScore) {
                        bestScore = score;
						best = compareCluster;
                    }
                }

                new_cluster.UpdateNewComers(best);

                if (best != null) {
                    best.matched = true;
                    new_cluster.id = best.id;
                } else {
                    new_cluster.id = id_counter++;
                }
            }

            //Debug.Log("old clusters:" + ToString(clusters) + "\nnewClusters: " + ToString(new_clusters));

            clusters = new_clusters;

			SortClusters(sort_type, useHysteresis);
    	}
			

		private void SortClusters(SortType sortType, bool useHysteresis) {
			if (sortType != SortType.NONE) {
				clusters.Sort (new Sorter (sortType, useHysteresis, last_cluster_ordering));
			}
							
			int numClusters = clusters.Count;
			Cluster cluster;
			for (int i = 0; i < numClusters; i++) {
				cluster = clusters [i];
				cluster.UpdateUniqueIds();
				if (sortType != SortType.NONE) {
					cluster.Sort (new Sorter (sortType, useHysteresis, cluster.last_ordering));
				}
            }
            
			last_cluster_ordering.Clear ();
			for (int i = 0; i < numClusters; i++) {
				cluster = clusters [i];
				cluster.UpdateLastOrdering ();
				last_cluster_ordering.Add(cluster.id);
            }
        }

        public static string ToString(List<Cluster> clusters) {
            string s = "";
            foreach (Cluster c in clusters) {
                s += "id:" + c.id + " " + EventHelper.ToString(c) + "  ";
            }
            return s;
        }
    		
    	public override string ToString() {
            return ToString(this.clusters);
    	}
    }
	
}