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
public enum BS_Speed
{
    Slow,
    Normal,
    Fast,
    Random
}
public enum BS_VerticalType
{
    Flat,
    Fixed,
    Random
}
public enum BS_SaberLimit
{
    Normal,
    Limit,
    Green
}

public class BS_Manager : MonoBehaviour
{
    int[] score = new int[4];
    List<BS_Box> inactiveBoxes_Blue = new List<BS_Box>();
    List<BS_Box> inactiveBoxes_Red = new List<BS_Box>();
    List<BS_Fracture> inactiveFragments_Blue = new List<BS_Fracture>();
    List<BS_Fracture> inactiveFragments_Red = new List<BS_Fracture>();
    List<BS_Fracture> inactiveFragments_Green = new List<BS_Fracture>();
    List<BS_Fracture> inactiveFragments_BlueFail = new List<BS_Fracture>();
    List<BS_Fracture> inactiveFragments_RedFail = new List<BS_Fracture>();
    List<BS_Halo> inactiveHalos_Blue = new List<BS_Halo>();
    List<BS_Halo> inactiveHalos_Red = new List<BS_Halo>();

    [Header("GameObjects")]
    public Transform verticalPivot;
    public Transform[] spawnerTransforms;
    public BS_Saber[] sabers;
    
    [Header("Prefab")]
    public GameObject BS_Box_Prefab_Blue;
    public GameObject BS_Box_Prefab_Red;
    public GameObject FracturePrefab_Blue;
    public GameObject FracturePrefab_Red;
    public GameObject FracturePrefab_Green;
    public GameObject FracturePrefab_BlueFail;
    public GameObject FracturePrefab_RedFail;
    public GameObject BS_Halo_Prefab_Blue;
    public GameObject BS_Halo_Prefab_Red;

    [Header("Settings")]
    [SerializeField] private float dist = 10;
    [SerializeField] private float[] speeds = { 3, 6, 12 };
    [SerializeField] private BS_SpinType spinType = BS_SpinType.RandomIncremental;
    [SerializeField] private BS_Speed speedType = BS_Speed.Slow;
    [SerializeField] private BS_VerticalType verticalAngleType = BS_VerticalType.Flat;
    [SerializeField] private BS_SaberLimit saberLimit = BS_SaberLimit.Normal;

    private bool paused = true;
    private float timer = 0;
    [SerializeField] private float delay = 0.5f;

    [Header("Texts")]
    public TextMesh textScore;
    public TextMesh textAngle;
    public TextMesh textSpeed;
    public TextMesh textVerticalAngle;
    public TextMesh textLimit;





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
        int speedIndex = (int)speedType;
        if (speedIndex == speeds.Length)
            speedIndex = Random.Range(0, speedIndex);
        SpawnBox(index, speeds[speedIndex], orientation, blue);
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
                break;
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

