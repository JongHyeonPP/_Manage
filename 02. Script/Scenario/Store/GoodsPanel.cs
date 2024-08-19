using EnumCollection;
using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GoodsPanel : MonoBehaviour
{
    [SerializeField] List<GoodsSlot> goodsSlots;
    List<Tuple<Item,int>> goodsPriceList = new();
    public void SetGoods(int _typeNum)//0:Weapon, 1:Skill, 2:Premium
    {
        AllocateGoods(_typeNum);
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

    private void AllocateGoods(int _typeNum)
    {
        int stageNum = StageScenarioBase.stageNum;
        switch (_typeNum)
        {
            case 0:

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
                    goodsPriceList.Add(new(selected, price));
                }
                break;
            case 1:
                List<Skill> ableSkills_0 = LoadManager.loadManager.skillsDict.Values.Where(data => data.categori != SkillCategori.Enemy).ToList();
                for (int i = 0; i < 4; i++)
                {
                    Skill selectedSkill = ableSkills_0[Random.Range(0, ableSkills_0.Count)];
                    ableSkills_0.Remove(selectedSkill);
                    int gradeIndex = stageNum == 0 ? 0 : Random.Range(0, 2);
                    SkillAsItem asItem = selectedSkill.GetAsItem(gradeIndex);
                    int price = StoreScenario.GetGoodsPrice(asItem);
                    goodsPriceList.Add(new(asItem, price));
                }
                break;
            case 2:
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
                        goodsPriceList.Add(new(selectedItem, price));
                        ableSkills_1.Remove(selectedItem);
                    }
                    else
                    {
                        selectedItem = ableWeapon[Random.Range(0, ableWeapon.Count)];
                        int price = StoreScenario.GetGoodsPrice(selectedItem);
                        goodsPriceList.Add(new(selectedItem, price));
                        ableWeapon.Remove(selectedItem);
                    }
                }
                break;
        }
    }
}
