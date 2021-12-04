using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private InputActionReference jumpActionreference;
    [SerializeField] private int jumpForce = 500;


    [Header("Reset Cam")]
    //[SerializeField] private InputActionReference resetCamOffsetActionreference;
    [SerializeField] private Transform playerOffset;
    [SerializeField] private Transform projector;
    [SerializeField] private Vector3 projectorOffset;
    //[SerializeField] private Vector3 projectorRotation;


    [Header("Change Cam")]
    [SerializeField] private InputActionReference changeCamActionreference;
    [SerializeField] private InputActionReference selectCamActionreference;
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] private VR_Camera[] projectorCameras;
    [SerializeField] private CamSelectorUI changeCamHandUI;
    private bool isChangingCamera = false;


    private XRRig xrRig;
    private Rigidbody body;
    private CapsuleCollider controllerCollider;
    private bool isGrounded;// => Physics.Raycast(new Vector2(transform.position.x, transform.position.y + 2.0f), Vector3.down, 2.0f);

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        xrRig = GetComponent<XRRig>();
        controllerCollider = GetComponent<CapsuleCollider>();
        jumpActionreference.action.performed += OnJump;
        //resetCamOffsetActionreference.action.performed += OnResetCam;
        changeCamActionreference.action.performed += OnChangeCam;
        selectCamActionreference.action.performed += ComputeChangeCam;
        projectorCameras[0].Activate(true);
    }

    void Update()
    {
        checkIsGrounded();
        var center = xrRig.cameraInRigSpacePos;
        controllerCollider.center = new Vector3(center.x, controllerCollider.center.y, center.z);
        controllerCollider.height = xrRig.cameraInRigSpaceHeight;
        //OnResetCam();
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        if(!isGrounded)
            return;
        body.AddForce(Vector3.up * jumpForce);
    }

    private void OnResetCam()   //(InputAction.CallbackContext obj)
    {
        Vector3 camPos = playerOffset.localPosition;
        camPos.y = 0;
        projector.localPosition = camPos + projectorOffset;
    }

    private void OnChangeCam(InputAction.CallbackContext obj)
    {
        moveProvider.enabled = isChangingCamera;
        isChangingCamera = !isChangingCamera;
        changeCamHandUI.gameObject.SetActive(isChangingCamera);
    }

    private void ComputeChangeCam(InputAction.CallbackContext context)
    {
        if (!isChangingCamera)
            return;

        Vector2 axis = context.ReadValue<Vector2>();
        if (axis.magnitude < 0.5f)
            return;

        float angle = Vector2.SignedAngle(axis, Vector2.right);
        changeCamHandUI.SetAngle(angle);
        float stepAngle = 360 / projectorCameras.Length;
        int selectedCam = 0;
        while (angle > -180 + (selectedCam + 1) * stepAngle)
            selectedCam++;

        foreach (VR_Camera image in projectorCameras)
            image.Activate(false);
        projectorCameras[selectedCam].Activate(true, changeCamHandUI);
    }

    private void checkIsGrounded()
    {
        Vector3 origin = bodyTransform.position; //playerOffset.position;
        origin.y += 0.2f;
        isGrounded = Physics.Raycast(origin, Vector3.down, 0.3f);
        //VR_DebugText.displayText(origin.y + " -> " + isGrounded);
    }
}
