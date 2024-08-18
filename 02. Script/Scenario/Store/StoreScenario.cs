using EnumCollection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class StoreScenario : MonoBehaviour
{
    public CookUi cookUi;
    public StoreUi storeUi;
    public ItemTooltip storeTooltip;
    public List<StoreSelect> storeSelects;
    public GameObject selectLight;
    public GameObject raycastBlock;
    public static readonly float ingredientPrice = 5f;
    public static readonly float normalSkillPrice = 30f;
    public static readonly float rareSkillPrice = 100f;
    public static readonly float uniqueSkillPrice = 300f;
    public static readonly float normalWeaponPrice = 30f;
    public static readonly float rareWeaponPrice = 100f;
    public static readonly float uniqueWeaponPrice = 300f;
    public Camera overlayCamera;
    private void Awake()
    {
        GameManager.storeScenario = this;
        cookUi.gameObject.SetActive(false);
        selectLight.SetActive(false);
        raycastBlock.SetActive(false);
        storeUi.gameObject.SetActive(false);
        storeTooltip.gameObject.SetActive(false);
    }
    private void Start()
    {
        var baseCameraData = Camera.main.GetUniversalAdditionalCameraData();
        var overlayCameraData = overlayCamera.GetUniversalAdditionalCameraData();

        // 먼저 카메라 스택에서 해당 오버레이 카메라를 제거
        if (baseCameraData.cameraStack.Contains(overlayCamera))
        {
            baseCameraData.cameraStack.Remove(overlayCamera);
        }

        // 그런 다음 카메라 스택의 가장 마지막에 다시 추가 (즉, 가장 위로 올림)
        baseCameraData.cameraStack.Add(overlayCamera);
    }
    public void NextButtonClicked()
    {
        SceneManager.LoadSceneAsync("Stage" + StageScenarioBase.stageNum);
    }

    public void OnMediumClicked(StoreCase _storeCase)
    {
        selectLight.SetActive(false);
        foreach (StoreSelect x in storeSelects)
        {
            x.imageMedium.SetActive(false);
        }
        switch (_storeCase)
        {
            case StoreCase.Cook:
                cookUi.gameObject.SetActive(true);
                break;
            case StoreCase.Store:
                storeUi.gameObject.SetActive(true);
                break;
        }
    }
    public void ActiveMediumImages()
    {
        foreach (var x in storeSelects)
        {
            x.imageMedium.SetActive(true);
        }
    }
    public void OnRaycastBlockClicked()
    {
        cookUi.gameObject.SetActive(false);
        storeUi.gameObject.SetActive(false);
    }
}
