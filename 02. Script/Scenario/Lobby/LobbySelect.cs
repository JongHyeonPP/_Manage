using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySelect : MonoBehaviour
{
    public LobbyCase lobbyCase;
    private GameObject selectLight;
    private void Start()
    {
        if (!GameManager.lobbyScenario)
            return;
        selectLight = GameManager.lobbyScenario.selectLight;
    }
    public void OnPointerEnter()
    {
        if (!GameManager.lobbyScenario)
            return;
        selectLight.SetActive(true);
        selectLight.transform.SetParent(transform);
        selectLight.transform.localPosition = Vector3.zero;
    }
    public void OnPointerExit()
    {
        if (!GameManager.lobbyScenario)
            return;
        selectLight.SetActive(false);
    }
    public void OnPointerClick()
    {
        if (!GameManager.lobbyScenario)
            return;
        GameManager.lobbyScenario.OnPointerClick(lobbyCase);   
    }
}
