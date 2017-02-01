using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tangible {
    
    public class FOVHelper {

		//      top
		//    0 <-- 1
		//    |     ^ rig
		//lef V     |
		//    3 --> 2
		//      bot

        public Vector2[] fov_source_ = new Vector2[4]; // millimeter
        public Vector2[] fov_target_ = new Vector2[4]; // millimeter

        public Vector2 StandCenter {
            get { return (fov_target_[0] + fov_target_[1]) * 0.5f; }
        }

		public static float[] DefaultDetectionAreaBounds() {
			float[] bounds = new float[8];
			/*
			 	// Measured with the yellow square center, iPad Air, base Gen 2
                bounds[0] = -69; // x top left
                bounds[1] = 88; // y top left
                bounds[2] = 90; // x top right
                bounds[3] = 88; // y top right
                bounds[4] = 161; // x bottom right
                bounds[5] = -181; // y bottom right
                bounds[6] = -110; // x bottom left
                bounds[7] = -181; // y bottom left
            */
			// Measured with the yellow square center, iPad Mini, base Gen 2
			bounds[0] = -52; // x top left
			bounds[1] = 88; // y top left
			bounds[2] = 70; // x top right
			bounds[3] = 88; // y top right
			bounds[4] = 130; // x bottom right
			bounds[5] = -181; // y bottom right
			bounds[6] = -108; // x bottom left
			bounds[7] = -181; // y bottom left
			return bounds;
		}

        public void Update(float[] bounds) {
			if (bounds == null) {
				bounds = DefaultDetectionAreaBounds();                
			}
            //InfoUI.ShowLabel("[" + Mathf.RoundToInt(bounds[0]) + ", " + Mathf.RoundToInt(bounds[1]) + "] [" + Mathf.RoundToInt(bounds[2]) + ", " + Mathf.RoundToInt(bounds[3]) + "] [" + Mathf.RoundToInt(bounds[4]) + ", " + Mathf.RoundToInt(bounds[5]) + "] [" + Mathf.RoundToInt(bounds[6]) + ", " + Mathf.RoundToInt(bounds[7]) + "]");
                 
            fov_source_[0].x = bounds[0]; // top left
            fov_source_[0].y = bounds[1];
            fov_source_[1].x = bounds[2]; // top right
            fov_source_[1].y = bounds[3];
            fov_source_[2].x = bounds[4]; // bootom right
            fov_source_[2].y = bounds[5];
            fov_source_[3].x = bounds[6]; // bottom left
            fov_source_[3].y = bounds[7];

            fov_target_[0].x = bounds[0]; // top left
            fov_target_[0].y = bounds[1];
            fov_target_[1].x = bounds[2]; // top right
            fov_target_[1].y = bounds[3];
            fov_target_[2].x = bounds[4]; // bootom right
            fov_target_[2].y = bounds[5];
            fov_target_[3].x = bounds[6]; // bottom left
            fov_target_[3].y = bounds[7];
        }
        
        public bool InsideLeftEdgeFov(Vector2 p, float distance_min) {
            return InsideEdgeFov(p, 3, 0, distance_min);
        }
        
        public bool InsideTopEdgeFov(Vector2 p, float distance_min) {
            return InsideEdgeFov(p, 0, 1, distance_min);
        }
        
        public bool InsideRightEdgeFov(Vector2 p, float distance_min) {
            return InsideEdgeFov(p, 1, 2, distance_min);
        }
        
        public bool InsideBackEdgeFov(Vector2 p, float distance_min) {
            return InsideEdgeFov(p, 2, 3, distance_min);
        }
        
        public bool InsideEdgeFov(Vector2 p, int corner0, int corner1, float distance_min) {
            if (!GeometryHelper.TestRightSide(fov_target_[corner0], fov_target_[corner1], p)) return false;
            if (distance_min != 0 && GeometryHelper.PointToSegmentSqr(p, fov_target_[corner0], fov_target_[corner1]) < distance_min * distance_min) return false;
            return true;
        }    
            
        public bool InsideEdgesFov(Vector2 p, float distance_min) {
            if (!GeometryHelper.TestRightSide(fov_target_[3], fov_target_[0], p)) return false;
            if (!GeometryHelper.TestRightSide(fov_target_[0], fov_target_[1], p)) return false;
            if (!GeometryHelper.TestRightSide(fov_target_[1], fov_target_[2], p)) return false;
            if (!GeometryHelper.TestRightSide(fov_target_[2], fov_target_[3], p)) return false;
            if (distance_min != 0 && GeometryHelper.PointToSegmentSqr(p, fov_target_[3], fov_target_[0]) < distance_min * distance_min) return false;
            // Do not restrict area next to the stand
            //if (distance_min != 0 && GeometryHelper.PointToSegmentSqr(p, fov_target_[0], fov_target_[1]) < distance_min * distance_min) return false;
            if (distance_min != 0 && GeometryHelper.PointToSegmentSqr(p, fov_target_[1], fov_target_[2]) < distance_min * distance_min) return false;
            if (distance_min != 0 && GeometryHelper.PointToSegmentSqr(p, fov_target_[2], fov_target_[3]) < distance_min * distance_min) return false;
            return true;
        }
        
		public void DebugDrawFov(LineHelper line_helper, Deck deck, float inside_margin = 0, bool clear = true) {
            if (clear) line_helper.Clear();
            
			float toScreen = deck.GetMillimeterToScreen();

            line_helper.DrawLine(fov_source_[0] * toScreen, fov_source_[1] * toScreen, Color.black, 4);
            line_helper.DrawLine(fov_source_[1] * toScreen, fov_source_[2] * toScreen, Color.black, 2);
            line_helper.DrawLine(fov_source_[2] * toScreen, fov_source_[3] * toScreen, Color.black, 2);
            line_helper.DrawLine(fov_source_[3] * toScreen, fov_source_[0] * toScreen, Color.black, 2);

            line_helper.DrawLine(fov_target_[0] * toScreen, fov_target_[1] * toScreen, Color.black, 4);
            line_helper.DrawLine(fov_target_[1] * toScreen, fov_target_[2] * toScreen, Color.gray, 2);
            line_helper.DrawLine(fov_target_[2] * toScreen, fov_target_[3] * toScreen, Color.gray, 2);
            line_helper.DrawLine(fov_target_[3] * toScreen, fov_target_[0] * toScreen, Color.gray, 2);

            if (inside_margin > 0) {
                Color inside_color = new Color(0.5f, 0.75f, 0);
                //      top
                //    0 <-- 1
                //    |     ^ rig
                //lef V     |
                //    3 --> 2
                //      bot
                //Vector2 d_top = (fov_target_[0] - fov_target_[1]).normalized; // top
                Vector2 d_rig = (fov_target_[1] - fov_target_[2]).normalized; // right
                Vector2 d_bot = (fov_target_[2] - fov_target_[3]).normalized; // bottom
                Vector2 d_lef = (fov_target_[3] - fov_target_[0]).normalized; // left

                //Vector2 o_top = new Vector2(-d_top.y, d_top.x); // top
                Vector2 o_rig = new Vector2(-d_rig.y, d_rig.x); // right
                Vector2 o_bot = new Vector2(-d_bot.y, d_bot.x); // bottom
                Vector2 o_lef = new Vector2(-d_lef.y, d_lef.x); // left

                line_helper.DrawLine(fov_target_[0] * toScreen, fov_target_[1] * toScreen, inside_color, 2);
                line_helper.DrawLine((fov_target_[1] + o_rig * inside_margin) * toScreen, (fov_target_[2] + o_rig * inside_margin) * toScreen, inside_color, 2);
                line_helper.DrawLine((fov_target_[2] + o_bot * inside_margin) * toScreen, (fov_target_[3] + o_bot * inside_margin) * toScreen, inside_color, 2);
                line_helper.DrawLine((fov_target_[3] + o_lef * inside_margin) * toScreen, (fov_target_[0] + o_lef * inside_margin) * toScreen, inside_color, 2);
            }

            if (clear) line_helper.Commit();
        }
    }
}