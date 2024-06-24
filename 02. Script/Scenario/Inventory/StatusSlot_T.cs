using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StatusSlot_T : MonoBehaviour
{
    public TMP_Text textValue;
    public void SetValue(float _value)
    {
        if (_value == 0)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
            textValue.text = _value.ToString();
        }
    }
}