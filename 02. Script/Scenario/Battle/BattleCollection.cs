using EnumCollection;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCollection
{

    public class SkillForm
    {
        public Dictionary<Language, string> name;
        public List<Dictionary<Language, string>> explain;
        public SkillCategori categori;
        public float cooltime = 4f;
        public bool isTargetEnemy = false;
        public List<List<SkillEffect>> effects;
        public bool isAnim;
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

        internal SkillForm SetIsAnim(bool _isAnim)
        {
            isAnim = _isAnim;
            return this;
        }
    }
    [Serializable]
    public class Skill
    {
        public Dictionary<Language, string> name;
        public Dictionary<Language, string> explain;
        public SkillCategori categori;
        public float cooltime;
        public List<SkillEffect> effects;
        public bool isTargetEnemy;
        public bool isAnim;
        public Skill(SkillForm _skillForm, byte _level)//Skill
        {
            name = _skillForm.name;
            if (_skillForm.explain != null)
                explain = _skillForm.explain[_level];
            categori = _skillForm.categori;
            cooltime = _skillForm.cooltime;
            isTargetEnemy = _skillForm.isTargetEnemy;
            effects = new();
            effects = _skillForm.effects[_level];
            isAnim = _skillForm.isAnim;
        }
        public Skill()//Default Attack
        {
            name = new() { { Language.En, "Default Attack" }, { Language.Ko, "기본 공격" } };
            explain = new() { { Language.En, "Default Attack" }, { Language.Ko, "기본 공격" } };
            cooltime = 1.5f;
            isTargetEnemy = true;
            isAnim = true;
            effects = new() { new SkillEffect().
                SetType(EffectType.Damage).
                SetCount(1).
                SetIsConst(false).
                SetValue(1f).
                SetDelay(0.5f).
                SetRange(EffectRange.Dot).
                SetIsPassive(false).
                SetVamp(0f)
            };
        }
    }
    public class SkillEffect
    {
        public float value = 1f;
        public int count = 1;
        public EffectType type;
        public bool isConst = false;
        public float delay = 0f;
        public EffectRange range = EffectRange.Dot;
        public bool isPassive = false;
        public float vamp = 0f;
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
        public SkillEffect SetIsConst(bool _isConst)
        {
            isConst = _isConst;
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
    }
    public class JobClass
    {
        public JobType jobType = JobType.None;
        public Dictionary<Language, string> name = null;
        public float hpCoef = 1f;
        public float abilityCoef = 1f;
        public float speedCoef = 1f;
        public JobClass SetJobType(JobType _jobType)
        {
            jobType = _jobType;
            return this;
        }
        public JobClass SetName(Dictionary<Language, string> _name)
        {
            name = _name;
            return this;
        }
        public JobClass SetHpCoef(float _hpCoef)
        {
            hpCoef = _hpCoef;
            return this;
        }
        public JobClass SetAbilityCoef(float _abilityCoef)
        {
            abilityCoef = _abilityCoef;
            return this;
        }
        public JobClass SetSpeedCoef(float _speedCoef)
        {
            speedCoef = _speedCoef;
            return this;
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
    }
    public class EnemyCase
    {
        public List<Tuple<string, int>> enemies;//id, index
        public List<int> levelRange;
        public EnemyCase SetEnemies(List<Tuple<string, int>> _enemies)
        {
            enemies = _enemies;
            return this;
        }
        public EnemyCase SetLevelRange(List<int> _levelRange)
        {
            levelRange = _levelRange;
            return this;
        }
    }

}