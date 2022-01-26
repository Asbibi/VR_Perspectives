using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VR_Slider : MonoBehaviour
{
    public UnityEvent onRight, onLeft, onValueChange;
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private int increment = 5;
    [SerializeField] private MeshRenderer lightRenderer;
    [SerializeField] private TextMesh textValue;

    private bool isRight;
    private float limit;
    private Vector3 zeroPosition;
    private ConfigurableJoint joint;
    private float lastVal = 0;

    private Material lightMat;

    private void Start()
    {
        zeroPosition = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
        limit = joint.linearLimit.limit*2;
        lightMat = lightRenderer.material;
    }

    private void Update()
    {
        float val = GetValue();
        if (!isRight && val + threshold >= 1)
            Right();
        else if (isRight && val - threshold <= 0)
            Left();

        int val_Int = (int)(val * increment);
        if (val_Int != lastVal)
            ChangeVal(val_Int);
    }

    private void Right()
    {
        isRight = true;
        lightMat.SetColor("_EmissionColor", Color.green);
        onRight.Invoke();
    }
    private void Left()
    {
        isRight = false;
        lightMat.SetColor("_EmissionColor", Color.red);
        onLeft.Invoke();
    }
    private void ChangeVal(int newVal)
    {
        lastVal = newVal;
        textValue.text = lastVal.ToString();
        onValueChange.Invoke();
    }
    private float GetValue()
    {
        float val = transform.localPosition.x - zeroPosition.x;
        val /= limit;
        val += 0.5f;

        return Mathf.Clamp(val, 0, 1);
    }
}
