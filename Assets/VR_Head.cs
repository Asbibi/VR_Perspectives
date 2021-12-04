using UnityEngine;

public class VR_Head : MonoBehaviour
{
    [SerializeField] private Transform rootObject, followObject, rightHand, leftHand;
    [SerializeField] private Vector3 positionOffset, rotationOffset, headBodyOffset;


    void Update()
    {
        transform.position = followObject.TransformPoint(positionOffset);
        transform.rotation = followObject.rotation * Quaternion.Euler(rotationOffset);

        rootObject.position = transform.position + headBodyOffset;
        Vector3 bodyForward = followObject.forward;
        bodyForward.y = 0;
        bodyForward.Normalize();
        Vector3 handsForward = (leftHand.position - rootObject.position) + (rightHand.position - rootObject.position);
        handsForward.y = 0;
        handsForward.Normalize();
        rootObject.forward = (bodyForward + handsForward).normalized;    //Vector3.ProjectOnPlane(followObject.up, Vector3.up).normalized;
        /*Debug.DrawRay(transform.position, rootObject.forward, Color.green);
        Debug.DrawRay(transform.position, bodyForward, Color.cyan);
        Debug.DrawRay(transform.position, handsForward, Color.yellow);*/
    }
}
