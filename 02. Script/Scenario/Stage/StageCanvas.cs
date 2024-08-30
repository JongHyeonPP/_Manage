using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using EnumCollection;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;
using StageCollection;
using UnityEngine.SceneManagement;

public class StageCanvas : MonoBehaviour
{
    public Transform panelMap;
    public DestinationNode startNode;
    public Transform parentNode;
    public Transform parentEdge;

    public DestinationNode currentNode;
    public List<DestinationNode> allNodes = new();
    public List<DestinationNode>[] nodePhases = new List<DestinationNode>[6] { new(), new(), new(), new(), new(), new() };
    public DestinationNode nodePhase_Last;

    public List<System.Tuple<int, int>> selectedEdgeTuple = new();

    public CharacterInStage characterInStage;
    public List<BackgroundType> nodeBackgroundTypes;
    Dictionary<System.Tuple<int, int>, Transform> fromToSemiparent = new();
    private GameObject smallDotPrefab;
    private int coroutineCount = 0;

    private List<Coroutine> coroutines = new List<Coroutine>();
    public int stageNum;
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
    private void Start()
    {
        if (GameManager.gameManager.characterList[0])
            characterInStage.characterHierarchy.CopyHierarchySprite(GameManager.gameManager.characterList[0].characterHierarchy);
    }


    public IEnumerator CharacterMove(DestinationNode _to)
    {
        System.Tuple<int, int> tuple = new(currentNode.index, _to.index);
        Transform semiParent = fromToSemiparent[tuple];
        selectedEdgeTuple.Add(tuple);
        List<Vector3> positions = semiParent.GetComponentsInChildren<Transform>().Where(item => item != semiParent).Select(item => item.position).ToList();
        positions.Add(_to.buttonDot.transform.position);
        yield return StartCoroutine(characterInStage.MoveToNewNode(positions));
    
    }

    public void SetLoadedNode()
    {
        DestinationNode prevNode = startNode;
        List<object> nodes = StageScenarioBase.nodes;
        string[] nodeTypes = StageScenarioBase.nodeTypes;
        //StageScenarioBase.state = StateInMap.NeedEnter;
        for (int i = 0; i < nodeTypes.Length; i++)
        {
            string nodeTypeStr = nodeTypes[i];
            NodeType nodeType;
            if (nodeTypeStr == null)
                nodeType = null;
            else
            {
                nodeType = LoadManager.loadManager.nodeTypesDict[nodeTypeStr];

            }
            allNodes[i].SetNodeType(nodeType);
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == null)
            {
                nodes.Remove(i);
                StageScenarioBase.state = StateInMap.NeedPhase;
                continue;
            }

            //parentNode.GetChild(i).gameObject.SetActive(true);
            List<int> arrayIndexes = new();
            bool isStrSetted = false;
            if (nodes[i] is string v)
            {
                if (true)
                {
                    string[] splitted = v.Split(":::");
                    foreach (var str in splitted)
                    {
                        arrayIndexes.Add(int.Parse(str));
                    }
                }
                isStrSetted = true;
            }
            else
            {
                arrayIndexes.Add((int)(long)nodes[i]);
            }
            if (isStrSetted)
            {
                StageScenarioBase.state = StateInMap.NeedMove;
                foreach (int arrayIndex in arrayIndexes)
                {
                    DestinationNode temp = SetNodeActive(i, arrayIndex);
                    if (i == nodes.Count - 1)
                    {
                            temp.imageTrigger.SetActive(true);
                    }
                    temp.ActiveWithObject(true);
                    List<GameObject> dots = ConnectDots(prevNode, temp);
                    foreach (var x in dots)
                        x.SetActive(true);
                }
            }
            else
            {

                DestinationNode temp = SetNodeActive(i, arrayIndexes[0]);
                if (i == nodes.Count - 1)
                {
                        temp.imageTrigger.SetActive(true);
                }
                temp.ActiveWithObject(true);
                if (prevNode.imageName)
                    prevNode.imageName.gameObject.SetActive(false);
                List<GameObject> dots = ConnectDots(prevNode, temp);
                foreach (var x in dots)
                    x.SetActive(true);
                selectedEdgeTuple.Add(new(prevNode.index, temp.index));
                if (prevNode.phaseNum != temp.phaseNum)
                    prevNode = temp;
                currentNode = temp;
            }

        }

