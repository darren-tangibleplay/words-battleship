/*
 * Authors: Pramod Sharma, Jerome Scholler
 * Date: 12/20/2012.
 *
 * This file declares the controller API and related data objects. API allows users
 * (i.e. games) to register (or unregister) a call-back function to receive input events from
 * controller process.
 */
using UnityEngine;
using System.Collections.Generic;

namespace Tangible {
     
	/*
	 * Example of input:
	 {
	  "detectionArea": [
	    -68.7530670166016,
	    111.020545959473,
	    -214.002746582031,
	    -383.412322998047,
	    234.257186889648,
	    -401.791381835938,
	    98.2976379394531,
	    107.634872436523
	  ],
	  "frameCounter": 935,
	  "items": [
	    {
	      "angle": -1.40163362026215,
	      "confidence": 0.976470589637756,
	      "id": 227,
	      "pt": {
	        "x": 69.748291015625,
	        "y": 68.156005859375
	      },
	      "type": "topcode"
	    },
	    {
	      "angle": -0.676650702953339,
	      "confidence": 0.980392158031464,
	      "id": 47,
	      "pt": {
	        "x": -42.4225044250488,
	        "y": 35.0098114013672
	      },
	      "type": "topcode"
	    },
	    {
	      "angle": -0.555820226669312,
	      "confidence": 0.988235294818878,
	      "id": 369,
	      "pt": {
	        "x": 36.3182563781738,
	        "y": 6.81909132003784
	      },
	      "type": "topcode"
	    }
	  ],
	  "setup": {
	    "Quality": 3,
	    "Response": 1.30360794067383,
	    "standRect": {
	      "h": 480,
	      "w": 36,
	      "x": 51,
	      "y": 0
	    },
	    "version": "v2"
	  },
	  "time": 127686126
	}
	* */

	[System.Serializable]
	public class VisionItem {
		public VisionItem(int _id, Vector2 _pt, float _angle, float _confidence = 1.0f) {
			id = _id;
			pt = _pt;
			angle = _angle;
			confidence = _confidence;
		}

		public int id;
		public Vector2 pt;
		public float angle;
		public float confidence;
	}

	[System.Serializable]
	public class VisionResponse {
		public int time; // milliseconds
		public int frameCounter;
		public VisionItem[] items;
		public float[] detectionArea; // [x0, y0, x1, y1, x2, y2, x3, y3]
		public SetupCalibration setupAndCalibration;

		[System.NonSerialized] public float dt; // computed by the low level filter
	}

    // Declare delegate -- defines required signature: 
    public delegate Event EventDelegate(Event processEvent);
		
	public class UserId {
		 public const int INVALID = -1;
	}
	
    interface Controller {

        /*
         * Register user to receive new input events via call-back. Input parameters:
         * uid: id of the user (or game)
         * config: controller configuration parameters specific to user
         */
		bool Register(int userid, Config config, EventDelegate processEvent, int automaticDumpInterval);

        /*
         * Un-register the user (e.g. when game ends). This could be used to turn off controller
         * process all together until new user comes along and flush out any input buffers.
         */
        void Unregister(int userid);
		
		/*
		 * Skip all events while it is in the muted state.
		 */
		void Mute(bool _mute);
    };
 
    /*
     * Input event
     */
    public class Event {
		public VisionResponse response;     // low level

		/*
         * Detected objects at every frame.
         * TODO(jerome): deprecate this
         */
		public TangibleObject[] objects;    // High level
		
        public float[] bounds; // [x0, y0, x1, y1, x2, y2, x3, y3]
        
		public string SimulateRaw() {
			// This must match the feed from native
			string raw = "";
			foreach (TangibleObject t in objects) {
				raw += t.raw + ";";
			}
			return raw;
		}

		public static VisionResponse SimulateTangibleObjectsToVisionResponse(TangibleObject[] objects) {
			// This must match the feed from native
			VisionResponse fake_response = new VisionResponse();

			fake_response.time = Mathf.RoundToInt(UnityEngine.Time.time * 1000);
			fake_response.detectionArea = Tangible.FOVHelper.DefaultDetectionAreaBounds();

			List<VisionItem> items = new List<VisionItem>();
			foreach (TangibleObject t in objects) {
				items.Add(new VisionItem(t.id, new Vector2(t.location.X, t.location.Y), t.location.Orientation));
			}
			fake_response.items = items.ToArray();
			return fake_response;
		}

