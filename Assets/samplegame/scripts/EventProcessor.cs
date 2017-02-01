using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tangible;

public class EventProcessor : MonoBehaviour {

	// whether or not play area is restricted to only pick up tiles within a certain distance of the screen
	// Numbers used this because the game was often detecting unintended tiles that were set off to the side
	readonly bool restrictPlayArea = false;

	// These can be adusted to determine how much the play area should be restricted, for easy reference in case
	// they start getting tweaked, Numbers shipped with 80.0f inner and 120.0f outer
	const float INSIDE_AREA_RADIUS = 80.0f; // millimeter
	const float OUTSIDE_AREA_RADIUS = 120.0f; // millimeter

    const float INSIDE_FOV_MARGIN = 30.0f; // millimeter
    const float CENTER_AREA_TO_SCREEN = 70.0f; // millimeter
    const float INCLUDE_DISTANCE = 1.75f; // relative to card size

    private FOVHelper fov_helper_ = new FOVHelper();
    private ClusterHelper cluster_helper_ = new ClusterHelper();
    private ConnectionHelper last_connections = new ConnectionHelper();
    private ConnectionHelper current_connections = new ConnectionHelper();
    
    private HistoryHelper card_history_ = new HistoryHelper(5.0f, 3);
    private HistoryHelper connection_history_ = new HistoryHelper(5.0f, 2);
    
    private HashSet<int> last_unique_ids_ = new HashSet<int>();
    private HashSet<int> current_unique_ids_ = new HashSet<int>();
    
    private HashSet<int> last_raw_ids_ = new HashSet<int>();
    public HashSet<int> Ids {
        get { return last_raw_ids_; }
    }

    private List<Tangible.TangibleObject> current_tangibles_ = new List<Tangible.TangibleObject>();
    
    private HashSet<int> visible_ids_;

    private float last_event_update_ = 0;

    Vector2 LocationVector2(TangibleObject tangible) {
        return new Vector2(tangible.location.X, tangible.location.Y);
    }

    bool InsideInnerPlayArea(Vector2 p) {
        bool inside_fov = fov_helper_.InsideEdgesFov(p, INSIDE_FOV_MARGIN);

		if (!restrictPlayArea) {
			return inside_fov;
		}
			
        Vector2 area_center = fov_helper_.StandCenter + Vector2.down * CENTER_AREA_TO_SCREEN;
        p.y = Mathf.Min(area_center.y, p.y);
        bool close_enough = Vector2.Distance(p, area_center) < INSIDE_AREA_RADIUS;
        
        return inside_fov && close_enough;
    }
    
    bool InsideOuterPlayArea(Vector2 p) {
#if UNITY_EDITOR
        bool inside_fov = fov_helper_.InsideEdgesFov(p, 0.0f);
#else
        bool inside_fov = true; // if it is detected we keep it.
#endif

		if (!restrictPlayArea) {
			return inside_fov;
		}
			
        Vector2 area_center = fov_helper_.StandCenter + Vector2.down * CENTER_AREA_TO_SCREEN;
        p.y = Mathf.Min(area_center.y, p.y);
        bool close_enough = Vector2.Distance(p, area_center) < OUTSIDE_AREA_RADIUS;

        return inside_fov && close_enough;
    }
    
    public List<TangibleObject> FilterPlayArea(TangibleObject[] tangibles) {
        List<TangibleObject> filtered = new List<TangibleObject>();
        List<TangibleObject> outside = new List<TangibleObject>();
        
        foreach (TangibleObject tangible in tangibles) {
            bool inside = false;
            if (InsideOuterPlayArea(LocationVector2(tangible))) {
                if (InsideInnerPlayArea(LocationVector2(tangible)) || last_unique_ids_.Contains(tangible.unique_id)) {
                    inside = true;
                }
            }
            
            if (inside) {
                filtered.Add(tangible);
            } else {
                outside.Add(tangible);
            }
        }
        
        // Try to keep cards that are close to the filtered ones
        int outside_count = 0;
        while (outside_count != outside.Count) {
            outside_count = outside.Count;
            foreach (TangibleObject tangible_outside in outside) {
                bool keep = false;
                bool was_inside = last_unique_ids_.Contains(tangible_outside.unique_id);
                foreach (TangibleObject tangible_inside in filtered) {
					if (EventHelper.IsClose(tangible_outside, tangible_inside, Game.Deck, was_inside ? INCLUDE_DISTANCE * 1.25f : INCLUDE_DISTANCE)) {
                        keep = true;
                        break;
                    }
                }
                if (keep) {
                    outside.Remove(tangible_outside);
                    filtered.Add(tangible_outside);
                    break;
                }
            }
        }
        
        return filtered;
    }
     
