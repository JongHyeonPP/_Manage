using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using static GUI_MapScenario;

public class EventSC_Shop : MonoBehaviour
{
    [SerializeField] internal ShopValues_SGT _SGT;
    [SerializeField] internal ShopValues_UTIL _UTIL;


    private void Awake()
    {
        SceneToSceneFuncSGT.InitSingleton(ref _SGT.STS);
        SceneToSceneFuncSGT.ArriveScene_Map();
        _UTIL.invenSC.setGUI_bySGT();

        // inven input
        if (true)
        {
            ProgressMap_preInput task = new(() => {
                if (_UTIL.invenCtrl.GetIsInvenActive() == false)
                {
                    EnterInventory();
                }
                else if (true)
                {
                    _UTIL.invenCtrl.BlurStart_InvenToMap();
                }
            });

            _UTIL.InputM.AddInputEventList(task, KeyCode.I);
        }
    }

    public void EnterInventory()
    {
        ProgressMap_preInput task = new(() => {; });
        ProgressMap_preInput rtnTask = new(() => {; });
        _UTIL.invenSC.setGUI_bySGT();
        _UTIL.invenCtrl.BlurStart(task, rtnTask);
    }

    
    public void ProgressMap(int index)
    {
        if (_UTIL.ALS.IsLoadedScene())
        {
            Debug.Log("cant");
            return;
        }

        ProgressMap_preInput task = new(() => {;});
        task += SceneChange;

        SceneToSceneFuncSGT.ExitScene_Map(task);

        // Scene 이동 추가 및 카메라 이동 시작
        if (true)
        {
            Debug.Log("MoveScene To stage 0");
            _UTIL.ALS.LoadScene_Asyc("stage 0");
        }
    }

    public void SceneChange()
    {
        _UTIL.ALS.TimeToSwitchScene();
    }
}

[Serializable]
internal struct ShopValues_SGT
{
    public SceneToSceneFuncSGT STS;
}

[Serializable]
internal struct ShopValues_UTIL
{
    public MyInputManager InputM;
    public _AsycLoadScene ALS;
    public InvenSC_Shop invenSC;
    public InvenCtrl_ShopSC invenCtrl;
}