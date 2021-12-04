using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSelectorUI : MonoBehaviour
{
    [SerializeField] private Transform arrow;
    [SerializeField] private TextMesh name;

    public void SetAngle(float angle)
    {
        arrow.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
    
    public void SetText(string cameraName)
    {
        name.text = cameraName;
    }
}
