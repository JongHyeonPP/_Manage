using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScenario : MonoBehaviour
{
    public GameObject selectLight;
    private void Awake()
    {
        GameManager.lobbyScenario = this;
        selectLight.SetActive(false);
    }
    public void OnPointerClick(LobbyCase _lobbyCase)
    {
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
            case LobbyCase.Chest:
                ChestCase();
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
    private void ChestCase()
    {
    
    }
    private void DepartCase()
    {
    
    }
}
