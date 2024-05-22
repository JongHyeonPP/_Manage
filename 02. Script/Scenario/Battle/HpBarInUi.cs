using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarInUi : HpBarBase
{
    public CharacterHierarchy characterHierarchy;
    public GameObject[] skillObject = new GameObject[2];
    public Image[] cooldownImages = new Image[2];
    public void StartCooldown(int index, float cooldownTime)
    {
        Image cooldownImage = cooldownImages[index];
        cooldownImage.fillAmount = 1f;

        StartCoroutine(CooldownCoroutine(cooldownImage, cooldownTime));
    }

    IEnumerator CooldownCoroutine(Image cooldownImage, float cooldownTime)
    {
        float startTime = Time.time;

        while (Time.time - startTime < cooldownTime)
        {
            float elapsedTime = Time.time - startTime;
            cooldownImage.fillAmount = 1 - (elapsedTime / cooldownTime);

            yield return null; // 대기 없이 다음 프레임으로 진행
        }

        // 쿨다운이 완료되면 이미지를 다시 초기화하거나 다른 작업을 수행할 수 있습니다.
        cooldownImage.fillAmount = 0f;
        // 쿨다운이 완료된 후에 다른 작업을 추가하려면 이 부분에 코드를 추가하세요.
    }

}
