using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_CameraTargetter : VR_Camera
{
    [SerializeField] private Transform target;
    [SerializeField] private float lerpForce = 0.5f;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, lerpForce);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, lerpForce);
    }
}
