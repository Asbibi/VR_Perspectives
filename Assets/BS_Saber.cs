using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_Saber : MonoBehaviour
{
    private MeshRenderer meshRend;
    private Material initialMaterial;
    public Material greenMaterial;
    public bool isBlue;

    private void Start()
    {
        meshRend = GetComponent<MeshRenderer>();
        initialMaterial = meshRend.material;
    }

    public void SetMaterial(bool _green)
    {
        if (_green)
            meshRend.material = greenMaterial;
        else
            meshRend.material = initialMaterial;
    }
}
