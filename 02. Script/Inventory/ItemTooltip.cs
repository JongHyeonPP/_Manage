using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ItemCollection;
using EnumCollection;

public class ItemTooltip : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text textItemName;
    public TMP_Text textCategori;
    public TMP_Text textGrade;
    public TMP_Text textCooltime;
    public TMP_Text textExplain;
    public StatusSlot_I hpSlot;
    public StatusSlot_I abilitySlot;
    public StatusSlot_I resistSlot;
    public StatusSlot_I speedSlot;
    public Transform parentStatus; 
    public RectTransform rectTransform;
    [SerializeField]GridLayoutGroup parentStatus_Glg;
    readonly float baseHeight = 170f;
    readonly float verticalSpace = 20f;
    [SerializeField] bool isInventory;
    public void SetTooltipInfo(Item _item)
    {
        if (_item == null)
            return;
        _item.SetSpriteToImage(itemImage);
        textItemName.text = _item.name[GameManager.language];
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
        string categoriColor = string.Empty;
        if (_item.itemType == ItemType.Skill)
        {
            SkillAsItem asSkill = (SkillAsItem)_item;
            switch (asSkill.categori)
            {
                case SkillCategori.Power:
                    categoriColor = "<color=#FF3E3D>";
                    break;
                case SkillCategori.Util:
                    categoriColor = "<color=#63FF00>";
                    break;
                case SkillCategori.Sustain:
                    categoriColor = "<color=#F2F200>";
                    break;
            }
            categoriStr = categoriColor + categoriStr;

            if (asSkill.cooltime == 0f)
            {
                textCooltime.gameObject.SetActive(false);
            }
            else
            {
                string cooltimeStr = asSkill.cooltime.ToString("F0");
                cooltimeStr += (GameManager.language == Language.Ko) ? "초" : "S";
                textCooltime.text = cooltimeStr;
                textCooltime.gameObject.SetActive(true);
            }
        }
        else
        {
            textCooltime.gameObject.SetActive(false);
        }
        textCategori.text = categoriStr;
        string gradeStr = string.Empty;
        if (_item.itemType != ItemType.Ingredient)
        {
            string gradeColor;
            switch (_item.itemGrade)
            {
                default:
                    gradeColor = "#ABABAB";
                    gradeStr = $"<color={gradeColor}>{(GameManager.language == Language.Ko ? "기본" : "Default")}";
                    break;
                case ItemGrade.Normal:
                    gradeColor = "#FFAF26";
                    gradeStr = $"<color={gradeColor}>{(GameManager.language == Language.Ko ? "노말" : "Normal")}";
                    break;
                case ItemGrade.Rare:
                    gradeColor = "#25FFF8";
                    gradeStr = $"<color={gradeColor}>{(GameManager.language == Language.Ko ? "레어" : "Rare")}";
                    break;
                case ItemGrade.Unique:
                    gradeColor = "#FF25EA";
                    gradeStr = $"<color={gradeColor}>{(GameManager.language == Language.Ko ? "유니크" : "Unique")}";
                    break;
            }
            textItemName.text = $"<color={gradeColor}>" + textItemName.text;
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
            }
        }
        textGrade.text = gradeStr;
        if (_item.itemType == ItemType.Food)
            textExplain.text = ((FoodClass)_item).GetExplain();
        else
            textExplain.text = _item.explain[GameManager.language];
        float width = rectTransform.rect.width;
        float height = baseHeight + textExplain.preferredHeight;
        float yCorrection =  textExplain.preferredHeight;
        if (_item.itemType == ItemType.Weapon)
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
        parentStatus.localPosition = new Vector2(0f,-(textExplain.preferredHeight + verticalSpace));
        if (!isInventory)
            yCorrection = 0f;
        transform.localPosition += Vector3.up * yCorrection;
    }
}