using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubLootSlot : MonoBehaviour
{
    public Image imageLoot;
    public TMP_Text textAmount;
    public void SetSubLoot(Sprite _sprite, int _amount)
    {
        imageLoot.sprite = _sprite;
        textAmount.text = _amount.ToString();
    }
}
