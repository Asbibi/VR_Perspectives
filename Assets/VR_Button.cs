using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ConfigurableJoint))]
public class VR_Button : MonoBehaviour
{
    public UnityEvent onPressed, onReleased;
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float deadzone = 0.25f;

    private bool isPressed;
    private Vector3 startPosition;
    private ConfigurableJoint joint;

    private void Start()
    {
        startPosition = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
    }

    private void Update()
    {
        float val = GetValue();
        if (!isPressed && val + threshold >= 1)
            Press();
        else if (isPressed && val - threshold <= 0)
            Release();
    }

    private void Press()
    {
        isPressed = true;
        onPressed.Invoke();
    }
    private void Release()
    {
        isPressed = false;
        onReleased.Invoke();
    }
    private float GetValue()
    {
        float val = Vector3.Distance(startPosition, transform.localPosition);
        val /= joint.linearLimit.limit;

        if (Mathf.Abs(val) < deadzone)
            return 0;

        return Mathf.Clamp(val, -1, 1);
    }
}
