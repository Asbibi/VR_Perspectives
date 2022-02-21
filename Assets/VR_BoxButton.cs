using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VR_BoxButton : MonoBehaviour
{
    public UnityEvent onPush;
    [SerializeField] private MeshRenderer visual;

    private void OnTriggerEnter(Collider other)
    {
        onPush.Invoke();
        visual.material.SetColor("_EmissionColor", Color.green);
    }
}
