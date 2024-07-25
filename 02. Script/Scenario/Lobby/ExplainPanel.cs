using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class ExplainPanel : MonoBehaviour
{
    private TMP_Text textExplain;
    private TMP_Text textInfo;
    float rectCorrection = 100f;
    public RectTransform rectTransform;
    void Awake()
    {
        textExplain = transform.GetChild(0).GetComponent<TMP_Text>();
        textInfo = transform.GetChild(1).GetComponent<TMP_Text>();
    }
    public void SetExplain(string _explain)
    {
        textExplain.text = _explain;
    }
    public void SetInfo(string _cur, string _next)
    {

        string currentText = (GameManager.language == EnumCollection.Language.Ko) ? "<b>����</b>" : "Current";
        string nextText = (GameManager.language == EnumCollection.Language.Ko) ? "<b>����</b>" : "Next";

        string curInfo = (_cur != string.Empty) ? $"{currentText} : {_cur}" : string.Empty;
        string nextInfo = (_next != string.Empty) ? $"{nextText} : {_next}" : string.Empty;

        textInfo.text = (curInfo != string.Empty) ? curInfo + "\n" + nextInfo : nextInfo;
    }
    public void SetSize()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector3(rectTransform.sizeDelta.x, textExplain.preferredHeight + textInfo.preferredHeight + rectCorrection);
    }

    internal void SetTalentExplain(string _explain)
    {
        textInfo.gameObject.SetActive(false);
        textExplain.text = _explain;
    }
}
