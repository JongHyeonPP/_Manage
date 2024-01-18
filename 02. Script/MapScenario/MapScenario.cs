using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using static GUI_MapScenario;

public class MapScenario : MonoBehaviour
{
    [SerializeField] internal Values_SGT _SGT;
    [SerializeField] internal Values_UTIL _UTIL;
    [SerializeField] internal Values_SCENARIO _SC;

    public delegate void OnClickFunc(int index);
    public MapHistoryData history;
    //public MapScenarioValues values_DEBUG;

    public bool isAction;

    bool trash = false;
    private void Awake()
    {
        trash = _SGT.mapDATA.CheckInitOn(this);

        SceneToSceneFuncSGT.InitSingleton(ref _SGT.STS);
        SceneToSceneFuncSGT.ArriveScene_Map();

        // inven input
        if (true)
        {
            ProgressMap_preInput task = new(() => {
                if (_SGT.mapDATA.gameObject.activeSelf == true)
                {
                    EnterInventory();
                }
                else if (true)
                {
                    _UTIL.CES.BlurStart_InvenToMap();
                }
            });

            _UTIL.InputM.AddInputEventList(task, KeyCode.I);
        }
    }
    private void Start()
    {
        if (trash)
            StartMapCreating();
        _SGT.mapDATA.gameObject.SetActive(true);
        _SC.mapGUI.SetInitState();
    }

    public void StartMapCreating()
    {
        // ㅈ수석 추가 안하면 ㅈ된다 나중에
        // 결정 전 단계를 GUI에게 맡김

        // 맵 진행 시 일어나야할 이벤트를 GUI에게 넣어줌
        _SC.mapGUI._selectGUI._InitSelectedFunc(new OnClickFunc(ProgressMap));

        // 해당 GUI 이벤트를 버튼에게 저장하기 위해 create map 에 넣어둠
        _SC.cs.SetOnClickFunc(_SC.mapGUI._selectGUI._SelectEvent);

        if (history.GetHistory().Length == 0)//(values.history == "Null")
        {
            history.SetHistory("");
            _SC.cs.NewGame(history.getSeed());
        }
        else
            LoadGame();

        _SC.mapGUI._selectGUI.SaveHighlight_Root();

        void LoadGame()
        {
            int seed = this.history.seed;             //(int)(long)GameManager.gameManager.progressDoc["Seed"];
            string history = this.history.GetHistory();    //(string)GameManager.gameManager.progressDoc["History"];

            string[] temp = history.Split('/');
            int[] result = new int[temp.Length - 1];

            for (int i = 0; i < temp.Length - 1; i++)
            {
                result[i] = int.Parse(temp[i]);
            }

            _SC.cs.LoadGame(seed, result);

        }
    }

    // dhk tlqkfwhw
    public void ProgressMap(int index)
    {
        if (_SGT.mapDATA.CurrMS._UTIL.ALS.IsLoadedScene())
        {
            Debug.Log("cant");
            return;
        }

        isAction = false;
        history.SetHistory(history.GetHistory() + index + "/");
        ProgressMap_preInput task = new(() => {; });
        Vector3 desV3 = _SC.cs.ProgressMap(index, ref task);

        if (desV3 != Vector3.zero) {; }
        else
        {
            _SGT.mapDATA.ApplyStageClearToHistory();
            _SGT.mapDATA.isHaveToCreate = true;
        }
        SceneToSceneFuncSGT.ExitScene_Map();

        // Scene 이동 추가 및 카메라 이동 시작
        if (true)
        {
            task += SceneChange;
            task += () => _SGT.mapDATA.gameObject.SetActive(false);
            Debug.Log("MoveScene by MapSC");
            _SGT.mapDATA.CurrMS._UTIL.ALS.LoadScene_Asyc("Battle"); //history.GetNextScene()  Shop
            _SC.mapGUI.moveCamFunc(desV3, task);
        }
    }

    public void SceneChange()
    {
        isAction = true;
        _SGT.mapDATA.CurrMS._UTIL.ALS.TimeToSwitchScene();
    }

    public void EnterInventory()
    {
        if (_UTIL.ALS.IsLoadedScene())
        {
            Debug.Log("cant");
            return;
        }

        _UTIL.CaptureBG.CaptureByMainCam(_SGT.mapDATA._mapGUI.CameraComp);
        ProgressMap_preInput task;
        ProgressMap_preInput rtnTask = new(() => {; });
        //_SC.invenSC.setGUI_bySGT();

        if (true)
        {
            ;// _SC.inven.SetState_DataInit();
        }       // Blur 작업 전 작업, 입력 방지 

        if (true)
        {
            task = new(() => {; });
            task += () => { _SGT.mapDATA.gameObject.SetActive(false); };
        }       // Blur 작업 후 작업 설정, 

        if (true)
        {
            rtnTask += () => { _SGT.mapDATA.gameObject.SetActive(true); };
        }       // Blur 작업 후 작업 설정, 

        _UTIL.CES.BlurStart(task, rtnTask);
    }
}

[Serializable]
public class MapHistoryData
{
    public List<string> history;
    public int seed;
    public int stage;

    public string GetHistory()
    {
        return history[stage];
    }

    public void SetHistory(string input)
    {
        history[stage] = input;
    }

    public int getSeed() { return seed * stage; }

    public void ClearLevel()
    {
        stage++;
    }

    public string GetNextScene()
    {
        return "Stage" + stage;
    }
}

[Serializable]
internal struct Values_SGT
{
    public MapDataSGT mapDATA;
    public GUI_MapScenario mapGUI;
    public SceneToSceneFuncSGT STS;
}

[Serializable]
internal struct Values_UTIL
{
    public OverlayTrick CaptureBG;
    public MyInputManager InputM;
    public InvenCtrl_MapSC CES;
    public _AsycLoadScene ALS;
}

[Serializable]
internal struct Values_SCENARIO
{
    public CreateStageScenario cs;
    public GUI_MapScenario mapGUI;
    public InvenSC_Map invenSC;
}
