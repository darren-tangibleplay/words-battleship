using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tangible;

public class PhysicalController : PhysicalControllerBase, Tangible.Controller {

#if UNITY_EDITOR
    
	private static bool _register_with_case_cards(string objectName, string functionName, Color32[] pixels, int with, int height, int subx, int suby, int count, bool letter) { return false; }
	private static bool _register_tangram(string objectName, string functionName) { return false; }
	private static bool _register_with_strawbies(string objectName, string functionName, Color32[] pixels, int width, int height, string json) { return false; }
	private static bool _register_with_strawbies2(string objectName, string functionName, Color32[] pixels, int width, int height, string json, int minSecondsBetweenDumps) { return false; }
	private static bool _register_with_numbers(string objectName, string functionName, Color32[] pixels, int with, int height, int subx, int suby, int count, bool register_with_color_coin) { return false; }
	private static bool _register_with_topcodes(string objectName, string functionName) { return false; }
	private static bool _unregister() { return false; }
    private static bool _hint_event(string events) { return false; }
    private static bool _set_color_coin_detection_with_numbers(bool enable) { return false; }
	
#elif UNITY_IPHONE

	[DllImport ("__Internal")]
	private static extern bool _register_with_case_cards(string objectName, string functionName, Color32[] pixels, int with, int height, int subx, int suby, int count, bool letter);
	[DllImport ("__Internal")]
	private static extern bool _register_tangram(string objectName, string functionName);
	[DllImport ("__Internal")]
	private static extern bool _register_with_strawbies(string objectName, string functionName, Color32[] pixels, int width, int height, string json);
	[DllImport ("__Internal")]
	private static extern bool _register_with_strawbies2(string objectName, string functionName, Color32[] pixels, int width, int height, string json, int minSecondsBetweenDumps);
	[DllImport ("__Internal")]
	private static extern bool _register_with_numbers(string objectName, string functionName, Color32[] pixels, int with, int height, int subx, int suby, int count, bool register_with_color_coin);
	[DllImport ("__Internal")]
	private static extern bool _register_with_topcodes(string objectName, string functionName);
	[DllImport ("__Internal")]
    private static extern bool _unregister();
    [DllImport ("__Internal")]
    private static extern bool _hint_event(string events);
    [DllImport ("__Internal")]
    private static extern bool _set_color_coin_detection_with_numbers(bool enable);

#else

	private static bool _register_with_case_cards(string objectName, string functionName, Color32[] pixels, int with, int height, int subx, int suby, int count, bool letter) { return false; }
	private static bool _register_tangram(string objectName, string functionName) { return false; }
	private static bool _register_with_strawbies(string objectName, string functionName, Color32[] pixels, int width, int height, string json) { return false; }
	private static bool _register_with_strawbies2(string objectName, string functionName, Color32[] pixels, int width, int height, string json, int minSecondsBetweenDumps) { return false; }
	private static bool _register_with_numbers(string objectName, string functionName, Color32[] pixels, int with, int height, int subx, int suby, int count, bool register_with_color_coin) { return false; }
	private static bool _register_with_topcodes(string objectName, string functionName) { return false; }
	private static bool _unregister() { return false; }
    private static bool _hint_event(string events) { return false; }
    private static bool _set_color_coin_detection_with_numbers(bool enable) { return false; }

#endif
	
    [SerializeField]
    private Transform card_prefab_;

	[SerializeField]
	private Transform tangram_prefab_;

	[SerializeField]
	private Transform topcode_prefab_;

    private UniqueIdHelper unique_id_helper_;
    private int userId = Tangible.UserId.INVALID;
    private Tangible.EventDelegate processEvent = null;
	private List<TangibleObject> tangibles_ = new List<TangibleObject>();
	private Tangible.Config.RecognitionMode currentRecognition;
	
    private List<PhysicalObject> debug_objects_ = new List<PhysicalObject>();
	private Deck deck;
	private bool show_debug_ = false;
	private bool recorded_ = false;
    private bool enable_color_coin_ = false;
	private string[] recorded_events_;
	private int eventIndex = 0;
	private int skip = 0;
	private float millimeterToScreen;

