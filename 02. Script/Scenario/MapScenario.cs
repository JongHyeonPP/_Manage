using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GUI_MapScenario;

public class MapScenario : MonoBehaviour
{
    public MapSingleton ms;
    public delegate void OnClickFunc(int index);

    public bool isDebug = false;
    public MapScenarioValues values;
    public MapScenarioValues values_DEBUG;

    public CreateStageScenario _cs;
    public GUI_MapScenario _mapGUI;

    public bool isAction;

    private void Awake()
    {
        if(ms.CheckInitOn(this))
            StartMapCreating();

        _mapGUI.SetInitState();
    }

    public void ApplyData_DEBUG()
    {
        _cs.SetOnClickFunc(new OnClickFunc(ProgressMap));
        values = values_DEBUG.DeepCopy();
    }

    public void StartMapCreating()
    {
        _cs.SetOnClickFunc(new OnClickFunc(ProgressMap));
        //_mapGUI.SetOnClickFunc(new SceneChangeFunc(ref SceneChange));

        if (values.history == "Null")
            _cs.NewGame(values.seed);
        else
            LoadGame();

        void LoadGame()
        {
            int seed = values.seed;             //(int)(long)GameManager.gameManager.progressDoc["Seed"];
            string history = values.history;    //(string)GameManager.gameManager.progressDoc["History"];
            values.history = history;
            string[] temp = history.Split('/');
            int[] result = new int[temp.Length];

            for (int i = 0; i < temp.Length; i++)
            {
                result[i] = int.Parse(temp[i]);
            }

            _cs.LoadGame(seed, result);
        }
    }

    public void CreateBG_DEBUG()
    {
        _cs.NewGame(values.seed);
    }

    public void ProgressMap(int index)
    {

        if (isAction == false)
            Debug.Log("sad");
            //return;

        isAction = false;
        values.history += "/" + index;

        ProgressMap_preInput task = new(() => {;});
        Vector3 desV3 = _cs.ProgressMap(index,ref task);

 
        task += () => SceneChange("_Backup0719");

        _mapGUI.moveCamFunc(desV3, task);
    }

    public void SceneChange(string targetScene)
    {
        isAction = false;
        SceneManager.LoadScene("CJH_Map");
        //ms.gameObject.SetActive(false);
    }
}

[Serializable]
public class MapScenarioValues
{
    public string history;
    public int seed;
    public int stage;

    public MapScenarioValues DeepCopy()
    {
        MapScenarioValues copy = new MapScenarioValues();
        copy.history = this.history;
        copy.seed = this.seed;
        copy.stage = this.stage;

        return copy;
    }
}