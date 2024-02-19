using System;
using System.Collections.Generic;
using UnityEngine;
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
    bool _checkCreated = false;

    private void Awake()
    {
        _checkCreated = _SGT.mapDATA.CheckInitOn(this);

        SceneToSceneFuncSGT.InitSingleton(ref _SGT.STS);
        _SGT.mapDATA.visualObj.enabled = (true);
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
        if (_checkCreated)
            StartMapCreating();

        _SGT.mapDATA.gameObject.SetActive(true);
        _SC.mapGUI.SetInitState();
    }


    public void StartMapCreating()
    {
        // ������ �߰� ���ϸ� ���ȴ� ���߿�
        // ���� �� �ܰ踦 GUI���� �ñ�

        // �� ���� �� �Ͼ���� �̺�Ʈ�� GUI���� �־���
        _SC.mapGUI._selectGUI._InitSelectedFunc(new OnClickFunc(ProgressMap));

        // �ش� GUI �̺�Ʈ�� ��ư���� �����ϱ� ���� create map �� �־��
        _SC.cs.SetOnClickFunc(_SC.mapGUI._selectGUI._SelectEvent);

        if (history.GetHistory().Length == 0)//(values.history == "Null")
        {
            //debug
            history.InitHistoryData(UnityEngine.Random.Range(0,500));
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

        if (desV3 != Vector3.zero) {;}
        else
        {
            _SGT.mapDATA.ApplyStageClearToHistory();
            _SGT.mapDATA.isHaveToCreate = true;
        }

        SceneToSceneFuncSGT.ExitScene_Map(
            new(() => {
                _SGT.mapDATA.visualObj.enabled = (false);
            }));


        // Scene �̵� �߰� �� ī�޶� �̵� ����
        if (true)
        {
            task += SceneChange;
            //Debug.Log("MoveScene by MapSC");

            for (int charIndex = 1; charIndex < 4; charIndex++)
            {
                string[] skillEquipped = new string[2] { "Null", "Null" };
                string[] weaponEquipped = new string[2] { "Null", "Null" };

                SGT_GUI_ItemData.GetCharInvenSGT(charIndex,ref skillEquipped, ref weaponEquipped);

                this.SetMapData_CharSkills(charIndex - 1, skillEquipped);
                this.SetMapData_CharEquips(charIndex - 1, weaponEquipped);
            }

            if (true)
            {
                // <<< Scene Name
                string dstSceneName = GameManager.gameManager.GetSceneName_byEventIndex(_SC.cs.GetIndex_atCurrFocusing());
                _SGT.mapDATA.CurrMS._UTIL.ALS.LoadScene_Asyc(dstSceneName); //history.GetNextScene()  Shop
                _SC.mapGUI.moveCamFunc(desV3, task);
            }
        }
    }
    public void ProgressCamp()
    {
        if (_UTIL.ALS.IsLoadedScene())
        {
            Debug.Log("cant");
            return;
        }

        ProgressMap_preInput task = new(() => {; });
        task += () => _UTIL.ALS.TimeToSwitchScene();
        task += () => _SGT.mapDATA.gameObject.SetActive(false);
        SceneToSceneFuncSGT.ExitScene_Map(task);

        // Scene �̵� �߰� �� ī�޶� �̵� ����
        if (true)
        {
            Debug.Log("MoveScene To Camp");
            _UTIL.ALS.LoadScene_Asyc("Camp");
        }
    }
    public void SceneChange()
    {
        isAction = true;
        _SGT.mapDATA.CurrMS._UTIL.ALS.TimeToSwitchScene();
        _SGT.mapDATA.gameObject.SetActive(false);
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
        }       // Blur �۾� �� �۾�, �Է� ���� 

        if (true)
        {
            task = new(() => {; });
            task += () => { _SGT.mapDATA.gameObject.SetActive(false); };
        }       // Blur �۾� �� �۾� ����, 

        if (true)
        {
            rtnTask += () => { _SGT.mapDATA.gameObject.SetActive(true); };
        }       // Blur �۾� �� �۾� ����, 

        _UTIL.CES.BlurStart(task, rtnTask);
    }
}

[Serializable]
public class MapHistoryData
{
    public List<string> history;
    public int seed;
    public int stage;

    public void InitHistoryData(int _seed)
    {
        seed = _seed;
        SetHistory("");
        return;
    }

    public string GetHistory()
    {
        string _history = history[stage];
        var splited = _history.Split('_');

        if (splited.Length == 1)
        {
            return "";
        }

        if (splited.Length == 2)
        {
            return splited[1];
        }

        Debug.Log("??? ->" + _history);
        return _history;
    }

    public void SetHistory(string input)
    {
        history[stage] = stage + "_" + input;
    }

    public int getSeed() { return seed * stage; }

    public void ClearLevel()
    {
        stage++;
    }

    public string GetNextScene()
    {
        return "Stage " + stage;
    }
}

[Serializable]
internal struct Values_SGT
{
    public MapDataSGT mapDATA;
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
