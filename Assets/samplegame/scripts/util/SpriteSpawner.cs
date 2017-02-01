using UnityEngine;
using System.Collections;

public class SpriteSpawner : MonoBehaviour {

    [SerializeField]
    SpriteRenderer sprite_renderer_prefab_;

    [SerializeField]
    Color color_ = Color.white;

    void Start() {
        SpriteRenderer renderer = GameObject.Instantiate(sprite_renderer_prefab_);
        renderer.transform.parent = transform;
        renderer.color = color_;
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localRotation = Quaternion.identity;
    }
}
