using DefaultCollection;
using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static SettingManager;
public class SettingUi : MonoBehaviour
{
    public SettingSet originSet;
    public SettingSet newSet;

    [SerializeField] TMP_Dropdown dropdownResolution;
    [SerializeField] TMP_Dropdown dropdownLanguage;
    [SerializeField] Toggle toggleFullScreen;
    [SerializeField] Transform parentSlider;
    Dictionary<VolumeType, VolumeSlider> volumeSliders = new();

    [SerializeField] TMP_Text textResolution;
    [SerializeField] TMP_Text textFullScreen;
    [SerializeField] TMP_Text textConvenience;
    [SerializeField] TMP_Text textSkillEffect;
    [SerializeField] TMP_Text textVolume;
    [SerializeField] TMP_Text textAll;
    [SerializeField] TMP_Text textSfx;
    [SerializeField] TMP_Text textBgm;
    [SerializeField] TMP_Text textLanguage;
    [SerializeField] TMP_Text textConfirm;
    [SerializeField] TMP_Text textCancel;
    public void InitSettingUi()
    {
        volumeSliders.Add(VolumeType.All, parentSlider.GetChild(0).GetComponent<VolumeSlider>());
        volumeSliders.Add(VolumeType.Sfx, parentSlider.GetChild(1).GetComponent<VolumeSlider>());
        volumeSliders.Add(VolumeType.Bgm, parentSlider.GetChild(2).GetComponent<VolumeSlider>());
        InitData();
        InitLanguage();
        InitScreen();
        InitSound();
    }

