using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StatusPanel : MonoBehaviour
{
    public TMP_Text textValue;
    private void Awake()
    {
        textValue.text = "-";
    }
}
