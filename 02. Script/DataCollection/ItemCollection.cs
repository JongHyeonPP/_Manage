using BattleCollection;
using EnumCollection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        protected Item()
        {
        }
        public virtual string GetExplain()
        {
            return explain[GameManager.language];
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
        public int num;
        public IngredientType type;

        public IngredientClass(ItemType _itemType, string _itemId, ItemGrade _itemGrade, Dictionary<Language, string> _name, Dictionary<Language, string> _explain, Sprite _sprite, Vector2 _scale, Vector2 _position) : base(_itemType,_itemId, _itemGrade, _name,_explain, _sprite, _scale, _position)
        {
            itemType = _itemType;
            itemId = _itemId;
            itemGrade = _itemGrade;
            name = _name;
            sprite = _sprite;
        }
        public IngredientClass SetPokerNum(int _num) 
        {
            num = _num;
            return this;
        }
        public IngredientClass SetIngredientType(IngredientType _type) 
        {
            type = _type;
            return this;
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
        public WeaponType type;
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
            type = _type;
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

    public class SkillForm
    {
        public List<Dictionary<Language, string>> explain;
        public SkillCategori categori;
        public float cooltime;
        public bool isTargetEnemy = false;
        public List<List<SkillEffect>> effects;
        public bool isAnim;
        public List<string> visualEffect;
        public bool isPre = false;
        public Dictionary<Language, string> name;
        public string id;
        public Sprite sprite;
        public Vector2 scale;
        public Vector2 position;

        public SkillForm SetName(Dictionary<Language, string> _name)
        {
            name = _name;
            return this;
        }
        public SkillForm SetExplain(List<Dictionary<Language, string>> _explain)
        {
            explain = _explain;
            return this;
        }
        public SkillForm SetCategori(SkillCategori _categori)
        {
            categori = _categori;
            return this;
        }
        public SkillForm SetCooltime(float _cooltime)
        {
            cooltime = _cooltime;
            return this;
        }
        public SkillForm SetIsTargetEnemy(bool _isTargetEnemy)
        {
            isTargetEnemy = _isTargetEnemy;
            return this;
        }
        public SkillForm SetEffects(List<List<SkillEffect>> _effects)
        {
            effects = _effects;
            return this;
        }
        public SkillForm SetIsAnim(bool _isAnim)
        {
            isAnim = _isAnim;
            return this;
        }
        public SkillForm SetVisualEffect(List<string> _visualEffect)
        {
            visualEffect = _visualEffect;
            return this;
        }
        public SkillForm SetIsPre(bool _isPre)
        {
            isPre = _isPre;
            return this;
        }
        public SkillForm SetScale(Vector2 _scale)
        {
            scale = _scale;
            return this;
        }
        public SkillForm SetPosition(Vector2 _position)
        {
            position = _position;
            return this;
        }
        public SkillForm SetSprite(Sprite _sprite)
        { 
            sprite = _sprite;
            return this;
        }
        public SkillForm SetId(string _id)
        {
            id = _id;
            return this;
        }

        public Skill LocalizeSkill(ItemGrade _grade)//Skill_n/n 형태의 x1을 기반으로 LoadManager에 있는 EffectForm을 가진 SkillStruct 접근해서 Effect를 가진 Skill를 리턴
        {

            return new Skill(this, _grade);
        }

    }
    [Serializable]
    public class Skill:Item
    {
        public SkillCategori categori;
        public float cooltime;
        public List<SkillEffect> effects;
        public bool isTargetEnemy;
        public bool isAnim;
        public VisualEffect visualEffect;
        public static float defaultAttackCooltime = 3f;
        public bool isPre;
        public Skill(ItemType _itemType, string _itemId, ItemGrade _itemGrade, Dictionary<Language, string> _name, Dictionary<Language, string> _explain, Sprite _sprite, Vector2 _scale, Vector2 _position) : base(_itemType, _itemId, _itemGrade, _name,_explain, _sprite, _scale, _position)
        {
        }
        public Skill(SkillForm _skillForm, ItemGrade _grade) : this(ItemType.Skill, _skillForm.id, _grade, _skillForm.name, _skillForm.explain[ConvertGradeToInt(_grade)], _skillForm.sprite, _skillForm.scale, _skillForm.position)
        {
            int gradeNum = ConvertGradeToInt(_grade);
            string gradeStr = ConvertGradeToString(_grade);
            if (_skillForm.explain != null)
                explain = _skillForm.explain[gradeNum];
            categori = _skillForm.categori;
            cooltime = _skillForm.cooltime;
            isTargetEnemy = _skillForm.isTargetEnemy;
            effects = new();
            effects = _skillForm.effects[gradeNum];
            isAnim = _skillForm.isAnim;
            if (_skillForm.visualEffect != null)
            {
                string veName = _skillForm.visualEffect[gradeNum];
                if (veName != string.Empty)
                    visualEffect = null;
            }
            isPre = _skillForm.isPre;
            itemId = $"{_skillForm.id}:::{gradeStr}";
        }

        public Skill()//Default Attack
        {
            cooltime = defaultAttackCooltime;
            isTargetEnemy = true;
            isAnim = true;
            isPre = false;
            effects = new()
            {
                new SkillEffect().
                SetType(EffectType.Damage).
                SetCount(1).
                SetValueBase(ValueBase.Ability).
                SetValue(1f).
                SetDelay(0.5f).
                SetRange(EffectRange.Dot).
                SetIsPassive(false).
                SetVamp(0f).
                SetByAtt(false)
            };
        }
        
        private static int ConvertGradeToInt(ItemGrade grade)
        {
            switch (grade)
            {
                case ItemGrade.Rare:
                    return 1;
                case ItemGrade.Unique:
                    return 2;
                default:
                    return 0;
            }
        }

        private static string ConvertGradeToString(ItemGrade grade)
        {
            switch (grade)
            {
                case ItemGrade.Rare:
                    return "Rare";
                case ItemGrade.Unique:
                    return "Unique";
                default:
                    return "Normal";
            }
        }
        public override string GetExplain()
        {
            string originStr = explain[GameManager.language];
            return ReplaceValues(originStr);

        }
        public string ReplaceValues(string original)
        {
            original = ReplaceByRegex(original, "Value");
            original = ReplaceByRegex(original, "Count");
            original = ReplaceByRegex(original, "Vamp");
            return original;

            string ReplaceByRegex(string original, string _field)
            {
                string fontColor;
                float fontSize;
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

                MatchCollection matches = regex.Matches(original);

                foreach (Match match in matches)
                {
                    int index = int.Parse(match.Groups[1].Value); // {Value_i}에서 i 추출
                    bool isPercent = match.Groups[2].Success; // % 기호 존재 여부 확인
                    if (index >= 0 && index < effects.Count)
                    {
                        string replaceStr;
                        switch (_field)
                        {
                            default://Value
                                replaceStr = effects[index].value.ToString();
                                if (effects[index].valueBase == ValueBase.Ability)
                                    replaceStr += "AB";
                                break;
                            case "Count":
                                replaceStr = effects[index].count.ToString();
                                break;
                            case "Vamp":
                                replaceStr = effects[index].vamp.ToString();
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
                        original = original.Replace(match.Value, replaceStr);
                        original = original.Replace("\\n", "\n");
                    }
                }

                return original;
            }
        }

    }
}