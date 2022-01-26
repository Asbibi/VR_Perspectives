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


    [Header("Block")]
    [SerializeField] private InputActionReference blockActionReference;
    [SerializeField] private GameObject blockLocomotionUI;
    private bool isLocomotionBlocked = false;
    private Vector3 rigCameraForward = Vector3.zero; 


    [Header("Camera")]
    [SerializeField] private InputActionReference changeCamActionreference;
    [SerializeField] private InputActionReference setCamActionreference;
    [SerializeField] private InputActionReference selectCamActionreference;
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] private VR_Camera[] projectorCameras;
    [SerializeField] private CamSelectorUI changeCamHandUI;
    [SerializeField] private GameObject editCamHandUI;
    private bool isChangingCamera = false;
    private bool isEditingCamera = false;
    private int selectedCam = 0;


    private XRRig xrRig;
    private Rigidbody body;
    private CapsuleCollider controllerCollider;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        xrRig = GetComponent<XRRig>();
        controllerCollider = GetComponent<CapsuleCollider>();
        jumpActionreference.action.performed += OnJump;
        blockActionReference.action.performed += OnBlockLocomotion;
        changeCamActionreference.action.performed += OnChangeCam;
        setCamActionreference.action.performed += OnEditCam;
        selectCamActionreference.action.performed += ComputeChangeCam;
        selectCamActionreference.action.performed += ComputeEditCam;
        projectorCameras[selectedCam].Activate(true);
    }

    void Update()
    {
        if (isLocomotionBlocked)
            xrRig.MatchRigUpCameraForward(Vector3.up, rigCameraForward);        
        else
            checkIsGrounded();

        var center = xrRig.cameraInRigSpacePos;
        controllerCollider.center = new Vector3(center.x, controllerCollider.center.y, center.z);
        controllerCollider.height = xrRig.cameraInRigSpaceHeight;
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        if(!isGrounded)
            return;
        body.AddForce(Vector3.up * jumpForce);
    }
    private void OnBlockLocomotion(InputAction.CallbackContext obj)
    {
        if (isChangingCamera || isEditingCamera)
            return;

        isLocomotionBlocked = !isLocomotionBlocked;
        blockLocomotionUI.SetActive(isLocomotionBlocked);
        moveProvider.enabled = isLocomotionBlocked;
        if (!isLocomotionBlocked)
            return;

        rigCameraForward = xrRig.cameraGameObject.transform.forward;
        isGrounded = false;
    }

    private void OnChangeCam(InputAction.CallbackContext obj)
    {
        if (isEditingCamera)
            OnEditCam(obj);
        else if (isLocomotionBlocked)
            OnBlockLocomotion(obj);

        moveProvider.enabled = isChangingCamera;
        isChangingCamera = !isChangingCamera;
        changeCamHandUI.gameObject.SetActive(isChangingCamera);
    }
    private void OnEditCam(InputAction.CallbackContext obj)
    {
        if (isChangingCamera)
            return;
        if (isLocomotionBlocked)
            OnBlockLocomotion(obj);

        moveProvider.enabled = isEditingCamera;
        isEditingCamera = !isEditingCamera;
        editCamHandUI.SetActive(isEditingCamera);
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
        selectedCam = 0;
        while (angle > -180 + (selectedCam + 1) * stepAngle)
            selectedCam++;

        foreach (VR_Camera image in projectorCameras)
            image.Activate(false);
        projectorCameras[selectedCam].Activate(true, changeCamHandUI);
    }
    private void ComputeEditCam(InputAction.CallbackContext context)
    {
        if (!isEditingCamera)
            return;

        projectorCameras[selectedCam].EditCam(context.ReadValue<Vector2>());
    }

    private void checkIsGrounded()
    {
        Vector3 origin = bodyTransform.position; //playerOffset.position;
        origin.y += 0.2f;
        isGrounded = Physics.Raycast(origin, Vector3.down, 0.3f);
        //VR_DebugText.displayText(origin.y + " -> " + isGrounded);
    }
}
