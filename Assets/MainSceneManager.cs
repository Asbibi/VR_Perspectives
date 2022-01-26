using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    public void LoadBeatSaberScene()
    {
        SceneManager.LoadScene("BeatSaber");
    }

    public void LoadHumanFallFlatScene()
    {
        SceneManager.LoadScene("HumanFallFlat");
    }
}
