using EnumCollection;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StructCollection
{
    public struct SettingStruct
    {
        public float allVolume;
        public float sfxVolume;
        public float bgmVolume;
        public Language language;
        public SettingStruct(float _allVolume, float _sfxVolume, float _bgmVolume, Language _language)
        {
            allVolume = _allVolume;
            sfxVolume = _sfxVolume;
            bgmVolume = _bgmVolume;
            language = _language;
        }
    }
    public class SkillForm
    {
        public Dictionary<Language, string> name;
        public List<Dictionary<Language, string>> explain;
        public SkillCategori categori;
        public float cooltime = 3f;
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
        public Skill(float _speed)//Default Attack
        {
            name = new() { {Language.En, "Default Attack" },{ Language.Ko, "기본 공격"} };
            explain = new() { { Language.En, "Default Attack" }, { Language.Ko, "기본 공격" } };
            cooltime = 2 / _speed;
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
        public Dictionary<Language ,string> name = null;
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
    public struct GuildStruct
    {
        public string name;
        public List<int> prices;
        public string explain;
        public GuildStruct(string _name, List<int> _prices, string _explain)
        {
            name = _name;
            prices = _prices;
            explain = _explain;
        }
    }
    public struct CandidateInfoStruct
    {
        public string name;
        public float ability;
        public float hp;
        public float resist;
        public List<TalentStruct> talents;
        public float speed;
        public CandidateInfoStruct SetName(string _name)
        {
            name = _name;
            return this;
        }
        public CandidateInfoStruct SetAbility(float _ability)
        {
            ability = _ability;
            return this;
        }
        public CandidateInfoStruct SetHp(float _hp)
        {
            hp = _hp;
            return this;
        }
        public CandidateInfoStruct SetResist(float _resist)
        {
            resist = _resist;
            return this;
        }
        public CandidateInfoStruct SetTalent(List<TalentStruct> _talents)
        {
            talents = _talents;
            return this;
        }
        public CandidateInfoStruct SetSpeed(float _speed)
        {
            speed = _speed;
            return this;
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
        public string type;
        public TalentEffectForm(string _value, string _type)
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
        public List<SkillEffect> effects;
        public TalentStruct(Dictionary<Language, string> _name, int _level, Dictionary<Language, string> _explain, List<SkillEffect> _effects)
        {
            effects = _effects;
            name = _name;
            level = _level;
            explain = _explain;
        }
    }
}