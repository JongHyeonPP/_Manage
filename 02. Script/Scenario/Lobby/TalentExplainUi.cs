using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
public class TalentExplainUi : MonoBehaviour
{
    public RectTransform rectTransform;
    public TMP_Text textTitle;
    public TMP_Text textExplain;
    public void SetTalentExplain(string _title, string _explain)
    {
        textTitle.text = _title;
        textExplain.text = _explain;
        float width = Mathf.Max(textTitle.preferredWidth, textExplain.preferredWidth) + 100f;
        float height =  textExplain.preferredHeight + 120f;
        Canvas.ForceUpdateCanvases();
        rectTransform.sizeDelta = new(width, height);
    }
}