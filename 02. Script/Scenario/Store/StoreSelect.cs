using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreSelect : MonoBehaviour
{
    public StoreCase storeCase;
    private GameObject selectLight;
    public GameObject imageMedium;
    private void Start()
    {
        if (!GameManager.storeScenario)
            return;
        selectLight = GameManager.storeScenario.selectLight;
    }
    public void OnPointerEnter()
    {
        if (!GameManager.storeScenario)
            return;
        selectLight.SetActive(true);
        selectLight.transform.SetParent(transform);
        selectLight.transform.localPosition = Vector3.zero;
    }
    public void OnPointerExit()
    {
        if (!GameManager.storeScenario)
            return;
        selectLight.SetActive(false);
    }
    public void OnPointerClick()
    {
        if (!GameManager.storeScenario)
            return;
        GameManager.storeScenario.OnMediumClicked(storeCase);
    }
}
