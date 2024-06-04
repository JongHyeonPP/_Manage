using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultCollection;
using ItemCollection;
using UnityEngine.UI;
public class LootUi : MonoBehaviour
{
    public Transform panelMain;
    public Transform panelSub;

    private List<MainLootSlot> mainLootSlots;
    private List<SubLootSlot> subLootSlots;
    public void InitLootUi()
    {
        mainLootSlots = new List<MainLootSlot>();
        subLootSlots = new List<SubLootSlot>();
        for (int i = 0; i < 3; i++)
        {
            MainLootSlot slot = panelMain.GetChild(i).GetComponent<MainLootSlot>();
            mainLootSlots.Add(slot);    
        }
        for (int i = 0; i < 8; i++)
        {
            SubLootSlot loot = panelSub.GetChild(i).GetComponent<SubLootSlot>();
            subLootSlots.Add(loot);
        }
    }
    public void SetLootAtUi(List<CountableItem> _main, List<CountableItem> _sub, int _gold)
    {
        //Main
        for (int i = 0; i < mainLootSlots.Count; i++)
        {
            MainLootSlot slot = mainLootSlots[i];
            if (i < _main.Count)//�� �ؾ���
            {
                slot.gameObject.SetActive(true);
                Item main = _main[i].item;
                slot.SetMainLootSlot(main);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
        //Sub
        for (int i = 0; i < subLootSlots.Count; i++)
        {
            SubLootSlot loot = subLootSlots[i];
            if (i < _sub.Count)
            {
                loot.gameObject.SetActive(true);
                Sprite sprite = _sub[i].item.sprite;
                int amount = _sub[i].amount;
                loot.SetSubLoot(sprite, amount);
            }
            else
            {
                loot.gameObject.SetActive(false);
            }
        }
    }
}
