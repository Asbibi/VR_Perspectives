using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_ZoomableCamera : VR_Camera
{
    [Header("Zoom")]
    [SerializeField] private float step = 1;
    [SerializeField] private float minDist = 0;
    [SerializeField] private float maxDist = 10;
    private float dist = 3;

    public TextMesh debugText;

    public override bool EditCam(Vector2 _input)
    {
        float _y = _input.y;
        if (Mathf.Abs(_y) < 0.05f)
            return false;

        dist -= _y * step * Time.deltaTime;
        if (dist < minDist)
            dist = minDist;
        else if (dist > maxDist)
            dist = maxDist;

        transform.localPosition = Vector3.back * dist;
        debugText.text = dist.ToString();
        return false;
    }
}
