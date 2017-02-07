// ﻿using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Tangible {
//
//     public class HistoryHelper {
//
//         class HistoryItem {
//             public HistoryItem(Vector2 _position, float dt) {
//                 position = _position;
//                 age = dt;
//             }
//             public bool Match(Vector2 other, float threshold) {
//                 Vector2 d = other - position;
//                 return d.SqrMagnitude() < threshold * threshold;
//             }
//             public void Merge(Vector2 other, float dt) {
//                 position = (position + other) * 0.5f;
//                 age += dt;
//             }
//             public Vector2 position;
//             public float age;
//         }
//
//         private float threshold_;
//         private int history_length_;
//         private Dictionary<int, List<HistoryItem>> history_ = new Dictionary<int, List<HistoryItem>>();
//
//         public HistoryHelper(float threshold, int history_length) {
//             threshold_ = threshold;
//             history_length_ = history_length;
//         }
//
//         // Returns true if the item can be used fo events
//         public bool Push(int id, Vector2 position, float dt) {
//             if (!history_.ContainsKey(id)) {
//                 history_[id] = new List<HistoryItem>();
//             }
//             List<HistoryItem> history = history_[id];
//             if (history.Count > 0) {
//                 if (history[0].Match(position, threshold_)) {
//                     history[0].Merge(position, dt);
//                     return true;
//                 }
//                 for (int i = 1; i < history.Count; i++) {
//                     HistoryItem item = history[i];
//                     if (item.Match(position, threshold_)) {
//                         item.Merge(position, dt);
//                         history.RemoveAt(i);
//                         history.Insert(0, item);
//                         return false;
//                     }
//                 }
//             }
//             while (history.Count >= history_length_) history.RemoveAt(history.Count - 1);
//             history.Insert(0, new HistoryItem(position, dt));
//             return true;
//         }
//
// 		public void DebugDraw(LineHelper line_helper, Deck deck) {
// 			float mmToScreen = deck.GetMillimeterToScreen ();
//             foreach(List<HistoryItem> history in history_.Values) {
//                 for (int i = 0; i < history.Count; i++) {
//                     HistoryItem item = history[i];
// 					line_helper.DrawCircle(item.position * mmToScreen, threshold_ * mmToScreen, item.age > 1.0f ? Color.blue : Color.red);
//                 }
//             }
//         }
//     }
// }