    public HashSet<int> GetRawIds(Tangible.Event e) {
        HashSet<int> ids = new HashSet<int>();
        foreach (TangibleObject tangible in e.objects) {
            ids.Add(tangible.id);
        }
        return ids;
    }
	
    public Tangible.Event processEvent(Tangible.Event e) {
        if (Game.ClusterProcessor == null) return e;

        float event_update = UnityEngine.Time.timeSinceLevelLoad;
        float dt = Mathf.Min(0.5f, event_update - last_event_update_);
        last_event_update_ = event_update;
        last_connections = current_connections;

		if (Game.EnableLineHelper) {
			Game.DebugLineHelper.gameObject.SetActive(e.objects.Length > 0);
			Game.DebugLineHelper.Clear();
		}
        
        // Frame bookkeeping.
        last_raw_ids_ = GetRawIds(e);
        last_unique_ids_ = current_unique_ids_;
        current_unique_ids_ = new HashSet<int>();
        
        fov_helper_.Update(e.bounds);

        current_tangibles_ = FilterPlayArea(e.objects);

		if (Game.EnableLineHelper) {
			fov_helper_.DebugDrawFov(Game.DebugLineHelper, Game.Deck, INSIDE_FOV_MARGIN, false);

			float toScreen = Game.Deck.GetMillimeterToScreen ();
            Vector2 area_center = fov_helper_.StandCenter + Vector2.down * CENTER_AREA_TO_SCREEN;
            Vector2 stand_center = fov_helper_.StandCenter;

			if (restrictPlayArea) {
				Color inside_color = new Color (0, 0.75f, 1);
				Game.DebugLineHelper.DrawCircle (area_center * toScreen, INSIDE_AREA_RADIUS * toScreen, 180, 360, inside_color, 2);
				Game.DebugLineHelper.DrawLine ((area_center + INSIDE_AREA_RADIUS * Vector2.right) * toScreen, (stand_center + INSIDE_AREA_RADIUS * Vector2.right) * toScreen, inside_color, 2);
				Game.DebugLineHelper.DrawLine ((area_center + INSIDE_AREA_RADIUS * Vector2.left) * toScreen, (stand_center + INSIDE_AREA_RADIUS * Vector2.left) * toScreen, inside_color, 2);

				Color outside_color = new Color (0, 0, 1);
				Game.DebugLineHelper.DrawCircle (area_center * toScreen, OUTSIDE_AREA_RADIUS * toScreen, 180, 360, outside_color, 2);
				Game.DebugLineHelper.DrawLine ((area_center + OUTSIDE_AREA_RADIUS * Vector2.right) * toScreen, (stand_center + OUTSIDE_AREA_RADIUS * Vector2.right) * toScreen, outside_color, 2);
				Game.DebugLineHelper.DrawLine ((area_center + OUTSIDE_AREA_RADIUS * Vector2.left) * toScreen, (stand_center + OUTSIDE_AREA_RADIUS * Vector2.left) * toScreen, outside_color, 2);
			
				Color close_color = new Color(1, 0.75f, 0, 0.5f);
				foreach (TangibleObject tangible in current_tangibles_) {
					float relative_to_screen = Mathf.Max(Game.Deck.GetWidthMillimeters (tangible.id) * Game.Deck.GetMillimeterToScreen(), 
						Game.Deck.GetHeightMillimeters (tangible.id) * Game.Deck.GetMillimeterToScreen());
					Game.DebugLineHelper.DrawCircle(new Vector2(tangible.location.X, tangible.location.Y) * toScreen, INCLUDE_DISTANCE * relative_to_screen, close_color, 1);
				}
			}
        }
        
		if (Game.EnableLineHelper) {
			foreach (TangibleObject tangible in e.objects) {
				if (!current_tangibles_.Contains (tangible)) {
					EventHelper.DebugDrawTangibleObject (Game.DebugLineHelper, tangible, Color.black, Game.Deck);
				}
			}
		}
        
        Dictionary<int, TangibleObject> id_to_tangible = new Dictionary<int, TangibleObject>();
        HashSet<int> temporally_new_ids = new HashSet<int>();
        HashSet<int> event_enabled_ids = new HashSet<int>();
        foreach (TangibleObject tangible in current_tangibles_) {
            current_unique_ids_.Add(tangible.unique_id);
            id_to_tangible[tangible.unique_id] = tangible;
            
            Vector2 p = new Vector2(tangible.location.X, tangible.location.Y);
            if (card_history_.Push(tangible.unique_id, p, dt)) {
                event_enabled_ids.Add(tangible.unique_id);
            }
            
            if (!last_unique_ids_.Contains(tangible.unique_id)) {
                temporally_new_ids.Add(tangible.unique_id);
            }
        }
        
		if (Game.EnableLineHelper) {
			connection_history_.DebugDraw (Game.DebugLineHelper, Game.Deck);
		}
        
		EventHelper.IsPairFunc is_pair_func = Game.IsPairFunc;
        
		cluster_helper_.UpdateCards(current_tangibles_.ToArray(), is_pair_func, Game.ClusterSort, Game.UseHysteresis, Game.Deck);
        //Debug.Log("cluster_helper_:\n" + cluster_helper_.ToString());
        
        current_connections = new ConnectionHelper();
		current_connections.UpdateCards(current_tangibles_, is_pair_func, Game.Deck);
		last_connections.RemoveAllExplicitlyUnconnected(current_tangibles_, is_pair_func, Game.Deck);
        HashSet<int> newly_connected_ids = current_connections.ComputeNewlyConnected(last_connections);
        //Debug.Log("current_connections:\n" + current_connections.ToString());
        
        List<List<ClusterHelper.Card>> value_cluster = new List<List<ClusterHelper.Card>>();
        
        int cluster_index = 0;
		IdConfig id_config = Game.Deck.GetIdConfig ();
        foreach (ClusterHelper.Cluster cluster in cluster_helper_.clusters) {
            if (cluster.Count == 0) continue;
            
            // Debug draw
			if (Game.EnableLineHelper) {
            	foreach (TangibleObject tangible in cluster) {
					EventHelper.DebugDrawTangibleObject(Game.DebugLineHelper, tangible, EventHelper.colors[cluster_index % EventHelper.colors.Length], Game.Deck);
            	}
                cluster_index++;
			}
            
            List<ClusterHelper.Card> cluster_cards = new List<ClusterHelper.Card>();
            foreach (TangibleObject tangible in cluster) {
                bool newcomer = cluster.newcomers.Contains(tangible.unique_id) || temporally_new_ids.Contains(tangible.unique_id);
                bool newly_connected = newly_connected_ids.Contains(tangible.unique_id);
                bool event_enabled = event_enabled_ids.Contains(tangible.unique_id);
				ClusterHelper.Card card = new ClusterHelper.Card(id_config.GetValueForId(tangible.id), tangible.unique_id, tangible.id, newcomer, newly_connected, event_enabled, tangible.location);
                cluster_cards.Add(card);
            }
            
            value_cluster.Add(cluster_cards);
        }

        Game.ClusterProcessor.UpdateClusters(value_cluster);
        
        if (Game.EnableLineHelper) {
			Game.DebugLineHelper.Commit ();
		}

        return e;
    }

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        /*
        float toScreen = EventHelper.millimeterToScreen;

        for (int i = 0; i < 1000; i++ ) {
            Vector2 p =  Random.insideUnitCircle * 200.0f + center_detection_mm_;
            if (InsideInnerPlayArea(p)) {
                Gizmos.color = Color.green;
            } else if (InsideOuterPlayArea(p)) {
                Gizmos.color = Color.yellow;
            } else {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawSphere(p * toScreen, 10);
        }
        */
    }
    #endif
}
