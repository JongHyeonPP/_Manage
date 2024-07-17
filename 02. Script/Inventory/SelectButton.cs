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
    public void OnClicked()
    {
        ItemManager.itemManager.inventoryUi.SelectButtonSelect(this);
    }

    public void ActiveHighlight(bool _isaActive)
    {
        imageHighlight.SetActive(_isaActive);
        if (imageHighlight_All)
            imageHighlight_All.SetActive(_isaActive);
    }
}
