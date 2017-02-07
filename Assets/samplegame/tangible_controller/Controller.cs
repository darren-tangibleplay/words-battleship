using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

/*
 * Authors: Pramod Sharma, Jerome Scholler
 * Date: 12/20/2012.
 *
 * This file declares the controller API and related data objects. API allows users
 * (i.e. games) to register (or unregister) a call-back function to receive input events from
 * controller process.
 */
interface Controller {

    /*
     * Register user to receive new input events via call-back. Input parameters:
     * uid: id of the user (or game)
     * config: controller configuration parameters specific to user
     */
    bool Register(TangibleObject.EventDelegate processEvent);

    /*
     * Un-register the user (e.g. when game ends). This could be used to turn off controller
     * process all together until new user comes along and flush out any input buffers.
     */
    void Unregister();

	/*
	 * Skip all events while it is in the muted state.
	 */
	void Mute(bool _mute);
};

/*
 * Formatting input in the way the JSON will come from vision platform
 */
[System.Serializable]
public class VisionEventInput {
	public List<VisionEventItem> items;
	public VisionEventSetup setup;

	public VisionEventInput() {
		items = new List<VisionEventItem>();
		setup = new VisionEventSetup();
	}

	public VisionEventInput(List<TangibleObject> objects) {
		items = new List<VisionEventItem>();

		foreach(TangibleObject obj in objects) {
			items.Add(new VisionEventItem(obj));
		}
	}
}

[System.Serializable]
public class VisionEventSetup {
	public int Quality;
	public float Response;
	public VisionStandRect standRect;
	public string version;

	public VisionEventSetup() {
	}
}

[System.Serializable]
public class VisionStandRect {
	public int h, w, x, y;

	public VisionStandRect() {
	}
}

[System.Serializable]
public class VisionEventItem {
	public string color;
	public float confidence;
	public string letter;
	public VisionEventLocation pt;

	public VisionEventItem() {
		pt = new VisionEventLocation();
	}

	public VisionEventItem(TangibleObject obj) {
		color = obj.Color;
		confidence = obj.confidence;
		letter = obj.letter.ToString();
		pt = obj.location;
	}
}

[System.Serializable]
public class VisionEventLocation {
	public float x;
	public float y;
	public float orientation;

	public VisionEventLocation() {
		x = y = orientation = 0f;
	}
}

// Internal representation we will pass around of a detected object
public class TangibleObject {
	public delegate void EventDelegate(VisionEventInput processEvent);

	public static int CARDS_PER_ALPHABET = 26;
	public static int NUM_ALPHABETS = 2;
	public static int NUM_PLAYERS = 2;

	public TangibleObject(VisionEventItem item) {
		if (string.IsNullOrEmpty(item.letter)) {
			letter = 'a';
		} else {
			if (item.letter.Length > 1) {
				Debug.LogWarning("[TangibleObject] letter string input too long: " + letter);
			}
			letter = item.letter[0];
		}

		player = ColorToPlayer(item.color);
		confidence = item.confidence;
		location = item.pt;
	}

	public TangibleObject(int id) {
		Id = id;
		location = new VisionEventLocation();
	}

	public char letter;
	public int player;
	public float confidence;
	public VisionEventLocation location;

	public int Id {
		get { return ToId(letter, player); }
		private set {
			letter = LetterFromId(value).ToString()[0];
			player = PlayerFromId(value);
			confidence = 1f;
		}
	}

	public string Color {
		get { return PlayerToColor(player); }
		private set { player = ColorToPlayer(value); }
	}

	public int Index {
		get { return ToIndex(letter, player); }
		private set { Id = IndexToId(value); }
	}

	public static int ToIndex(char letter, int player) {
		int offset = 0;
		int index = 0;

		if (char.IsLower(letter)) {
			index = (int)letter - 'a';
			offset = CARDS_PER_ALPHABET * NUM_PLAYERS;
		} else {
			index = (int)letter - 'A';
			offset = 0;
		}

		offset += player * CARDS_PER_ALPHABET;
		return index + offset;
	}

	public static int ToId(char letter, int player) {
		return (player * 1024) + (int)letter;
	}

	public static char LetterFromId(int id) {
		return (char)(id % 1024);
	}

	public static int PlayerFromId(int id) {
		return id / 1024;
	}

	public override string ToString() {
		return letter + " player=" + player;
	}

	public static string PlayerToColor(int player) {
		return DeckCard.TEAMCOLORS.ContainsKey(player) ? DeckCard.TEAMCOLORS[player] : DeckCard.TEAMCOLORS[ColorToPlayer("")];
	}

	public static int ColorToPlayer(string color) {
		if (DeckCard.TEAMNAMES.ContainsKey(color)) {
			return DeckCard.TEAMNAMES[color];
		} else {
			Debug.LogWarning("[Controller] Unknown player color: " + color);
			return 0;
		}
	}

	public static int IdToIndex(int id) {
		return ToIndex(LetterFromId(id), PlayerFromId(id));
	}

	public static int IndexToId(int index) {
		bool uppercase = index < CARDS_PER_ALPHABET * NUM_PLAYERS;
		if (!uppercase) {
			index -= CARDS_PER_ALPHABET * NUM_PLAYERS;
		}

		int player = index / CARDS_PER_ALPHABET;
		int letter = index % CARDS_PER_ALPHABET;
		return ToId((char)(letter + (uppercase ? 'A' : 'a')), player);
	}
};
