using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public Dictionary<Language, string> typeStr;
    public TMP_Text typeText;
    public GameObject imageInactive;
    public Button button;
    public void SetStrOnLangauge(string _koStr, string _enStr)
    {
        typeStr = new()
        {
            { Language.Ko, _koStr },
            { Language.En, _enStr }
        };
    }
    public void TextChangeOnLanguage()
    {
        typeText.text = typeStr[GameManager.language];
    }
    public void InactiveButton()
    {
        imageInactive.SetActive(true);
        button.enabled = false;
    }
}
