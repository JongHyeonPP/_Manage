using EnumCollection;
using StageCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class DestinationNode : MonoBehaviour
{
    public int index;

    public Image imageName;
    public TMP_Text textName;
    public Image buttonDot;

    public Image imageObject;

    public Image buttonEnter;
    public TMP_Text textEnter;
    public int arrayIndex;
    public BackgroundType backGroundType;
    public int phaseNum;

    public NodeType nodeType;
    public readonly static Color storeColor = new Color(0.773f, 0.94f, 0.773f, 0.7f);
    private void Awake()
    {
        OnLangaugeChange();
        SettingManager.LanguageChangeEvent += OnLangaugeChange;
    }
    private void OnLangaugeChange()
    {
        if (textEnter && textEnter.gameObject.activeSelf)
            textEnter.text = GameManager.language == Language.Ko ? "들어가기" : "Enter";
        if (textName && textName.gameObject.activeSelf&&nodeType!=null)
            textName.text = nodeType.name[GameManager.language];
        
    }
    
    public IEnumerator GraduallyAscendBaseAlpha()
    {
        float curAlpha = 0f;
        float totalTime = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            curAlpha = Mathf.Lerp(0f, 1f, elapsedTime / totalTime);
            elapsedTime += Time.deltaTime;
            SetBaseAlpha(curAlpha);
            yield return null; // 한 프레임 대기
        }
        curAlpha = 1f;
        SetBaseAlpha(curAlpha);
    }
    public void SetBaseAlpha(float _alpha)
    {
        Color imageNameColor = imageName.color;
        imageName.color = new Color(imageNameColor.r, imageNameColor.g, imageNameColor.b, _alpha * 0.7f);

        Color buttonDotColor = buttonDot.color;
        buttonDot.color = new Color(buttonDotColor.r, buttonDotColor.g, buttonDotColor.b, _alpha);

        Color imageObjectColor = imageObject.color;
        imageObject.color = new Color(imageObjectColor.r, imageObjectColor.g, imageObjectColor.b, _alpha);

        if (textName)
        {
            Color textNameColor = textName.color;
            textName.color = new Color(textNameColor.r, textNameColor.g, textNameColor.b, _alpha);
        }
    }


    public void OnNodeClicked()
    {
        if (StageScenarioBase.state != StateInMap.NeedMove)
            return;
        StageScenarioBase.phase++;
        StageScenarioBase.state = StateInMap.None;
        StartCoroutine(MoveWaitCor());

        StageScenarioBase.nodes.RemoveAt(StageScenarioBase.nodes.Count - 1);
        StageScenarioBase.nodes.Add(arrayIndex);
        Dictionary<string, object> docDict = new()
        {
            { "Nodes", StageScenarioBase.nodes },
        };
        DataManager.dataManager.SetDocumentData(docDict, "Progress", GameManager.gameManager.Uid);
    }

    private IEnumerator MoveWaitCor()
    {
        if (transform.position.x > StageScenarioBase.stageCanvas.currentNode.transform.position.x)
        {
            StageScenarioBase.stageCanvas.characterInStage.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f,0f));
        }
        yield return StageScenarioBase.stageCanvas.CharacterMove(this);
        StageScenarioBase.stageCanvas.characterInStage.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        if (StageScenarioBase.stageCanvas.currentNode.imageName)
            StartCoroutine(GameManager.FadeUi(StageScenarioBase.stageCanvas.currentNode.imageName, 1f, false));
        if (StageScenarioBase.stageCanvas.currentNode.textName)
            StartCoroutine(GameManager.FadeUi(StageScenarioBase.stageCanvas.currentNode.textName, 1f, false));

        StageScenarioBase.stageCanvas.currentNode = this;

        buttonEnter.gameObject.SetActive(true);
        SetEnterAlpha(0f);
        imageName.gameObject.SetActive(true);

        // 두 개의 코루틴을 동시에 실행

        StartCoroutine(GraduallyAscendEnterAlpha());
        StartCoroutine(StageScenarioBase.stageCanvas.HideDeselectedEdgeNodeCor());
        GameManager.stageScenario.MoveMapVia(false);
        GameManager.stageScenario.ExtendVia(false);
        yield return new WaitForSeconds(1.5f);
        StageScenarioBase.state = StateInMap.NeedEnter;
    }
    public IEnumerator GraduallyAscendEnterAlpha()
    {
        float curAlpha = 0f;
        float totalTime = 0.5f;
        float elapsedTime = 0f;
        yield return new WaitForSeconds(1f);
        while (elapsedTime < totalTime)
        {
            curAlpha = Mathf.Lerp(0f, 1f, elapsedTime / totalTime);
            elapsedTime += Time.deltaTime;
            SetEnterAlpha(curAlpha);
            yield return null; // 한 프레임 대기
        }
        curAlpha = 1f;
        SetBaseAlpha(curAlpha);
    }
    private void SetEnterAlpha(float _alpha)
    {
        buttonEnter.color = new Color(1f, 1f, 1f, _alpha);
        textEnter.color = new Color(1f, 1f, 1f, _alpha);
    }
    public async void OnEnterButtonClick()
    {
        if (StageScenarioBase.state != StateInMap.NeedEnter)
            return;
        //StageScenarioBase.stageBaseCanvas.HideAndFadeOutDeselectedEdgeNodes();
        //StageScenarioBase.stageBaseCanvas.gameObject.SetActive(false);
        //buttonEnter.gameObject.SetActive(false);
        if (nodeType.backgroundType == BackgroundType.Store)
            LoadingScenario.LoadScene("Store");
        else
        {
            BattleScenario.currentBackground = nodeType.backgroundType;
            LoadingScenario.LoadScene("Battle");
        }
        GameManager.gameManager.destinationNum++;
        await DataManager.dataManager.SetDocumentData("DestinationNum", GameManager.gameManager.destinationNum, "Progress", GameManager.gameManager.Uid);
    }
    public void ActiveWithObject(bool _isAcitve)
    {
        buttonDot.gameObject.SetActive(_isAcitve);
        imageName.gameObject.SetActive(_isAcitve);
    }

    public void InitNode(int _index, int _arrayIndex, int _phaseNum, BackgroundType _backgroundType)
    {
        index = _index;
        arrayIndex = _arrayIndex;
        backGroundType = _backgroundType;

        phaseNum = _phaseNum;
        ActiveWithObject(false);
        buttonEnter.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    public void SetNodeType(NodeType _nodeType)
    {
        nodeType = _nodeType;
        if (_nodeType == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            imageObject.sprite = nodeType.objectSprite;
            imageObject.SetNativeSize();
            RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
            if (imageObject.sprite)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                // 위치 초기화
                rectTransform.anchoredPosition = Vector2.zero;
            }
            if (nodeType.backgroundType == BackgroundType.Store)
            {
                if (imageName)
                    imageName.color = storeColor;
            }
        }

    }
    public void SetNameText()
    {
        textName.text = nodeType.name[GameManager.language];
        RectTransform rectTransform = imageName.rectTransform;

        rectTransform.sizeDelta = new Vector2(Mathf.Max(180f, textName.preferredWidth*0.8f + 50) , rectTransform.sizeDelta.y);
        //Canvas.ForceUpdateCanvases();
    }
    public void OnPointerEnter()
    {
        if (nodeType==null||nodeType.backgroundType == BackgroundType.Store)
        {
            return;
        }
        switch (StageScenarioBase.state)
        {
            case StateInMap.NeedEnter:
                if (this == StageScenarioBase.stageCanvas.currentNode)
                    ActiveLootExplain();
                break;
            case StateInMap.NeedMove:
                if (StageScenarioBase.phase < phaseNum)
                    ActiveLootExplain();
                break;
        }
    }

    private void ActiveLootExplain()
    {
        LootExplain lootExplain = GameManager.stageScenario.lootExplain;

        ItemManager.GetLootTypesByBackgroundType(nodeType.backgroundType, out SkillCategori skillCategori, out ItemGrade skillGrade, out IngredientType ingredientType);
        lootExplain.SetExplain(skillCategori, skillGrade, ingredientType);
        lootExplain.transform.position = transform.position;
        float screenYProportion = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position).y / Screen.height;
        //Debug.Log(screenYProportion);
        if (screenYProportion > 0.64f)
            lootExplain.transform.localPosition = new Vector3(lootExplain.transform.localPosition.x, 100f, lootExplain.transform.localPosition.y);
        else
            lootExplain.transform.localPosition += new Vector3(0f, 20f);
        lootExplain.gameObject.SetActive(true);
    }

    public void OnPointerExit()
    {
        GameManager.stageScenario.lootExplain.gameObject.SetActive(false);
    }
}
