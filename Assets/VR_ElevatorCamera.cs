using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_ElevatorCamera : VR_Camera
{
    [Header("Elevator Camera")]
    [SerializeField] Transform playerHead;
    [SerializeField] float yOffset = -1.8f;
    [SerializeField] MeshRenderer[] wallsOnBack;
    [SerializeField] MeshRenderer[] wallsOnFront;

    Vector3 initialPos;
    private bool facingFront = true;
    private Transform pivot;

    private void Start()
    {
        pivot = transform.parent != null ? transform.parent : transform;
        initialPos = pivot.position;
    }

    private void Update()
    {
        float y = playerHead.position.y + yOffset;
        pivot.position = initialPos + Vector3.up * y;
    }

    public override bool EditCam(Vector2 _input)
    {
        facingFront = !facingFront;
        pivot.rotation *= Quaternion.Euler(0, 180, 0);

        foreach (var wall in wallsOnFront)
            wall.enabled = !facingFront;
        foreach (var wall in wallsOnBack)
            wall.enabled = facingFront;

        return true;
    }
}