		public static List<TangibleObject> VisionResponseToTangibleObjects(VisionResponse response, Config.RecognitionMode recognitionMode) {

			List<TangibleObject> tangibles = new List<TangibleObject>();
			foreach (VisionItem item in response.items) {
				TangibleObject tangible = new TangibleObject(item.id, item.id, recognitionMode);
				tangible.location.X = item.pt.x;
				tangible.location.Y = item.pt.y;
				tangible.location.Orientation = item.angle;
				tangible.visible = true;
				tangible.lastVisible = response.time / 1000.0f; // ms -> s
				tangibles.Add(tangible);
			}

			return tangibles;
		}
    };

    /*
     * Tangible Object
     */
    public class TangibleObject {
		public enum Shape {
			none = -1,
			card = 0,
			tangram_big_triangle = 1,
			tangram_medium_triangle = 2,
			tangram_small_triangle = 3,
			tangram_square = 4,
			tangram_parallelogram_front = 5,
			tangram_parallelogram_back = 6,
            card_number = 7,
            card_dot = 8,
            color_coin = 9,
            secret_number = 10,
        };

		public TangibleObject (int _id, int _unique_id, Shape _shape) {
			id = _id;
			unique_id = _unique_id;
			shape = _shape;
			location = new Location();
		}

		public TangibleObject (int _id, int _unique_id, Config.RecognitionMode recognitionMode) {
            id = _id;
            unique_id = _unique_id;
			shape = GetShape(id, recognitionMode);
			location = new Location();
        }
        
        public TangibleObject(TangibleObject tangible) {
            id = tangible.id;
            unique_id = tangible.unique_id;
            shape = tangible.shape;
            visible = tangible.visible;
            lastVisible = tangible.lastVisible;
            raw = tangible.raw;
            location = new Location(tangible.location);
        }

		Shape GetShape(int id, Config.RecognitionMode recognitionMode) {
			if (recognitionMode == Config.RecognitionMode.TANGRAMS) {
				return TangramHelper.GetShape (id);
			}
			return Shape.card;
		}
		
		public override string ToString() {
            return id + "," + location.ToString() + "," + (visible? "1" : "0");
		}

        public string ToNiceString() {
            return unique_id + " (" + id + ") ," + location.ToString() + "," + (visible? "visible" : "hidden");
        }

        public readonly int id;
        public int unique_id;
        public readonly Shape shape;
		public string raw;
		public bool visible;
        public Location location;
		public float lastVisible;
    };
 
    /*
     * Object location on physical surface.
     * TODO: Add transformation details from physical to digital surface.
     */
    public class Location {
        public Location (float _x = 0.0f, float _y = 0.0f, float _orientation = 0.0f) {
            x = _x;
            y = _y;
            orientation = _orientation;
        }
        
        public Location(Location location) {
            x = location.x;
            y = location.y;
            orientation = location.orientation;
        }
     
        private float x;

        public float X {
            get { return x; }
            set { x = value; }
        }
     
        private float y;

        public float Y {
            get { return y; }
            set { y = value; }
        }
     
        private float orientation;

        public float Orientation {
            get { return orientation; }
            set { orientation = value; }
        }
		
		public override string ToString() {
			return X + "," + Y + "," + Orientation;
		}
    };
 
    public class Config {
		// the values matches the c++ VisionPlatform values.
		public enum RecognitionMode {
			NONE = 0,
			IMAGECARD = 1,
			TANGRAMS = 2,
			LETTER_TILES = 3,
			MATH_MANIPULATIVES = 27,
			STRAWBIES = 31,
			TOPCODE = 45
		};
		
		public Config(RecognitionMode _recognition) {
			recognition = _recognition;
		}
		
		private RecognitionMode recognition;
		
		public RecognitionMode Recognition {
            get { return recognition; }
        }
    };
     
}

