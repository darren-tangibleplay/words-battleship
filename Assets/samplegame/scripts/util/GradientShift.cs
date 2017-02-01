using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class GradientShift : MonoBehaviour {
    void Start() {
        if (!GetComponent<Renderer>().material.HasProperty("_GradientShift")) {
            Debug.LogError("Object '" + name + "' does not have an _GradientShift propertie on its material.");
        }
    }

    public float Shift { 
        get { float c = GetComponent<Renderer>().material.GetFloat("_GradientShift"); return c; } 
        set { GetComponent<Renderer>().material.SetFloat("_GradientShift", value);  }
    }
}