        if (StageScenarioBase.state == StateInMap.NeedEnter)
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
                if (_arrayIndex == i)
                {
                    returnValue = node;
                }
            }
            returnValue.gameObject.SetActive(true);
            return returnValue;
        }
    }

    public void SetCharacterToCurrentNode()
    {
        if (currentNode == null)
            currentNode = startNode;
        characterInStage.transform.position = currentNode.buttonDot.transform.position;
    }

    protected IEnumerator ConnectDotsCoroutine(List<GameObject> dotList)
    {
        int numberOfDots = dotList.Count;
        float initialDelay = 0.2f;
        float finalDelay = 0.3f;
        for (int i = 0; i < numberOfDots; i++)
        {
            float timeDelay = Mathf.Lerp(initialDelay, finalDelay, (float)i / (numberOfDots - 1)); // �ð� ������ ���������� ����
            dotList[i].SetActive(true); // ���� Ȱ��ȭ
            yield return new WaitForSeconds(timeDelay); // �ε巯�� �ð� �������� �� ����
        }

        coroutineCount--;
    }
    protected List<GameObject> ConnectDots(DestinationNode startDot, DestinationNode endDot)
    {
        float dotSpacing = 0.3f;
        endDot.SetNameText();
        Vector3 startPosition = startDot.buttonDot.transform.position;
        Vector3 endPosition = endDot.buttonDot.transform.position;
        Vector3 direction = (endPosition - startPosition).normalized; // ���� ����
        float distance = Vector3.Distance(startPosition, endPosition);
        int numberOfDots = Mathf.FloorToInt(distance / dotSpacing);
        Transform semiParent = new GameObject($"From{startDot.index}_To{endDot.index}").transform;
        semiParent.SetParent(parentEdge);
        semiParent.localScale = Vector3.one;
        semiParent.localPosition = Vector3.zero;
        fromToSemiparent.Add(new(startDot.index, endDot.index), semiParent);

        List<GameObject> dotList = new List<GameObject>();

        for (int i = 1; i <= numberOfDots - 1; i++) // ������ ���� �����ϱ� ���� numberOfDots - 1 ���
        {
            Vector3 position = startPosition + direction * (i * dotSpacing);

            // �����ϰ� �ణ�� ������ �߰�
            Vector3 randomOffset = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0f);
            position += randomOffset;

            var obj = Instantiate(smallDotPrefab, position, Quaternion.identity, transform);
            obj.transform.SetParent(semiParent);
            obj.SetActive(false); // ó������ ��Ȱ��ȭ
            dotList.Add(obj); // ����Ʈ�� �߰�
        }

        return dotList; // �� ����Ʈ ��ȯ
    }
    public IEnumerator HideDeselectedEdgeNodeCor()
    {
        List<System.Tuple<int, int>> allEdgeParent = fromToSemiparent.Keys.ToList();
        foreach (System.Tuple<int, int> x in selectedEdgeTuple)
        {
            allEdgeParent.Remove(x);
        }
        List<Image> targetImages = new();
        List<DestinationNode> targetNodes = new(nodePhases[StageScenarioBase.phase]);
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
        yield return StartCoroutine(FadeOutImages(targetImages, targetNodes, 1.0f));
        foreach (System.Tuple<int, int> x in allEdgeParent)
        {
            Destroy(fromToSemiparent[x].gameObject);
            fromToSemiparent.Remove(x);
        }
        foreach (var x in targetNodes)
        {
            x.imageName.gameObject.SetActive(false);
        }
    }
    private IEnumerator FadeOutImages(List<Image> _images, List<DestinationNode> _nodes, float _duration)
    {
        float currentTime = 0f;
        float alpha = 1f;

        while (currentTime < _duration)
        {
            alpha = Mathf.Lerp(1f, 0f, currentTime / _duration);

            foreach (Image img in _images)
            {
                if (img != null)
                {
                    Color currentColor = img.color;
                    img.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                }
            }

            foreach (DestinationNode x in _nodes)
            {
                if (x)
                {
                    // imageName�� ���� ���� 0.7 * alpha�� ����
                    Color imageNameColor = x.imageName.color;
                    x.imageName.color = new Color(imageNameColor.r, imageNameColor.g, imageNameColor.b, alpha * 0.7f);

                    // textName�� ���� ���� alpha�� ����
                    Color textNameColor = x.textName.color;
                    x.textName.color = new Color(textNameColor.r, textNameColor.g, textNameColor.b, alpha);

                    // buttonDot�� ���� ���� alpha�� ����
                    Color buttonDotColor = x.buttonDot.color;
                    x.buttonDot.color = new Color(buttonDotColor.r, buttonDotColor.g, buttonDotColor.b, alpha);
                }
            }

            currentTime += Time.deltaTime;
            yield return null;
        }
    }


    public void HideAndFadeOutDeselectedEdgeNodes()
    {
        // Step 1: Get all edge parent tuples and remove selected ones
        List<System.Tuple<int, int>> allEdgeParent = fromToSemiparent.Keys.ToList();
        foreach (System.Tuple<int, int> x in selectedEdgeTuple)
        {
            allEdgeParent.Remove(x);
        }

        // Step 2: Prepare the list of target images and nodes
        List<Image> targetImages = new();
        List<DestinationNode> targetNodes = new(nodePhases[StageScenarioBase.phase]);
        targetNodes.Remove(currentNode);

        foreach (System.Tuple<int, int> x in allEdgeParent)
        {
            Transform parent = fromToSemiparent[x];
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform edgeParent = parent.GetChild(i);
                Image img = edgeParent.GetComponent<Image>();
                if (img != null)
                {
                    targetImages.Add(img);
                }
            }
        }

        // Step 3: Set alpha of all target images and nodes to 0 (immediate fade out)
        foreach (Image img in targetImages)
        {
            Color currentColor = img.color;
            img.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
        }

        foreach (var node in targetNodes)
        {
            if (node)
            {
                node.imageName.color = new Color(1f, 1f, 1f, 0);
                node.textName.color = new Color(1f, 1f, 1f, 0);
                node.buttonDot.color = new Color(1f, 1f, 1f, 0);
            }
        }

        // Step 4: Destroy all edge parent objects and remove from dictionary
        foreach (System.Tuple<int, int> x in allEdgeParent)
        {
            Destroy(fromToSemiparent[x].gameObject);
            fromToSemiparent.Remove(x);
        }

        // Step 5: Deactivate node images and gradients
        foreach (var node in targetNodes)
        {
            node.imageName.gameObject.SetActive(false);
        }
    }
    public void RemoveDeselectedEdge()
    {

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
        StageScenarioBase.state = StateInMap.NeedMove;
    }
    public void NextDestination()
    {

        List<DestinationNode> to = nodePhases[StageScenarioBase.phase+1];

        parentNode.GetChild(StageScenarioBase.phase+1).gameObject.SetActive(true);
        List<DestinationNode> tempTo;
        switch (StageScenarioBase.phase)
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
                tempTo[removeIndex].gameObject.SetActive(false);
                tempTo.RemoveAt(removeIndex);
                break;
            case -1:
                tempTo = new(to);
                removeIndex = Random.Range(0, 3);
                tempTo[removeIndex].gameObject.SetActive(false);
                tempTo.RemoveAt(removeIndex);
                break;
            case 4:
                tempTo = new(to);
                break;

        }
        string choiseNode = string.Empty;
        bool isStore = false;
        if (StageScenarioBase.phase != 4)
            if (currentNode.nodeType != null)
                if (currentNode.nodeType.backgroundType != BackgroundType.Store)
                    isStore = GameManager.CalculateProbability(1f);
        int storeIndex = -1;
        if (isStore)
        {
            storeIndex = Random.Range(0, tempTo.Count);
        }
        for (int i = 0; i < tempTo.Count; i++)
        {

            DestinationNode node = tempTo[i];
            List<KeyValuePair<string, NodeType>> kvps;
            node.imageTrigger.SetActive(true);
            if (i == storeIndex)
            {
                kvps = LoadManager.loadManager.nodeTypesDict.Where(item => item.Value.backgroundType == BackgroundType.Store).ToList();
            }
            else
            {

                kvps = LoadManager.loadManager.nodeTypesDict.Where(item => item.Value.backgroundType == node.backGroundType).ToList();
            }
            KeyValuePair<string, NodeType> selected = kvps[Random.Range(0, kvps.Count)];
            node.SetNodeType(selected.Value);
            StageScenarioBase.nodeTypes[node.index] = selected.Key;
            node.SetBaseAlpha(0f);
            List<GameObject> dotObjects = ConnectDots(currentNode, node);
            coroutines.Add(StartCoroutine(ConnectDotsCoroutine(dotObjects)));
            if (i == 0)
            {
                choiseNode = node.arrayIndex.ToString();
            }
            else
            {
                choiseNode += ":::" + node.arrayIndex;
            }


            //bool isStore = false;


        }
        StageScenarioBase.nodes.Add(choiseNode);
        
        Dictionary<string, object> docDict = new()
        {
            { "Nodes", StageScenarioBase.nodes },
            { "NodeTypes", StageScenarioBase.nodeTypes },
        };
        DataManager.dataManager.SetDocumentData(docDict, "Progress", GameManager.gameManager.uid);
        StartCoroutine(WaitAllConnect(tempTo));
    }
    public IEnumerator MoveMapCoroutine(float targetX, float targetY, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = panelMap. transform.localPosition;

        while (elapsedTime < duration)
        {
            // �ð� ����� ���� ���� ���
            float t = elapsedTime / duration;

            // Lerp�� ����Ͽ� ���������� x��ǥ ����
            float newX = Mathf.Lerp(startPos.x, targetX, t);
            float newY = Mathf.Lerp(startPos.y, targetY, t);
            // ���ο� ��ġ�� �̵�
            panelMap.transform.localPosition = new Vector3(newX, newY);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���� ��ġ ����
        panelMap.transform.localPosition = new Vector3(targetX, targetY);
    }
}
