using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using EnumCollection;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;
using MapCollection;

public class StageBaseCanvas : MonoBehaviour
{
    public DestinationNode currentNode;

    public Transform parentNode;
    public Transform parentEdge;

    public List<DestinationNode> allNodes = new();
    public List<DestinationNode>[] nodePhases = new List<DestinationNode>[6] { new(), new(), new(), new(), new(), new() };
    public DestinationNode nodePhase_Last;
    public DestinationNode startNode;
    public List<System.Tuple<int, int>> selectedEdgeTuple = new();

    public CharacterInMap characterInMap;
    public List<BackgroundType> nodeBackgroundTypes;
    Dictionary<System.Tuple<int, int>, Transform> fromToSemiparent = new();
    public GameObject smallDotPrefab;
    private int coroutineCount = 0;

    private List<Coroutine> coroutines = new List<Coroutine>();

    private void Awake()
    {
        smallDotPrefab = GameManager.gameManager.smallDotPrefab;
        startNode.phaseNum = -1;
        startNode.index = -1;
        AddToAllNodePhase();

        void AddToAllNodePhase()
        {
            int totalNodeNum = 0;
            for (int i = 0; i < nodePhases.Length; i++)
            {
                Transform parent = parentNode.GetChild(i);
                for (int j = 0; j < parent.childCount; j++)
                {
                   nodePhases[i].Add(parent.GetChild(j).GetComponent<DestinationNode>());
                }

                allNodes.AddRange(nodePhases[i]);
                Transform nodesPhase = parentNode.GetChild(i);
                for (int arrayIndex = 0; arrayIndex < nodesPhase.childCount; arrayIndex++)
                {
                    DestinationNode node = nodesPhase.GetChild(arrayIndex).GetComponent<DestinationNode>();
                    node.InitNode(totalNodeNum, arrayIndex, i, nodeBackgroundTypes[totalNodeNum]);
                    totalNodeNum++;
                }
            }
        }

    }



    public IEnumerator CharacterMove(DestinationNode _to)
    {
        System.Tuple<int, int> tuple = new(currentNode.index, _to.index);
        Transform semiParent = fromToSemiparent[tuple];
        selectedEdgeTuple.Add(tuple);
        var transforms = semiParent.GetComponentsInChildren<RectTransform>();
        GameManager.mapScenario.MoveCameraXVia(_to, false);
        yield return StartCoroutine(characterInMap.MoveToNewNode(transforms, _to.imageDot.GetComponent<RectTransform>()));

    }

