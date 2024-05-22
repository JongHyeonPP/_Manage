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
    public TMP_Text textCategori;
    //Frame
    public GameObject panelFrame;
    public List<GameObject> frames;
    //Name
    public Image panelName;
    public TMP_Text textName;
    private Dictionary<Language, string> nameDict;
    private readonly Color powerColor = new(1f, 0.12f,0.12f);
    private readonly Color sustainColor = new(1f, 1f, 0.34f);
    private readonly Color utilColor = new(0.2003195f, 1f, 0.0235849f);
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
        imageWeapon.sprite = _item.sprite;
        imageWeapon.transform.localScale = _item.scale;
        imageWeapon.transform.localPosition = _item.position;
        Material nameMat;
        Material itemMat;
        switch (_item.itemGrade)
        {
            default:
                itemMat = ItemManager.itemManager.itemMat_Normal;
                nameMat = ItemManager.itemManager.nameMat_Normal;
                break;
            case ItemGrade.Rare:
                itemMat = ItemManager.itemManager.itemMat_Rare;
                nameMat = ItemManager.itemManager.nameMat_Rare;
                break;
            case ItemGrade.Unique:
                itemMat = ItemManager.itemManager.itemMat_Unique;
                nameMat = ItemManager.itemManager.nameMat_Unique;
                break;
        }
        panelWeapon.material = itemMat;
        panelName.material = nameMat;
        if (nameDict != null)
            textName.text = nameDict[GameManager.language];
    }
    private void SetSkillAtSlot(Item _item)
    {
        imageSkill.sprite = _item.sprite;
        switch (((Skill)_item).categori)
        {
            default:
                textCategori.text = "P";
                textCategori.color = powerColor;
                textCategori.GetComponent<RectTransform>().anchoredPosition = new Vector3(1.13f, 0.49f,0f);
                break;
            case SkillCategori.Util:
                textCategori.text = "U";
                textCategori.color = utilColor;
                textCategori.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.4300003f, 0.3599997f,0f);
                break;
            case SkillCategori.Sustain:
                textCategori.text = "S";
                textCategori.color = sustainColor;
                textCategori.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.02999973f, 0.32f, 0f);
                break;
        }
        if (nameDict != null)
            textName.text = $"{((GameManager.language == Language.En)?"Skill":"½ºÅ³")} : {nameDict[GameManager.language]}";
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
