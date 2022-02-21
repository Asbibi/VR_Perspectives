using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HFF_Manager : MonoBehaviour
{
    [Header("Dither Stuff")]
    [SerializeField] private Transform playerHead;
    [SerializeField] private VR_FadeOnDistance_Base[] renderers;
    [SerializeField] private float inverseFadeDistance;

    [Header("Box Stuff")]
    [SerializeField] private Rigidbody cube;
    [SerializeField] private Transform cubeSpawner;
    [SerializeField] private Transform cubeOut;
    [SerializeField] private GameObject doorObstruder;
    [SerializeField] private GameObject trapdoor;
    [SerializeField] private VR_Hand[] hands;


    private void Update()
    {
        float yPlayer = playerHead.position.y;
        foreach(var rend in renderers)
        {
            float opacity = 1 - inverseFadeDistance * (rend.transform.position.y - yPlayer);
            rend.SetTransparency(opacity);
        }
    }


    public void ResetCube()
    {
        MoveCube(cubeSpawner);
    }
    public void ConveyCube()
    {
        MoveCube(cubeOut);
    }
    private void MoveCube(Transform tf)
    {
        if (tf == null)
            return;

        foreach (var hand in hands)
        {
            hand.ExternalRelease();
        }

        cube.transform.position = tf.position;
        cube.transform.rotation = tf.rotation;
        cube.velocity = Vector3.zero;
        cube.angularVelocity = Vector3.zero;
    }

    public void OnLeverActivate()
    {
        doorObstruder.SetActive(false);
    }

    public void OnBoxButtonTrigger()
    {
        trapdoor.SetActive(false);
    }

    public void Exit()
    {
        SceneManager.LoadScene("Main");
    }
}
