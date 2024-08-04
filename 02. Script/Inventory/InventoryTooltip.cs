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
        switch (_item.itemType)
        {
            case ItemType.Weapon:
                categoriStr = (GameManager.language == Language.Ko) ? "����" : "Weapon";
                break;
            case ItemType.Skill:
                string categoriStr0;
                switch (_item.itemId.Split('_')[0])
                {
                    default:
                        categoriStr0 = (GameManager.language == Language.Ko) ? "�Ŀ�" : "Power";
                        break;
                    case "Util":
                        categoriStr0 = (GameManager.language == Language.Ko) ? "��ƿ" : "Util";
                        break;
                    case "Sustain":
                        categoriStr0 = (GameManager.language == Language.Ko) ? "��������" : "Sustain";
                        break;
                }
                string categoriStr1 = (GameManager.language == Language.Ko) ? "��ų" : "Skill";
                categoriStr = categoriStr0 +" "+ categoriStr1;
                break;
            case ItemType.Food:
                categoriStr = (GameManager.language == Language.Ko) ? "����" : "Food";
                break;
            case ItemType.Ingredient:
                categoriStr = (GameManager.language == Language.Ko) ? "�丮 ���" : "Ingredient";
                break;
        }
        categori.text = categoriStr;
        string gradeStr = "";
        switch (_item.itemGrade)
        {
            case ItemGrade.Normal:
                gradeStr = (GameManager.language == Language.Ko) ? "�븻" : "Normal";
                break;
            case ItemGrade.Rare:
                gradeStr = (GameManager.language == Language.Ko) ? "����" : "Rare";
                break;
            case ItemGrade.Unique:
                gradeStr = (GameManager.language == Language.Ko) ? "����ũ" : "Unique";
                break;
        }
        if (_item.itemType == ItemType.Ingredient)
        {
            switch (((IngredientClass)_item).ingredientType)
            {
                case IngredientType.Meat:
                    gradeStr = (GameManager.language == Language.Ko) ? "���" : "Meat";
                    break;
                case IngredientType.Fish:
                    gradeStr = (GameManager.language == Language.Ko) ? "�ػ깰" : "Sea Food";
                    break;
                case IngredientType.Fruit:
                    gradeStr = (GameManager.language == Language.Ko) ? "����" : "Fruit";
                    break;
                case IngredientType.Vegetable:
                    gradeStr = (GameManager.language == Language.Ko) ? "��ä" : "Vegetiable";
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
        parentStatus.localPosition = new Vector2(0f,-(explain.preferredHeight + verticalSpace));
        transform.localPosition = _localPosition + Vector3.up * yCorrection;
    }
}