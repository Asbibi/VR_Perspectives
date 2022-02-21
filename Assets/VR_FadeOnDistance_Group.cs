using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_FadeOnDistance_Group : VR_FadeOnDistance_Base
{
    [SerializeField] private VR_FadeOnDistance_Base[] rends;

    public override void SetTransparency(float opacity)
    {
        foreach(var rend in rends)
        {
            rend.SetTransparency(opacity);
        }
    }
}
