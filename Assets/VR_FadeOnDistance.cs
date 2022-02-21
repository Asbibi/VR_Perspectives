using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class VR_FadeOnDistance : VR_FadeOnDistance_Base
{
    private MeshRenderer rend;
    private Material opaqueMat;
    [SerializeField] private Material transparentMat;
    [SerializeField] private float opaqueSwitchLimit = 0.95f;
    private bool opaque = true;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        opaqueMat = rend.material;
    }

    public override void SetTransparency(float opacity)
    {
        if(opacity > opaqueSwitchLimit) // Should be opaque
        {
            if (opaque)
                return;

            rend.material = opaqueMat;
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            opaque = true;
        }
        else                            // Should be transparent
        {
            if(opaque)
            {
                rend.material = transparentMat;
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                opaque = false;
            }
            rend.material.SetFloat("_Opacity", opacity);
        }
    }
}
