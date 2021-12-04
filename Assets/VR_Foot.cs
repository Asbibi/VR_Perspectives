using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Foot : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private Transform body;
    [SerializeField] private VR_Foot otherFoot;

    [SerializeField] private float speed = 5;
    [SerializeField] private float stepDistance = .3f;
    [SerializeField] private float stepLength = .3f;
    [SerializeField] private float stepHeight = .3f;

    [SerializeField] private Vector3 footPosOffset;
    [SerializeField] private Vector3 footRotOffset;

    private float _footSpacing, _lerp;
    private Vector3 _oldPos, _currentPos, _newPos;
    private Vector3 _oldNorm, _currentNorm, _newNorm;
    private bool isFirstStep = true;


    // Start is called before the first frame update
    void Start()
    {
        _footSpacing = transform.localPosition.x;
        _currentPos = _newPos = _oldPos = transform.position;
        _oldNorm = _currentNorm = _newNorm = transform.up;
        _lerp = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _currentPos + footPosOffset;
        transform.rotation = Quaternion.LookRotation(_currentNorm) * Quaternion.Euler(footRotOffset + GetRotOffsetFromBody());

        Ray ray = new Ray(body.position + (body.right * _footSpacing) + (Vector3.up * 2), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 10, terrainLayer.value))
        {
            if((Vector3.Distance(_newPos, hit.point) > stepDistance && !otherFoot.IsMoving()) || isFirstStep)
            {
                _lerp = 0;
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(_newPos).z ? 1 : -1;
                _newPos = hit.point + (body.forward * (direction * stepLength));
                _newNorm = hit.normal;
                isFirstStep = false;
            }
        }

        if (_lerp < 1)
        {
            Vector3 _tempPos = Vector3.Lerp(_oldPos, _newPos, _lerp);
            _tempPos.y += Mathf.Sin(_lerp * Mathf.PI) * stepHeight;

            _currentPos = _tempPos;
            _currentNorm = Vector3.Lerp(_oldNorm, _newNorm, _lerp);
            _lerp += Time.deltaTime * speed;
        }
        else
        {
            _oldPos = _newPos;
            _oldNorm = _newNorm;
        }
    }

    public bool IsMoving()
    {
        return _lerp < 1;   // foot is in the air, still moving
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_newPos, 0.1f);
    }

    private Vector3 GetRotOffsetFromBody()
    {
        Vector3 bodyRotOffset = Vector3.zero;
        bodyRotOffset.x = -body.rotation.eulerAngles.y;
        //VR_DebugText.displayText(bodyRotOffset.ToString());
        return bodyRotOffset;
    }
}
