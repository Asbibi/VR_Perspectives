using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_Saber : MonoBehaviour
{
    public bool isBlue;
    private Transform tip;
    public Vector3 GetTipPosition() { return tip.position; }

    private void Start()
    {
        tip = transform.GetChild(0);
    }
}
