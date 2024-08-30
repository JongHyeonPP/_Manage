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
    public AudioMixer masterMixer;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private Dictionary<string, AudioClip> bgmList;
    [SerializeField] private Dictionary<string, AudioClip> sfxList;
    readonly string bgmPath = "Sound/Bgm";
    readonly string sfxPath = "Sound/Sfx";
    private void Update()
    {
        //masterMixer.GetFloat("BGM", out float value);
        //Debug.Log(value);
    }
    private void Awake()
    {
        if (!soundManager)
        {
            soundManager = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
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
    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        AudioClip audioClip;
        if (arg0.name == "Battle")
        {
            int phaseIndex = StageScenarioBase.nodes.Count == 6 ? 1 : 0;
            if (bgmList.TryGetValue($"Battle_{StageScenarioBase.stageNum}_{phaseIndex}", out audioClip))
            {
                bgmSource.clip = audioClip;
                bgmSource.Play();
            }
        }
        else if (arg0.name.Contains("Stage"))
        {
            if (bgmList.TryGetValue("Stage", out audioClip))
            {
                bgmSource.clip = audioClip;
                bgmSource.Play();
            }
        }
        else
        {
            if (bgmList.TryGetValue(arg0.name, out audioClip))
            {
                bgmSource.clip = audioClip;
                bgmSource.Play();
            }
        }
    }
    public static void SfxPlay(string _name, float _scale=1f)
    {
        soundManager.sfxSource.PlayOneShot(soundManager.sfxList[_name], _scale);
    }
}