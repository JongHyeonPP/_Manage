using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class UpgradeExplainUi : MonoBehaviour
{
    [SerializeField] TMP_Text textExplain;
    [SerializeField] TMP_Text textInfo;
    [SerializeField] Transform infoParent;
    float widthCorrection = 100f;
    float heightCorrection = 110f;
    public RectTransform rectTransform;
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
        Canvas.ForceUpdateCanvases();
        float height = textExplain.preferredHeight + textInfo.preferredHeight + heightCorrection;
        textInfo.transform.localPosition = new Vector3(0f,Mathf.Min(-50f, -textExplain.preferredHeight), 0f);
        GetComponent<RectTransform>().sizeDelta = new Vector3(750f, Mathf.Max(230f, height));
    }
}
