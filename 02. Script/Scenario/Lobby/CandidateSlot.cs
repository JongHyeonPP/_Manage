using BattleCollection;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using LobbyCollection;

public class CandidateSlot : MonoBehaviour
{

    private Transform transformCandidate;
    private Transform transformSelected;
    public GameObject objectButton;
    private GameObject friendlyObject;
    public CandiInfo candiInfo;
    private void Awake()
    {
        objectButton = transform.GetChild(0).gameObject;
        objectButton.SetActive(false);
        friendlyObject = Instantiate(Resources.Load<GameObject>("Prefab/Friendly/Friendly_000"));
        friendlyObject.transform.SetParent(transform);
        friendlyObject.transform.localScale = Vector3.one;
        friendlyObject.transform.localPosition = new Vector3(0f, -20f, 0f);
        friendlyObject.AddComponent<SortingGroup>().sortingOrder = 7;
        friendlyObject.transform.GetChild(0).GetComponent<Animator>().enabled = false;
    }
    private void Start()
    {
        transformCandidate = GameManager.lobbyScenario.panelRecruit.transform.GetChild(0);
        transformSelected = GameManager.lobbyScenario.panelRecruit.transform.GetChild(1);
    }
    public void OnCandidateClicked()
    {
        if (transform.parent == transformCandidate)
        {
            GameManager.lobbyScenario.OnCandidateClicked(this);
            
        }
        else
        {
            //선택->후보
            transform.SetParent(transformCandidate);
            transform.position = Vector3.zero;
            friendlyObject.transform.localScale = Vector3.one;
            friendlyObject.transform.localPosition = new Vector3(0f, -20f, 0f);
            GameManager.lobbyScenario.RefreshCandidate();
            GameManager.lobbyScenario.selected.Remove(this);
            GameManager.lobbyScenario.candidates.Add(this);
        }
            
    }
    public void OnBtnClicked()
    {
        //후보->선택
        if (GameManager.lobbyScenario.selected.Count == 3)
        {
            //더 이상 선택할 수 없다고 표시하기
        }
        else
        {
            transform.SetParent(transformSelected);
            transform.position = Vector3.zero;
            friendlyObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            friendlyObject.transform.localPosition = new Vector3(0f, -15f, 0f);
            objectButton.SetActive(false);
            GameManager.lobbyScenario.selected.Add(this);
            GameManager.lobbyScenario.candidates.Remove(this);
            GameManager.lobbyScenario.RefreshCandidate();
        }
    }
}