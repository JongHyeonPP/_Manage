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
        textConent.text = (GameManager.language == Language.Ko) ? "진행 중이던 내용을 삭제하고\n새로 시작하시겠습니까?" : "Do you want to delete the current progress and start anew?";
        textYes.text = (GameManager.language == Language.Ko) ? "예" : "Yes";
        textNo.text = (GameManager.language == Language.Ko) ? "아니오" : "No";
        textTitle.text = (GameManager.language == Language.Ko) ? "새로하기" : "New Game";
    }
    private void OnDisable()
    {
        SoundManager.SfxPlay("PopThin");
    }
}
