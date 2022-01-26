using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFF_Manager : MonoBehaviour
{
    [SerializeField] private Color[] debugColors;
    private Material mat;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    public void Debug_DisplayColor(int i)
    {
        if (i < 0 || i >= debugColors.Length)
            return;

        mat.SetColor("_EmissionColor", debugColors[i]);
    }
}
