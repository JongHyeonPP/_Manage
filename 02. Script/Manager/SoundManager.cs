using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using EnumCollection;
public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager;
    [Header("SoundSetting")]
    public AudioMixer masterMixer;
    public Slider SliderAll;
    public Slider SliderSfx;
    public Slider SliderBgm;
    [SerializeField] private AudioSource BgmSource;
    [SerializeField] private List<AudioClip> BgmList;
    private void Awake()
    {
        if (!soundManager)
        {
            soundManager = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            string[] BgmPaths = Resources.LoadAll("BGM", typeof(AudioClip)).Select(x => x.name).ToArray();
            BgmSource = GetComponent<AudioSource>();
            BgmList = new List<AudioClip>();
            BgmList.AddRange(Resources.LoadAll<AudioClip>("BGM"));
        }
    }
    private void Start()
    {
        //Slider Value Changed에서도 사용하는 메서드들
        ALLControl();
        BgmControl();
        SfxControl();
    }
    public void ALLControl()
    {
        float sound = SliderAll.value;
        if (sound == -30f)
            sound = -80f;
        SettingManager.settingManager.ChangeSound(EVolume.All, sound);
        masterMixer.SetFloat("Master", sound);
    }
    public void BgmControl()
    {
        float sound = SliderBgm.value;
        if (sound == -30f)
            sound = -80f;
        SettingManager.settingManager.ChangeSound(EVolume.Bgm, sound);
        masterMixer.SetFloat("BGM", sound);
    }
    public void SfxControl()
    {
        float sound = SliderSfx.value;
        if (sound == -30f)
            sound = -80f;
        SettingManager.settingManager.ChangeSound(EVolume.Sfx, sound);
        masterMixer.SetFloat("SFX", sound);
    }

    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < BgmList.Count; i++)
        {
            if (arg0.name == BgmList[i].name)
            {
                BgmSource.clip = BgmList[i];
                BgmSource.Play();
            }
        }
    }
}