using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public class VR_Hand : MonoBehaviour
{
    // Physic
    [SerializeField] private ActionBasedController controller;
    [SerializeField] private float followSpeed = 30f;
    //[SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

    [Header("Grabbing")]
    [SerializeField] private bool GrabEnable = true;
    [SerializeField] private Transform palm;
    [SerializeField] private float reachDistance = 0.1f;
    [SerializeField] private float joinDistance = 0.05f;
    [SerializeField] private LayerMask grabbableLayer;

    private Transform followTarget;
    private Rigidbody body;
    private bool isGrabbing = false;
    private GameObject _heldObject;
    private Transform grabPoint;
    private FixedJoint joint1, joint2;



    // Start is called before the first frame update
    void Start()
    {
        // Physicq Movement
        followTarget = controller.transform;
        body = GetComponent<Rigidbody>();
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.mass = 20f;
        //body.maxAngularVelocity = 20f;

        // Input Setup
        controller.selectAction.action.started += Grab;
        controller.selectAction.action.canceled += Release;

        // Teleport Hand
        body.position = followTarget.position;
        body.rotation = followTarget.rotation;

        //testImage.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsMove();
    }

    void PhysicsMove()
    {
        // Position
        Vector3 positionWithOffset = followTarget.TransformPoint(positionOffset);
        float distance = Vector3.Distance(positionWithOffset, transform.position);
        body.velocity = (positionWithOffset - transform.position).normalized * followSpeed * distance;

        // Rotation
        Quaternion rotationWithOffset = followTarget.rotation * Quaternion.Euler(rotationOffset);
        Quaternion q = rotationWithOffset * Quaternion.Inverse(body.rotation);
        q.ToAngleAxis(out float angle, out Vector3 axis);
        body.transform.rotation = rotationWithOffset;
    }

    private void Grab(InputAction.CallbackContext context)
    {
        if (!GrabEnable)
            return;

        if (isGrabbing || _heldObject)
            return;

        Collider[] grabColliders = Physics.OverlapSphere(palm.position, reachDistance, grabbableLayer);
        if (grabColliders.Length == 0)
            return;

        GameObject objectToGrab = grabColliders[0].transform.gameObject;
        if (objectToGrab == null)
            return;

        Rigidbody objectBody = objectToGrab.GetComponent<Rigidbody>();

        if (objectBody != null)
        {
            _heldObject = objectToGrab;
        }
        else
        {
            objectBody = objectToGrab.GetComponentInParent<Rigidbody>();
            if (objectBody != null)
            {
                _heldObject = objectBody.gameObject;
            }
            else
            {
                return;
            }
        }

        StartCoroutine(GrabObject(grabColliders[0], objectBody));
    }

    private IEnumerator GrabObject(Collider collider, Rigidbody targetBody)
    {
        isGrabbing = true;

        // Create a grab point
        grabPoint = new GameObject().transform;
        grabPoint.position = collider.ClosestPoint(palm.position);
        grabPoint.parent = _heldObject.transform;

        // Move hand to grab point
        followTarget = grabPoint;

        // Wait for hand to reach grab point
        while (isGrabbing && grabPoint != null && Vector3.Distance(grabPoint.position, palm.position) > joinDistance)
        {
            yield return new WaitForEndOfFrame();
        }

        // Freeze hand and object motion
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;    // ?
        targetBody.velocity = Vector3.zero;
        targetBody.angularVelocity = Vector3.zero;

        targetBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        targetBody.interpolation = RigidbodyInterpolation.Interpolate;

        // Attach Joints
        joint1 = gameObject.AddComponent<FixedJoint>();
        joint1.connectedBody = targetBody;
        joint1.breakForce = float.PositiveInfinity;
        joint1.breakTorque = float.PositiveInfinity;

        joint1.connectedMassScale = 1;
        joint1.massScale = 1;
        joint1.enableCollision = false;
        joint1.enablePreprocessing = false;


        joint2 = _heldObject.AddComponent<FixedJoint>();
        joint2.connectedBody = body;
        joint2.breakForce = float.PositiveInfinity;
        joint2.breakTorque = float.PositiveInfinity;
             
        joint2.connectedMassScale = 1;
        joint2.massScale = 1;
        joint2.enablePreprocessing = false;
        joint2.enableCollision = false;

        // Reset follow target
        followTarget = controller.gameObject.transform;
    }


    private void Release(InputAction.CallbackContext context)
    {
        if (!GrabEnable)
            return;

        if (joint1 != null)
            Destroy(joint1);
        if (joint2 != null)
            Destroy(joint2);
        if (grabPoint != null)
            Destroy(grabPoint.gameObject);

        if (_heldObject != null)
        {
            Rigidbody targetBody = _heldObject.GetComponent<Rigidbody>();
            targetBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            targetBody.interpolation = RigidbodyInterpolation.None;
            _heldObject = null;
        }

        isGrabbing = false;
        followTarget = controller.gameObject.transform;
    }
}
