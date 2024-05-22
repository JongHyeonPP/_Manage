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

            yield return null; // ��� ���� ���� ���������� ����
        }

        // ��ٿ��� �Ϸ�Ǹ� �̹����� �ٽ� �ʱ�ȭ�ϰų� �ٸ� �۾��� ������ �� �ֽ��ϴ�.
        cooldownImage.fillAmount = 0f;
        // ��ٿ��� �Ϸ�� �Ŀ� �ٸ� �۾��� �߰��Ϸ��� �� �κп� �ڵ带 �߰��ϼ���.
    }

}
