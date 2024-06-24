using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowConfirm : MonoBehaviour
{
    public void ConfirmButtonClicked()
    {
        ItemManager.itemManager.throwSlot.ClearSlot();
        ItemManager.itemManager.throwSlot = null;
        gameObject.SetActive(false);
    }
    public void CancelButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
