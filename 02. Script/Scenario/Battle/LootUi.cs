using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultCollection;
using ItemCollection;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using EnumCollection;
public class LootUi : MonoBehaviour
{
    [SerializeField] TMP_Text textTitle;
    public Transform panelMain;
    public Transform panelSub;
    [SerializeField] TMP_Text textGold;
    private List<MainLootSlot> mainLootSlots;
    private List<SubLootSlot> subLootSlots;
    private void Awake()
    {
        SettingManager.LanguageChangeEvent += OnLanguageChange;
        OnLanguageChange();
    }
    private void OnLanguageChange()
    {
        textTitle.text = GameManager.language == Language.Ko ? "Àü¸®Ç°" : "Loot";
    }
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
    public void SetLootAtUi(List<CountableItem> _main, List<CountableItem> _sub, int _gold, int _goldAscend)
    {
        //Main
        for (int i = 0; i < mainLootSlots.Count; i++)
        {
            MainLootSlot slot = mainLootSlots[i];
            if (i < _main.Count)
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
                loot.SetSubLoot((IngredientClass)_sub[i].item , _sub[i].amount);
            }
            else
            {
                loot.gameObject.SetActive(false);
            }
        }
        textGold.text = _gold.ToString();
        if(_goldAscend>0)
        textGold.text += $" <color=#EFCE0D>(+{_goldAscend})";
    }
}
