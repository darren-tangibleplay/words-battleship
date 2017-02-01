using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class OnScreenController : MonoBehaviour, Tangible.Controller {
	const float VERTICAL_BUFFER = 10f;
	const float HORIZONTAL_BUFFER = 10f;

    // Config
    [SerializeField]
    private float areaWidth = 768;
    public float AreaWidth { get { return areaWidth; } }

    [SerializeField]
    private float areaHeight = 1024;
    public float AreaHeight { get { return areaHeight; } }

    [SerializeField]
    private Transform margins;

    // Controller
    private int userId = Tangible.UserId.INVALID;
    private Tangible.Config config = null;
    private Tangible.EventDelegate processEvent = null;
    private float fovFactor = 1.0f;
    private bool dispatchRequested = false;
    
    private List<Tangible.TangibleObject> tangibles_ = new List<Tangible.TangibleObject>();
    private Tangible.UniqueIdHelper unique_id_helper_;

    private Deck deck;
	private float originalCameraFOV;
	private float originalOrthographicSize;
    private bool random_trigger_ = false;
	private bool mute = false;
	
    //----------
    // Methods
    //----------
				
	public void Init(Deck _deck) {
		deck = _deck;
		unique_id_helper_ = new Tangible.UniqueIdHelper (deck);
	}

	private void Activate() {
		CreateOnScreenObjects();

		originalCameraFOV = Camera.main.fieldOfView;
		originalOrthographicSize = Camera.main.orthographicSize;
		float multiplier = fovFactor;
		if (Camera.main.orthographic) {
			multiplier *= 1.14f;
			Camera.main.orthographicSize *= multiplier;
		} else {
			Camera.main.fieldOfView *= multiplier;
		}

		UIManager uiManager = FindObjectOfType<UIManager>();
		if (uiManager != null) {
			CanvasScaler scaler = uiManager.GetComponent<CanvasScaler> ();
			if (scaler != null && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize) {
				scaler.referenceResolution *= multiplier;
			}
		}
			
        random_trigger_ = true;
        StartCoroutine(OnRandomTrigger());
	}
	
	private void Deactivate() {
        random_trigger_ = false;

		DeleteCards();

		Camera.main.fieldOfView = originalCameraFOV;
		Camera.main.orthographicSize = originalOrthographicSize;
	}
	
    public void OnObjectEnter(Tangible.TangibleObject tangible) {
        tangibles_.Add(tangible);
        dispatchRequested = true;
    }

    public void OnObjectUpdate(Tangible.TangibleObject tangible) {
        dispatchRequested = true;
    }

    public void OnObjectExit(Tangible.TangibleObject tangible) {
        tangibles_.Remove(tangible);
        dispatchRequested = true;
    }
	
	public void AnimateToOriginal() {
		OnScreenObject[] objs = gameObject.GetComponentsInChildren<OnScreenObject>();
		foreach (OnScreenObject obj in objs) {
			obj.AnimateToOriginal();
		}
	}
    	
	public void OnMotionEnd(OnScreenObject last) {
		List<Vector2[]> polygons = new List<Vector2[]>();
		OnScreenObject[] objs = gameObject.GetComponentsInChildren<OnScreenObject>();
		foreach (OnScreenObject obj in objs) {
			if (obj == last) continue;
            polygons.Add(obj.snapPolygon);
		}
        Vector2 offset = Tangible.SnapHelper.SnapOffset(last.snapPolygon, polygons);
		last.MoveBy(offset);
	}

    IEnumerator OnRandomTrigger() {
        while (random_trigger_) {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            dispatchRequested = true;
        }
    }

    private void DispatchEvent() {
        if (processEvent == null) {
			Debug.LogError("NO processEvent set");
            return;
		}
        dispatchRequested = false;
		if (mute) return;

        List<Tangible.TangibleObject> tangibles = unique_id_helper_.UpdateUniqueIds(tangibles_);

        Tangible.Event e = new Tangible.Event();
		e.objects = tangibles.ToArray();
        e.bounds = null;
        
        Tangible.Event predicted_event = processEvent(e);
        HintEvent(predicted_event);
    }
    
    void HintEvent(Tangible.Event e) {
        string s = "";
        if (e != null) {
            foreach (Tangible.TangibleObject tangible in e.objects) {
				Tangible.TangibleObject t = new Tangible.TangibleObject(tangible);
                t.location.Orientation = -t.location.Orientation;
                s += t.ToString() + ";";
            }
        }
        //Debug.Log(s);
    }
	
    void Dump() {
        Debug.Log(config.ToString());
    }

    void CreateOnScreenObjects() {
		if (deck == null) Debug.LogError("A Deck must be specified on the Controller");
        Vector3 p = margins.localPosition;
		float marginZ = p.z;
		float xPos = areaWidth * -0.5f;
		float yPos = areaHeight * 0.5f;
		List<int> cardIds = deck.GetIdsForOnScreenCards ();
		int numCards = cardIds.Count;
		int side = 0;
		float topBorder = 0f;
		float bottomBorder = 0f;
		float leftBorder = 0f;
		float rightBorder = 0f;
		Dictionary<string,int> stackKeyToPlace = new Dictionary<string, int>();
		for (int i = 0; i < numCards; i++) {
			int id = cardIds [i];
			//int index = deck.GetIndex (id);
			// if this is only the pressed state for another id, don't add a separate card for it
			string stackKey = deck.GetStackKey(id);
			if(!stackKeyToPlace.ContainsKey(stackKey)){
				stackKeyToPlace.Add(stackKey, stackKeyToPlace.Count);
			}
			int index = stackKeyToPlace[stackKey];
			if (deck.IsPressedState (id)) {
				continue;
			}
				
			side = side % 4;
			if (side == 0) {
				p.x = xPos + deck.GetWidthScreen (id) * 0.5f;
				p.y = yPos + deck.GetHeightScreen (id) * 0.5f + VERTICAL_BUFFER;
				topBorder = Mathf.Max (deck.GetHeightScreen (id) + VERTICAL_BUFFER, topBorder);
				xPos += (deck.GetWidthScreen (id) + HORIZONTAL_BUFFER);
				if (xPos >= areaWidth * 0.5f) {
					xPos = areaWidth * 0.5f;
					side++;
				}
			} else if (side == 1) {
				p.y = yPos - deck.GetHeightScreen (id) * 0.5f;
				p.x = xPos + deck.GetWidthScreen (id) * 0.5f + HORIZONTAL_BUFFER;
				rightBorder = Mathf.Max (deck.GetWidthScreen (id) + HORIZONTAL_BUFFER, rightBorder);
				yPos -= (deck.GetHeightScreen (id) + VERTICAL_BUFFER);
				if (yPos <= areaHeight * -0.5f) {
					yPos = areaHeight * -0.5f;
					side++;
				}
			} else if (side == 2) {
				p.x = xPos - deck.GetWidthScreen (id) * 0.5f;
				p.y = yPos - deck.GetHeightScreen (id) * 0.5f - VERTICAL_BUFFER;
				bottomBorder = Mathf.Max (deck.GetHeightScreen (id) + VERTICAL_BUFFER, bottomBorder);
				xPos -= (deck.GetWidthScreen (id) + HORIZONTAL_BUFFER);
				if (xPos <= areaWidth * -0.5f) {
					xPos = areaWidth * -0.5f;
					side++;
				}
			} else if (side == 3) {
				p.y = yPos + deck.GetHeightScreen (id) * 0.5f;
				p.x = xPos - deck.GetWidthScreen (id) * 0.5f - HORIZONTAL_BUFFER;
				leftBorder = Mathf.Max (deck.GetWidthScreen (id) + HORIZONTAL_BUFFER, leftBorder);
				yPos += (deck.GetHeightScreen (id) + VERTICAL_BUFFER);
				if (yPos >= areaHeight * 0.5f) {
					yPos = areaHeight * 0.5f;
					side++;
				}
			}
				  
			p.z = marginZ + deck.GetTileZMod (id);
			int idCount = deck.GetIdConfig ().GetCountForId (id);
			for (int j = 0; j < idCount; j++) {
				OnScreenObject card = Instantiate (deck.GetPrefab (), p, Quaternion.identity) as OnScreenObject;
				int unique_id = id;
				card.Init (this, id, unique_id, deck.GetShape (index), deck.GetCardDisplayScaleX (id), 
					deck.GetCardDisplayScaleY (id), deck, deck.GetDefaultRotationForId (id));
				if (deck.HasPressedState (id)) {
					int pressedId = deck.GetIdForPressedState (id);
					card.AddPressedId (pressedId, pressedId);
				}
				if (deck.IsButton (id)) {
					card.AddSimpleButton ();
				}
				card.onMotionEnd = OnMotionEnd;
			}
		}
		fovFactor = 1.0f + Mathf.Max((leftBorder + rightBorder)/ (areaWidth * 2.0f), (topBorder + bottomBorder)/(areaHeight * 2.0f));
    }
	
	void DeleteCards() {
		OnScreenObject[] objs = gameObject.GetComponentsInChildren<OnScreenObject>();
		foreach (OnScreenObject obj in objs) {
			GameObject.Destroy(obj.gameObject);
		}

        unique_id_helper_.Reset();
	}

    //----------------
    // MonoBehaviour
    //----------------
	
	void Tangible.Controller.Mute(bool _mute) {
        if (mute != _mute) {
            // Reset cached data
            unique_id_helper_.Reset();
        }

		mute = _mute;
	}

    void Update() {
        if (dispatchRequested) {
            DispatchEvent();
        }
    }

    //----------------------
    // Tangible.Controller
    //----------------------

	bool Tangible.Controller.Register(int _userId, Tangible.Config _config, Tangible.EventDelegate _processEvent, int automaticDumpInterval) {
        userId = _userId;
        config = _config;
        processEvent = _processEvent;
		
		Activate();
		
		return true;
    }

    void Tangible.Controller.Unregister(int _userId) {
        if (userId == _userId) {
            userId = Tangible.UserId.INVALID;
            config = null;
            processEvent = null;
			
			Deactivate();
        }
    }
}
