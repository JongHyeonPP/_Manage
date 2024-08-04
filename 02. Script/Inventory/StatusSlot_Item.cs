using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StatusSlot_I : MonoBehaviour
{
    public TMP_Text textValue;
    public void SetValue(float _value)
    {
            gameObject.SetActive(true);
            textValue.text = _value.ToString();
    }
}