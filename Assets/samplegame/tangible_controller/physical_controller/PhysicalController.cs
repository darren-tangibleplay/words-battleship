using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tangible;
using UnityEngine;

public class PhysicalController : PhysicalControllerBase, Controller
{
	#if UNITY_EDITOR

	private static bool _register_with_case_cards(string objectName, string functionName, Color32[] pixels, int with, int height, int subx, int suby, int count, bool letter) {
		return false;
	}

	private static bool _unregister() {
		return false;
	}

	#elif UNITY_IPHONE

	[DllImport ("__Internal")]
	private static extern bool _register_with_case_cards(string objectName, string functionName, Color32[] pixels, int with, int height, int subx, int suby, int count, bool letter);
	[DllImport ("__Internal")]
    private static extern bool _unregister();

	#else

	private static bool _register_with_case_cards(string objectName, string functionName, Color32[] pixels, int with, int height, int subx, int suby, int count, bool letter) { return false; }
	private static bool _unregister() { return false; }

	#endif

	//private static float CONFIDENCE_CUTOFF = 0.0f;

	public Transform cardPrefab;
	public float sizeScreen;
	public float persistence; // keep the card in the event list up to X seconds after it was last seen

	private TangibleObject.EventDelegate processEvent = null;
	private List<TangibleObject> objects = new List<TangibleObject>();
	//private List<PhysicalObject> debugObjects = new List<PhysicalObject>();
	private Dictionary<string, Dictionary<string, float>> seenObjects = new Dictionary<string, Dictionary<string, float>>();


	private Deck deck;
	private bool recorded = false;
	private string[] recordedEvents;
	private int eventIndex = 0;
	private int skip = 0;
	private float millimeterToScreen;

	public void Init(Deck _deck) {
		deck = _deck;
		millimeterToScreen = sizeScreen / deck.GetSizeMillimeter();

		foreach(string team in DeckCard.TEAMNAMES.Keys) {
			seenObjects.Add(team, new Dictionary<string, float>());
		}
	}

	/*
	public void SetDebug(bool _showDebug) {
		foreach(PhysicalObject obj in debugObjects) {
			obj.gameObject.SetActive(_showDebug);
		}
	}
	*/

	public void SetRecorded(bool _recorded, string events) {
		recorded = _recorded;
		recordedEvents = events.Split('|');
		eventIndex = 0;
	}

	public bool Supported() {
		return HardwareSupported() || recorded;
	}

	private Vector3 MillimeterToScreen(float x, float y) {
		return new Vector3(millimeterToScreen * x, millimeterToScreen * y);
	}

