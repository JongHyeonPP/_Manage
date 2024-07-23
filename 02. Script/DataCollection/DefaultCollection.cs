using EnumCollection;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultCollection
{
    public class TalentClass
    {
        public Dictionary<Language, string> name;
        public Dictionary<Language, string> explain;
        public int level;
        public int order;
        public List<TalentEffect> effects;
        public TalentClass(Dictionary<Language, string> _name, int _level, Dictionary<Language, string> _explain, List<TalentEffect> _effects, int _order)
        {
            effects = _effects;
            name = _name;
            explain = _explain;
            level = _level;
            order = _order;
        }
    }
    public class TalentEffect
    {
        public List<float> value;
        public EffectType type;
        public TalentEffect(List<float> _value, EffectType _type)
        {
            value = _value;
            type = _type;
        }
    }
    public class BodyPartClass
    {
        public Sprite head;
        public Sprite armL;
        public Sprite armR;
    }
    public class EyeClass
    {
        public Sprite front;
        public Sprite back;
    }

}