    private SetupPlatform setup_hardware_;
	
	public void Init(Deck _deck) {
		deck = _deck;
		unique_id_helper_ = new UniqueIdHelper (deck);
		millimeterToScreen = deck.GetMillimeterToScreen ();
	}
	
    public void SetDebug(bool show_debug) {
		show_debug_ = show_debug;
        UpdateDebugVisibility();
	}
	
	public void SetRecorded(bool _recorded, string events) {
		recorded_ = _recorded;
		recorded_events_ = events.Split('|');
		eventIndex = 0;
		Debug.Log("RECORDED EVENT: playing " + recorded_events_.Length + " frames");
	}
	
	public bool Supported() {
		return HardwareSupported() || recorded_; 
	}

	private Vector3 MillimeterToScreen(float x, float y) {
		return new Vector3(millimeterToScreen * x, millimeterToScreen * y);
	}

    protected bool isMini() {
#if UNITY_IPHONE
        return UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPadMini1Gen || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPadMini2Gen;
#else
		return false;
#endif
	}

	// This is less than optimal... ideally, vision would use the same orientation convention
	private static VisionResponse VisionPlatformFixup(VisionResponse response, Config.RecognitionMode recognition) {
		for (int i = 0; i < response.items.Length; i++) {
			if (recognition == Config.RecognitionMode.TOPCODE) {
				response.items[i].angle *= Mathf.Rad2Deg;		
			}
			response.items[i].angle *= -1;
		}
		float tmp = response.detectionArea [2];
		response.detectionArea[2] = response.detectionArea[6];	
		response.detectionArea [6] = tmp;
		tmp = response.detectionArea [3];
		response.detectionArea [3] = response.detectionArea [7];
		response.detectionArea [7] = tmp;
		return response;
	}

