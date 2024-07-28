using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class UpgradeExplainUi : MonoBehaviour
{
    private TMP_Text textExplain;
    private TMP_Text textInfo;
    float widthCorrection = 100f;
    float heightCorrection = 100f;
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

        string currentText = (GameManager.language == EnumCollection.Language.Ko) ? "<b>현재</b>" : "Current";
        string nextText = (GameManager.language == EnumCollection.Language.Ko) ? "<b>다음</b>" : "Next";

        string curInfo = (_cur != string.Empty) ? $"{currentText} : {_cur}" : string.Empty;
        string nextInfo = (_next != string.Empty) ? $"{nextText} : {_next}" : string.Empty;

        textInfo.text = (curInfo != string.Empty) ? curInfo + "\n" + nextInfo : nextInfo;
    }
    public void SetSize()
    {
        float width = Mathf.Max(textInfo.preferredWidth + widthCorrection, 500f);
        float height = textExplain.preferredHeight + textInfo.preferredHeight + heightCorrection;
        GetComponent<RectTransform>().sizeDelta = new Vector3(width, height);
    }
}
