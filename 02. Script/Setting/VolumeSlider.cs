using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static SettingManager;
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
        string str;
        float shiftedValue = slider.value - slider.minValue;

        // 0부터 maxValue - minValue까지의 범위에서의 비율 계산
        float ratio = shiftedValue / (slider.maxValue - slider.minValue);

        // 0부터 100까지의 범위로 확장
        float normalizedValue = Mathf.Lerp(0f, 100f, ratio);

        textNum.text = normalizedValue.ToString("F0");

        switch (volumeType)
        {
            default:
                str = "Master";
                break;
            case VolumeType.Sfx:
                str = "SFX";
                break;
            case VolumeType.Bgm:
                str = "BGM";
                break;
        }
        float value;
        if (slider.value == -30f)
            value = -80f;
        else
            value = slider.value;
        if (OnOff)
            SoundManager.soundManager.masterMixer.SetFloat(str, value);

    }
    public void OnSliderValueChanged()
    {
        settingManager.settingUi.newSet.volume[volumeType] = slider.value;
        VolumeControl();
    }
    public void OnOnOffBtnClicked()
    {
        OnOff = !OnOff;
        settingManager.settingUi.newSet.onOff[volumeType] = onOff;
        OnOffAtMasterMix(OnOff);
    }
    public void SetOnOff(bool _isOn)
    {
        OnOff = _isOn;
        OnOffAtMasterMix(OnOff);
    }
    public void OnOffAtMasterMix(bool _onOff)
    {
        string str;
        switch (volumeType)
        {
            default:
                str = "Master";
                break;
            case VolumeType.Sfx:
                str = "SFX";
                break;
            case VolumeType.Bgm:
                str = "BGM";
                break;
        }
        if (_onOff)
        {
            if (slider.value == -30f)
                SoundManager.soundManager.masterMixer.SetFloat(str, -80f);
            else
                SoundManager.soundManager.masterMixer.SetFloat(str, slider.value);
        }
        else
            SoundManager.soundManager.masterMixer.SetFloat(str, -80f);
    }
}