	protected override bool DispatchEvent(string events) {
		bool isJson = false;
		if (events != null) {
			events = events.Trim ();
			if (events.StartsWith ("{")) {
				isJson = true;
			}
		}

		if (base.DispatchEvent(events)) return true;
		
		if (skip > 0) {
			skip--;
			return true;
		}
		
        if (events != null && events.Length > 0 && show_debug_) Debug.Log(events);
		
        // Regular tangible events
		tangibles_.Clear();
		
		if (processEvent == null) return false;
		if (mute) return true;

		Tangible.Event e = new Tangible.Event();
		
		if (isJson) {
			e.response = VisionPlatformFixup (JsonUtility.FromJson<VisionResponse> (events), currentRecognition);
			tangibles_ = Tangible.Event.VisionResponseToTangibleObjects (e.response, currentRecognition);
		} else {
			// index0,x0,y0,orientation0,visible0;index1,x1,y1,orientation1 ...
			string[] eventList = events.Split (';');
			foreach (string values in eventList) {
				if (values.Length == 0)
					continue;
			
				string[] valueList = values.Split (',');
				if (valueList.Length != 5) {
					Debug.LogError ("Unknown value list: " + values);
					continue;
				}
			
				int id = int.Parse (valueList [0]);			
				float x = float.Parse (valueList [1]);
				float y = float.Parse (valueList [2]);
				float orientation = float.Parse (valueList [3]);
				bool visible = int.Parse (valueList [4]) == 1;

				if (currentRecognition == Config.RecognitionMode.TOPCODE) {
					orientation *= Mathf.Rad2Deg;		
				}
				orientation = -orientation;

				if (!deck.ContainsId (id)) {
					continue;
				}
			
				TangibleObject tangible = new TangibleObject (id, id, currentRecognition);
				tangible.location.X = x;
				tangible.location.Y = y;
				tangible.location.Orientation = orientation;
				tangible.visible = visible;
				tangible.raw = values;
				if (visible)
					tangible.lastVisible = UnityEngine.Time.timeSinceLevelLoad;

				tangibles_.Add (tangible);
			}
		}

        // Re-assign unique ids and add some Tangibles from last frames
        tangibles_ = unique_id_helper_.UpdateUniqueIds(tangibles_);

        // Match with debug objects
	    // Only do this if we're actually showing debug objects right now as it's a big performance impact to create
		// the physical objects.  If we start showing debug, this function is called often enough that the objects should
	    // get created really quickly after the point they are actually needed
		List<PhysicalObject> new_physical_objects = null;
		if (show_debug_) {
			Debug.Log ("creating debug objects");
			new_physical_objects = new List<PhysicalObject>();

			foreach (TangibleObject tangible in tangibles_) {

				PhysicalObject physical_object = debug_objects_.Find (delegate(PhysicalObject obj) {
					return obj.UniqueId == tangible.unique_id;
				});

				if (physical_object == null) {
					Transform prefab = card_prefab_;
					if (currentRecognition == Tangible.Config.RecognitionMode.TANGRAMS) {
						prefab = tangram_prefab_;				
					} else if (currentRecognition == Tangible.Config.RecognitionMode.TOPCODE) {
						prefab = topcode_prefab_;				
					}
					Transform cardTransform = Instantiate (prefab, Vector3.zero, Quaternion.identity) as Transform;
					cardTransform.localScale = new Vector3 (deck.GetWidthScreen (tangible.id), 
						deck.GetHeightScreen (tangible.id), 1.0f);
					cardTransform.parent = transform;
				
					// Set the card texture and uv rectangle
					deck.AssignGraphics (deck.GetIndex (tangible.id), cardTransform.gameObject);
				
					// Set the card properties
					physical_object = cardTransform.GetComponent (typeof(PhysicalObject)) as PhysicalObject;
				} else {
					debug_objects_.Remove (physical_object);
				}

				physical_object.Init (tangible, show_debug_ || recorded_);
				new_physical_objects.Add (physical_object);

				float x = tangible.location.X;
				float y = tangible.location.Y;
				float orientation = tangible.location.Orientation;
				physical_object.UpdateScreenPosition (MillimeterToScreen (x, y), Quaternion.Euler (0, 0, orientation), tangible.visible);
			}
		}
		foreach (PhysicalObject obj in debug_objects_) {
			GameObject.Destroy(obj.gameObject);
		}
		debug_objects_.Clear();
		if (new_physical_objects != null) {
			debug_objects_ = new_physical_objects;
		}

        e.objects = tangibles_.ToArray();
		if (isJson) {
			e.bounds = e.response.detectionArea;	
		} else {
			e.bounds = GetDetectionArea ();
		}
        
        Tangible.Event predicted_event = processEvent(e);
        HintEvent(predicted_event);
        return true;
    }
    
    void HintEvent(Tangible.Event e) {
        string s = "";
        if (e != null) {
            foreach (TangibleObject tangible in e.objects) {
                tangible.location.Orientation = -tangible.location.Orientation;
                s += tangible.ToString() + ";";
            }
        }
        _hint_event(s);
    }
	
	void DeleteCards() {
		foreach (PhysicalObject obj in debug_objects_) {
			GameObject.Destroy(obj.gameObject);
		}
		debug_objects_.Clear();

        unique_id_helper_.Reset();
	}
    
    void UpdateDebugVisibility() {
        foreach (PhysicalObject obj in debug_objects_) {
            obj.gameObject.SetActive(show_debug_ && !mute);
        }
    }

    public void EnableColorCoin(bool enable_extra_input) {
        enable_color_coin_ = enable_extra_input;
        _set_color_coin_detection_with_numbers(enable_color_coin_);
    }

    //----------------------
    // Tangible.Controller
    //----------------------
	
	void Tangible.Controller.Mute(bool _doMute) {
        if (mute != _doMute) {
            // Reset cached data
            unique_id_helper_.Reset();
        }

		base.Mute(_doMute);
        
        (this as PhysicalController).UpdateDebugVisibility();
	}

