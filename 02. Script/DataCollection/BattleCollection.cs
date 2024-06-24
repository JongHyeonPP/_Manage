using DefaultCollection;
using EnumCollection;
using ItemCollection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BattleCollection
{

    [Serializable]
    public class SkillEffect
    {
        public float value = 1f;
        public int count = 1;
        public EffectType type;
        public float delay = 0f;
        public EffectRange range = EffectRange.Dot;
        public bool isPassive = false;
        public float vamp = 0f;
        public bool byAtt = false;
        public ValueBase valueBase;
        public SkillEffect SetValue(float _value)
        {
            value = _value;
            return this;
        }
        public SkillEffect SetCount(int _count)
        {
            count = _count;
            return this;
        }
        public SkillEffect SetType(EffectType _type)
        {
            type = _type;
            return this;
        }
        public SkillEffect SetDelay(float _delay)
        {
            delay = _delay;
            return this;
        }
        public SkillEffect SetRange(EffectRange _range)
        {
            range = _range;
            return this;
        }
        public SkillEffect SetIsPassive(bool _isPassive)
        {
            isPassive = _isPassive;
            return this;
        }
        public SkillEffect SetVamp(float _vamp)
        {
            vamp = _vamp;
            return this;
        }
        public SkillEffect SetByAtt(bool _byAtt)
        {
            byAtt = _byAtt;
            return this;
        }
        public SkillEffect SetValueBase(ValueBase _valueBase)
        { 
            valueBase  = _valueBase;
            return this;
        }
    }
    [Serializable]
    public class JobClass
    {
        public Dictionary<Language, string> name = null;
        public List<SkillEffect> effects;
        public Dictionary<Language, string> effectExplain = null;
        public Dictionary<ClothesPart, Sprite> spriteDict;

        public JobClass(Dictionary<Language,string> _name, List<SkillEffect> _effects,
            Dictionary<Language, string> _effectExplain, Dictionary<ClothesPart, Sprite>_spriteDict)
        {
            name = _name;
            effects = _effects;
            effectExplain = _effectExplain;
            spriteDict = _spriteDict;
        }
    }
    public class EnemyClass
    {
        public Dictionary<Language, string> name;
        public float ability;
        public float hp;
        public float resist;
        public List<Skill> skills;
        public float speed;
        public bool isMonster;
        public string type;
        public int enemyLevel;
        public EnemyClass SetName(Dictionary<Language, string> _name)
        {
            name = _name;
            return this;
        }
        public EnemyClass SetAbility(float _ability)
        {
            ability = _ability;
            return this;
        }
        public EnemyClass SetHp(float _hp)
        {
            hp = _hp;
            return this;
        }
        public EnemyClass SetResist(float _resist)
        {
            resist = _resist;
            return this;
        }
        public EnemyClass SetSkills(List<Skill> _skills)
        {
            skills = _skills;
            return this;
        }
        public EnemyClass SetSpeed(float _speed)
        {
            speed = _speed;
            return this;
        }
        public EnemyClass SetIsMonster(bool _isMonster)
        {
            isMonster = _isMonster;
            return this;
        }
        public EnemyClass SetType(string _type)
        {
            type = _type;
            return this;
        }
        public EnemyClass SetEnemyLevel(int _enemyLevel)
        {
            enemyLevel = _enemyLevel;
            return this;
        }
    }
    public class EnemyCase
    {
        public List<EnemyPieceForm> pieces;//id, index
        public List<int> levelRange;
        public EnemyCase SetEnemies(List<EnemyPieceForm> _pieces)
        {
            pieces = _pieces;
            return this;
        }
        public EnemyCase SetLevelRange(List<int> _levelRange)
        {
            levelRange = _levelRange;
            return this;
        }
    }
    public class EnemyPieceForm
    {
        //셋 중 하나만 있어야 함
        public string id;
        public string type;
        public int enemyLevel = -1;
        //반드시 필요함
        public int index;
        public EnemyPieceForm SetId(string _id)
        {
            id = _id;
            return this;
        }
        public  EnemyPieceForm SetLevel(int _enemyLevel)
        {
            enemyLevel = _enemyLevel;
            return this;
        }
        public EnemyPieceForm SetType(string _type)
        {
            type = _type;
            return this;
        }
        public EnemyPieceForm SetIndex(int _index)
        {
            index = _index;
            return this;
        }
    }
    public class EnemyPiece
    {
        public string id;
        public int index;
        public EnemyPiece(string _id, int _index)
        {
            id = _id;
            index = _index;
        }
    }
    public class VisualEffect
    {
        public GameObject effectObject;
        public float duration;
        public string sound;
        public bool fromRoot;
        public VisualEffect(GameObject _effectObject, float _duration, string _sound, bool _fromRoot)
        {
            effectObject = _effectObject;
            duration = _duration;
            sound = _sound;
            fromRoot = _fromRoot;
        }
    }

}