	protected override bool DispatchEvent(string json) {
		// Special messages
		if (base.DispatchEvent(json))
			return true;

		if (skip > 0) {
			skip--;
			return true;
		}

		// Regular tangible events
		objects.Clear();

		if (processEvent == null)
			return false;
		if (mute)
			return true;

		foreach(string team in seenObjects.Keys) {
			seenObjects[team].Clear();
		}

		//Debug.Log("[PhysicalController] DispatchEvent(" + json + ")");

		VisionEventInput inputEvent = null;

		try {
			inputEvent = JsonUtility.FromJson<VisionEventInput>(json);
		} catch(Exception e) {
			Debug.LogError("[PhysicalController] Error parsing VisionPlatform event JSON, perhaps version mismatch? Expecting v2: JSON=" + json);
			Debug.LogError(e.ToString());
		}

		if (inputEvent == null || inputEvent.items == null) {
			return false;
		}

		foreach(VisionEventItem obj in inputEvent.items) {
			// vision platform as of 2.0.6 the y coordinate was flipped, so it's easiest to swap it back here to what we expected previously in all game logic
			obj.pt.y = -obj.pt.y;

			TangibleObject tobj = new TangibleObject(obj);
			if (deck.ContainsId(tobj.Id)) {
				//Debug.Log("Found: " + obj.ToString() + " (valid)");
				if (!seenObjects[obj.color].ContainsKey(obj.letter)) {
					seenObjects[obj.color].Add(obj.letter, obj.confidence);
				} else {
					seenObjects[obj.color][obj.letter] = obj.confidence;

					ErrorTracker.CrashlyticsRecordCustomException(
						"[PhysicalController] DispatchEvent",
						string.Format("Duplicate letter in seenObjects: color={0} letter={0}", obj.color, obj.letter),
						new System.Diagnostics.StackTrace());
				}
			} else {
				//Debug.Log("Found: " + obj.ToString() + " (invalid)");
			}
		}

	/*
		// Try to reuse a card
		PhysicalObject debugObject = null;
		foreach (PhysicalObject search in debugObjects) {
			if (search.Id == id) {
				debugObject = search;
				break;
			}
		}

		if (debugObject == null) {
			Transform cardTransform = Instantiate(currentRecognition != Config.RecognitionMode.tangram ? cardPrefab : tangramPrefab, Vector3.zero, Quaternion.identity) as Transform;
			cardTransform.localScale = new Vector3(sizeScreen, sizeScreen, 1.0f);
			cardTransform.parent = transform;

			// Set the card texture and uv rectangle
			MeshRenderer cardRenderer = cardTransform.GetComponentInChildren(typeof(MeshRenderer)) as MeshRenderer;
			deck.AssignGraphics(deck.GetIndex(id), cardRenderer);

			// Set the card properties
			debugObject = cardTransform.GetComponent(typeof(PhysicalObject)) as PhysicalObject;
			debugObject.Init(id, shape, showDebug || recorded);
		} else {
			debugObjects.Remove(debugObject);
		}

		debugObject.UpdateScreenPosition(MillimeterToScreen(x, y), Quaternion.Euler(0, 0, orientation), visible);

		objects.Add(debugObject.UpdatePosition(x, y, orientation, visible, values));
		newDebugObjects.Add(debugObject);
	*/
	/*
		// Add all the object within the persitence time frame
		float threshold = UnityEngine.Time.timeSinceLevelLoad - persistence;
		foreach (PhysicalObject p in debugObjects) {
			if (p.LastVisible > threshold && !p.IsCoveringAny(objects)) {
				objects.Add(p.Persist());
				newDebugObjects.Add(p);
			} else {
				GameObject.Destroy(p.gameObject);
			}
		}
		debugObjects.Clear();
		debugObjects = newDebugObjects;
	*/

		//Debug.Log("xxxxxx 11");
		processEvent(inputEvent);
		return true;
	}


	void DeleteCards() {
	/*
		foreach(PhysicalObject obj in debugObjects) {
			GameObject.Destroy(obj.gameObject);
		}
		debugObjects.Clear();
	*/
	}


	//----------------------
	// Controller
	//----------------------

	void Controller.Mute(bool _doMute) {
		base.Mute(_doMute);
	}

	bool Controller.Register(TangibleObject.EventDelegate _processEvent) {
		if (!Supported())
			return false;

		if (recorded) {
			StartCoroutine(PlayRecordedInput());
		} else {
			DeckCard d = deck as DeckCard;
			if (d != null) {
				System.Diagnostics.Stopwatch t = System.Diagnostics.Stopwatch.StartNew();
				Color32[] pixels = d.atlas.GetPixels32();
				if (pixels == null) {
					Debug.LogError("Controller.Register: the atlas texture could not be read.");
					return false;
				}
				if (!_register_with_case_cards(name, "DispatchEvent", pixels, d.atlas.width, d.atlas.height, d.widthSubdivision, d.heightSubdivision, d.GetCount(), true)) {
					Debug.LogError("Controller.Register: cards failed to register in the plugin.");
					return false;
				}
				t.Stop();
				Debug.Log("Registering " + d.GetCount() + " cards in " + t.ElapsedMilliseconds + " ms.");
			}

			skip = 3;
		}

		processEvent = _processEvent;
		DeleteCards();

		return true;
	}

	void Controller.Unregister() {
		_unregister();
		processEvent = null;

		DeleteCards();
	}

	public IEnumerator PlayRecordedInput() {
		yield return new WaitForSeconds(1.0f);
		DispatchEvent(recordedEvents[eventIndex]);
		eventIndex++;
		if (eventIndex < recordedEvents.Length) {
			StartCoroutine(PlayRecordedInput());
		}
	}
}
