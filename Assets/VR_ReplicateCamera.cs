using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_ReplicateCamera : VR_Camera
{
    [Header("Replicate Camera")]
    [SerializeField] Transform cameraToReplicate;
    private Quaternion originRotation;  // transform.rotation * Inverse(cameraToReplicate.rotation) when the camera begin active


    // Update is called once per frame
    void Update()
    {
        if (IsActive())
        {
            transform.rotation = originRotation * cameraToReplicate.rotation;
        }
    }

    public override void Activate(bool active, CamSelectorUI cameraSelector = null)
    {
        base.Activate(active, cameraSelector);
        if (active)
            originRotation = transform.rotation * Quaternion.Inverse(cameraToReplicate.rotation);
    }
}
