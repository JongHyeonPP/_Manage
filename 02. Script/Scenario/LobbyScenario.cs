using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScenario : MonoBehaviour
{
    public GameObject selectLight;
    public GameObject buttonNext;
    public List<GameObject> phaseList;
    private void Awake()
    {
        GameManager.lobbyScenario = this;
        selectLight.SetActive(false);
        phaseList[0].SetActive(true);
        phaseList[0].SetActive(true);
        phaseList[1].SetActive(false);
    }
    public void OnPointerClick(LobbyCase _lobbyCase)
    {
        Debug.Log(_lobbyCase + " Clicked");
        switch (_lobbyCase)
        {
            case LobbyCase.Pub:
                PubCase();
                break;
            case LobbyCase.Guild:
                GuildCase();
                break;
            case LobbyCase.Incruit:
                IncruitCase();
                break;
            case LobbyCase.Depart:
                DepartCase();
                break;
        }
    }
    private void PubCase()
    {
    
    }
    private void GuildCase()
    {
    
    }
    private void IncruitCase()
    {
    
    }
    private void DepartCase()
    {
    
    }
    public void NextPhase()
    {
        phaseList[0].SetActive(false);
        phaseList[1].SetActive(true);
        buttonNext.SetActive(false);
    }
}
