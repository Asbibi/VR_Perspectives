using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BS_BoxOrientation
{
    Up,
    Down,
    Left,
    Right
}
public enum BS_BoxCut
{
    Miss,
    WrongSaber,
    CorrectCut
}

public class BS_Box : MonoBehaviour
{
    Transform mesh;
    BS_Manager manager;
    float speed;
    BS_BoxOrientation orientation;
    bool isBlue;
    private Material initialMaterial;
    public Material ditherMaterial;
    public Material greenMaterial;



    public void Init(BS_Manager _manager, bool _isBlue, float _speed, float _dist, BS_BoxOrientation _orientation)
    {
        manager = _manager;
        isBlue = _isBlue;
        mesh = transform.GetChild(0);
        initialMaterial = mesh.GetComponent<MeshRenderer>().material;
        InitCommon(_speed, _dist, _orientation);
    }
    public void Reinit(Transform _initTransform, float _speed, float _dist, BS_BoxOrientation _orientation)
    {
        transform.position = _initTransform.position;
        transform.rotation = _initTransform.rotation;
        InitCommon(_speed, _dist, _orientation);
    }

    private void InitCommon(float _speed, float _dist, BS_BoxOrientation _orientation)
    {
        mesh.GetComponent<MeshRenderer>().material = manager.GetSaberLimit() == BS_SaberLimit.Green ? greenMaterial : initialMaterial;
        InitOrientaion(_orientation);
        gameObject.SetActive(true);
        InitDeath(_speed, _dist);
    }
    private void InitOrientaion(BS_BoxOrientation _orientation)
    {
        orientation = _orientation;
        switch(orientation)
        {
        //    default:
        //    case BS_BoxOrientation.Up:
        //        break;
            case BS_BoxOrientation.Down:
                mesh.localRotation *= Quaternion.Euler(0, 180, 0);
                break;
            case BS_BoxOrientation.Left:
                mesh.localRotation *= Quaternion.Euler(0, 90, 0);
                break;
            case BS_BoxOrientation.Right:
                mesh.localRotation *= Quaternion.Euler(0, -90, 0);
                break;
        }
    }
    private void InitDeath(float _speed, float _dist)
    {
        speed = _speed;
        StartCoroutine(DeathCoroutine(_dist/_speed));
    }


    // ----------------------------------

    IEnumerator DeathCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        manager.DespawnBox(this, isBlue, BS_BoxCut.Miss);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeSelf)
            return;

        BS_Saber saber = other.GetComponent<BS_Saber>();
        if (saber == null)
            return;

        bool correctSaber = isBlue == saber.isBlue;
        BS_SaberLimit limit = manager.GetSaberLimit();
        if (limit == BS_SaberLimit.Green)
            correctSaber = true;

        if (correctSaber || limit != BS_SaberLimit.Limit)
            manager.DespawnBox(this, isBlue, correctSaber ? BS_BoxCut.CorrectCut : BS_BoxCut.WrongSaber);
        else
            mesh.GetComponent<MeshRenderer>().material = ditherMaterial;
    }

    // ----------------------------------


    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
