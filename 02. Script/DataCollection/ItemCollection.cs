using BattleCollection;
using EnumCollection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ItemCollection
{
    [Serializable]
    public class CountableItem
    {
        public Item item { get; private set; }
        public int amount;
        public CountableItem(Item _item,  int _amount =1)
        {
            item = _item;
            amount = _amount;
        }
    }
    public abstract class Item//무기, 요리, 스킬
    {
        public ItemType itemType;
        public string itemId;//DB에 셋하기 위함
        public ItemGrade itemGrade;
        public Dictionary<Language, string> name;
        public Sprite sprite { get; set; }
        public Vector2 scale { get; set; }
        public Vector2 position { get; set; }
        public bool isCountable;
        public Dictionary<Language, string> explain;
        protected Item(ItemType _itemType, string _itemId, ItemGrade _itemGrade,
            Dictionary<Language, string> _name, Dictionary<Language, string> _explain, Sprite _sprite, Vector2 _scale, Vector2 _position)
        {
            itemType = _itemType;
            itemId = _itemId;
            itemGrade = _itemGrade;
            name = _name;
            sprite = _sprite;
            scale = _scale;
            position = _position;
            explain = _explain;
        }
        public virtual void SetSpriteToImage(Image _image)
        {
            _image.sprite = sprite;
            _image.transform.localScale= scale;
            _image.transform.localPosition= position;
        }
    }
    public class IngredientClass:Item
    {
        public int pokerNum;
        public IngredientType ingredientType;

        public IngredientClass(ItemType _itemType, string _itemId, ItemGrade _itemGrade, Dictionary<Language, string> _name, Dictionary<Language, string> _explain, Sprite _sprite, Vector2 _scale, Vector2 _position) : base(_itemType,_itemId, _itemGrade, _name,_explain, _sprite, _scale, _position)
        {
            itemType = _itemType;
            itemId = _itemId;
            itemGrade = _itemGrade;
            name = _name;
            sprite = _sprite;
        }
        public IngredientClass SetPokerNum(int _pokerNum) 
        {
            pokerNum = _pokerNum;
            return this;
        }
        public IngredientClass SetIngredientType(IngredientType _ingredientType) 
        {
            ingredientType = _ingredientType;
            return this;
        }
        public Color GetPokerNumColor()
        {
            switch (ingredientType)
            {
                default:
                    return new(0.9716981f, 0.1979341f, 0.08708613f);
                case IngredientType.Bread:
                    return Color.yellow;
                case IngredientType.Fruit:
                    return new Color(0.2971698f, 0.496f, 1f);
                case IngredientType.Vegetable:
                    return Color.green;
            }
        }
    }
    public class FoodClass:Item
    {
        public int degree;//1~5

        public FoodClass(ItemType _itemType, string _itemId, ItemGrade _itemGrade, Dictionary<Language, string> _name, Dictionary<Language, string> _explain, Sprite _sprite, Vector2 _scale, Vector2 _position) : base(_itemType, _itemId, _itemGrade, _name,_explain, _sprite, _scale, _position)
        {
            itemType = _itemType;
            itemId = _itemId;
            itemGrade = _itemGrade;
            name = _name;
            sprite = _sprite;
            position = _position;
            scale = _scale;
        }
        public FoodClass SetDegree(int _degree)
        {
            degree = _degree;
            return this; }

    }
    public class WeaponClass:Item
    {
        public WeaponType weaponType;
        public List<SkillEffect> effects;
        public float ability, hp, resist, speed;

        public VisualEffect defaultVisualEffect;
        public VisualEffect skillVisualEffect;

        public WeaponClass(ItemType _itemType, string _itemId, ItemGrade _itemGrade, Dictionary<Language, string> _name, Dictionary<Language, string> _explain, Sprite _sprite, Vector2 _scale, Vector2 _position)
            : base(_itemType, _itemId, _itemGrade, _name, _explain, _sprite,_scale, _position)
        {
        }

        public WeaponClass SetType(WeaponType _type)
        {
            weaponType = _type;
            return this;
        }
        public WeaponClass SetGrade(ItemGrade _itemGrade)
        {
            itemGrade = _itemGrade;
            return this;
        }
        public WeaponClass SetStatus(float _ability, float _hp, float _resist, float _speed)
        {
            ability = _ability;
            hp = _hp;
            resist = _resist;
            speed = _speed;
            return this;
        }
        public WeaponClass SetEffects(List<SkillEffect> _effects)
        {
            effects = _effects;
            return this;
        }
        public WeaponClass SetVisualEffect(VisualEffect _defaultVisualEffect, VisualEffect _skillVisualEffect)
        {
            defaultVisualEffect = _defaultVisualEffect;
            skillVisualEffect = _skillVisualEffect;
            return this;
        }
    }

    public class LootClass
    {
        public List<CountableItem> main = new();//무기 + 스킬
        public List<CountableItem> sub = new();//요리 재료
        public int gold;
    }

    [Serializable]
    public class Skill
    {
        public string skillId;
        public SkillCategori categori;
        public float cooltime;
        public List<List<SkillEffect>> effectsList;
        public bool isAnim;
        public List<VisualEffect> visualEffects;
        public bool isPre;
        public Dictionary<Language, string> name;
        public List<Dictionary<Language, string>> explain;
        public Sprite skillSprite;
        public Skill(string _skillId, SkillCategori _categori, float _cooltime, List<List<SkillEffect>> _effectsList, bool _isAnim, List<VisualEffect> _visualEffect, bool _isPre, Dictionary<Language, string> _name, List<Dictionary<Language, string>> _explain, Sprite _skillSprite)
        {
            skillId = _skillId;
            categori = _categori;
            cooltime = _cooltime;
            effectsList = _effectsList;
            isAnim = _isAnim;
            visualEffects = _visualEffect;
            isPre = _isPre;
            name = _name;
            explain = _explain;
            skillSprite = _skillSprite;
            if (explain != null)
                SkillExplainReplace();
        }
        public SkillAsItem GetAsItem(int _level)
        {
            SkillAsItem skillAsItem = new SkillAsItem(ItemType.Skill, skillId, (ItemGrade)_level, name, explain[_level], skillSprite, new Vector2(1f, 1f), Vector2.zero, categori, cooltime);
            return skillAsItem;
        }
        public SkillInBattle GetInBattle(int _level)
        {
            VisualEffect visualEffect;
            if (visualEffects!=null&& visualEffects.Count < _level)
                visualEffect = visualEffects[_level];
            else
                visualEffect = null;
            SkillInBattle skillInBattle = new SkillInBattle(cooltime, effectsList[_level], isAnim, visualEffect, isPre);
            return skillInBattle;
        }

        public void SkillExplainReplace()
        {
            for (int i = 0; i < explain.Count; i++)
                explain[i] = ReplaceByRegex(i);

            Dictionary<Language, string> ReplaceByRegex(int _index)
            {
                List<SkillEffect> skillEffects = effectsList[_index];
                Dictionary<Language, string> originDict = explain[_index];
                Dictionary<Language, string> explainDict = new();
                List<Language> allLangauge = new() {Language.Ko, Language.En };
                List<string> allField = new() { "Value", "Count", "Vamp" };
                foreach (var language in allLangauge)
                {
                    string replacedStr = originDict[language];
                    foreach (var field in allField)
                    {
                        replacedStr = ReplaceByRegex(replacedStr, field);
                    }
                    explainDict.Add(language, replacedStr);
                }

                string ReplaceByRegex(string _origin, string _field)
                {
                    string fontColor;
                    float fontSize;
                    string replacedStr = _origin;
                    switch (_field)
                    {
                        default://Value
                            fontColor = "#0096FF";
                            fontSize = 17.5f;
                            break;
                        case "Count":
                            fontColor = "#4C4CFF";
                            fontSize = 17.5f;
                            break;
                        case "Vamp":
                            fontColor = "#4C4CFF";
                            fontSize = 17.5f;
                            break;
                    }
                    // 정규식 패턴 문자열 생성, % 포함 여부도 확인
                    string pattern = $@"\{{{_field}_(\d+)\}}(\%)?";
                    Regex regex = new Regex(pattern);

                    MatchCollection matches = regex.Matches(_origin);

                    foreach (Match match in matches)
                    {
                        int index = int.Parse(match.Groups[1].Value); // {Value_i}에서 i 추출
                        bool isPercent = match.Groups[2].Success; // % 기호 존재 여부 확인
                        if (index >= 0 && index < effectsList.Count)
                        {
                            string replaceStr;
                                switch (_field)
                                {
                                    default://Value
                                        replaceStr = skillEffects[index].value.ToString();
                                        if (skillEffects[index].valueBase == ValueBase.Ability)
                                            replaceStr += "AB";
                                        break;
                                    case "Count":
                                        replaceStr = skillEffects[index].count.ToString();
                                        break;
                                    case "Vamp":
                                        replaceStr = skillEffects[index].vamp.ToString();
                                        break;
                                }
                                if (isPercent)
                                {
                                    double value = double.Parse(replaceStr) * 100;
                                    replaceStr = value.ToString("0.##") + "%"; // % 기호를 결과에 포함
                                }

                                string richF = $"<color={fontColor}><size={fontSize}><b>";
                                string richB = "</b></size></color>";
                                replaceStr = richF + replaceStr + richB;
                                replacedStr = replacedStr.Replace(match.Value, replaceStr);


                        }
                    }
                    replacedStr = replacedStr.Replace("\\n", "\n");
                    return replacedStr;
                }
                return explainDict;
            }
        }

    }
    public class SkillAsItem:Item
    {
        public SkillCategori categori;
        public float cooltime;
        public SkillAsItem(ItemType _itemType, string _itemId, ItemGrade _itemGrade, Dictionary<Language, string> _name,
            Dictionary<Language, string> _explain, Sprite _sprite, Vector2 _scale, Vector2 _position, SkillCategori _categori, float _cooltime)
    : base(_itemType, _itemId, _itemGrade, _name, _explain, _sprite, _scale, _position)
        {
            categori = _categori;
            cooltime = _cooltime;
        }
    }
}