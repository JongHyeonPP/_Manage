using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ItemCollection;
using EnumCollection;
using UnityEditor.ShaderGraph.Internal;

public class InventoryTooltip : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text categori;
    public TMP_Text grade;
    public TMP_Text explain;
    public StatusSlot_T hpSlot;
    public StatusSlot_T abilitySlot;
    public StatusSlot_T resistSlot;
    public StatusSlot_T speedSlot;
    public Transform parentStatus; 
    RectTransform rect;
    GridLayoutGroup parentStatus_Glg;
    readonly float baseHeight = 140f;
    readonly float verticalSpace = 10f;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        parentStatus_Glg = parentStatus.GetComponent<GridLayoutGroup>();
    }
    public void SetTooltipInfo(Item _item)
    {
        _item.SetSpriteToImage(itemImage);
        itemName.text = _item.name[GameManager.language];
        string categoriStr = "";
        switch (_item.itemType)
        {
            case ItemType.Weapon:
                categoriStr = (GameManager.language == Language.Ko) ? "무기" : "Weapon";
                break;
            case ItemType.Skill:
                string categoriStr0;
                switch (_item.itemId.Split('_')[0])
                {
                    default:
                        categoriStr0 = (GameManager.language == Language.Ko) ? "파워" : "Power";
                        break;
                    case "Util":
                        categoriStr0 = (GameManager.language == Language.Ko) ? "유틸" : "Util";
                        break;
                    case "Sustain":
                        categoriStr0 = (GameManager.language == Language.Ko) ? "서스테인" : "Sustain";
                        break;
                }
                string categoriStr1 = (GameManager.language == Language.Ko) ? "스킬" : "Skill";
                categoriStr = categoriStr0 +" "+ categoriStr1;
                break;
            case ItemType.Food:
                categoriStr = (GameManager.language == Language.Ko) ? "음식" : "Food";
                break;
            case ItemType.Ingredient:
                categoriStr = (GameManager.language == Language.Ko) ? "요리 재료" : "Ingredient";
                break;
        }
        categori.text = categoriStr;
        string gradeStr = "";
        switch (_item.itemGrade)
        {
            case ItemGrade.Normal:
                gradeStr = (GameManager.language == Language.Ko) ? "노말" : "Normal";
                break;
            case ItemGrade.Rare:
                gradeStr = (GameManager.language == Language.Ko) ? "레어" : "Rare";
                break;
            case ItemGrade.Unique:
                gradeStr = (GameManager.language == Language.Ko) ? "유니크" : "Unique";
                break;
        }
        grade.text = gradeStr;
        explain.text = _item.GetExplain();
        if (_item.itemType == ItemType.Weapon)
        {
            WeaponClass _weapon = (WeaponClass)_item;
            hpSlot.SetValue(_weapon.hp);
            abilitySlot.SetValue(_weapon.ability);
            resistSlot.SetValue(_weapon.resist);
            speedSlot.SetValue(_weapon.speed);
        }
        else
        {
            hpSlot.gameObject.SetActive(false);
            abilitySlot.gameObject.SetActive(false);
            resistSlot.gameObject.SetActive(false);
            speedSlot.gameObject.SetActive(false);
        }
        int statusNum = 0;
        foreach (Transform x in parentStatus)
        {
            if (x.gameObject.activeSelf)
                statusNum++;
        }
        rect.sizeDelta = new Vector2(rect.rect.width, baseHeight + explain.preferredHeight + (parentStatus_Glg.cellSize.y + parentStatus_Glg.spacing.y) * (statusNum+1 / 2));
        parentStatus.localPosition = new Vector3(0f,-(explain.preferredHeight + verticalSpace),0f);
    }
}
