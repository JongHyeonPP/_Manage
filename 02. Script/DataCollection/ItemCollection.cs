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

    public abstract class Item//����, �丮, ��ų
    {
        public ItemType itemType;
        public string itemId;//DB�� ���ϱ� ����
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
                case IngredientType.Fish:
                    return new Color(0.2971698f, 0.496f, 1f);
                case IngredientType.Fruit:
                    return Color.yellow;
                case IngredientType.Vegetable:
                    return Color.green;
            }
        }
    }
    public class FoodClass : Item
    {
        public PokerCombination pokerCombination;
        public FoodEffect foodEffect;
        public StatusType statusType;

        public FoodClass(ItemType _itemType, string _itemId, ItemGrade _itemGrade, Dictionary<Language, string> _name, Dictionary<Language, string> _explain, Sprite _sprite, Vector2 _scale, Vector2 _position) : base(_itemType, _itemId, _itemGrade, _name, _explain, _sprite, _scale, _position)
        {
            itemType = _itemType;
            itemId = _itemId;
            itemGrade = _itemGrade;
            name = _name;
            sprite = _sprite;
            position = _position;
            scale = _scale;

        }
        public FoodClass SetPokerCombination(PokerCombination _pokerCombination)
        {
            pokerCombination = _pokerCombination;
            return this;
        }
        public FoodClass SetFoodEffect(FoodEffect _foodEffect)
        {
            foodEffect = _foodEffect;
            return this;
        }
        public FoodClass SetStatusType(StatusType _statusType)
        {
            statusType = _statusType;
            return this;
        }
        public string GetExplain()
        {
            List<string> allField = new() { "HealAmount", "StatusType", "TargetRange", "StatusValue" };
            string replacedStr = explain[GameManager.language];
            foreach (var field in allField)
            {
                replacedStr = ReplaceByRegex(replacedStr, field);
            }
            return replacedStr;
            string ReplaceByRegex(string _origin, string _field)
            {
                string fontColor = string.Empty;
                string fontSize = string.Empty;
                string replacedStr = _origin;
                switch (_field)
                {
                    case "HealAmount":
                        fontColor = "#0096FF";
                        fontSize = "120%";
                        break;
                    case "StatusValue":
                        fontColor = "#4C4CFF";
                        fontSize = "120%";
                        break;
                }
                // ���Խ� ���� ���ڿ� ����, % ���� ���ε� Ȯ��
                string pattern = $@"\{{{_field}\}}(\%)?";
                Regex regex = new Regex(pattern);

                MatchCollection matches = regex.Matches(_origin);

                foreach (Match match in matches)
                {
                    bool isPercent = match.Groups[1].Success; // % ��ȣ ���� ���� Ȯ��

                    string valueStr = string.Empty;
                    switch (_field)
                    {
                        default://HealAMount
                            float healValue = foodEffect.healAmount;
                            if (isPercent)
                            {
                                healValue *= 100;
                            }
                            valueStr = healValue.ToString("F0");
                            break;
                        case "StatusType":
                            switch (statusType)
                            {
                                case StatusType.HpMax:
                                    valueStr = GameManager.language == Language.Ko ? "�ִ� ä����" : "Max Hp";
                                    break;
                                case StatusType.Ability:
                                    valueStr = GameManager.language == Language.Ko ? "�ɷ���" : "Ability";
                                    break;
                                case StatusType.Resist:
                                    valueStr = GameManager.language == Language.Ko ? "���׷���" : "Resist";
                                    break;
                                case StatusType.Speed:
                                    valueStr = GameManager.language == Language.Ko ? "�ӵ���" : "Speed";
                                    break;
                            }
                            break;
                        case "StatusValue":
                            if (statusType == StatusType.Speed)
                            {
                                valueStr = foodEffect.statusValue[statusType].ToString("F1");
                            }
                            else
                            {
                                valueStr = foodEffect.statusValue[statusType].ToString("F0");
                            }
                            break;
                        case "TargetRange":
                            switch (foodEffect.targetRange)
                            {
                                case FoodTargetRange.One:
                                    valueStr = GameManager.language == Language.Ko ? "�� ��" : "One";
                                    break;
                                case FoodTargetRange.All:
                                    valueStr = GameManager.language == Language.Ko ? "���" : "All";
                                    break;
                            }
                            break;
                    }

                    if (isPercent)
                        valueStr += "%";
                    switch (_field)
                    {
                        case "HealAmount":
                        case "StatusValue":
                            string richF = $"<color={fontColor}><size={fontSize}><b>";
                            string richB = "</b></size></color>";
                            valueStr = richF + valueStr + richB;
                            break;
                    }
                    replacedStr = replacedStr.Replace(match.Value, valueStr);
                }

                replacedStr = replacedStr.Replace("\\n", "\n");
                return replacedStr;
            }
        }
    }
    public class FoodEffect
    {
        public float healAmount;
        public FoodTargetRange targetRange;
        public Dictionary<StatusType, float> statusValue;
        public FoodEffect(float _healAmount, FoodTargetRange _targetRange,
            Dictionary<StatusType, float> _statusValue)
        {
            healAmount = _healAmount;
            targetRange = _targetRange;
            statusValue = _statusValue;
        }
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
        public List<CountableItem> main = new();//���� + ��ų
        public List<CountableItem> sub = new();//�丮 ���
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
        public bool isPre;
        public Dictionary<Language, string> name;
        public List<Dictionary<Language, string>> explain;
        public Sprite skillSprite;
        public Skill(string _skillId, SkillCategori _categori, float _cooltime, List<List<SkillEffect>> _effectsList, bool _isAnim, bool _isPre, Dictionary<Language, string> _name, List<Dictionary<Language, string>> _explain, Sprite _skillSprite)
        {
            skillId = _skillId;
            categori = _categori;
            cooltime = _cooltime;
            effectsList = _effectsList;
            isAnim = _isAnim;
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
            SkillInBattle skillInBattle = new SkillInBattle(cooltime, effectsList[_level], isPre, categori);
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
                List<Language> allLangauge = new() { Language.Ko, Language.En };
                List<string> allField = new() { "Value", "Count", "Vamp","Duration", "Probability" };
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
                    string fontSize;
                    string replacedStr = _origin;
                    switch (_field)
                    {
                        default://Value
                            fontColor = "#FFFFFF";
                            fontSize = "100%";
                            break;
                        case "Value"://Value
                            fontColor = "#0096FF";
                            fontSize = "120%";
                            break;
                        case "Count":
                            fontColor = "#4C4CFF";
                            fontSize = "120%";
                            break;
                        case "Vamp":
                            fontColor = "#4C4CFF";
                            fontSize = "120%";
                            break;
                    }
                    // ���Խ� ���� ���ڿ� ����, % ���� ���ε� Ȯ��
                    string pattern = $@"\{{{_field}_(\d+)\}}(\%)?";
                    Regex regex = new Regex(pattern);

                    MatchCollection matches = regex.Matches(_origin);

                    foreach (Match match in matches)
                    {
                        int index = int.Parse(match.Groups[1].Value); // {Value_i}���� i ����
                        bool isPercent = match.Groups[2].Success; // % ��ȣ ���� ���� Ȯ��
                        if (index >= 0 && index < effectsList.Count)
                        {
                            float value;
                            switch (_field)
                            {
                                default://Value
                                    value = skillEffects[index].value;
                                    break;
                                case "Count":
                                    value = skillEffects[index].count;
                                    break;
                                case "Vamp":
                                    value = skillEffects[index].vamp;
                                    break;
                                case "Duration":
                                    value = skillEffects[index].duration;
                                    break;
                                case "Probability":
                                    value = skillEffects[index].probability;
                                    break;
                            }
                            if (isPercent)
                            {
                                value *= 100;
                            }
                            string replaceStr = value.ToString("0.##");
                            if (isPercent)
                                replaceStr += "%"; // % ��ȣ�� ����� ����
                            if (_field == "Value" && skillEffects[index].valueBase == ValueBase.Ability)
                                replaceStr += "AB";
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
    [Serializable]
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
