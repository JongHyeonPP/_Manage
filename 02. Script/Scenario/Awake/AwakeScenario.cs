using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AwakeScenario : MonoBehaviour
{
    public Image logoImage;
    private readonly float fadeInTime = 1f;
    private readonly float fadeOutTime = 1f;
    private readonly float totalPlayTime = 5f;
    private readonly float waitTime = 1f;

    private bool isTestMode = true;
    [SerializeField] InputIdUi inputIdUi;
    private void Awake()
    {
        Color imageColor = logoImage.color;
        imageColor.a = 0f;
        logoImage.color = imageColor;
        //StartCoroutine(ShowJhLogo());
    }

    private async void Start()
    {
        if (isTestMode)
        {
            inputIdUi.gameObject.SetActive(true);
        }
        else
        {
            inputIdUi.gameObject.SetActive(false);
            await GameManager.gameManager.LoadProgressDoc();
            SceneManager.LoadScene("Start");
        }
    }



    private IEnumerator ShowJhLogo()
    {
        Sprite sprite = Resources.Load<Sprite>("Texture/Title/JhLogo");
        if (sprite != null)
        {
            logoImage.sprite = sprite;
            yield return new WaitForSeconds(waitTime); // 추가: FadeIn 이전에 waitTime 동안 대기
            yield return StartCoroutine(FadeInJhLogo());
            yield return new WaitForSeconds(totalPlayTime - fadeInTime - fadeOutTime);
            yield return StartCoroutine(FadeOutJhLogo());
            SceneManager.LoadScene("Start");
        }
        else
        {
            Debug.LogError("Load logo fail");
        }
    }

    IEnumerator FadeInJhLogo()
    {
        Color imageColor = logoImage.color;
        imageColor.a = 0f;
        logoImage.color = imageColor;

        while (logoImage.color.a < 1f)
        {
            imageColor.a += Time.deltaTime / fadeInTime;
            logoImage.color = imageColor;

            yield return null;
        }
    }

    IEnumerator FadeOutJhLogo()
    {
        Color imageColor = logoImage.color;

        while (logoImage.color.a > 0f)
        {
            imageColor.a -= Time.deltaTime / fadeOutTime;
            logoImage.color = imageColor;

            yield return null;
        }
    }

    public async void ProgressWithId(string _gameId)
    {
        List<DocumentSnapshot> snapShots = await DataManager.dataManager.GetDocumentSnapshots("User");
        DocumentSnapshot snapShot = snapShots.Where(data => (string)data.ToDictionary()["GameId"] == _gameId).FirstOrDefault();
        if (snapShot == null)
        {
            GameManager.gameManager.uid = await DataManager.dataManager.GetNewDocumentId("User", _gameId);
        }
        else
        {
            GameManager.gameManager.uid = snapShot.Id;
        }

        await GameManager.gameManager.LoadProgressDoc();
        SceneManager.LoadScene("Start");
    }
}
