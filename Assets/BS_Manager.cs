using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BS_SpinType
{
    None,
    Random,
    RandomIncremental
}

public class BS_Manager : MonoBehaviour
{
    int[] score = new int[4];
    List<BS_Box> inactiveBoxes_Blue = new List<BS_Box>();
    List<BS_Box> inactiveBoxes_Red = new List<BS_Box>();
    List<BS_Fracture> inactiveFragments_Blue = new List<BS_Fracture>();
    List<BS_Fracture> inactiveFragments_Red = new List<BS_Fracture>();
    
    public Transform[] spawnerTransforms;
    public GameObject BS_Box_Prefab_Blue;
    public GameObject BS_Box_Prefab_Red;
    public GameObject FracturePrefab_Blue;
    public GameObject FracturePrefab_Red;
    public float dist = 10;
    public float speed = 3;
    public BS_SpinType spinType = BS_SpinType.RandomIncremental;

    public TextMesh text;

    private bool paused = true;
    private float timer = 0;
    public float delay = 0.5f;



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UnPause(1.5f));
    }

    // Update is called once per frame
    void Update()
    {
        if (paused)
            return;

        timer += Time.deltaTime;
        if (timer < delay)
            return;

        timer -= delay;
        int index = Random.Range(0, spawnerTransforms.Length);
        BS_BoxOrientation orientation = (BS_BoxOrientation)Random.Range(0, 4);
        bool blue = Random.Range(0, 1.0f) > 0.5f;
        Spin();
        SpawnBox(index, speed, orientation, blue);
    }

    IEnumerator UnPause(float delay)
    {
        yield return new WaitForSeconds(delay);
        paused = false;
    }


    private void Spin()
    {
        switch(spinType)
        {
            case BS_SpinType.None:
                return;
            case BS_SpinType.Random:
            {
                int angle = Random.Range(0, 360);
                transform.rotation = Quaternion.Euler(0, angle, 0);
                break;

            }
            case BS_SpinType.RandomIncremental:
            {
                int angle = Random.Range(-50,50);
                transform.rotation *= Quaternion.Euler(0, angle, 0);
                break;
            }
        }
    }
    private void SpawnBox(int _spwanerIndex, float _speed, BS_BoxOrientation _orientation, bool _isBlue)
    {
        Transform boxInitTransform = spawnerTransforms[_spwanerIndex];
        if (_isBlue)
        {
            if (inactiveBoxes_Blue.Count > 0)
            {
                inactiveBoxes_Blue[0].Reinit(boxInitTransform, _speed, dist, _orientation);
                inactiveBoxes_Blue.RemoveAt(0);
            }
            else
            {
                GameObject boxObj = GameObject.Instantiate(BS_Box_Prefab_Blue, boxInitTransform.position, boxInitTransform.rotation);
                BS_Box box = boxObj.GetComponent<BS_Box>();
                box.Init(this, true, _speed, dist, _orientation);
            }
        }
        else
        {
            if (inactiveBoxes_Red.Count > 0)
            {
                inactiveBoxes_Red[0].Reinit(boxInitTransform, _speed, dist, _orientation);
                inactiveBoxes_Red.RemoveAt(0);
            }
            else
            {
                GameObject boxObj = GameObject.Instantiate(BS_Box_Prefab_Red, boxInitTransform.position, boxInitTransform.rotation);
                BS_Box box = boxObj.GetComponent<BS_Box>();
                box.Init(this, false, _speed, dist, _orientation);
            }
        }
    }

    public void DespawnBox(BS_Box _box, bool _isBlue, BS_BoxCut _cutType)
    {
        if (_isBlue)
            inactiveBoxes_Blue.Add(_box);
        else
            inactiveBoxes_Red.Add(_box);

        _box.StopAllCoroutines();
        _box.gameObject.SetActive(false);
        if (_cutType != BS_BoxCut.Miss)
        {
            if (_isBlue)
            {
                if (inactiveFragments_Blue.Count > 0)
                {
                    inactiveFragments_Blue[0].Reset(_box.transform);
                    inactiveFragments_Blue.RemoveAt(0);
                }
                else
                {
                    GameObject fragObj = GameObject.Instantiate(FracturePrefab_Blue, _box.transform.position, _box.transform.rotation);
                    BS_Fracture frag = fragObj.GetComponent<BS_Fracture>();
                    frag.SetManager(this);
                }
            }
            else
            {
                if (inactiveFragments_Red.Count > 0)
                {
                    inactiveFragments_Red[0].Reset(_box.transform);
                    inactiveFragments_Red.RemoveAt(0);
                }
                else
                {
                    GameObject fragObj = GameObject.Instantiate(FracturePrefab_Red, _box.transform.position, _box.transform.rotation);
                    BS_Fracture frag = fragObj.GetComponent<BS_Fracture>();
                    frag.SetManager(this);
                }
            }
        }

        score[(int)_cutType]++;
        text.text = "Good Cut: " + score[(int)BS_BoxCut.CorrectCut] + "\nWng Dir: " + score[(int)BS_BoxCut.WrongOrientation] + "\nWng Saber: " + score[(int)BS_BoxCut.WrongSaber] + "\nMissed: " + score[(int)BS_BoxCut.Miss];
    }

    public void DespawnFracture(BS_Fracture _fracture, bool _isBlue)
    {
        if (_isBlue)
            inactiveFragments_Blue.Add(_fracture);
        else
            inactiveFragments_Red.Add(_fracture);
    }


    public void ResetScore()
    {
        for (int i = 0; i < 4; i++)
            score[i] = 0;

        paused = true;
        StartCoroutine(UnPause(3));
    }
    public void SwapSpinType()
    {
        if (spinType == BS_SpinType.None)
            spinType = BS_SpinType.Random;
        else if (spinType == BS_SpinType.Random)
            spinType = BS_SpinType.RandomIncremental;
        else if (spinType == BS_SpinType.RandomIncremental)
            spinType = BS_SpinType.None;

        transform.rotation = Quaternion.identity;
        paused = true;
        StartCoroutine(UnPause(3));
    }
    public void LeaveScene()
    {
        SceneManager.LoadScene("Main");
    }
}
