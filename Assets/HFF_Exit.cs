using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HFF_Exit : MonoBehaviour
{
    public UnityEvent onEnter;

    private void OnTriggerEnter(Collider other)
    {
        onEnter.Invoke();
    }
}
