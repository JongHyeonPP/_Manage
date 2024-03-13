using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static GUI_MapScenario;

public class EventSC_Default : MonoBehaviour
{
    [SerializeField] internal EventSC_CombValues_SGT _SGT;
    [SerializeField] internal EventSC_CombValues_UTIL _UTIL;
    [SerializeField] internal UnityEvent _ArriveEvent;
    [SerializeField] internal UnityEvent _ExitEvent;

    private void Awake()
    {
        SceneToSceneFuncSGT.InitSingleton(ref _SGT.STS);
        SceneToSceneFuncSGT.ArriveScene_Map(() => _ArriveEvent.Invoke());
        
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

    public void SceneChange()
    {
        _UTIL.ALS.TimeToSwitchScene();
    }

    public void ProgressMap()
    {
        _ExitEvent.Invoke();
        if (_UTIL.ALS.IsLoadedScene())
        {
            Debug.Log("cant");
            return;
        }

        ProgressMap_preInput task = new(() => {; });
        task += SceneChange;

        SceneToSceneFuncSGT.ExitScene_Map(task);

        // Scene 이동 추가 및 카메라 이동 시작
        if (true)
        {
            _UTIL.ALS.LoadScene_Asyc("Stage 0");
        }
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