using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelNewGame : MonoBehaviour
{
    public TMP_Text textConent;
    public TMP_Text textYes;
    public TMP_Text textNo;
    public TMP_Text textTitle;
    private void Awake()
    {
        SetTextByLanguage();
        SettingManager.LanguageChangeEvent += SetTextByLanguage;
    }
    public void OnYesButtonClick()
    {
        SoundManager.SfxPlay("PopThin");
        GameManager.startScenario.NewGame();
    }
    public void OnNoButtonClick()
    {
        SoundManager.SfxPlay("PopThin");
        gameObject.SetActive(false);
    }
    private void SetTextByLanguage()
    {
        textConent.text = (GameManager.language == Language.Ko) ? "���� ���̴� ������ �����ϰ�\n���� �����Ͻðڽ��ϱ�?" : "Do you want to delete the current progress and start anew?";
        textYes.text = (GameManager.language == Language.Ko) ? "��" : "Yes";
        textNo.text = (GameManager.language == Language.Ko) ? "�ƴϿ�" : "No";
        textTitle.text = (GameManager.language == Language.Ko) ? "�����ϱ�" : "New Game";
    }
    private void OnDisable()
    {
        SoundManager.SfxPlay("PopThin");
    }
}
