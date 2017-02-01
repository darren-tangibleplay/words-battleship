using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {

    public Quaternion rest_rotation = Quaternion.identity;
    public float rotation_speed = 0.05f;
    public float settle_speed = 0.1f;
    private float settle_distance_treshold = 0.3f;
    private float settle_distance_hysteresis = 0.4f;

    private Vector3 smooth_position_;
    private bool lerping_ = false;

    void Start() {
        transform.rotation =  rest_rotation;
    }
    
    void Update() {
        smooth_position_ = Vector3.Lerp(smooth_position_, transform.position, settle_speed);

        float distance = Vector3.Distance(smooth_position_, transform.position);
        if (distance < settle_distance_treshold || lerping_ && distance < settle_distance_hysteresis) {
            lerping_ = true;
            transform.rotation = Quaternion.Lerp(transform.rotation, rest_rotation, rotation_speed);
        } else {
            lerping_ = false;
        }
    }
}
