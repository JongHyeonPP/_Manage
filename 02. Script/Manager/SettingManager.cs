using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using EnumCollection;
using System;
using BattleCollection;

public class SettingManager : MonoBehaviour
{
    public delegate void OnLanguageChange(Language language);
    public static event OnLanguageChange onLanguageChange;

    public static SettingManager settingManager;
    internal SettingClass settingClass;
    public TMP_Dropdown dropdownResolution;
    public TMP_Dropdown dropdownLanguage;
    public Toggle toggleFullScreen;
    public GameObject panelSetting;
    public GameObject buttonSetting;
    private Dictionary<TMP_Text, Dictionary<Language, string>> texts;
    TMP_Text 
        textResolution, 
        textFullScreen, 
        textConvenience, 
        textQuickBattle, 
        textSound, 
        textAll, 
        textSfx, 
        textBgm, 
        textLanguage, 
        textConfirm, 
        textCancel;
    
    private void Awake()
    {
        if (!settingManager)
        {
            settingClass = new SettingClass();
            settingManager = this;
            DontDestroyOnLoad(GameObject.FindWithTag("CANVASSETTING"));

            panelSetting.SetActive(false);
            onLanguageChange += LanguageChange;
            //UI초기화
            textResolution = panelSetting.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            textFullScreen = panelSetting.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetComponent<TMP_Text>();
            textConvenience = panelSetting.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
            textQuickBattle = panelSetting.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<TMP_Text>();
            textSound = panelSetting.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            textAll = panelSetting.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
            textSfx = panelSetting.transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_Text>();
            textBgm = panelSetting.transform.GetChild(1).GetChild(0).GetChild(3).GetChild(0).GetComponent<TMP_Text>();
            textLanguage = panelSetting.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
            textConfirm = panelSetting.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
            textCancel = panelSetting.transform.GetChild(3).GetChild(0).GetComponent<TMP_Text>();
            //텍스트 초기화
            texts =
                new()
                {
                    {
                        textResolution,
                        new()
                        {
                            { Language.Ko, "해상도" },
                            { Language.En, "Start" }
                        }
                    },
                    {
                        textFullScreen,
                        new()
                        {
                            { Language.Ko, "전체 화면" },
                            { Language.En, "Full Screen" }
                        }
                    },
                    {
                        textConvenience,
                        new()
                        {
                            { Language.Ko, "편의 기능" },
                            { Language.En, "Convenience" }
                        }
                    },
                    {
                        textQuickBattle,
                        new()
                        {
                            { Language.Ko, "빠른 전투" },
                            { Language.En, "Quick Battle" }
                        }
                    },
                    {
                        textSound,
                        new()
                        {
                            { Language.Ko, "음향" },
                            { Language.En, "Sound" }
                        }
                    },
                    {
                        textAll,
                        new()
                        {
                            { Language.Ko, "전체" },
                            { Language.En, "ALL" }
                        }
                    },
                    {
                        textSfx,
                        new()
                        {
                            { Language.Ko, "효과음" },
                            { Language.En, "SFX" }
                        }
                    },
                    {
                        textBgm,
                        new()
                        {
                            { Language.Ko, "배경음" },
                            { Language.En, "BGM" }
                        }
                    },
                    {
                        textLanguage,
                        new()
                        {
                            { Language.Ko, "언어" },
                            { Language.En, "Language" }
                        }
                    },
                    {
                        textConfirm,
                        new()
                        {
                            { Language.Ko, "확인" },
                            { Language.En, "Confirm" }
                        }
                    },
                    {
                        textCancel,
                        new()
                        {
                            { Language.Ko, "취소" },
                            { Language.En, "Cancel" }
                        }
                    }
                };


        }
    }
    private void LanguageChange(Language _language)
    {
        foreach (KeyValuePair<TMP_Text, Dictionary<Language, string>> keyValue in texts)
        {
            keyValue.Key.text = keyValue.Value[_language];
        }
    }
    private void Start()
    {
        settingClass.InitSettingClass();
    }
    public void ResolutionOptionChange(int x)
    {
        settingClass.ResolutionOptionChange(x);
    }
    public void LanguageOptionChange(int x)
    {
        settingClass.LanguageOptionChange((Language)x);
    }
    public void FullScreenToggle(bool isFull)
    {
        settingClass.FullScreenToggle(isFull);
    }
    public void ConfirmBtnClick()
    {
        settingClass.ConfirmBtnClick();
    }
    public void CancelBtnClick()
    {
        settingClass.CancelBtnClick();
    }
    public void SettingBtnClick()
    {
        settingClass.SettingBtnClick();
    }
    public void ChangeSound(EVolume eVolume, float volume)
    {
        switch (eVolume)
        {
            case EVolume.All:
                settingClass.newSet.allVolume = volume;
                break;
            case EVolume.Sfx:
                settingClass.newSet.sfxVolume = volume;
                break;
            case EVolume.Bgm:
                settingClass.newSet.bgmVolume = volume;
                break;
        }
    }
    public void ExecuteLangaugeChange(Language _language) => onLanguageChange(_language);
}

