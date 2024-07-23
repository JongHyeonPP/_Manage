using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BattleCollection;
using Unity.VisualScripting;
using EnumCollection;
using UnityEngine.UI;
public class GetJobUi : MonoBehaviour
{
    string jobName;
    //phase0
    public GameObject phase0;
    public CharacterHierarchy from;
    public CharacterHierarchy to;
    public TMP_Text toText;
    public Image imageJobIcon;
    //phase1
    public GameObject phase1;
    public CharacterHierarchy jobCh;
    public TMP_Text textExplain;

    public ParticleSystem particle;
    readonly Dictionary<Language, string> explainStr = new Dictionary<Language, string> { { Language.Ko, "{jobName}로 전직했습니다!" }, { Language.En, "Promoted to {jobName}!" } };
    private void Awake()
    {
        from.animator.enabled = false;
        to.animator.enabled = false;

    }
    private void OnEnable()
    {
        phase0.SetActive(true);
        phase1.SetActive(false);
        textExplain.gameObject.SetActive(false);
        
        ItemManager.itemManager.backgroundInventoryAdd.SetActive(true);
    }
    private void OnDisable()
    {
        ItemManager.itemManager.backgroundInventoryAdd.SetActive(false);
    }

    public void SetInfo(CharacterData _character)
    {
        from.CopyHierarchySprite(_character.characterHierarchy);
        to.CopyHierarchySprite(_character.characterHierarchy);
        JobClass job = GameManager.gameManager.GetJob(_character.skillAsIItems[0].itemId, _character.skillAsIItems[1].itemId);
        to.SetJobSprite(job);
        imageJobIcon.sprite = job.jobIcon;
        jobName = job.name[GameManager.language];
        toText.text = jobName;
    }
    public void SetJob()
    {
        ItemManager.itemManager.SetJobAtSelectedCharacter();
    }
    public void NextPhase()
    {
        jobCh.animator.enabled = false;
        StartCoroutine(NextPhaseCor());
    }

    private IEnumerator NextPhaseCor()
    {
        SetJob();
        jobCh.CopyHierarchySprite(from);
        phase0.gameObject.SetActive(false);
        phase1.gameObject.SetActive(true);
        particle.Simulate(2, true, true);
        particle.Play();
        StartCoroutine(FadeOutParticleSystemAlpha());
        yield return StartCoroutine(jobCh.GraduallyChangeAlpha(false, 3f));
        jobCh.CopyHierarchySprite(to);
        StartCoroutine(jobCh.GraduallyChangeAlpha(true,3f));
        textExplain.gameObject.SetActive(true);
        string replaced = explainStr[GameManager.language].Replace("{jobName}", jobName);
        textExplain.text = replaced;
        jobCh.animator.enabled = true;
    }
    IEnumerator FadeOutParticleSystemAlpha()
    {
        ParticleSystem.MainModule mainModule = particle.main;
        float startAlpha = mainModule.startColor.color.a; // 초기 알파값 저장
        float elapsedTime = 0f;
        float fadeDuration = 3f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, 0, elapsedTime / fadeDuration);
            ParticleSystem.MinMaxGradient startColor = mainModule.startColor;
            Color newColor = startColor.color;
            newColor.a = alpha;
            mainModule.startColor = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 완전히 투명하게 설정
        ParticleSystem.MinMaxGradient finalColor = mainModule.startColor;
        Color color = finalColor.color;
        color.a = 0;
        mainModule.startColor = color;
    }
}
