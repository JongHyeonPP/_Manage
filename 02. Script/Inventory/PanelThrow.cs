using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumCollection;
using TMPro;
public class PanelThrow : MonoBehaviour
{
    public TMP_Text textExplain;
    Dictionary<Language, string> thorwStr = new(){ {Language.Ko, "���� �����ðڽ��ϱ�?" },{Language.En,"haha" } }; 
    public void ConfirmButtonClicked()
    {
        ItemManager.itemManager.inventoryUi.throwSlot.ClearSlot();
        ItemManager.itemManager.inventoryUi.throwSlot = null;
        gameObject.SetActive(false);
    }

    public void CancelButtonClicked()
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        textExplain.text = thorwStr[GameManager.language];
    }
}
