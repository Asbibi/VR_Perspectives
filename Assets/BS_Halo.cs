using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_Halo : MonoBehaviour
{
    private Material initialMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private bool isBlue;
    private Transform mesh;
    private BS_Manager manager;
    private MeshRenderer[] renderers;

    private void Awake()
    {
        mesh = transform.GetChild(0);
        renderers = new MeshRenderer[2];
        renderers[0] = mesh.GetComponent<MeshRenderer>();
        renderers[1] = mesh.GetChild(0).GetComponent<MeshRenderer>();
        initialMat = renderers[0].material;
    }

    // Update is called once per frame
    void Update()
    {
        float sz = mesh.localScale.z;
        if (sz < 0)
        {
            manager.DespawnHalo(this, isBlue);
            return;
        }

        sz -= 0.01f;
        mesh.localScale = new Vector3(0.75f, 1, sz);
    }


    public void Init(BS_Manager _manager)
    {
        manager = _manager;
        CommonInit(transform);
    }
    public void Reset(Transform _blockTransform)
    {
        CommonInit(_blockTransform);
        mesh.localScale = new Vector3(0.75f, 1, 0.1f);
        gameObject.SetActive(true);
    }
    private void CommonInit(Transform _blockTransform)
    {
        transform.position = new Vector3(_blockTransform.position.x, 0, _blockTransform.position.z);
        transform.rotation = Quaternion.Euler(0, _blockTransform.rotation.eulerAngles.y, 0);
        Material _mat = manager.GetSaberLimit() == BS_SaberLimit.Green ? greenMat : initialMat;
        renderers[0].material = _mat;
        renderers[1].material = _mat;
    }
}
