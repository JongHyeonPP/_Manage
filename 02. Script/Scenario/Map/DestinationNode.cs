using EnumCollection;
using StageCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestinationNode : MonoBehaviour
{
    public int index;
    public Image imageGradient;

    public Image imageName;
    public TMP_Text textName;

    public Image imageDot;
    public Image imageDotGradient;

    public Image imageObject;

    public Image buttonEnter;
    public TMP_Text textEnter;
    public int arrayIndex;
    public BackgroundType backGroundType;
    public int phaseNum;

    public NodeType nodeType;
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
        imageName.color = new Color(1f, 1f, 1f, _alpha);
        imageDot.color = new Color(1f, 1f, 1f, _alpha);
        imageDotGradient.color = new Color(1f, 1f, 1f, _alpha);
        imageGradient.color = new Color(1f, 1f, 1f, _alpha);
        imageObject.color = new Color(1f, 1f, 1f, _alpha);

        if (textName)
            textName.color = new Color(1f, 1f, 1f, _alpha);
    }

    public void OnNodeClicked()
    {
        if (StageScenarioBase.state != StateInMap.NeedMove)
            return;
        StageScenarioBase.state = StateInMap.None;
        GameManager.mapScenario.MoveCameraXVia(this, false);
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
        yield return StageScenarioBase.stageBaseCanvas.CharacterMove(this);

        StageScenarioBase.stageBaseCanvas.currentNode = this;

        buttonEnter.gameObject.SetActive(true);
        SetEnterAlpha(0f);
        imageName.gameObject.SetActive(true);

        // 두 개의 코루틴을 동시에 실행

        StartCoroutine(GraduallyAscendEnterAlpha());
        StartCoroutine(StageScenarioBase.stageBaseCanvas.HideDeselectedEdgeNodeCor());
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
            LoadingScenario.LoadScene("Battle");
        GameManager.gameManager.destinationNum++;
        await DataManager.dataManager.SetDocumentData("DestinationNum", GameManager.gameManager.destinationNum, "Progress", GameManager.gameManager.Uid);
    }
    public void ActiveWithObject(bool _isAcitve)
    {
        imageDotGradient.gameObject.SetActive(_isAcitve);
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
            RectTransform gradientRect = imageGradient.GetComponent<RectTransform>();
            gradientRect.sizeDelta = new(imageObject.sprite.rect.width, imageObject.sprite.rect.height);
        }
    }
    public void SetNameText() => textName.text = nodeType.name[GameManager.language];
}
