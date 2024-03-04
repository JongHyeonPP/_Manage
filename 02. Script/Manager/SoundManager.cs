using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EnumCollection;
public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager;
    [Header("SoundSetting")]
    public AudioMixer masterMixer;
    public Slider sliderAll;
    public Slider sliderSfx;
    public Slider sliderBgm;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private Dictionary<string, AudioClip> bgmList;
    [SerializeField] private Dictionary<string, AudioClip> sfxList;
    readonly string bgmPath = "Sound/Bgm";
    readonly string sfxPath = "Sound/Sfx";
    private void Awake()
    {
        if (!soundManager)
        {
            soundManager = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            bgmSource = GetComponents<AudioSource>()[0];
            sfxSource = GetComponents<AudioSource>()[1];
            bgmList = new ();
            foreach (AudioClip x in Resources.LoadAll<AudioClip>(bgmPath))
            {
                bgmList.Add(x.name, x);
            }
            sfxList = new ();
            foreach (AudioClip x in Resources.LoadAll<AudioClip>(sfxPath))
            {
                sfxList.Add(x.name, x);
            }
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
        float sound = sliderAll.value;
        if (sound == -30f)
            sound = -80f;
        SettingManager.settingManager.ChangeSound(EVolume.All, sound);
        masterMixer.SetFloat("Master", sound);
    }
    public void BgmControl()
    {
        float sound = sliderBgm.value;
        if (sound == -30f)
            sound = -80f;
        SettingManager.settingManager.ChangeSound(EVolume.Bgm, sound);
        masterMixer.SetFloat("BGM", sound);
    }
    public void SfxControl()
    {
        float sound = sliderSfx.value;
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
        if (bgmList.TryGetValue(arg0.name, out AudioClip audioClip))
        {
            bgmSource.clip = audioClip;
            bgmSource.Play();
        }
    }
    public void SfxPlay(string _name)
    {
        sfxSource.PlayOneShot(sfxList[_name]);
    }
    public void ReturnToOriginSet(float _all, float _sfx, float _bgm)
    {
        sliderAll.value = _all;
        sliderSfx.value = _sfx;
        sliderBgm.value = _bgm;
        ALLControl();
        SfxControl();
        BgmControl();
    }
}