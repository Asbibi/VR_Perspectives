using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VR_Slider : MonoBehaviour
{
    public UnityEvent onRight, onLeft, onValueChange;
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private int increment = 5;
    [SerializeField] private MeshRenderer lightRenderer = null;
    [SerializeField] private TextMesh textValue;

    private bool isRight;
    private float limit;
    private Vector3 zeroPosition;
    private ConfigurableJoint joint;
    private int lastVal = 0;

    private Material lightMat;

    private void Start()
    {
        zeroPosition = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
        limit = joint.linearLimit.limit*2;
        lightMat = lightRenderer != null ? lightRenderer.material : null;
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
        if (lightMat != null)
            lightMat.SetColor("_EmissionColor", Color.green);
        onRight.Invoke();
    }
    private void Left()
    {
        isRight = false;
        if (lightMat != null)
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


    public int GetLastVal()
    {
        return lastVal;
    }
    public void SetValue(int _val)
    {
        float val_float = (float)_val / (float)increment;
        val_float -= 0.5f;
        val_float *= limit;

        Vector3 localPos = transform.localPosition;
        localPos.x = val_float + zeroPosition.x;
        transform.localPosition = localPos;
    }
}
