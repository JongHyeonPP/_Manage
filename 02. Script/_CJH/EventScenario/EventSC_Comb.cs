using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GUI_MapScenario;

public class EventSC_Comb : MonoBehaviour
{
    [SerializeField] internal EventSC_CombValues_SGT _SGT;
    [SerializeField] internal EventSC_CombValues_UTIL _UTIL;
    private void Awake()
    {
        SceneToSceneFuncSGT.InitSingleton(ref _SGT.STS);
        SceneToSceneFuncSGT.ArriveScene_Map();
        /*
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
        }*/
    }

    void Start()
    {
        
    }

    public void SceneChange()
    {
        _UTIL.ALS.TimeToSwitchScene();
    }
}


[Serializable]
internal struct EventSC_CombValues_SGT
{
    public SceneToSceneFuncSGT STS;
}

[Serializable]
internal struct EventSC_CombValues_UTIL
{
    public MyInputManager InputM;
    public _AsycLoadScene ALS;
}