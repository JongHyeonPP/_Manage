using EnumCollection;
using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GoodsPanel : MonoBehaviour
{
    public TMP_Text textTitle;
    [SerializeField] List<GoodsSlot> goodsSlots;
    List<Tuple<Item, int>> goodsPriceList = new();
    public GoodsType goodsType;
    readonly float goodsPriceRange = 1.5f;
    public void SetNewGoods()//0:Weapon, 1:Skill, 2:Premium
    {
        AllocateGoods();
        SetListToSlot();
        foreach (var x in goodsSlots)
        {
            x.imageSoldOut.SetActive(false);
        }
    }
    public void SetExistingGoods(List<object> _goodsDataList)
    {
        ParseDataToGoods(_goodsDataList);
        SetListToSlot();
    }

    private void ParseDataToGoods(List<object> _goodsDataList)
    {
        for (int i = 0; i < _goodsDataList.Count; i++)
        {
            Dictionary<string, object> goodsData = _goodsDataList[i] as Dictionary<string, object>;
            string[] splittedId = ((string)goodsData["Id"]).Split(":::");
            bool isSoldOut = (bool)goodsData["IsSoldOut"];
            ItemType itemType;

            Item item = null;
            int price = (int)(long)goodsData["Price"];
            if (Enum.TryParse((string)goodsData["ItemType"], out itemType))
            {
                switch (itemType)
                {
                    case ItemType.Weapon:
                        WeaponType weaponType;
                        if (Enum.TryParse(splittedId[0], out weaponType))
                            item = LoadManager.loadManager.weaponDict[weaponType][splittedId[1]];
                        break;
                    case ItemType.Skill:
                        int gradeIndex;
                        switch (splittedId[1])
                        {
                            default:
                                gradeIndex = 0;
                                break;
                            case "Rare":
                                gradeIndex = 1;
                                break;
                            case "Unique":
                                gradeIndex = 2;
                                break;
                        }
                        item = LoadManager.loadManager.skillsDict[splittedId[0]].GetAsItem(gradeIndex);
                        break;
                }
            }
            goodsPriceList.Add(new(item, price));
            if (isSoldOut)
                goodsSlots[i].SoldOut();
            else
                goodsSlots[i].imageSoldOut.SetActive(false);
        }
    }

    private void SetListToSlot()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < goodsPriceList.Count)
            {
                goodsSlots[i].SetItem(goodsPriceList[i].Item1, goodsPriceList[i].Item2);
            }
            else
            {
                goodsSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetGoodsAtDb()
    {
        List<Dictionary<string, object>> dictList = new();
        foreach (GoodsSlot slot in goodsSlots)
        {
            if (slot.item == null)
                continue;
            Dictionary<string, object> dict = new();
            string itemId;
            switch (slot.item.itemType)
            {
                default:
                    itemId = slot.item.itemId;
                    break;
                case ItemType.Skill:
                    itemId = slot.item.itemId + ":::" + slot.item.itemGrade.ToString();
                    break;
            }
            dict.Add("Id", itemId);
            dict.Add("IsSoldOut", slot.isSoldOut);
            dict.Add("ItemType", slot.item.itemType.ToString());
            dict.Add("Price", slot.price);
            dictList.Add(dict);
        }
        DataManager.dataManager.SetDocumentData(goodsType.ToString(), dictList, $"Progress/{GameManager.gameManager.uid}/Store", "Data");
    }

    private void AllocateGoods()
    {
        int stageNum = StageScenarioBase.stageNum;
        switch (goodsType)
        {
            case GoodsType.Weapon:

                foreach (Dictionary<string, WeaponClass> dict in LoadManager.loadManager.weaponDict.Values)
                {
                    List<WeaponClass> ableValues;
                    ItemGrade selectedGrade;
                    if (stageNum == 0)
                        selectedGrade = ItemGrade.Normal;
                    else
                    {
                        if (GameManager.CalculateProbability(0.5f))
                        {
                            selectedGrade = ItemGrade.Normal;
                        }
                        else
                        {
                            selectedGrade = ItemGrade.Rare;
                        }
                    }
                        ableValues = dict.Select(data => data.Value).Where(data => data.itemGrade == selectedGrade).ToList();
                    WeaponClass selected = ableValues[Random.Range(0, ableValues.Count)];
                    int price = StoreScenario.GetGoodsPrice(selected);
                    price = GetRangedPrice(price);
                    goodsPriceList.Add(new(selected, price));
                }
                break;
            case GoodsType.Skill:
                List<Skill> ableSkills_0 = LoadManager.loadManager.skillsDict.Values.Where(data => data.categori != SkillCategori.Enemy).ToList();
                for (int i = 0; i < 4; i++)
                {
                    Skill selectedSkill = ableSkills_0[Random.Range(0, ableSkills_0.Count)];
                    ableSkills_0.Remove(selectedSkill);
                    int gradeIndex = stageNum == 0 ? 0 : Random.Range(0, 2);
                    SkillAsItem asItem = selectedSkill.GetAsItem(gradeIndex);
                    int price = StoreScenario.GetGoodsPrice(asItem);
                    price = GetRangedPrice(price);
                    goodsPriceList.Add(new(asItem, price));
                }
                break;
            case GoodsType.Premium:
                List<Item> ableSkills_1 = LoadManager.loadManager.skillsDict.Values.Where(data=>data.categori != SkillCategori.Enemy).Select(data => data.GetAsItem(stageNum == 0 ? 1 : 2)).Select(data=>(Item)data).ToList();
                List<Item> ableWeapon = new();
                foreach (Dictionary<string, WeaponClass> dict in LoadManager.loadManager.weaponDict.Values)
                {
                    ableWeapon.AddRange(dict.Values.Where(data => data.itemGrade == (stageNum == 0 ? ItemGrade.Rare : ItemGrade.Unique)));
                }
                for (int i = 0; i < 2; i++)
                {
                    Item selectedItem;
                    bool isSkill = GameManager.CalculateProbability(0.5f);
                    if (isSkill)
                    {
                        selectedItem = ableSkills_1[Random.Range(0, ableSkills_1.Count)];
                        int price = StoreScenario.GetGoodsPrice(selectedItem);
                        price = GetRangedPrice(price);
                        goodsPriceList.Add(new(selectedItem, price));
                        ableSkills_1.Remove(selectedItem);
                    }
                    else
                    {
                        selectedItem = ableWeapon[Random.Range(0, ableWeapon.Count)];
                        int price = StoreScenario.GetGoodsPrice(selectedItem);
                        price = GetRangedPrice(price);
                        goodsPriceList.Add(new(selectedItem, price));
                        ableWeapon.Remove(selectedItem);
                    }
                }
                break;
        }
    }
    int GetRangedPrice(int _price)
    {
        return (int)(_price * Random.Range(1f, goodsPriceRange));
    }
}
