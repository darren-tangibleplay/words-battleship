using UnityEngine;
using System.Collections;

public class NumbersIdConfig : MonoBehaviour, IdConfig {

    readonly int[] id_to_value = {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
        1, 2, 5, 1, 5
    };
    
    readonly Color[] id_to_color = {
        new Color32(0xf2, 0x69, 0x26, 0xff), // 0
        new Color32(0xf1, 0x53, 0x28, 0xff),
        new Color32(0xef, 0x43, 0x26, 0xff),
        new Color32(0xee, 0x30, 0x24, 0xff),
        new Color32(0xea, 0x30, 0x44, 0xff),
        new Color32(0xea, 0x27, 0x5c, 0xff), // 5
        new Color32(0xec, 0x0e, 0x72, 0xff),
        new Color32(0xd3, 0x29, 0x7d, 0xff),
        new Color32(0xb9, 0x36, 0x89, 0xff),
        new Color32(0x9e, 0x41, 0x97, 0xff), // 9
        new Color32(0xf1, 0x53, 0x28, 0xff), // 1
        new Color32(0xee, 0x30, 0x24, 0xff), // 2
        new Color32(0xec, 0x0e, 0x72, 0xff), // 5
        new Color32(0xfc, 0xdd, 0x0c, 0xff), // Yellow
        new Color32(0xf6, 0x3a, 0x15, 0xff), // Red
    };

    readonly int[] count_per_id = {
        6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
        15, 9, 6, 17, 17,
    };

    readonly int[] unique_group_per_id = {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        1, 1, 1, 1, 1,
    };

	public int GetNumIds() {
		return count_per_id.Length;
	}

	public int GetValueForId(int id) {
		return id_to_value [id];
	}

	public Color GetColorForId(int id) {
		return id_to_color [id];
	}

	public int GetCountForId(int id) {
		return count_per_id [id];
	}

	public int GetUniqueGroupForId(int id) {
		return unique_group_per_id [id];
	}
}
