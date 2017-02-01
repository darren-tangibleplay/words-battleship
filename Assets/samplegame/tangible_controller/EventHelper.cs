using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tangible {
	
    public class EventHelper {

        public static Color[] colors = {
            Color.yellow,
            Color.cyan,
            Color.green,
            Color.magenta,
            Color.red,
            Color.blue,
        };

		public delegate bool IsPairFunc(TangibleObject o1, TangibleObject o2, bool strict, Deck deck);

    	private static int CompareUniqueId(TangibleObject a, TangibleObject b) {
    		return a.unique_id.CompareTo(b.unique_id);
    	}

        public static int PairKey(TangibleObject a, TangibleObject b) {
            int aa = Mathf.Min(a.unique_id, b.unique_id);
            int bb = Mathf.Max(a.unique_id, b.unique_id);
            return (aa << 16) + bb;
        }

    	public static float NormalizeOrientation(float orientation) {
    		float angle = (orientation + 360.0f) % 90;
    		if (angle > 45.0f) angle -= 90.0f;
    		return angle;
    	}

        public static float NormalizeOrientation(float orientation, float modulo) {
            float angle = (orientation + 360.0f) % modulo;
            if (angle > modulo/2) angle -= modulo;
            return angle;
        }
			
		public static Vector3 GetPoint3dMillimeters(Vector3 point, TangibleObject tangible, bool flipped = false) {
			float orientation = tangible.location.Orientation;
			if (flipped) {
				orientation += 180f;
			}
			Quaternion q = Quaternion.Euler (0, 0, orientation);
			Vector3 t = new Vector3(tangible.location.X, tangible.location.Y, 0);
			point = q * point;
			point += t;
			return point;
		}
			
        public static Vector3[] GetPoints3dMillimeters (TangibleObject tangible, Deck deck) {
			Vector3[] points = deck.GetEdgePointsMillimeters (tangible.id);

            Quaternion q = Quaternion.Euler (0, 0, tangible.location.Orientation);
			Vector3 t = new Vector3(tangible.location.X, tangible.location.Y, 0);
			int numPoints = points.Length;
			for (int i = 0; i < numPoints; i++) {
				points[i] = q * points[i];
                points[i] += t;
            }
            return points;
        }

		public static void DebugDrawTangibleObject(LineHelper line_helper, TangibleObject tangible, Color color, Deck deck, bool clear = false) {
            if (clear) line_helper.Clear();

			Vector3[] p = GetPoints3dMillimeters (tangible, deck);
			int numPoints = p.Length; 
			for (int i = 0; i < numPoints; i++) {
				line_helper.DrawLine(p[i] * deck.GetMillimeterToScreen(), p[(i+1)%numPoints] * deck.GetMillimeterToScreen(), color, 2);
			}

            if (clear) line_helper.Commit();
            
        }
    		
		public static List<TangibleObject> FilterCircle(TangibleObject[] objects, float radius, Deck deck) {
            float interestRadiusSqr = radius * radius;
    			
    		// keep the ones next to the center and the ones next to them
    		List<TangibleObject> candidates = new List<TangibleObject>();
    		List<TangibleObject> keep = new List<TangibleObject>();
    		foreach (TangibleObject t in objects) {
    			float x = t.location.X;
    			float y = t.location.Y > 0 ? 0 : t.location.Y;
    			if (x * x + y * y < interestRadiusSqr) {
    				keep.Add(t);
    			} else {
    				candidates.Add(t);
    			}	
    		}
    		int lastKeepCount = 0;
    		while(lastKeepCount != keep.Count) {
    			lastKeepCount = keep.Count;
    			bool found = false;
    			for (int i = 0; i < keep.Count && !found; i++) {
    				for (int j = 0; j < candidates.Count && !found; j++) {
    					if (IsClose(keep[i], candidates[j], deck)) {
    						keep.Add(candidates[j]);
    						candidates.RemoveAt(j);
    						found = true;
    					}
    				}
    			}
    		}
    		/*
    		string s = "keep ";
    		foreach (TangibleObject t in keep) {
    			s += t.unique_id + " ";
    		}
    		s += " ignore ";
    		foreach (TangibleObject t in candidates) {
    			s += t.unique_id + " ";
    		}
    		Debug.Log(s);
    		*/
    		return keep;
    	}
    		
		public static bool IsClose(TangibleObject o1, TangibleObject o2, Deck deck, float relative_threshold = 2.0f) {
    		float dx = o1.location.X - o2.location.X;
    		float dy = o1.location.Y - o2.location.Y;
    		float distSqr = dx * dx + dy * dy;
			float size1_mm = Mathf.Max(deck.GetWidthMillimeters(o1.id), deck.GetHeightMillimeters(o1.id));
			float size2_mm = Mathf.Max(deck.GetWidthMillimeters(o2.id), deck.GetHeightMillimeters(o2.id));
			float size_mm = size1_mm * 0.5f + size2_mm * 0.5f;
            return distSqr < size_mm * size_mm * relative_threshold * relative_threshold;
    	}

		public static bool IsPairNever(TangibleObject o1, TangibleObject o2, bool strict, Deck deck) {
			return false;
		}

		// edge is the side of o1 that must match the opposite side of o2
		public static bool IsPairAtAttachPoint(TangibleObject o1, TangibleObject o2, PairEdge edge, float positionError, Deck deck) {
			Vector3 attach1 = deck.GetAttachPointMillimeters (o1.id, edge);
			Vector3 attach2 = deck.GetAttachPointMillimeters(o2.id, deck.GetOppositeEdge(edge));
			Vector3 edgePt1 = GetPoint3dMillimeters(attach1, o1);
			Vector3 edgePt2 = GetPoint3dMillimeters(attach2, o2);
			if (Vector3.Distance (edgePt1, edgePt2) <= positionError) {
				return true;
			}
			if (!deck.ShouldCheckFlip (o1.id) && !deck.ShouldCheckFlip (o2.id)) {
				return false;
			}
			// check for cases where vision might be returning something with a flipped orientation
			Vector3 oppositePt1 = GetPoint3dMillimeters(attach1, o1, true);
			Vector3 oppositePt2 = GetPoint3dMillimeters(attach2, o2, true);
			if (deck.ShouldCheckFlip (o1.id) && deck.ShouldCheckFlip (o2.id) && Vector3.Distance (oppositePt1, oppositePt2) <= positionError) {
				return true;
			}
			if (deck.ShouldCheckFlip (o1.id) && Vector3.Distance (oppositePt1, edgePt2) <= positionError) {
				return true;
			}
			if (deck.ShouldCheckFlip (o2.id) && Vector3.Distance (edgePt1, oppositePt2) <= positionError) {
				return true;
			}	
			return false;
		}
			
		// edge returned is how o1 relates to o2
		public static PairEdge GetPairWithEdge(TangibleObject o1, TangibleObject o2, float positionError, Deck deck, bool requireSameOrientation = false) {
			Vector3[] p1 = GetPoints3dMillimeters(o1, deck);
			Vector3[] p2 = GetPoints3dMillimeters(o2, deck);

			HashSet<int> matches1 = new HashSet<int> ();
			HashSet<int> matches2 = new HashSet<int> ();
			int numPts1 = p1.Length;
			int numPts2 = p2.Length;

			for (int i = 0; i < numPts1; i++) {
				for (int j = 0; j < numPts2; j++) {
					if (Vector3.Distance (p1 [i], p2 [j]) <= positionError) {
						matches1.Add (i);
						matches2.Add (j);
					}
				}
			}

			PairEdge edge1 = deck.GetEdge (matches1);
			PairEdge edge2 = deck.GetEdge (matches2);

			// return right edge if it's a valid edge and either aligning orientation doesn't matter
			// or the second edge is oriented in a way to match the first
			if (edge1 != PairEdge.NONE) {
				if (!requireSameOrientation || edge2 == deck.GetOppositeEdge (edge1)) {
					return edge1;
				}
			}

			// if same orientation is required we haven't found a valid pair.  otherwise allow for the possibility
			// that the first object is small and matches along multiple edges and get the edge based on the second object
			if (requireSameOrientation) {
				return PairEdge.NONE;
			} else {
				return deck.GetOppositeEdge (edge2);
			}
		}
    		
		public static bool IsPair(TangibleObject o1, TangibleObject o2, bool strict, Deck deck) {
			// considered a pair if any edge of one object is close enough to any edge of the other
			float positionError = strict ? deck.GetPositionErrorStrict() : deck.GetPositionErrorLoose();
			return GetPairWithEdge (o1, o2, positionError, deck) != PairEdge.NONE;
    	}

		// connected horizontally based on the edges of the tiles
		public static bool IsPairHorizontalEdge(TangibleObject o1, TangibleObject o2, bool strict, Deck deck) {
			float positionError = strict ? deck.GetPositionErrorStrict() : deck.GetPositionErrorLoose();
			PairEdge edge = GetPairWithEdge (o1, o2, positionError, deck);
			return edge == PairEdge.RIGHT || edge == PairEdge.LEFT;
		}

		// connected vertically based on the edges of the tiles
		public static bool IsPairVerticalEdge(TangibleObject o1, TangibleObject o2, bool strict, Deck deck) {
			float positionError = strict ? deck.GetPositionErrorStrict() : deck.GetPositionErrorLoose();
			PairEdge edge = GetPairWithEdge (o1, o2, positionError, deck);
			return edge == PairEdge.BOTTOM || edge == PairEdge.TOP;
		}

		// aligned roughly horizontally in relation to the iPad
		public static bool IsPairHorizontal(TangibleObject o1, TangibleObject o2, bool strict, Deck deck) {
            if (IsPair(o1, o2, strict, deck)) {
				float cardHeight = deck.GetHeightMillimeters (o1.id) * 0.5f + deck.GetHeightMillimeters (o2.id) * 0.5f;
				float y_distance_threshold = cardHeight * (strict ? deck.GetVerticalErrorStrict() : deck.GetVerticalErrorLoose());
                float dy = Mathf.Abs(o1.location.Y - o2.location.Y);
                return dy < y_distance_threshold;
            }
            return false;
        }

		// doesn't matter how close they are together horizontally as long as the y values are within a threshold
		// to be considered part of the same row
		public static bool IsPairSameRow(TangibleObject o1, TangibleObject o2, bool strict, Deck deck) {
			float cardHeight = deck.GetHeightMillimeters (o1.id) * 0.5f + deck.GetHeightMillimeters (o2.id) * 0.5f;
			float y_distance_threshold = cardHeight * (strict ? deck.GetVerticalErrorStrict() : deck.GetVerticalErrorLoose());
			float dy = Mathf.Abs(o1.location.Y - o2.location.Y);
			return dy < y_distance_threshold;
		}

		// get whether this has any type of connection
		public static bool IsPairStrawbies(TangibleObject o1, TangibleObject o2, bool strict, Deck deck) {
			return IsPairInstruction (o1, o2, strict, deck) || IsPairActionList (o1, o2, strict, deck);
		}

		// checking if this is a pair that is part of the same instruction row
		public static bool IsPairInstruction(TangibleObject o1, TangibleObject o2, bool strict, Deck deck) {
			ProgrammingIdConfig id_config = deck.GetIdConfig () as ProgrammingIdConfig;
			ProgrammingType type1Left = id_config.GetTileTypeAtEdge (o1.id, PairEdge.LEFT);
			ProgrammingType type2Left = id_config.GetTileTypeAtEdge (o2.id, PairEdge.LEFT);
			ProgrammingType type1Right = id_config.GetTileTypeAtEdge (o1.id, PairEdge.RIGHT);
			ProgrammingType type2Right = id_config.GetTileTypeAtEdge (o2.id, PairEdge.RIGHT);
			ProgrammingAction action1 = id_config.GetActionForId (o1.id);
			ProgrammingAction action2 = id_config.GetActionForId (o2.id);

			float defaultError = strict ? deck.GetPositionErrorStrict() : deck.GetPositionErrorLoose();

			if (type1Right == ProgrammingType.Action) {
				// can have quantifier or a direction if it's not a repeat
				if (type2Left == ProgrammingType.Quantifier || (type2Left == ProgrammingType.Direction && action1 != ProgrammingAction.REPEAT)) {
					if (EventHelper.IsPairAtAttachPoint (o1, o2, PairEdge.RIGHT, defaultError, deck)) {
						return true;
					}
				}
			}

			// magic can have an if block to the right
			if (type1Right == ProgrammingType.Magic) {
				if (type2Left == ProgrammingType.If) {
					if (EventHelper.IsPairAtAttachPoint (o1, o2, PairEdge.RIGHT, defaultError, deck)) {
						return true;
					}
				}
			}

			// direction can have an action (not repeat) to the left or a quantifier to the right
			if (type1Left == ProgrammingType.Direction) {
				if (type2Right == ProgrammingType.Action && action2 != ProgrammingAction.REPEAT) {
					if (EventHelper.IsPairAtAttachPoint (o1, o2, PairEdge.LEFT, defaultError, deck)) {
						return true;
					}
				}
			}

			if (type1Right == ProgrammingType.Direction) {
				if (type2Left == ProgrammingType.Quantifier) {
					if (EventHelper.IsPairAtAttachPoint (o1, o2, PairEdge.RIGHT, defaultError, deck)) {
						return true;
					}
				}
			}

			// quantifier can have an action or direction to the left or an if block to the right
			if (type1Left == ProgrammingType.Quantifier) {
				if (type2Right == ProgrammingType.Action || type2Right == ProgrammingType.Direction) {
					if (EventHelper.IsPairAtAttachPoint (o1, o2, PairEdge.LEFT, defaultError, deck)) {
						return true;
					}
				}
			}

			if (type1Right == ProgrammingType.Quantifier) {
				if (type2Left == ProgrammingType.If) {
					if (EventHelper.IsPairAtAttachPoint (o1, o2, PairEdge.RIGHT, defaultError, deck)) {
						return true;
					}
				}
			}

			// if block can have a quantifier or magic to the left
			if (type1Left == ProgrammingType.If) {
				if (type2Right == ProgrammingType.Quantifier || type2Right == ProgrammingType.Magic) {
					if (EventHelper.IsPairAtAttachPoint (o1, o2, PairEdge.LEFT, defaultError, deck)) {
						return true;
					}
				}
			}

			return false;
		}

		// checking if these are actions that can be grouped together to form a longer action list
		public static bool IsPairActionList(TangibleObject o1, TangibleObject o2, bool strict, Deck deck) {
			ProgrammingIdConfig id_config = deck.GetIdConfig () as ProgrammingIdConfig;
			ProgrammingAction action1 = id_config.GetActionForId (o1.id);
			ProgrammingAction action2 = id_config.GetActionForId (o2.id);

			float defaultError = strict ? deck.GetPositionErrorStrict() : deck.GetPositionErrorLoose();

			// ignore anything that is not the core action for an instruction
			if (action1 == ProgrammingAction.NONE || action1 == ProgrammingAction.IF ||
				action2 == ProgrammingAction.NONE || action2 == ProgrammingAction.NONE) {
				return false;
			}

			// everything but repeat has a slot on top
			bool topSlot1 = !(action1 == ProgrammingAction.REPEAT);
			bool topSlot2 = !(action2 == ProgrammingAction.REPEAT);

			// everything but end block has a slot on bottom
			bool bottomSlot1 = action1 != ProgrammingAction.END_ACTIVE && action1 != ProgrammingAction.END_INACTIVE && action1 != ProgrammingAction.END_OCCLUDED;
			bool bottomSlot2 = action2 != ProgrammingAction.END_ACTIVE && action2 != ProgrammingAction.END_INACTIVE && action2 != ProgrammingAction.END_OCCLUDED;

			// see if there is a top or bottom pair where the slots line up, otherwise not a pair
			if (topSlot1 && bottomSlot2 && EventHelper.IsPairAtAttachPoint (o1, o2, PairEdge.TOP, defaultError, deck)) {
				return true;
			}
			if (bottomSlot1 && topSlot2 && EventHelper.IsPairAtAttachPoint (o1, o2, PairEdge.BOTTOM, defaultError, deck)) {
				return true;
			}
			return false;
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
    			s += t.unique_id + " ";
    		}
    		s += "]";
    		return s;
    	}

        public static string ToString(List<int> ints) {
            string s = "[";
            foreach (int i in ints) {
                s += i + " ";
            }
            s += "]";
            return s;
        }

        public static string ToString(HashSet<int> ints) {
            string s = "[";
            foreach (int i in ints) {
                s += i + " ";
            }
            s += "]";
            return s;
        }

    }
}