	bool Tangible.Controller.Register(int _userId, Tangible.Config _config, Tangible.EventDelegate _processEvent, int automaticDumpInterval) {
		if (!Supported()) return false;
		//Debug.Log("Unity: start register Physical (" + name + ")");
		currentRecognition = _config.Recognition;
		if (recorded_) {
			StartCoroutine(PlayRecordedInput());
		} else {
            // The setup is re-computed every 100 frames.
            if (setup_hardware_ == null) setup_hardware_ = gameObject.AddComponent<SetupPlatform>();
			SetupPlatform.SetResolution (100);

			if (currentRecognition == Config.RecognitionMode.MATH_MANIPULATIVES || currentRecognition == Config.RecognitionMode.LETTER_TILES ||
				currentRecognition == Config.RecognitionMode.IMAGECARD) {
				DeckCard d = deck as DeckCard;
				if (d == null) {
					Debug.Log ("Tangible.Controller.Register: recognition mode " + currentRecognition + " requires a DeckCard");
					return false;
				}
				System.Diagnostics.Stopwatch t = System.Diagnostics.Stopwatch.StartNew();
				Color32[] pixels = d.atlas.GetPixels32();
				if (pixels == null) {
					Debug.LogError("Tangible.Controller.Register: the atlas texture could not be read.");
					return false;
				}
				if (currentRecognition == Config.RecognitionMode.MATH_MANIPULATIVES) {
					if (!_register_with_numbers (name, "DispatchEvent", pixels, d.atlas.width, d.atlas.height, d.widthSubdivision, d.heightSubdivision, d.GetCount (), enable_color_coin_)) {
						Debug.LogError ("Tangible.Controller.Register: cards failed to register in the plugin.");
						return false;
					}
				} else {
					bool letter = currentRecognition == Config.RecognitionMode.LETTER_TILES;				
					if (!_register_with_case_cards (name, "DispatchEvent", pixels, d.atlas.width, d.atlas.height, d.widthSubdivision, d.heightSubdivision, d.GetCount (), letter)) {
						Debug.LogError ("Tangible.Controller.Register: cards failed to register in the plugin.");
						return false;
					}
				} 
				t.Stop();
				Debug.Log("Registering " + d.GetCount() + " cards in " + t.ElapsedMilliseconds + " ms.");
			} else if (currentRecognition == Config.RecognitionMode.TANGRAMS) {
				if (!_register_tangram (name, "DispatchEvent")) {
					Debug.LogError ("Tangible.Controller.Register: tangram failed to register in the plugin.");
					return false;
				}
			} else if (currentRecognition == Config.RecognitionMode.TOPCODE) {
				if (!_register_with_topcodes (name, "DispatchEvent")) {
					Debug.LogError ("Tangible.Controller.Register: topcodes failed to register in the plugin.");
					return false;
				}
			} else if (currentRecognition == Config.RecognitionMode.STRAWBIES) {
				DeckProgramming d = deck as DeckProgramming;
				if (d == null) {
					Debug.Log ("Tangible.Controller.Register: programming recognition mode requires a DeckProgramming");
					return false;
				}
				Color32[] pixels = d.atlas.GetPixels32 ();
				if (pixels == null) {
					Debug.LogError ("Tangible.Controller.Register: the atlas texture could not be read.");
					return false;
				}
				if (!_register_with_strawbies2 (name, "DispatchEvent", pixels, d.atlas.width, d.atlas.height, d.GetConfigJson (), automaticDumpInterval)) {
					Debug.LogError ("Tangible.Controller.Register: strawbies failed to register in the plugin.");
					return false;
				} 
			} else {
				Debug.LogError ("Tangible.Controller.Register: failed to register, need to add support for recognition mode " + currentRecognition);
				return false;
			}
            skip = 3;
		}
        userId = _userId;
        processEvent = _processEvent;
        DeleteCards();
		//Debug.Log("Unity: end register Physical (" + name + ")");
		return true;
    }

    void Tangible.Controller.Unregister(int _userId) {
		//Debug.Log("Unity: start unregister Physical (" + name + ")");
        if (userId == _userId) {
			_unregister();
			userId = Tangible.UserId.INVALID;
            processEvent = null;
			
			DeleteCards();
        }
		//Debug.Log("Unity: end unregister Physical (" + name + ")");
    }
	
	public IEnumerator PlayRecordedInput() {
		yield return new WaitForSeconds(1.0f);
		DispatchEvent(recorded_events_[eventIndex]);
		eventIndex++;
        if (eventIndex >= recorded_events_.Length) eventIndex = 0;
		StartCoroutine(PlayRecordedInput());
	}
}
