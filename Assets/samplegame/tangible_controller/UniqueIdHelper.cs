// ﻿using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
//
// namespace Tangible {
//
//     public class UniqueIdHelper {
//
//         int frame = 0;
//
//         // Need to validate both tests:
//         const float max_persistence_time = 0.5f; // 1/2 second
//         const int max_persistence_frame = 4; // 4 frames
//
// 		Deck deck_;
// 		IdConfig id_config_;
//
//         class UniqueIdPosition {
//             public readonly int id;
//             public readonly int unique_id;
//             public Vector2 pos;
//             public bool assigned = false;
//
//             // for persitences
//             public float last_time;
//             public int last_frame;
//             public TangibleObject tangible;
//
//             public UniqueIdPosition(int _id, int _unique_id, Vector2 _pos) {
//                 id = _id;
//                 unique_id = _unique_id;
//                 pos = _pos;
//             }
//
//             public bool CanPersist(float time, int frame) {
//                 return time - last_time <= max_persistence_time && frame - last_frame <= max_persistence_frame;
//             }
//         }
//
//         List<UniqueIdPosition> unique_id_positions = new List<UniqueIdPosition>();
//
// 		public UniqueIdHelper(Deck deck) {
// 			deck_ = deck;
// 			id_config_ = deck.GetIdConfig ();
//             int unique_id_counter = 0;
//             int current_unique_group = 0;
// 			int numIds = id_config_.GetNumIds ();
// 			for (int index = 0; index < numIds; index++) {
// 				if (deck_.GetUniqueGroup(index) != current_unique_group) {
// 					current_unique_group = deck_.GetUniqueGroup(index);
//                     unique_id_counter = 0;
//                 }
// 				for (int i = 0; i< deck_.GetCount(index); i++) {
//                     int unique_id = unique_id_counter;
//                     unique_id_counter++;
//
//                     Vector2 pos = new Vector2(unique_id, 10000); // Something far enough to be outside the playing field. (but small enough to be squared)
// 					unique_id_positions.Add(new UniqueIdPosition(deck_.GetId(index), unique_id, pos));
//                 }
//             }
//         }
//
//         public void Reset() {
//             for (int i = 0; i < unique_id_positions.Count; i++) {
//                 unique_id_positions[i].assigned = false;
//                 unique_id_positions[i].tangible = null;
//             }
//         }
//
//         public List<TangibleObject> UpdateUniqueIds(List<TangibleObject> tangibles) {
//             float now = UnityEngine.Time.time;
//             frame++;
//
//             // Light reset
//             for (int i = 0; i < unique_id_positions.Count; i++) {
//                 unique_id_positions[i].assigned = false;
//             }
//
//             // Match new tangibles to last positions
//             List<TangibleObject> tangibles_to_keep = new List<TangibleObject>();
//             foreach (TangibleObject tangible in tangibles) {
//                 UniqueIdPosition unique_position = Closest(tangible);
//                 if (unique_position != null) {
//                     tangible.unique_id = unique_position.unique_id;
//
//                     unique_position.assigned = true;
//                     unique_position.tangible = tangible;
//                     unique_position.pos = new Vector2(tangible.location.X, tangible.location.Y);
//                     unique_position.last_frame = frame;
//                     unique_position.last_time = now;
//
//                     tangibles_to_keep.Add(tangible);
// 				} else {
// 					Debug.LogError("Could not find a Unique Id for tangible: " + tangible.ToString());
// 				}
//             }
//
//             // Persist unassigned recent tangibles
//             //string str = "";
//             for (int i = 0; i < unique_id_positions.Count; i++) {
//                 if (unique_id_positions[i].assigned) continue;
//                 if (unique_id_positions[i].tangible == null) continue;
//                 if (unique_id_positions[i].CanPersist(now, frame)) {
//                     TangibleObject tangible = unique_id_positions[i].tangible;
//                     tangible.visible = false;
//                     tangibles_to_keep.Add(tangible);
//                     //str += "Adding: " + tangible.ToNiceString();
//                 } else {
//                     unique_id_positions[i].tangible = null;
//                 }
//             }
//             //if (str.Length > 0 ) Debug.Log(str);
//
//             // Sort tangibles
//             tangibles_to_keep.Sort(delegate (TangibleObject a, TangibleObject b) {
//                 return a.unique_id.CompareTo(b.unique_id);
//             });
//
//             return tangibles_to_keep;
//         }
//
//         private UniqueIdPosition Closest(TangibleObject tangible) {
//             float best_squared_distance = float.MaxValue;
//             UniqueIdPosition best = null;
//             for (int i = 0; i < unique_id_positions.Count; i++) {
// 				if (deck_.GetIdFromTangibleId(tangible.id) != unique_id_positions[i].id) continue;
//                 if (unique_id_positions[i].assigned) continue;
//
//                 float dx = tangible.location.X - unique_id_positions[i].pos.x;
//                 float dy = tangible.location.Y - unique_id_positions[i].pos.y;
//                 float dd = dx * dx + dy * dy;
//
//                 if (dd < best_squared_distance) {
//                     best_squared_distance = dd;
//                     best = unique_id_positions[i];
//                 }
//             }
//             return best;
//         }
//
//         public string ToString(List<TangibleObject> tangibles) {
//             string str = "";
//             for (int i = 0; i < tangibles.Count; i++) {
//                 str += tangibles[i].ToNiceString();
//                 str += "\n";
//             }
//             return str;
//         }
//
//         public override string ToString() {
//             string str = "";
//             for (int i = 0; i < unique_id_positions.Count; i++) {
//                 str += "[" + unique_id_positions[i].unique_id.ToString() + "]";
//                 str += " id:" + unique_id_positions[i].id.ToString();
//                 str += " p:" + unique_id_positions[i].pos.ToString();
//                 str += " t:" + ((unique_id_positions[i].tangible == null)? "null" : "ok");
//                 str += " last_frame:" + unique_id_positions[i].last_frame;
//                 str += " last_time:" + unique_id_positions[i].last_time;
//                 str += "\n";
//             }
//             return str;
//         }
//     }
// }
