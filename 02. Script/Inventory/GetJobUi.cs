using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BattleCollection;
using Unity.VisualScripting;
using EnumCollection;
using UnityEngine.UI;
using Firebase.Firestore;
public class GetJobUi : MonoBehaviour
{
    string jobName;
    //phase0
    public GameObject phase0;
    public CharacterHierarchy from;
    public CharacterHierarchy to;
    public TMP_Text fromText;
    public TMP_Text toText;
    public TMP_Text textExplain0;
    public TMP_Text textNo;
    public TMP_Text textYes;
    public Image imageJobIcon;
    //phase1
    public GameObject phase1;
    public CharacterHierarchy jobCh;
    public TMP_Text textExplain1;

    public ParticleSystem particle;

    [SerializeField] Image imageReturnButton;
    [SerializeField] Image imageConfirmButton;
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
        textExplain1.gameObject.SetActive(false);
        
        ItemManager.itemManager.InventoryRayBlock.SetActive(true);
        fromText.text = GameManager.language == Language.Ko ? "무직" : "Jobless";
        textExplain0.text = GameManager.language == Language.Ko ? "전직 이후에는 스킬 변경이 불가능하며 스킬 강화를 할 수 있습니다." : "After changing job, you cannot modify skills, but you can enhance them.";
        textNo.text = GameManager.language == Language.Ko ? "돌아가기" : "Go Back";
        textYes.text = GameManager.language == Language.Ko ? "전직하기" : "Change Job";
    }
    private void OnDisable()
    {
        ItemManager.itemManager.InventoryRayBlock.SetActive(false);
        SoundManager.SfxPlay("PopThin");
    }

    public void SetInfo(CharacterData _character)
    {
        from.CopyHierarchySprite(_character.characterHierarchy);
        to.CopyHierarchySprite(_character.characterHierarchy);
        JobClass job = GameManager.gameManager.GetJob(_character.skillAsItems[0].itemId, _character.skillAsItems[1].itemId);
        to.SetJobSprite(job);
        imageJobIcon.sprite = job.jobIcon;
        jobName = job.name[GameManager.language];
        toText.text = jobName;
    }
    public void SetJob()
    {
        ItemManager.itemManager.SetJobAtSelectedCharacter();
    }
    public void NextJobPhase()
    {
        jobCh.animator.enabled = false;
        StartCoroutine(NextJobPhaseCor());
        
    }

    private IEnumerator NextJobPhaseCor()
    {
        SoundManager.SfxPlay("GetJob");
        SetJob();
        ItemManager.itemManager.SetInventoryAtDb();
        jobCh.CopyHierarchySprite(from);
        phase0.gameObject.SetActive(false);
        phase1.gameObject.SetActive(true);
        particle.Simulate(2, true, true);
        particle.Play();
        StartCoroutine(FadeOutParticleSystemAlpha());
        yield return StartCoroutine(jobCh.GraduallyChangeAlpha(false, 3f));
        jobCh.CopyHierarchySprite(to);
        StartCoroutine(jobCh.GraduallyChangeAlpha(true,3f));
        textExplain1.gameObject.SetActive(true);
        string replaced = explainStr[GameManager.language].Replace("{jobName}", jobName);
        textExplain1.text = replaced;
        jobCh.animator.enabled = true;


    }
    IEnumerator FadeOutParticleSystemAlpha()
    {
        ParticleSystem.MainModule mainModule = particle.main;
        float startAlpha = 1f; // 초기 알파값 저장
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
