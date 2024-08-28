using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DepartUi : LobbyUiBase
{
    [SerializeField] TMP_Text textTitle;
    [SerializeField] TMP_Text textExplain;
    [SerializeField] TMP_Text textNo;
    [SerializeField] TMP_Text textYes;
    private void Awake()
    {
        OnLanguageChange();
        SettingManager.LanguageChangeEvent += OnLanguageChange;
    }
    public void DepartSelected()
    {
        GameManager.lobbyScenario.DepartAsync();

    }
    private void OnLanguageChange()
    {
        textTitle.text = GameManager.language == Language.Ko ? "���" : "Depart";
        textExplain.text = GameManager.language == Language.Ko ? "����Ͻðڽ��ϱ�?" : "Are you ready to depart?";
        textNo.text = GameManager.language == Language.Ko ? "����..." : "Not yet...";
        textYes.text = GameManager.language == Language.Ko ? "���!" : "Let's go!";
    }
}
