using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VR_Lever : MonoBehaviour
{
    public UnityEvent onRight, onLeft, onValueChange;
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private int increment = 5;
    [SerializeField] private MeshRenderer lightRenderer;
    [SerializeField] private TextMesh textValue;

    [SerializeField] private bool onlyOnce = false;

    private bool isRight;
    private float limit;
    private HingeJoint joint;
    private float lastVal = 0;

    private Material lightMat;

    private void Start()
    {
        joint = GetComponent<HingeJoint>();
        limit = joint.limits.max * 2;
        lightMat = lightRenderer.material;
    }

    private void Update()
    {
        if (onlyOnce && isRight)
            return;

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
        float val = transform.localEulerAngles.z;
        if (val > 180)
            val -= 360;
        val /= limit;
        val += 0.5f;

        return Mathf.Clamp(1-val, 0, 1);
    }
}
