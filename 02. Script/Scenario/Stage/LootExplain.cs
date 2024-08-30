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
        textExplain.text = GameManager.language == Language.Ko ? "획득 가능한 보상" : "Obtainable rewards";
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
               text_0 =  GameManager.language == Language.Ko ? "노말" : "Normal";
                break;
            case ItemGrade.Rare:
                text_0 = GameManager.language == Language.Ko ? "레어" : "Rare";
                break;
            case ItemGrade.Unique:
                text_0 = GameManager.language == Language.Ko ? "유니크" : "Unique";
                break;
        }
        string text_1 = GameManager.language == Language.Ko ? "스킬" : "skill";
        string text_2;
        switch (_skillCategori)
        {
            default:
                text_2 = GameManager.language == Language.Ko ? "랜덤" : "Random";
                break;
            case SkillCategori.Power:
                text_2 = GameManager.language == Language.Ko ? "파워" : "Power";
                break;
            case SkillCategori.Util:
                text_2 = GameManager.language == Language.Ko ? "유틸" : "Util";
                break;
            case SkillCategori.Sustain:
                text_2 = GameManager.language == Language.Ko ? "서스테인" : "Sustain";
                break;
        }
        text = $"{text_0} {text_1} :\n{text_2}";
        lootSlotSkill.SetContent(sprite,text,_skillGrade );
        switch (_ingredientType)
        {
            case IngredientType.Meat:
                sprite = ItemManager.itemManager.ingredient_Meat;
                text = GameManager.language == Language.Ko ? "고기" : "Meat";
                break;
            case IngredientType.Fish:
                sprite = ItemManager.itemManager.ingredient_Fish;
                text = GameManager.language == Language.Ko ? "해산물" : "Seafood";
                break;
            case IngredientType.Fruit:
                sprite = ItemManager.itemManager.ingredient_Fruit;
                text = GameManager.language == Language.Ko ? "과일" : "Fruit";
                break;
            case IngredientType.Vegetable:
                sprite = ItemManager.itemManager.ingredient_Vegetable;
                text = GameManager.language == Language.Ko ? "야체" : "Vegetable";
                break;
            case IngredientType.All:
                sprite = ItemManager.itemManager.ingredient_Random;
                text = GameManager.language == Language.Ko ? "랜덤" : "Random";
                break;
        }
        text = $"{(GameManager.language==Language.Ko?"요리 재료": "Ingredient")} :\n{text}";
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
