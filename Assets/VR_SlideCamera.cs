using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_SlideCamera : VR_Camera
{
    [Header("Side Camera")]
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset;
    [SerializeField] bool LockZ = true;

    private void Update()
    {
        Vector3 position = player.position;
        position += offset;
        if(LockZ)
            position.z = transform.position.z;
        transform.position = position;
    }
}
