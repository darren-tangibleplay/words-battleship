using UnityEngine;
using System.Collections;

public class TangramIdConfig : MonoBehaviour, IdConfig {
	readonly Color[] id_to_color = {
		new Color32(0xf2, 0x0a, 0x0a, 0xff), 
		new Color32(0x19, 0x2f, 0xd3, 0xff),
		new Color32(0x24, 0xd0, 0x26, 0xff),
		new Color32(0xd1, 0x13, 0x9e, 0xff),
		new Color32(0x0c, 0xb8, 0xc4, 0xff),
		new Color32(0xf3, 0xde, 0x00, 0xff), 
		new Color32(0xf4, 0x94, 0x02, 0xff),
		new Color32(0xf4, 0x94, 0x02, 0xff)
	};

	public int GetNumIds() {
		return id_to_color.Length;
	}
		
	public int GetValueForId(int id) {
		return id;
	}

	public Color GetColorForId(int id) {
		return id_to_color[id];
	}

	public int GetCountForId(int id) {
		return 1;
	}

	public int GetUniqueGroupForId(int id) {
		return 0;
	}
}
