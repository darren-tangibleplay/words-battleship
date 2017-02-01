using UnityEngine;
using System.Collections;

public class DebugSolve : MonoBehaviour {

    void OnMouseUpAsButton() {
        Game.singleton.DebugSolve();
	}
}
