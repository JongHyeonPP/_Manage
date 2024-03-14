using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public VolumeType volumeType;
    public TMP_Text textNum;
    public Slider slider;
    public Image onImage;
    public Image offImage;
    public bool OnOff 
    { 
        get 
        {
            return onOff;
        }
        set 
        {
            onImage.enabled = value;
            offImage.enabled = !value;
            onOff = value;
        } 
    }
    private bool onOff;
    public void VolumeControl()
    {
        // 0���� 30������ ���������� ���� ���
        float ratio = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);

        // 0���� 100������ ������ Ȯ��
        float normalizedValue = Mathf.Lerp(0f, 100f, ratio);
        textNum.text = normalizedValue.ToString("F0");
        SettingManager.settingManager.VolumeControl(volumeType);
    }
    public void OnBtnClicked()
    {
        OnOff= !OnOff;
        SettingManager.settingManager.OnOffBtnClicked(volumeType, OnOff);
        Debug.Log(OnOff);

    }
}
