// using UnityEngine;
//
// public class GeometryHelper {
//
// 	public static readonly Vector2 INVALID = new Vector2 (float.NaN, float.NaN);
//
// 	// find all places where a line defined by 2 points intersects a circle
// 	public static int FindLineCircleIntersections(
// 		float cx, float cy, float radius,
// 		Vector2 pointA, Vector2 pointB,
// 		out Vector2 intersection1, out Vector2 intersection2)
// 	{
// 		float baX = pointB.x - pointA.x;
// 		float baY = pointB.y - pointA.y;
// 		float caX = cx - pointA.x;
// 		float caY = cy - pointA.y;
//
// 		float a = baX * baX + baY * baY;
//
// 		// short circuit here if a is 0 to avoid divide by 0
// 		if (a <= 0.00000001) {
// 			intersection1 = INVALID;
// 			intersection2 = INVALID;
// 			return 0;
// 		}
//
// 		float bBy2 = baX * caX + baY * caY;
// 		float c = caX * caX + caY * caY - radius * radius;
//
// 		float pBy2 = bBy2 / a;
// 		float q = c / a;
//
// 		float disc = pBy2 * pBy2 - q;
// 		if (disc < 0) {
// 			// No real solutions.
// 			intersection1 = INVALID;
// 			intersection2 = INVALID;
// 			return 0;
// 		}
//
// 		float tmpSqrt = Mathf.Sqrt(disc);
// 		float abScalingFactor1 = -pBy2 + tmpSqrt;
// 		float abScalingFactor2 = -pBy2 - tmpSqrt;
//
// 		intersection1 = new Vector2(pointA.x - baX * abScalingFactor1, pointA.y
// 		                     - baY * abScalingFactor1);
//
// 		// tangent, only one solution
// 		if (disc == 0) {
// 			intersection2 = INVALID;
// 			return 1;
// 		}
//
// 		intersection2 = new Vector2(pointA.x - baX * abScalingFactor2, pointA.y
// 		                     - baY * abScalingFactor2);
// 		return 2;
// 	}
//
// 	// gets whether a point on a line is inside the line segment between 2 points
// 	public static bool IsInsideLineSegment(Vector2 testPt, Vector2 point1, Vector2 point2) {
// 		if ((point1.x > point2.x && (testPt.x > point1.x || testPt.x < point2.x)) ||
// 		    (point2.x > point1.x && (testPt.x > point2.x || testPt.x < point1.x)) ||
// 		    (point1.y > point2.y && (testPt.y > point1.y || testPt.y < point2.y)) ||
// 		    (point2.y > point1.y && (testPt.y > point2.y || testPt.y < point1.y))) {
// 			return false;
// 		}
// 		return true;
// 	}
//
// 	// find points where a line segment intersects a circle
// 	// first figure out where the line intersects, then remove any of the points that are not on the line segment
// 	public static int FindLineSegmentCircleIntersections(
// 		float cx, float cy, float radius,
// 		Vector2 point1, Vector2 point2,
// 		out Vector2 intersection1, out Vector2 intersection2)
// 	{
// 		int lineIntersects = FindLineCircleIntersections (cx, cy, radius, point1, point2, out intersection1, out intersection2);
// 		if (lineIntersects > 0) {
// 			if (!IsInsideLineSegment(intersection1, point1, point2)) {
// 				lineIntersects--;
// 				intersection1 = intersection2;
// 				intersection2 = INVALID;
// 				if (lineIntersects > 0) {
// 					if (!IsInsideLineSegment(intersection1, point1, point2)) {
// 						lineIntersects--;
// 						intersection1 = INVALID;
// 					}
// 				}
// 			} else {
// 				if (lineIntersects > 1) {
// 					if (!IsInsideLineSegment(intersection2, point1, point2)) {
// 						lineIntersects--;
// 						intersection2 = INVALID;
// 					}
// 				}
// 			}
// 		}
// 		return lineIntersects;
// 	}
//
// 	public static float SolveForY(Vector2 pointInLine, float slope, float x) {
// 		return slope * (x - pointInLine.x) + pointInLine.y;
// 	}
//
// 	public static float SolveForX(Vector2 pointInLine, float slope, float y) {
// 		return (y - pointInLine.y)/slope + pointInLine.x;
// 	}
//
// 	public static Vector3 GetPositionOnLineSegmentBoundsCrossed(Vector2 innerPos, Vector2 outerPos, Bounds bounds) {
// 		// straight vertical line, must intersect top or bottom
// 		if (innerPos.x == outerPos.x) {
// 			Vector2 testMin = new Vector2 (innerPos.x, bounds.min.y);
// 			if (IsInsideLineSegment (testMin, innerPos, outerPos)) {
// 				return testMin;
// 			} else {
// 				return new Vector2 (innerPos.x, bounds.max.y);
// 			}
// 		}
//
// 		// straight horizontal line
// 		if (innerPos.y == outerPos.y) {
// 			Vector2 testMin = new Vector2 (bounds.min.x, innerPos.y);
// 			if (IsInsideLineSegment (testMin, innerPos, outerPos)) {
// 				return testMin;
// 			} else {
// 				return new Vector2 (bounds.max.x, innerPos.y);
// 			}
// 		}
//
// 		// for non vertical/horizontal slope should be valid, find line equation and solve for possible intersection points
// 		float slope = (outerPos.y - innerPos.y) / (outerPos.x - innerPos.x);
// 		Vector2 testPos = Vector2.zero;
// 		// right bound
// 		testPos.x = bounds.max.x;
// 		testPos.y = SolveForY (innerPos, slope, testPos.x);
// 		if (IsInsideLineSegment (testPos, innerPos, outerPos)) {
// 			return testPos;
// 		}
// 		// left bound
// 		testPos.x = bounds.min.x;
// 		testPos.y = SolveForY (innerPos, slope, testPos.x);
// 		if (IsInsideLineSegment (testPos, innerPos, outerPos)) {
// 			return testPos;
// 		}
// 		// top bound
// 		testPos.y = bounds.max.y;
// 		testPos.x = SolveForX (innerPos, slope, testPos.y);
// 		if (IsInsideLineSegment (testPos, innerPos, outerPos)) {
// 			return testPos;
// 		}
// 		// bottom bound
// 		testPos.y = bounds.min.y;
// 		testPos.x = SolveForX (innerPos, slope, testPos.y);
// 		if (IsInsideLineSegment (testPos, innerPos, outerPos)) {
// 			return testPos;
// 		}
//
// 		// somehow we failed, just return the inside position
// 		return innerPos;
// 	}
// }
//
