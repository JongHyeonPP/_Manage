using EnumCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LootExplain : MonoBehaviour
{
    [SerializeField] TMP_Text textExplain;
    [SerializeField] TMP_Text textGold;

    [SerializeField] LootSlot_Stage lootSlotSkill;
    [SerializeField] LootSlot_Stage lootSlotIngredient;
    internal void SetExplain(SkillCategori _skillCategori, ItemGrade _skillGrade, IngredientType _ingredientType,Tuple<int,int> _goldAmount)
    {
        textExplain.text = GameManager.language == Language.Ko ? "ȹ�� ������ ����" : "Obtainable rewards";
        Sprite sprite;
        string text;
        switch (_skillCategori)
        {
            default:
                sprite = ItemManager.itemManager.book_Default;
                break;
            case SkillCategori.Power:
                sprite = ItemManager.itemManager.book_P;
                break;
            case SkillCategori.Util:
                sprite = ItemManager.itemManager.book_U;
                break;
            case SkillCategori.Sustain:
                sprite = ItemManager.itemManager.book_S;
                break;
        }
        string text_0;
        switch (_skillGrade)
        {
            default:
               text_0 =  GameManager.language == Language.Ko ? "�븻" : "Normal";
                break;
            case ItemGrade.Rare:
                text_0 = GameManager.language == Language.Ko ? "����" : "Rare";
                break;
            case ItemGrade.Unique:
                text_0 = GameManager.language == Language.Ko ? "����ũ" : "Unique";
                break;
        }
        string text_1 = GameManager.language == Language.Ko ? "��ų" : "skill";
        string text_2;
        switch (_skillCategori)
        {
            default:
                text_2 = GameManager.language == Language.Ko ? "����" : "Random";
                break;
            case SkillCategori.Power:
                text_2 = GameManager.language == Language.Ko ? "�Ŀ�" : "Power";
                break;
            case SkillCategori.Util:
                text_2 = GameManager.language == Language.Ko ? "��ƿ" : "Util";
                break;
            case SkillCategori.Sustain:
                text_2 = GameManager.language == Language.Ko ? "��������" : "Sustain";
                break;
        }
        text = $"{text_0} {text_1} :\n{text_2}";
        lootSlotSkill.SetContent(sprite,text,_skillGrade );
        switch (_ingredientType)
        {
            case IngredientType.Meat:
                sprite = ItemManager.itemManager.ingredient_Meat;
                text = GameManager.language == Language.Ko ? "���" : "Meat";
                break;
            case IngredientType.Fish:
                sprite = ItemManager.itemManager.ingredient_Fish;
                text = GameManager.language == Language.Ko ? "�ػ깰" : "Seafood";
                break;
            case IngredientType.Fruit:
                sprite = ItemManager.itemManager.ingredient_Fruit;
                text = GameManager.language == Language.Ko ? "����" : "Fruit";
                break;
            case IngredientType.Vegetable:
                sprite = ItemManager.itemManager.ingredient_Vegetable;
                text = GameManager.language == Language.Ko ? "��ü" : "Vegetable";
                break;
            case IngredientType.All:
                sprite = ItemManager.itemManager.ingredient_Random;
                text = GameManager.language == Language.Ko ? "����" : "Random";
                break;
        }
        text = $"{(GameManager.language==Language.Ko?"�丮 ���": "Ingredient")} :\n{text}";
        lootSlotIngredient.SetContent(sprite, text, ItemGrade.None);
        textGold.text = $"{_goldAmount.Item1} ~ {_goldAmount.Item2}";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