internal class SettingClass
{
    List<Resolution> resolutions = new();
    internal SettingStruct originSet;
    internal SettingStruct newSet;
    GameObject panelSetting;
    int originResolution;
    int newResolution;
    FullScreenMode originScreenMode;
    internal void InitSettingClass()
    {
        InitData();
        InitScreen();
        InitSound();
        InitLanguage();
        panelSetting = SettingManager.settingManager.panelSetting;
    }
    private void InitData()
    {
        originSet = new SettingStruct();
        newSet = new SettingStruct();
        #region Volume
        try
        {
            originSet.allVolume = float.Parse(DataManager.dataManager.GetConfigData(DataSection.SoundSetting, "AllVolume"));
        }
        catch
        {
            originSet.allVolume = -1;
        }
        try
        {
            originSet.sfxVolume = float.Parse(DataManager.dataManager.GetConfigData(DataSection.SoundSetting, "SfxVolume"));
        }
        catch
        {
            originSet.sfxVolume = -1;
        }
        try
        {
            originSet.bgmVolume = float.Parse(DataManager.dataManager.GetConfigData(DataSection.SoundSetting, "BgmVolume"));
        }
        catch
        {
            originSet.bgmVolume = -1;
        }
        #endregion
        try
        {
            string temp = DataManager.dataManager.GetConfigData(DataSection.Language, "Language");
            switch (temp)
            {
                case "En":
                    originSet.language = Language.En;
                    break;
                default:
                    originSet.language = Language.Ko;
                    break;
            }
        }
        catch
        {
            originSet.language = Language.Ko;
        }
        DeepCopy();
    }

    private void DeepCopy()
    {
        newSet.sfxVolume = originSet.sfxVolume;
        newSet.bgmVolume = originSet.bgmVolume;
        newSet.language = originSet.language;
    }
    private void InitScreen()
    {
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
        SettingManager.settingManager.dropdownResolution.options.Clear();

        int temp = 0;
        foreach (Resolution item in resolutions)
        {
            {
                //옵션 세팅
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = item.width + "x" + item.height;
                SettingManager.settingManager.dropdownResolution.options.Add(option);
                //로컬 값이 없다면 기본 화면 해상도를 선택

                if (item.width == Screen.width && item.height == Screen.height)
                {
                    SettingManager.settingManager.dropdownResolution.value = originResolution=temp;
                }
                temp++;
            }
        }
        SettingManager.settingManager.dropdownResolution.RefreshShownValue();
        SettingManager.settingManager.toggleFullScreen.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow);
        originScreenMode = Screen.fullScreenMode;
    }
    private void InitSound()
    {
        if (originSet.allVolume == -1)
            SoundManager.soundManager.sliderAll.value = -15f;
        else
            SoundManager.soundManager.sliderAll.value = originSet.allVolume;
        if (originSet.sfxVolume == -1)
            SoundManager.soundManager.sliderSfx.value = -15f;
        else
            SoundManager.soundManager.sliderSfx.value = originSet.sfxVolume;

        if (originSet.bgmVolume == -1)
            SoundManager.soundManager.sliderBgm.value = -15f;
        else
            SoundManager.soundManager.sliderBgm.value = originSet.bgmVolume;
    }
    private void InitLanguage()
    {
        SettingManager.settingManager.ExecuteLangaugeChange(originSet.language);
        SettingManager.settingManager.dropdownLanguage.value = (int)originSet.language;
    }
    internal void ResolutionOptionChange(int _x)
    {
        Screen.SetResolution(resolutions[_x].width, resolutions[_x].height, originScreenMode);
        newResolution = _x;
    }
    internal void LanguageOptionChange(Language _language)
    {
        GameManager.language = _language;
        SettingManager.settingManager.ExecuteLangaugeChange(_language);
        newSet.language = _language;
    }
    internal void FullScreenToggle(bool _isFull)
    {
        Screen.SetResolution(Screen.width, Screen.height, _isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
    }
    internal void ConfirmBtnClick()
    {
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "AllVolume", newSet.allVolume);
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "SfxVolume", newSet.sfxVolume);
        DataManager.dataManager.SetConfigData(DataSection.SoundSetting, "BgmVolume", newSet.bgmVolume);
        DataManager.dataManager.SetConfigData(DataSection.Language, "Language", newSet.language);
        SettingManager.settingManager.panelSetting.SetActive(false);
        originResolution = newResolution;
        originSet = newSet;
        originScreenMode = Screen.fullScreenMode;
    }
    internal void CancelBtnClick()
    {
        Screen.SetResolution(resolutions[originResolution].width, resolutions[originResolution].height, originScreenMode);
        SettingManager.settingManager.panelSetting.SetActive(false);
        SettingManager.settingManager.dropdownResolution.value = originResolution;
        SettingManager.settingManager.toggleFullScreen.isOn = originScreenMode == FullScreenMode.FullScreenWindow;

        SettingManager.settingManager.dropdownLanguage.value = (int)originSet.language;
        newSet = originSet;
    }
    internal void SettingBtnClick()
    {
        if (panelSetting.activeSelf)
        {
            newSet = new();
        }
        panelSetting.SetActive(!panelSetting.activeSelf);
    }
}

public struct SettingStruct
{
    public float allVolume;
    public float sfxVolume;
    public float bgmVolume;
    public Language language;
    public SettingStruct(float _allVolume, float _sfxVolume, float _bgmVolume, Language _language)
    {
        allVolume = _allVolume;
        sfxVolume = _sfxVolume;
        bgmVolume = _bgmVolume;
        language = _language;
    }
}