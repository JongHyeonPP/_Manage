
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public abstract class MapScenarioBase : MonoBehaviour
{
    public CharacterInMap characterInMap;

    public Volume volume;
    protected Vignette vignette;
    public DestinationNode startNode;
    public GameObject smallDotPrefab;
    public Canvas canvasMap;
    public DestinationNode currentNode;

    public Transform parentNode;
    public Transform parentEdge;

    protected List<DestinationNode> nodePhase_0 = new();
    protected List<DestinationNode> nodePhase_1 = new();
    protected List<DestinationNode> nodePhase_2 = new();
    protected List<DestinationNode> nodePhase_3 = new();
    protected List<DestinationNode> nodePhase_4 = new();
    protected List<DestinationNode> nodePhase_5 = new();
    protected DestinationNode nodePhase_Last;

    public int TotalNodeNum;
    public int phase;

    private List<Coroutine> coroutines = new List<Coroutine>();
    private Coroutine vignetteCoroutine;

    private int coroutineCount = 0;
    Dictionary<System.Tuple<int, int>, Transform> fromToSemiparent = new();
    public bool isMove;

    private void Awake()
    {
        currentNode = startNode;
        //smallDotPrefab = GameManager.gameManager.smallDotPrefab;
        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            // 초기 Intensity 값을 설정할 수 있습니다.
            vignette.intensity.value = 0.45f;
            
        }
        else
        {
            Debug.LogError("Vignette component not found in the volume profile.");
        }
        TotalNodeNum = 1;
        InitNode(nodePhase_0, 0);
        InitNode(nodePhase_1, 1);
        InitNode(nodePhase_2, 2);
        InitNode(nodePhase_3, 3);
        InitNode(nodePhase_4, 4);
        InitNode(nodePhase_5, 5);
        InitComposition();
         vignetteCoroutine =  NextVignette(1);
        EnterPhase(nodePhase_0);
        GameManager.mapScenario = this;
    }
    private void InitNode(List<DestinationNode> _nodes, int _phaseNum)
    {
        if (parentNode.childCount <= _phaseNum) return;
        Transform nodes = parentNode.GetChild(_phaseNum);
        for (int i = 0; i < nodes.childCount; i++)
        {
            DestinationNode node = nodes.GetChild(i).GetComponent<DestinationNode>();
            node.index = TotalNodeNum++;
            _nodes.Add(node);
            node.HideAlpha();
        }
    }
    protected IEnumerator OscillateVignetteIntensity(float minIntensity, float maxIntensity, float duration, float _smoothness)
    {
        vignette.smoothness.value = _smoothness;
        while (true)
        {
            yield return StartCoroutine(ChangeIntensity(minIntensity, maxIntensity, duration));
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(ChangeIntensity(maxIntensity, minIntensity, duration));
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator ChangeIntensity(float startIntensity, float endIntensity, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newIntensity = Mathf.Lerp(startIntensity, endIntensity, elapsedTime / duration);
            SetVignetteIntensity(newIntensity);
            yield return null;
        }
        SetVignetteIntensity(endIntensity);
    }
    public void SetVignetteIntensity(float intensity)
    {
        if (vignette != null)
        {
            vignette.intensity.value = intensity;
        }
    }
    protected IEnumerator ConnectDotsCoroutine(DestinationNode startDot, DestinationNode endDot, float dotSpacing)
    {
        Vector3 startPosition = startDot.nodeDot.position;
        Vector3 endPosition = endDot.nodeDot.position;
        Vector3 direction = (endPosition - startPosition).normalized; // 방향 벡터
        float distance = Vector3.Distance(startPosition, endPosition);
        int numberOfDots = Mathf.FloorToInt(distance / dotSpacing);
        Transform semiParent =  new GameObject($"From{startDot.index}_To{endDot.index}").transform;
        semiParent.SetParent(parentEdge);
        fromToSemiparent.Add(new(startDot.index, endDot.index), semiParent);
        for (int i = 1; i <= numberOfDots-1; i++) // 마지막 점을 생략하기 위해 numberOfDots - 1 사용
        {
            Vector3 position = startPosition + direction * (i * dotSpacing);

            // 랜덤하게 약간의 변위를 추가
            Vector3 randomOffset = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0f);
            position += randomOffset;

            var obj = Instantiate(smallDotPrefab, position, Quaternion.identity, canvasMap.transform);
            obj.transform.SetParent(semiParent);
            yield return new WaitForSeconds(0.3f); // 점을 생성하는 간격
        }
        coroutineCount--;
    }
    protected void EnterPhase( List<DestinationNode> _to)
    {
        phase++;
        for (int i = 0; i < _to.Count; i++)
        {
            DestinationNode node = _to[i];
            coroutines.Add(StartCoroutine(ConnectDotsCoroutine(currentNode, node, 0.3f)));
        }
        StartCoroutine(WaitAllConnect(_to));

    }

    private IEnumerator WaitAllConnect(List<DestinationNode> _to)
    {
        coroutineCount = coroutines.Count;
        yield return new WaitUntil(() => coroutineCount <= 0);
        Debug.Log("All Connected");
        foreach (var x in _to)
        {
            StartCoroutine(x.GraduallyAscendAlpha());
        }
        coroutines.Clear();
        if (phase != 1)
        {
            StopCoroutine(vignetteCoroutine);
            yield return new WaitForSeconds(1f);
            //확장
            vignetteCoroutine = NextVignette(phase);
        }
    }
    public void CharacterMove(DestinationNode _to)
    {
        Transform semiParent = fromToSemiparent[new(currentNode.index, _to.index)];
        var transforms = semiParent.GetComponentsInChildren<RectTransform>();
        StartCoroutine( characterInMap.MoveToNewNode(transforms, _to.nodeDot.GetComponent<RectTransform>()));
    }
    protected abstract void InitComposition();
    protected abstract Coroutine NextVignette(int _phase);
    
}

