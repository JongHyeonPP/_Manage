using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ItemCollection;
using EnumCollection;

public class InventoryTooltip : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text categori;
    public TMP_Text grade;
    public TMP_Text explain;
    public StatusSlot_I hpSlot;
    public StatusSlot_I abilitySlot;
    public StatusSlot_I resistSlot;
    public StatusSlot_I speedSlot;
    public Transform parentStatus; 
    RectTransform rectTransform;
    GridLayoutGroup parentStatus_Glg;
    readonly float baseHeight = 170f;
    readonly float verticalSpace = 20f;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentStatus_Glg = parentStatus.GetComponent<GridLayoutGroup>();
    }
    public void SetTooltipInfo(Item _item, Vector3 _localPosition)
    {
        _item.SetSpriteToImage(itemImage);
        itemName.text = _item.name[GameManager.language];
        string categoriStr = "";
        switch (_item.weaponType)
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
        string categoriColor = string.Empty;
        if (_item.weaponType == ItemType.Skill)
        {
            switch (((SkillAsItem)_item).categori)
            {
                case SkillCategori.Power:
                    categoriColor = "<color=#FF3E3D>";
                    break;
                case SkillCategori.Util:
                    categoriColor =  "<color=#63FF00>";
                    break;
                case SkillCategori.Sustain:
                    categoriColor = "<color=#F2F200>";
                    break;
            }
            categoriStr = categoriColor + categoriStr;
        }
        categori.text = categoriStr;
        string gradeStr = string.Empty;
        if (_item.weaponType != ItemType.Ingredient)
        {
            switch (_item.itemGrade)
            {
                default:
                    gradeStr = "<color=#ABABAB>" + ((GameManager.language == Language.Ko) ? "기본" : "Default");
                    break;
                case ItemGrade.Normal:
                    gradeStr = "<color=#FFAF26>" + ((GameManager.language == Language.Ko) ? "노말" : "Normal");
                    break;
                case ItemGrade.Rare:
                    gradeStr = "<color=#25FFF8>" + ((GameManager.language == Language.Ko) ? "레어" : "Rare");
                    break;
                case ItemGrade.Unique:
                    gradeStr = "<color=#FF25EA>" + ((GameManager.language == Language.Ko) ? "유니크" : "Unique");
                    break;
            }

        }
        else
        {
            switch (((IngredientClass)_item).ingredientType)
            {
                case IngredientType.Meat:
                    gradeStr = (GameManager.language == Language.Ko) ? "고기" : "Meat";
                    break;
                case IngredientType.Fish:
                    gradeStr = (GameManager.language == Language.Ko) ? "해산물" : "Sea Food";
                    break;
                case IngredientType.Fruit:
                    gradeStr = (GameManager.language == Language.Ko) ? "과일" : "Fruit";
                    break;
                case IngredientType.Vegetable:
                    gradeStr = (GameManager.language == Language.Ko) ? "야채" : "Vegetiable";
                    break;
                case IngredientType.Special:
                    
                    break;
            }
        }
        grade.text = gradeStr;
        explain.text = _item.explain[GameManager.language];
        float width = rectTransform.rect.width;
        float height = baseHeight + explain.preferredHeight;
        float yCorrection = explain.preferredHeight;
        if (_item.weaponType == ItemType.Weapon)
        {
            WeaponClass _weapon = (WeaponClass)_item;
            hpSlot.SetValue(_weapon.hp);
            abilitySlot.SetValue(_weapon.ability);
            resistSlot.SetValue(_weapon.resist);
            speedSlot.SetValue(_weapon.speed);
            height += (parentStatus_Glg.cellSize.y + parentStatus_Glg.spacing.y)*2;
            yCorrection += (parentStatus_Glg.cellSize.y + parentStatus_Glg.spacing.y) * 2;
        }
        else
        {
            hpSlot.gameObject.SetActive(false);
            abilitySlot.gameObject.SetActive(false);
            resistSlot.gameObject.SetActive(false);
            speedSlot.gameObject.SetActive(false);
        }
        Canvas.ForceUpdateCanvases();

        rectTransform.sizeDelta = new Vector2(width, height);
        parentStatus.localPosition = new Vector2(0f,-(explain.preferredHeight + verticalSpace));
        transform.localPosition = _localPosition + Vector3.up * yCorrection;
    }
}