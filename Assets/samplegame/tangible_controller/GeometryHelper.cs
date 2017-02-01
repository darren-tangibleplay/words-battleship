using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tangible {
    
    public class GeometryHelper {

        public static float PointToSegmentSqr(Vector2 p, Vector2 s0, Vector2 s1) {
            Vector2 s01 = s1 - s0;
            Vector2 s0p = p - s0;
            float lSqr = Vector2.Dot(s01, s01);
            if (lSqr < 0.00001f) return Vector2.Dot(s0p, s0p);
            float t = Vector2.Dot(s0p, s01) / lSqr;
            if (t < 0) return Vector2.Dot(s0p, s0p);
            if (t > 1) {
                Vector2 s1p = p - s1;
                return Vector2.Dot(s1p, s1p);
            }
            Vector2 q = s0 + t * s01;
            Vector2 qp = p - q;
            return Vector2.Dot(qp, qp);
        }
        
        public static Vector2 ProjectPointOnLine(Vector2 p, Vector2 s0, Vector2 s1) {
            Vector2 s01 = s1 - s0;
            Vector2 s0p = p - s0;
            float lSqr = Vector2.Dot(s01, s01);
            if (lSqr < 0.00001f) return s0;
            float t = Vector2.Dot(s0p, s01) / lSqr;
            return s0 + t * s01;
        }
        
        public static float DistanceAngle(float a1, float a2, float period) {
            float dAngle = Mathf.Abs(a1 - a2) % period;
            return Mathf.Min(dAngle, period - dAngle);
        }

		public static void TestDistanceAngle() {
			float a, b;
			a = 0; 
			b = 0;
			Debug.Log("DistanceAngle( " + a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(a, b, 90));
			a = 1; 
			b = -1;
			Debug.Log("DistanceAngle( " + a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(a, b, 90));
			Debug.Log("DistanceAngle( " + b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(b, a, 90));
			a = 45; 
			b = -45;
			Debug.Log("DistanceAngle( " + a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(a, b, 90));
			Debug.Log("DistanceAngle( " + b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(b, a, 90));
			a = 44; 
			b = -44;
			Debug.Log("DistanceAngle( " + a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(a, b, 90));
			Debug.Log("DistanceAngle( " + b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(b, a, 90));
			a = 46; 
			b = -46;
			Debug.Log("DistanceAngle( " + a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(a, b, 90));
			Debug.Log("DistanceAngle( " + b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(b, a, 90));
			a = 89; 
			b = 1;
			Debug.Log("DistanceAngle( " + a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(a, b, 90));
			Debug.Log("DistanceAngle( " + b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(b, a, 90));
			Debug.Log("DistanceAngle( " + -a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(-a, b, 90));
			Debug.Log("DistanceAngle( " + -b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(-b, a, 90));
			a = 180; 
			b = -182;
			Debug.Log("DistanceAngle( " + a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(a, b, 90));
			Debug.Log("DistanceAngle( " + b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(b, a, 90));
			Debug.Log("DistanceAngle( " + -a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(-a, b, 90));
			Debug.Log("DistanceAngle( " + -b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(-b, a, 90));
			a = 359; 
			b = 1;
			Debug.Log("DistanceAngle( " + a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(a, b, 90));
			Debug.Log("DistanceAngle( " + b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(b, a, 90));
			Debug.Log("DistanceAngle( " + -a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(-a, b, 90));
			Debug.Log("DistanceAngle( " + -b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(-b, a, 90));
			a = 1; 
			b = 34;
			Debug.Log("DistanceAngle( " + a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(a, b, 90));
			Debug.Log("DistanceAngle( " + b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(b, a, 90));
			Debug.Log("DistanceAngle( " + -a + ", " + b + ", 90) => " + GeometryHelper.DistanceAngle(-a, b, 90));
			Debug.Log("DistanceAngle( " + -b + ", " + a + ", 90) => " + GeometryHelper.DistanceAngle(-b, a, 90));
		}

        private static float NormalizeAngle(float a, float period) {
            a = ((a % 360.0f) + 360.0f) % period;
            if (a > (period / 2.0f)) a -= period;
            return a;
        }
        
        public static float NormalizeAngle(float a) {
            a = ((a % 360.0f) + 360.0f) % 360.0f;
            if (a > 180.0f) a -= 360.0f;
            return a;
        }
        
        public static float NormalizeAngle(float a, TangibleObject.Shape shape) {
            if (shape == TangibleObject.Shape.tangram_square) return NormalizeAngle(a, 90);
            if (shape == TangibleObject.Shape.tangram_parallelogram_back) return NormalizeAngle(a, 180);
            if (shape == TangibleObject.Shape.tangram_parallelogram_front) return NormalizeAngle(a, 180);
            return NormalizeAngle(a, 360);
        }
        
        public static float DistanceAngle(float a1, float a2, TangibleObject.Shape shape) {
            if (shape == TangibleObject.Shape.tangram_square) return DistanceAngle(a1, a2, 90);
            if (shape == TangibleObject.Shape.tangram_parallelogram_back) return DistanceAngle(a1, a2, 180);
            if (shape == TangibleObject.Shape.tangram_parallelogram_front) return DistanceAngle(a1, a2, 180);
            return DistanceAngle(a1, a2, 360);
        }

        public static bool TestRightSide(Vector2 s0, Vector2 s1, Vector2 p) {
            Vector3 side1 = (s1 - s0);
            Vector3 side2 = (p - s0);
            return Vector3.Cross(side1, side2).z < 0;
        }
    }
}