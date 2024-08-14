using EnumCollection;
using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainLootSlot : MonoBehaviour
{
    //Weapon
    public Image panelWeapon;
    public Image imageWeapon;
    //Skill
    public GameObject panelSkill;
    public Image imageSkill;
    //Frame
    public GameObject panelFrame;
    public List<GameObject> frames;
    //Name
    public Image panelName;
    public TMP_Text textName;
    private Dictionary<Language, string> nameDict;

    private void Awake()
    {
        SettingManager.LanguageChangeEvent += OnLanguageChange;
    }
    public void SetMainLootSlot(Item _item)
    {
        nameDict = _item.name;
        if (_item.itemType == ItemType.Weapon)
        {
            panelWeapon.gameObject.SetActive(true);
            panelSkill.gameObject.SetActive(false);
            SetWeaponAtSlot(_item);
        }
        else if (_item.itemType == ItemType.Skill)
        {
            panelWeapon.gameObject.SetActive(false);
            panelSkill.gameObject.SetActive(true);
            SetSkillAtSlot(_item);
        }
        SetFrameByGrade(_item.itemGrade);

    }

    private void SetWeaponAtSlot(Item _item)
    {
        _item.SetSpriteToImage(imageWeapon);
        Sprite nameSprite;
        Sprite itemSprite;
        switch (_item.itemGrade)
        {
            default:
                itemSprite = ItemManager.itemManager.item_Normal;
                nameSprite = ItemManager.itemManager.name_Normal;
                break;
            case ItemGrade.Rare:
                itemSprite = ItemManager.itemManager.item_Rare;
                nameSprite = ItemManager.itemManager.name_Rare;
                break;
            case ItemGrade.Unique:
                itemSprite = ItemManager.itemManager.item_Unique;
                nameSprite = ItemManager.itemManager.name_Unique;
                break;
        }
        panelWeapon.sprite = itemSprite;
        panelName.sprite = nameSprite;
        if (nameDict != null)
            textName.text = nameDict[GameManager.language];
    }
    private void SetSkillAtSlot(Item _item)
    {
        imageSkill.sprite = _item.sprite;
        if (nameDict != null)
            textName.text = $"{((GameManager.language == Language.En) ? "Skill" : "½ºÅ³")} : {nameDict[GameManager.language]}";
        Sprite nameSprite;
        switch (_item.itemGrade)
        {
            default:
                nameSprite = ItemManager.itemManager.name_Normal;
                break;
            case ItemGrade.Rare:
                nameSprite = ItemManager.itemManager.name_Rare;
                break;
            case ItemGrade.Unique:
                nameSprite = ItemManager.itemManager.name_Unique;
                break;
        }
        panelName.sprite = nameSprite;
    }


    private void SetFrameByGrade(ItemGrade _itemGrade)
    {
        int frameIndex;
        switch (_itemGrade)
        {
            default:
                frameIndex = 0;
                break;
            case ItemGrade.Rare:
                frameIndex = 1;
                break;
            case ItemGrade.Unique:
                frameIndex = 2;
                break;
        }
        for (int i = 0; i < frames.Count; i++)
        {
            frames[i].SetActive(i == frameIndex);
        }
    }
    private void OnLanguageChange()
    {
        textName.text = nameDict[GameManager.language];
    }
}
