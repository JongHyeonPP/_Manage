using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSingleton : MonoBehaviour
{
    private static MapSingleton mapSingleton;
    public CreateStageScenario _cs;
    public GUI_MapScenario _mapGUI;
    public bool CheckInitOn(MapScenario _scenario)
    {
        if (mapSingleton == null)
        {
            Application.targetFrameRate = 60;
            mapSingleton = this;
            DontDestroyOnLoad(this);

            _UpdateData();
            return true;
        }
        else if (mapSingleton == this)
        {
            Debug.Log("it is strange");
            _UpdateData();
            return false;
        }
        else
        {
            mapSingleton.gameObject.SetActive(true);
            _scenario.ms = mapSingleton;
            Destroy(gameObject);
            _UpdateData();
            return false;
        }

        void _UpdateData()
        {
            _scenario.ms = mapSingleton;
            _scenario._cs = mapSingleton._cs;
            _scenario._mapGUI = mapSingleton._mapGUI;
        }
    }
}
