using DefaultCollection;
using EnumCollection;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using static SettingManager;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class SettingManager : MonoBehaviour
{
    public delegate void LanguageChange();
    public static event LanguageChange LanguageChangeEvent;

    public static SettingManager settingManager;
    public static List<VolumeType> volumeTypes;

    public SettingUi settingUi;

    public static List<Resolution> resolutions = new();
    public GameObject raycastBlock;


    private void Start()//DataManager가 먼저 Awake되고 실행돼야함
    {
        if (!settingManager)
        {
            settingManager = this;
            settingUi.ConnectLangaugeChange();
            volumeTypes = new List<VolumeType>((VolumeType[])Enum.GetValues(typeof(VolumeType)));
            settingUi.gameObject.SetActive(false);
            raycastBlock.SetActive(false);
            settingUi.InitSettingUi();
            
        }
    }
    public void ExecuteLangaugeChange() => LanguageChangeEvent();
}