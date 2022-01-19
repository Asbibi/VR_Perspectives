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
    WrongOrientation,
    CorrectCut
}

public class BS_Box : MonoBehaviour
{
    Transform mesh;
    BS_Manager manager;
    float speed;
    BS_BoxOrientation orientation;
    bool isBlue;

    BS_Saber cuttingSaber = null;
    Vector3 cuttingStart = Vector3.zero;


    public void Init(BS_Manager _manager, bool _isBlue, float _speed, float _dist, BS_BoxOrientation _orientation)
    {
        manager = _manager;
        mesh = transform.GetChild(0);
        isBlue = _isBlue;
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
        cuttingSaber = null;
        cuttingStart = Vector3.zero;
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

        if (cuttingSaber != null)
            return;
        cuttingSaber = saber;
        cuttingStart = saber.GetTipPosition();
        StartCoroutine(EndCut());
    }
    /*private void OnCollisionEnter(Collision collision)
    {
        if (!gameObject.activeSelf)
            return;

        BS_Saber saber = collision.collider.GetComponent<BS_Saber>();
        if (saber == null || collision.contactCount == 0)
            return;

        if (cuttingSaber != null)
            return;
        cuttingSaber = saber;
        cuttingStart = collision.GetContact(0).point;
        Debug.Log(saber.gameObject.name + " started to cut");
    }*/
    IEnumerator EndCut()
    {
        yield return new WaitForSeconds(0.1f);

        if (!gameObject.activeSelf || cuttingSaber == null)
            yield break;


        Vector3 cutVect = cuttingStart - cuttingSaber.GetTipPosition();
        //Debug.DrawLine(cuttingStart, cuttingSaber.GetTipPosition(), Color.red, 1.2f);
        //Debug.DrawLine(cuttingStart, cuttingStart + transform.up, Color.cyan, 1.2f);

        Vector3 cutVectProj = Vector3.ProjectOnPlane(-cutVect, transform.forward);
        if (cutVectProj.magnitude < 0.001f)
        {
            manager.DespawnBox(this, isBlue, BS_BoxCut.WrongOrientation);
            yield break;
        }

        float angle = 180 - Vector3.SignedAngle(transform.up, cutVectProj.normalized, transform.forward);
        TryCut(angle, cuttingSaber.isBlue);
        //Debug.Log("End cut: " + angle);
    }
    /*private void OnTriggerExit(Collider other)
    {
        if (!gameObject.activeSelf)
            return;

        BS_Saber saber = other.GetComponent<BS_Saber>();
        if (saber == null)
            return;

        if (cuttingSaber != saber)
            return;

        //Vector3 cutVect = cuttingStart - other.ClosestPoint(transform.position);
        Vector3 cutVect = cuttingStart - saber.GetTipPosition();
        Debug.DrawLine(cuttingStart, saber.GetTipPosition(), Color.red, 1.2f);
        Debug.DrawLine(cuttingStart, cuttingStart + transform.up, Color.cyan, 1.2f);

        Vector3 cutVectProj = Vector3.ProjectOnPlane(-cutVect, transform.forward);
        if (cutVectProj.magnitude < 0.001f)
        {
            manager.DespawnBox(this, isBlue, BS_BoxCut.WrongOrientation);
            return;
        }

        float angle = 180 - Vector3.SignedAngle(transform.up, cutVectProj.normalized, transform.forward);
        TryCut(angle, cuttingSaber.isBlue);
        Debug.Log("End cut: " + angle);
    }*/
    /*private void OnCollisionExit(Collision collision)
    {
        if (!gameObject.activeSelf)
            return;

        BS_Saber saber = collision.collider.GetComponent<BS_Saber>();
        if (saber == null || collision.contactCount == 0)
            return;

        if (cuttingSaber != saber)
            return;

        float angle = Vector3.Angle(transform.right, cuttingStart - collision.GetContact(0).point);
        TryCut(angle, cuttingSaber.isBlue);
        Debug.Log(saber.gameObject.name + " end cut");
    }*/

    public void TryCut(float angle, bool saberBlue)
    {
        // Good Saber ?
        if (isBlue != saberBlue)
        {
            manager.DespawnBox(this, isBlue, BS_BoxCut.WrongSaber);
            return;
        }

        // Bound Angle
        while (angle < 0)
            angle += 360;
        while (angle > 360)
            angle -= 360;

        // Is it a good cut ?
        bool goodCut = false;
        switch (orientation)
        {
            case BS_BoxOrientation.Up:  // using a 50° margin instead of strict 45
                goodCut = angle < 50 || 310 < angle; //310 = 360 -50
                break;
            case BS_BoxOrientation.Right:
                goodCut = 40 < angle && angle < 140; // 90-50 = 40 ; 90+50 = 140
                break;
            case BS_BoxOrientation.Down:
                goodCut = 130 < angle && angle < 230; // 180-50 = 130 ; 180+50 = 230
                break;
            case BS_BoxOrientation.Left:
                goodCut = 220 < angle && angle < 320; // 270-50 = 220 ; 270+50 = 320
                break;
        }
        manager.DespawnBox(this, isBlue, goodCut ? BS_BoxCut.CorrectCut : BS_BoxCut.WrongOrientation);
    }

    // ----------------------------------


    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
