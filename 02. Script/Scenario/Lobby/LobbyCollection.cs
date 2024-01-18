using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobbyCollection
{
    public class GuildClass
    {
        public Dictionary<Language, string> name;
        public int index;
        public List<GuildContent> content;
        public string explain;
        public GuildEffectType type;
        public GuildClass(Dictionary<Language, string> _name, int _index, List<GuildContent> _content, string _explain, GuildEffectType _type)
        {
            name = _name;
            index = _index;
            content = _content;
            explain = _explain;
            type = _type;
        }
    }
    public class GuildContent
    {
        public int price;
        public int value;
        public GuildContent(int _price, int _value)
        {
            price = _price;
            value = _value;
        }
    }
    public struct TalentFormStruct
    {
        public Dictionary<Language, string> name;
        public Dictionary<Language, string> explain;
        public int level;
        public int order;
        public List<TalentEffectForm> effects;
        public TalentFormStruct(Dictionary<Language, string> _name, int _level, Dictionary<Language, string> _explain, List<TalentEffectForm> _effects, int _order)
        {
            effects = _effects;
            name = _name;
            explain = _explain;
            level = _level;
            order = _order;
        }
    }
    public struct TalentEffectForm
    {
        public string value;
        public EffectType type;
        public TalentEffectForm(string _value, EffectType _type)
        {
            value = _value;
            type = _type;
        }
    }
    public struct TalentStruct
    {
        public int level;
        public Dictionary<Language, string> name;
        public Dictionary<Language, string> explain;
        public List<TalentEffect> effects;
        public TalentStruct(Dictionary<Language, string> _name, int _level, Dictionary<Language, string> _explain, List<TalentEffect> _effects)
        {
            effects = _effects;
            name = _name;
            level = _level;
            explain = _explain;
        }
    }
    public struct TalentEffect
    {
        public float value;
        public EffectType type;
        public TalentEffect(float _value, EffectType _type)
        {
            value = _value;
            type = _type;
        }
    }
    [System.Serializable]
    public class CandiInfo
    {
        public float hp;
        public float ability;
        public float resist;
        public float speed;
        public string name;
        public List<TalentStruct> talents;
        public CandiInfo(float _hp, float _ability, float _resist, float _speed, string _name, List<TalentStruct> _talents)
        {
            hp = _hp;
            ability = _ability;
            resist = _resist;
            speed = _speed;
            name = _name;
            talents = _talents;
        }
    }
}