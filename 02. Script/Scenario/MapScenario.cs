using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapScenario : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("Battle");
    }
}