    public void ConnectLangaugeChange()
    {
        LanguageChangeEvent += OnLanguageChange;
    }
    public void LanguageOptionChange(int x)
    {
        switch (x)
        {
            default:
                GameManager.language = newSet.language = Language.Ko;
                break;
            case 1:
                GameManager.language = newSet.language = Language.En;
                break;
        }
        settingManager.ExecuteLangaugeChange();
    }
    public void ResolutionOptionChange(int _x)
    {
        Screen.SetResolution(resolutions[_x].width, resolutions[_x].height, newSet.fullScreenMode);
        newSet.resolutionIndex = _x;
    }
    private void OnLanguageChange()
    {
        textResolution.text = (GameManager.language == Language.Ko) ? "해상도" : "Resolution";
        textFullScreen.text = (GameManager.language == Language.Ko) ? "전체 화면" : "Full Screen";
        textConvenience.text = (GameManager.language == Language.Ko) ? "편의 기능" : "Convenience";
        textSkillEffect.text = (GameManager.language == Language.Ko) ? "스킬 이펙트" : "Skill Effect";
        textVolume.text = (GameManager.language == Language.Ko) ? "음향" : "Volume";
        textAll.text = (GameManager.language == Language.Ko) ? "전체" : "ALL";
        textSfx.text = (GameManager.language == Language.Ko) ? "효과음" : "SFX";
        textBgm.text = (GameManager.language == Language.Ko) ? "배경음" : "BGM";
        textLanguage.text = (GameManager.language == Language.Ko) ? "언어" : "Language";
        textConfirm.text = (GameManager.language == Language.Ko) ? "확인" : "Confirm";
        textCancel.text = (GameManager.language == Language.Ko) ? "취소" : "Cancel";
    }
    private void InitData()
    {
        Language originLanguage;
        Dictionary<VolumeType, float> originVolume = new();
        Dictionary<VolumeType, bool> originOnOff = new();
        FullScreenMode originFullScreenMode;
        int originResolutionIndex = 0;
        //Language
        try
        {
            string language = DataManager.dataManager.GetConfigData(DataSection.Language, "Language");
            switch (language)
            {
                case "En":
                    originLanguage = Language.En;
                    break;
                default:
                    originLanguage = Language.Ko;
                    break;
            }
        }
        catch
        {
            originLanguage = Language.Ko;
        }
        //Volume, OnOff
        foreach (VolumeType type in volumeTypes)
        {
            try
            {
                string value = DataManager.dataManager.GetConfigData(DataSection.SoundSetting, type + "Volume");
                originVolume.Add(type, float.Parse(value));

            }
            catch
            {
                originVolume.Add(type, 15f);
            }
            try
            {
                string value = DataManager.dataManager.GetConfigData(DataSection.SoundSetting, type + "OnOff");
                originOnOff.Add(type, bool.Parse(value));
            }
            catch
            {
                originOnOff.Add(type, true);
            }
        }
        //IsFullScreen
        try
        {
            string value = DataManager.dataManager.GetConfigData(DataSection.Screen, "FullScreenMode");
            switch (value)
            {
                default:
                    originFullScreenMode = FullScreenMode.Windowed;
                    break;
                case "FullScreenWindow":
                    originFullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
            }
        }
        catch
        {
            originFullScreenMode = FullScreenMode.Windowed;
        }
        //ResolutionIndex
        int curWidth = 0;
        int curHeight = 0;
        foreach (Resolution item in Screen.resolutions)
        {
            if (curWidth != item.width && curHeight != item.height)
            {
                curWidth = item.width;
                curHeight = item.height;
                resolutions.Add(item);
            }
        }
        try
        {
            string value = DataManager.dataManager.GetConfigData(DataSection.Screen, "ResolutionIndex");
            originResolutionIndex = int.Parse(value);
        }
        catch
        {

            int temp = 0;
            foreach (Resolution item in resolutions)
            {
                {
                    //옵션 세팅
                    TMP_Dropdown.OptionData option = new();
                    option.text = item.width + "x" + item.height;
                    //settingManager.dropdownResolution.options.Add(option);
                    //로컬 값이 없다면 기본 화면 해상도를 선택

                    if (item.width == Screen.width && item.height == Screen.height)
                    {
                        originResolutionIndex = temp;
                        break;
                    }
                    temp++;
                }
            }
        }
        //Set
        originSet = new(originLanguage, originVolume, originOnOff, originFullScreenMode, originResolutionIndex);
        newSet = originSet.GetDeepCopySet();
    }
    public void InitScreen()
    {

        dropdownResolution.options.Clear();
        foreach (Resolution resolution in resolutions)
        {
            TMP_Dropdown.OptionData option = new();
            option.text = resolution.width + "x" + resolution.height;
            dropdownResolution.options.Add(option);
        }
        dropdownResolution.RefreshShownValue();
        toggleFullScreen.isOn = originSet.fullScreenMode == FullScreenMode.FullScreenWindow;
        dropdownResolution.value = originSet.resolutionIndex;
    }
    public void InitSound()
    {
        foreach (VolumeType type in volumeTypes)
        {
            VolumeSlider volumeSlider = volumeSliders[type];

            float value = originSet.volume[type];
            volumeSlider.slider.value = value;
            bool onOff = originSet.onOff[type];
            
            volumeSlider.VolumeControl();
            volumeSlider.SetOnOff(onOff);
        }
    }
    public void InitLanguage()
    {
        GameManager.language = originSet.language;
        settingManager.ExecuteLangaugeChange();
        dropdownLanguage.value = (int)originSet.language;
    }
    public void FullScreenToggle()
    {
        bool _isFull = toggleFullScreen.isOn;
        FullScreenMode screenMode = _isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(Screen.width, Screen.height, screenMode);
        newSet.fullScreenMode = screenMode;
    }
    public void OnCancelBtnClick()
    {
        Screen.SetResolution(resolutions[originSet.resolutionIndex].width, resolutions[originSet.resolutionIndex].height, originSet.fullScreenMode);

        dropdownResolution.value = originSet.resolutionIndex;
        toggleFullScreen.isOn = originSet.fullScreenMode == FullScreenMode.FullScreenWindow;

        dropdownLanguage.value = (int)originSet.language;

        foreach (VolumeType type in volumeTypes)
        {
            VolumeSlider volumeSlider = volumeSliders[type];
            volumeSlider.slider.value = originSet.volume[type];
            volumeSlider.VolumeControl();
            volumeSlider.OnOffAtMasterMix(originSet.onOff[type]);
        }
        gameObject.SetActive(false);
        newSet = originSet.GetDeepCopySet();
    }
    public void ConfirmBtnClick()
    {
        DataManager.dataManager.SetConfigData(DataSection.Language, "Language", newSet.language);
        
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "AllVolume", newSet.volume[VolumeType.All]);
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "SfxVolume", newSet.volume[VolumeType.Sfx]);
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "BgmVolume", newSet.volume[VolumeType.Bgm]);

        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "AllOnOff", newSet.onOff[VolumeType.All]);
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "SfxOnOff", newSet.onOff[VolumeType.Sfx]);
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "BgmOnOff", newSet.onOff[VolumeType.Bgm]);

        DataManager.dataManager.SetConfigData(DataSection.Screen, "FullScreenMode", newSet.fullScreenMode);
        DataManager.dataManager.SetConfigData(DataSection.Screen, "ResolutionIndex", newSet.resolutionIndex);

        gameObject.SetActive(false);
        originSet = newSet.GetDeepCopySet();
    }
    internal void SetNewSet()
    {
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "AllVolume", newSet.volume[VolumeType.All]);
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "SfxVolume", newSet.volume[VolumeType.Sfx]);
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "BgmVolume", newSet.volume[VolumeType.Bgm]);

        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "AllOnOff", newSet.onOff[VolumeType.All]);
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "SfxOnOff", newSet.onOff[VolumeType.Sfx]);
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "BgmOnOff", newSet.onOff[VolumeType.Bgm]);

        DataManager.dataManager.SetConfigData(DataSection.Language, "Language", newSet.language);

        DataManager.dataManager.SetConfigData(DataSection.Screen, "FullScreenMode", newSet.fullScreenMode);
        DataManager.dataManager.SetConfigData(DataSection.Screen, "ResolutionIndex", newSet.resolutionIndex);

        originSet = newSet;
        newSet = null;
    }
}