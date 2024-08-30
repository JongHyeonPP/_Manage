using EnumCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectButton : MonoBehaviour
{
    public GameObject imageHighlight;
    public GameObject imageHighlight_All;
    public ItemType type;
    [SerializeField] bool isInStore;//true : Inventory, false : Store
    public void OnClicked()
    {
        SoundManager.SfxPlay("PopThin");
        if (isInStore)
            GameManager.storeScenario.storeUi.SelectButtonSelect(this);
        else
            ItemManager.itemManager.inventoryUi.SelectButtonSelect(this);
    }

    public void ActiveHighlight(bool _isaActive)
    {
        imageHighlight.SetActive(_isaActive);
        if (imageHighlight_All)
            imageHighlight_All.SetActive(_isaActive);
    }
}
