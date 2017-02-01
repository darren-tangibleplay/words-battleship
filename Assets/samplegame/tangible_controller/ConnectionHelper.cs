using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tangible {
	
    // Self sufficient class
    public class ConnectionHelper {
    	//public delegate void OnNewPair(TangibleObject o1, TangibleObject o2);

    	public class Connection {
    		public readonly int unique_id_1;
    		public readonly int unique_id_2;
            public readonly Vector2 center;
            public readonly int key;
    		public Connection(TangibleObject _o1, TangibleObject _o2) {
    			unique_id_1 = _o1.unique_id;
    			unique_id_2 = _o2.unique_id;
                center = new Vector2((_o1.location.X + _o2.location.X) * 0.5f, (_o1.location.Y + _o2.location.Y) * 0.5f);
                key = EventHelper.PairKey(_o1, _o2);
    		}
    	}
    		
    	private static int CompareUniqueId(TangibleObject a, TangibleObject b) {
            return a.unique_id.CompareTo(b.unique_id);
    	}
    		
        public Dictionary<int, Connection> connections = new Dictionary<int, Connection>();

        private bool HasConnection(TangibleObject _o1, TangibleObject _o2) {
            return connections.ContainsKey(EventHelper.PairKey(_o1, _o2));        
        }

    	private Connection FindConnection(TangibleObject _o1, TangibleObject _o2) {
            int key = EventHelper.PairKey(_o1, _o2);
            return connections.ContainsKey(key) ? connections[key] : null;
    	}
    		
		public void UpdateCards(List<TangibleObject> candidates, EventHelper.IsPairFunc is_pair, Deck deck) {
            Dictionary<int, Connection> newConnections = new Dictionary<int, Connection>();
    		for (int i = 0; i < candidates.Count; i++) {
    			for (int j = i + 1; j < candidates.Count; j++) {
                    int key = EventHelper.PairKey(candidates[i], candidates[j]);
                    if (connections.ContainsKey(key) && is_pair(candidates[i], candidates[j], false, deck) || is_pair(candidates[i], candidates[j], true, deck)) {
                        newConnections[key] = new Connection(candidates[i], candidates[j]);
    				}
    			}
    		}
    		connections = newConnections;
    	}

		public void RemoveAllExplicitlyUnconnected(List<TangibleObject> tangibles, EventHelper.IsPairFunc is_pair, Deck deck) {
            List<int> keys_to_remove = new List<int>();
            Dictionary<int, TangibleObject> tangibles_by_id = new Dictionary<int, TangibleObject>();
            foreach (TangibleObject tangible in tangibles) {
                tangibles_by_id[tangible.unique_id] = tangible;
            }
            foreach (int key in connections.Keys) {
                int a_id = connections[key].unique_id_1;
                int b_id = connections[key].unique_id_2;
                if (!tangibles_by_id.ContainsKey(a_id)) continue;
                if (!tangibles_by_id.ContainsKey(b_id)) continue;
                if (!is_pair(tangibles_by_id[a_id], tangibles_by_id[b_id], false, deck)) keys_to_remove.Add(key);
            }
            foreach (int key in keys_to_remove) {
                connections.Remove(key);
            }
        }

        public HashSet<int> ComputeNewlyConnected(ConnectionHelper last_frame_connections) {
            HashSet<int> newly_connected_ids = new HashSet<int>();
            foreach (int key in connections.Keys) {
                if (!last_frame_connections.connections.ContainsKey(key)) {
                    newly_connected_ids.Add(connections[key].unique_id_1);
                    newly_connected_ids.Add(connections[key].unique_id_2);
                }
            }
            return newly_connected_ids;
        }

        public override string ToString() {
            string s = "";
            foreach (int key in connections.Keys) {
                s += "[" + connections[key].unique_id_1.ToString() + " - " + connections[key].unique_id_2.ToString() + "]\n";
            }
            return s;
        }
    }
	
}