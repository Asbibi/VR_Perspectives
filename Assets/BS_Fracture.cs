using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BS_Fracture : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private bool isBlue = true;
    [SerializeField] private bool isGreen = true;
    [SerializeField] private bool isFail = false;

    private BS_Manager manager;
    private AudioSource audio;
    KeyValuePair<Rigidbody, Vector3>[] fragmentInitialPosition;

    private void Awake()
    {
        fragmentInitialPosition = new KeyValuePair<Rigidbody, Vector3>[transform.childCount];
        for(int i =0; i < transform.childCount; i++)
        {
            fragmentInitialPosition[i] = new KeyValuePair<Rigidbody, Vector3>(transform.GetChild(i).GetComponent<Rigidbody>(), transform.GetChild(i).localPosition);
        }
        audio = GetComponent<AudioSource>();
        StartCoroutine(Die());
    }

    public void SetManager(BS_Manager _manager)
    {
        manager = _manager;
    }
    public void Reset(Transform _origin)
    {
        // Reset Framgents
        Quaternion resetRotation = Quaternion.Euler(-90, 0, 0);
        foreach (var elem in fragmentInitialPosition)
        {
            elem.Key.transform.localPosition = elem.Value;
            elem.Key.transform.localRotation = resetRotation;
            elem.Key.velocity = Vector3.zero;
            elem.Key.angularVelocity = Vector3.zero;
        }

        // Set Position and rotation of parent
        transform.position = _origin.position;
        transform.rotation = _origin.rotation;

        // ReActive
        gameObject.SetActive(true);

        // Plan deactive
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        audio.Play();
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
        manager.DespawnFracture(this, isBlue, isFail, isGreen);
    }
}
