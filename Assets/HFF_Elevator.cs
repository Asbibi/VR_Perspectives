using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFF_Elevator : MonoBehaviour
{
    [SerializeField] private VR_Slider floorSlider;
    [SerializeField] private float[] floorHeights;
    [SerializeField] private float speed = 0.5f;
    //[SerializeField] private float snapZone = 0.1f;
    //[SerializeField] private Transform player;


    private int moveElevatorDir = 0;
    private int targetFloor = 0;


    public void UpdateElevator()
    {
        int _target = floorSlider.GetLastVal();
        if (targetFloor == _target || _target >= floorHeights.Length)
            return;

        targetFloor = _target;
        moveElevatorDir = floorHeights[_target] > transform.position.y ? 1 : -1;
    }
    public void CallElevatorAt(int floor)
    {
        if (floor == targetFloor)
            return;

        floorSlider.SetValue(floor);
    }

    private void Update()
    {
        if (moveElevatorDir == 0)
            return;

        Vector3 pos = transform.position;
        pos.y += moveElevatorDir * speed * Time.deltaTime;
        transform.position = pos;


        if ((moveElevatorDir == 1 && pos.y >= floorHeights[targetFloor]) ||
            (moveElevatorDir == -1 && pos.y <= floorHeights[targetFloor]))
        {
            moveElevatorDir = 0;
            pos.y = floorHeights[targetFloor];
            transform.position = pos;
        }
    }
}