        switch(verticalAngleType)
        {
            case BS_VerticalType.Flat:
            case BS_VerticalType.Fixed:
                break;
            case BS_VerticalType.Random:
            {
                int angle = -Random.Range(0, 45);
                verticalPivot.localRotation = Quaternion.Euler(angle, 0, 0);
                break;
            }
        }
    }
    private void SpawnBox(int _spwanerIndex, float _speed, BS_BoxOrientation _orientation, bool _isBlue)
    {
        Transform boxInitTransform = spawnerTransforms[_spwanerIndex];
        if (_isBlue)
        {
            // Block
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

            // Halo
            if (inactiveHalos_Blue.Count > 0)
            {
                inactiveHalos_Blue[0].Reset(boxInitTransform);
                inactiveHalos_Blue.RemoveAt(0);
            }
            else
            {
                GameObject boxObj = GameObject.Instantiate(BS_Halo_Prefab_Blue, boxInitTransform.position, boxInitTransform.rotation);
                BS_Halo halo = boxObj.GetComponent<BS_Halo>();
                halo.Init(this);
            }
        }
        else
        {
            // Block
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

            // Halo
            if (inactiveHalos_Red.Count > 0)
            {
                inactiveHalos_Red[0].Reset(boxInitTransform);
                inactiveHalos_Red.RemoveAt(0);
            }
            else
            {
                GameObject boxObj = GameObject.Instantiate(BS_Halo_Prefab_Red, boxInitTransform.position, boxInitTransform.rotation);
                BS_Halo halo = boxObj.GetComponent<BS_Halo>();
                halo.Init(this);
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
        if (saberLimit == BS_SaberLimit.Green)
        {
            if (inactiveFragments_Green.Count > 0)
            {
                inactiveFragments_Green[0].Reset(_box.transform);
                inactiveFragments_Green.RemoveAt(0);
            }
            else
            {
                GameObject fragObj = GameObject.Instantiate(FracturePrefab_Green, _box.transform.position, _box.transform.rotation);
                BS_Fracture frag = fragObj.GetComponent<BS_Fracture>();
                frag.SetManager(this);
            }
        }
        else if (_cutType == BS_BoxCut.CorrectCut)
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
        else if (_cutType == BS_BoxCut.WrongSaber)
        {
            if (_isBlue)
            {
                if (inactiveFragments_BlueFail.Count > 0)
                {
                    inactiveFragments_BlueFail[0].Reset(_box.transform);
                    inactiveFragments_BlueFail.RemoveAt(0);
                }
                else
                {
                    GameObject fragObj = GameObject.Instantiate(FracturePrefab_BlueFail, _box.transform.position, _box.transform.rotation);
                    BS_Fracture frag = fragObj.GetComponent<BS_Fracture>();
                    frag.SetManager(this);
                }
            }
            else
            {
                if (inactiveFragments_RedFail.Count > 0)
                {
                    inactiveFragments_RedFail[0].Reset(_box.transform);
                    inactiveFragments_RedFail.RemoveAt(0);
                }
                else
                {
                    GameObject fragObj = GameObject.Instantiate(FracturePrefab_RedFail, _box.transform.position, _box.transform.rotation);
                    BS_Fracture frag = fragObj.GetComponent<BS_Fracture>();
                    frag.SetManager(this);
                }
            }
        }

        score[(int)_cutType]++;
        textScore.text = "Good Cut: " + score[(int)BS_BoxCut.CorrectCut] + "\nWng Saber: " + score[(int)BS_BoxCut.WrongSaber] + "\nMissed: " + score[(int)BS_BoxCut.Miss];
    }

    public void DespawnFracture(BS_Fracture _fracture, bool _isBlue, bool _isFail, bool _isGreen)
    {
        if(_isGreen)
        {
            inactiveFragments_Green.Add(_fracture);
            return;
        }

        if(_isFail)
        {
            if (_isBlue)
                inactiveFragments_BlueFail.Add(_fracture);
            else
                inactiveFragments_RedFail.Add(_fracture);
        }
        else
        {
            if (_isBlue)
                inactiveFragments_Blue.Add(_fracture);
            else
                inactiveFragments_Red.Add(_fracture);
        }
    }

    public void DespawnHalo(BS_Halo _halo, bool _isBlue)
    {
        _halo.gameObject.SetActive(false);
        if (_isBlue)
            inactiveHalos_Blue.Add(_halo);
        else
            inactiveHalos_Red.Add(_halo);
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

        textAngle.text = "Angle Type\n" + spinType.ToString();
        transform.rotation = Quaternion.identity;
        paused = true;
        StartCoroutine(UnPause(3));
    }
    public void SwapSpeed()
    {
        if (speedType == BS_Speed.Slow)
            speedType = BS_Speed.Normal;
        else if (speedType == BS_Speed.Normal)
            speedType = BS_Speed.Fast;
        else if (speedType == BS_Speed.Fast)
            speedType = BS_Speed.Random;
        else if (speedType == BS_Speed.Random)
            speedType = BS_Speed.Slow;

        textSpeed.text = "Speed\n" + speedType.ToString();
        paused = true;
        StartCoroutine(UnPause(3));
    }
    public void SwapVerticalAngle()
    {
        if (verticalAngleType == BS_VerticalType.Flat)
            verticalAngleType = BS_VerticalType.Fixed;
        else if (verticalAngleType == BS_VerticalType.Fixed)
            verticalAngleType = BS_VerticalType.Random;
        else if (verticalAngleType == BS_VerticalType.Random)
            verticalAngleType = BS_VerticalType.Flat;

        textVerticalAngle.text = "Vertical Angle\n" + verticalAngleType.ToString();
        verticalPivot.localRotation = (verticalAngleType == BS_VerticalType.Fixed) ? Quaternion.Euler(-30,0,0) : Quaternion.identity;
        paused = true;
        StartCoroutine(UnPause(3));
    }
    public void SwapLimitSaber()
    {
        if (saberLimit == BS_SaberLimit.Normal)
            saberLimit = BS_SaberLimit.Limit;
        else if (saberLimit == BS_SaberLimit.Limit)
            saberLimit = BS_SaberLimit.Green;
        else if (saberLimit == BS_SaberLimit.Green)
            saberLimit = BS_SaberLimit.Normal;

        textLimit.text = "Limit Saber\n" + saberLimit.ToString();
        bool _isGreen = saberLimit == BS_SaberLimit.Green;
        sabers[0].SetMaterial(_isGreen);
        sabers[1].SetMaterial(_isGreen);
        paused = true;
        StartCoroutine(UnPause(3));
    }
    public BS_SaberLimit GetSaberLimit()
    {
        return saberLimit;
    }

    public void LeaveScene()
    {
        SceneManager.LoadScene("Main");
    }
}