    public void SetLoadedNode()
    {
        MapScenarioBase.state = StateInMap.NeedEnter;
        DestinationNode prevNode = startNode;
        List<object> nodes = MapScenarioBase.nodes;
        string[] nodeTypes = MapScenarioBase.nodeTypes;
        for (int i = 0; i < nodeTypes.Length; i++)
        {
            string nodeTypeStr = nodeTypes[i];
            NodeType nodeType;
            if (nodeTypeStr == null)
                nodeType = null;
            else
                nodeType = LoadManager.loadManager.nodeTypesDict[nodeTypeStr];
            allNodes[i].SetNodeType(nodeType);
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            parentNode.GetChild(i).gameObject.SetActive(true);
            List<int> arrayIndexes = new();
            if (nodes[i] is string v)
            {
                string[] splitted = v.Split(":::");
                foreach (var str in splitted)
                {
                    arrayIndexes.Add(int.Parse(str));
                }
            }
            else
            {
                arrayIndexes.Add((int)(long)nodes[i]);
            }
            if (arrayIndexes.Count == 1)
            {
                DestinationNode temp = SetNodeActive(i, arrayIndexes[0]);

                temp.ActiveWithObject(true);

                ConnectDots(prevNode, temp, 0.3f);
                selectedEdgeTuple.Add(new(prevNode.index, temp.index));
                if (prevNode.phaseNum != temp.phaseNum)
                    prevNode = temp;
                currentNode = temp;
            }
            else
            {
                MapScenarioBase.state = StateInMap.NeedMove;
                foreach (int arrayIndex in arrayIndexes)
                {
                    DestinationNode temp = SetNodeActive(i, arrayIndex);

                    temp.ActiveWithObject(true);
                    ConnectDots(prevNode, temp, 0.3f);
                }
            }
            
        }
        if (currentNode == null)
            currentNode = startNode;
        characterInMap.transform.position = currentNode.imageDot.transform.position;
        if (MapScenarioBase.state == StateInMap.NeedEnter)
        {
            currentNode.buttonEnter.gameObject.SetActive(true);
        }
       
        DestinationNode SetNodeActive(int _phaseNum, int _arrayIndex)
        {
            DestinationNode returnValue = null;
            List<DestinationNode> phaseNodes = nodePhases[_phaseNum];
            for (int i = 0; i < phaseNodes.Count; i++)
            {
                DestinationNode node = phaseNodes[i];
                node.gameObject.SetActive(true);
                if (_arrayIndex == i)
                {
                    returnValue = node;
                }
            }
            return returnValue;
        }
    }
    protected IEnumerator ConnectDotsCoroutine(DestinationNode startDot, DestinationNode endDot, float dotSpacing)
    {
        Vector3 startPosition = startDot.imageDot.transform.position;
        Vector3 endPosition = endDot.imageDot.transform.position;
        Vector3 direction = (endPosition - startPosition).normalized; // 방향 벡터
        float distance = Vector3.Distance(startPosition, endPosition);
        int numberOfDots = Mathf.FloorToInt(distance / dotSpacing);
        Transform semiParent = new GameObject($"From{startDot.index}_To{endDot.index}").transform;
        semiParent.SetParent(parentEdge);
        fromToSemiparent.Add(new(startDot.index, endDot.index), semiParent);
        float initialDelay = 0.5f;  // 최초 지연 시간
        float finalDelay = 0.1f;    // 최종 지연 시간
        for (int i = 1; i <= numberOfDots - 1; i++) // 마지막 점을 생략하기 위해 numberOfDots - 1 사용
        {
            Vector3 position = startPosition + direction * (i * dotSpacing);

            // 랜덤하게 약간의 변위를 추가
            Vector3 randomOffset = new(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0f);
            position += randomOffset;

            var obj = Instantiate(smallDotPrefab, position, Quaternion.identity, transform);
            obj.transform.SetParent(semiParent);

            float timeDelay = Mathf.Lerp(initialDelay, finalDelay, (float)i / (numberOfDots - 1)); // 시간 간격을 점진적으로 줄임
            yield return new WaitForSeconds(timeDelay); // 부드러운 시간 간격으로 점 생성
        }
        coroutineCount--;
    }
    protected void ConnectDots(DestinationNode startDot, DestinationNode endDot, float dotSpacing)
    {
        endDot.SetNameText();
        Vector3 startPosition = startDot.imageDot.transform.position;
        Vector3 endPosition = endDot.imageDot.transform.position;
        Vector3 direction = (endPosition - startPosition).normalized; // 방향 벡터
        float distance = Vector3.Distance(startPosition, endPosition);
        int numberOfDots = Mathf.FloorToInt(distance / dotSpacing);
        Transform semiParent = new GameObject($"From{startDot.index}_To{endDot.index}").transform;
        semiParent.SetParent(parentEdge);
        fromToSemiparent.Add(new(startDot.index, endDot.index), semiParent);

        for (int i = 1; i <= numberOfDots - 1; i++) // 마지막 점을 생략하기 위해 numberOfDots - 1 사용
        {
            Vector3 position = startPosition + direction * (i * dotSpacing);

            // 랜덤하게 약간의 변위를 추가
            Vector3 randomOffset = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0f);
            position += randomOffset;

            var obj = Instantiate(smallDotPrefab, position, Quaternion.identity, transform);
            obj.transform.SetParent(semiParent);
        }
    }
    public IEnumerator HideDeselectedEdgeNode()
    {
        List<System.Tuple<int, int>> allEdgeParent = fromToSemiparent.Keys.ToList();
        foreach (System.Tuple<int, int> x in selectedEdgeTuple)
        {
            allEdgeParent.Remove(x);
        }
        List<Image> targetImages = new();
        List<DestinationNode> targetNodes = new(nodePhases[MapScenarioBase.phase]);
        targetNodes.Remove(currentNode);
        foreach (System.Tuple<int, int> x in allEdgeParent)
        {
            Transform parent = fromToSemiparent[x];
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform edgeParent = parent.GetChild(i);
                targetImages.Add(edgeParent.GetComponent<Image>());
            }
        }
        yield return StartCoroutine(FadeOutImages(targetImages, targetNodes, 5.0f));
        foreach (System.Tuple<int, int> x in allEdgeParent)
        {
            Destroy(fromToSemiparent[x].gameObject);
            fromToSemiparent.Remove(x);
        }
    }
    private IEnumerator FadeOutImages(List<Image> _images, List<DestinationNode> _nodes, float _duration)
    {
        float currentTime = 0f;
        float alpha = 1f;
        while (currentTime < _duration)
        {
            alpha = Mathf.Lerp(alpha, 0, currentTime / _duration);
            foreach (Image img in _images)
            {
                if (img != null)
                {
                    Color currentColor = img.color;
                    img.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                }
            }
            foreach (var x in _nodes)
            {
                if (x)
                {
                    x.imageName.color = new Color(1f, 1f, 1f, alpha);
                    x.textName.color = new Color(1f, 1f, 1f, alpha);
                    x.imageDot.color = new Color(1f, 1f, 1f, alpha);
                    x.imageDotGradient.color = new Color(1f, 1f, 1f, alpha);
                }
            }
            currentTime += Time.deltaTime;
            yield return null;
        }

        // 마지막으로 모든 Image의 알파값을 확실히 0으로 설정
        foreach (Image img in _images)
        {
            if (img != null)
            {
                Color finalColor = img.color;
                img.color = new Color(finalColor.r, finalColor.g, finalColor.b, 0);
            }
        }
    }
    private IEnumerator WaitAllConnect(List<DestinationNode> _to)
    {
        coroutineCount = coroutines.Count;
        yield return new WaitUntil(() => coroutineCount <= 0);
        Debug.Log("All Connected");
        foreach (DestinationNode x in _to)
        {
            x.gameObject.SetActive(true);
            x.ActiveWithObject(true);
            StartCoroutine(x.GraduallyAscendBaseAlpha());
            x.SetNameText();
        }
        coroutines.Clear();

        if (MapScenarioBase.phase != 0)
        {
            GameManager.mapScenario.ExtendVia(false);
        }
        MapScenarioBase.state = StateInMap.NeedMove;
    }
    public void EnterPhase()
    {
        List<DestinationNode> to = nodePhases[MapScenarioBase.phase];

        parentNode.GetChild(MapScenarioBase.phase).gameObject.SetActive(true);
        List<DestinationNode> tempTo;
        switch (MapScenarioBase.phase)
        {
            default:
                tempTo = new();
                switch (currentNode.arrayIndex)
                {
                    case 0:
                    case 1:
                        tempTo.Add(to[0]);
                        tempTo.Add(to[1]);
                        tempTo.Add(to[2]);
                        break;
                    case 2:
                    case 3:
                        tempTo.Add(to[1]);
                        tempTo.Add(to[2]);
                        tempTo.Add(to[3]);
                        break;
                }
                int removeIndex = Random.Range(0, 3);
                tempTo[removeIndex].ActiveWithObject(false);
                tempTo.RemoveAt(removeIndex);
                break;
            case 0:
                tempTo = new(to);
                removeIndex = Random.Range(0, 3);
                tempTo[removeIndex].ActiveWithObject(false);
                tempTo.RemoveAt(removeIndex);
                break;
            case 5:
                tempTo = new(to);
                break;
        }
        string choiseNode = string.Empty;
        for (int i = 0; i < tempTo.Count; i++)
        {
            DestinationNode node = tempTo[i];
            coroutines.Add(StartCoroutine(ConnectDotsCoroutine(currentNode, node, 0.3f)));
            if (i == 0)
            {
                choiseNode = node.arrayIndex.ToString();
            }
            else
            {
                choiseNode += ":::" + node.arrayIndex;
            }
            List<KeyValuePair<string, NodeType>> kvps = LoadManager.loadManager.nodeTypesDict.Where(item =>item.Value.backgroundType == node.backGroundType).ToList();
            KeyValuePair<string, NodeType> selected = kvps[Random.Range(0, kvps.Count)];
            node.SetNodeType(selected.Value);
            MapScenarioBase.nodeTypes[node.index] = selected.Key;

        }
        MapScenarioBase.nodes.Add(choiseNode);
        
        Dictionary<string, object> docDict = new()
        {
            { "Nodes", MapScenarioBase.nodes },
            { "NodeTypes", MapScenarioBase.nodeTypes },
        };
        DataManager.dataManager.SetDocumentData(docDict, "Progress", GameManager.gameManager.Uid);
        StartCoroutine(WaitAllConnect(tempTo));
    }
}
