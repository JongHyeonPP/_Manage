using EnumCollection;
using System.Collections.Generic;

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
        public float coolTime;
        public bool isTargetEnemy;
        public List<List<SkillEffect>> effects;
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
        public SkillForm SetCoolTime(float _coolTime)
        {
            coolTime = _coolTime;
            return this;
        }
        public SkillForm SetIsSelf(bool _isSelf)
        {
            isTargetEnemy = _isSelf;
            return this;
        }
        public SkillForm SetEffects(List<List<SkillEffect>> _effects)
        {
            effects = _effects;
            return this;
        }
    }
    public class Skill
    {
        public Dictionary<Language, string> name;
        public Dictionary<Language, string> explain;
        public SkillCategori categori;
        public float coolTime;
        public List<SkillEffect> effects;
        public bool isTargetEnemy;
        public Skill(SkillForm _skillForm, byte _level)//Skill
        {
            name = _skillForm.name;
            explain = _skillForm.explain[_level];
            categori = _skillForm.categori;
            coolTime = _skillForm.coolTime;
            isTargetEnemy = _skillForm.isTargetEnemy;
            effects = new();
            foreach (var x in _skillForm.effects[_level])
            {
                effects.Add(new SkillEffect().
                    SetCount(x.count).
                    SetIsConst(x.isConst).
                    SetType(x.type).
                    SetValue(x.value).
                    SetDelay(x.delay)
                    );
            }
        }
        public Skill(float _speed)//Default Attack
        {
            name = new() { {Language.En, "Default Attack" },{ Language.Ko, "기본 공격"} };
            explain = new() { { Language.En, "Default Attack" }, { Language.Ko, "기본 공격" } };
            coolTime = 1 / _speed;
            isTargetEnemy = true;
            effects = new() { new SkillEffect().
                SetType(EffectType.Damage).
                SetCount(1).
                SetIsConst(false).
                SetValue(1f).
                SetDelay(0f)
            };
        }
    }
    public class SkillEffect
    {
        public float value;
        public int count;
        public EffectType type;
        public bool isConst;
        public float delay;
        public EffectRange range;
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
    }
    public struct JobStruct
    {
        public string name;
        public float hpCoef;
        public float abilityCoef;
        public JobStruct(string _name, float _hpCoef, float _abilityCoef)
        {
            name = _name;
            hpCoef = _hpCoef;
            abilityCoef = _abilityCoef;
        }
    }
    public struct EnemyStruct
    {
        public string name;
        public float ability;
        public float hp;
        public float resist;
        public List<Skill> skills;
        public float speed;
        public EnemyStruct SetName(string _name)
        {
            name = _name;
            return this;
        }
        public EnemyStruct SetAbility(float _ability)
        {
            ability = _ability;
            return this;
        }
        public EnemyStruct SetHp(float _hp)
        {
            hp = _hp;
            return this;
        }
        public EnemyStruct SetResist(float _resist)
        {
            resist = _resist;
            return this;
        }
        public EnemyStruct SetSkills(List<Skill> _skills)
        {
            skills = _skills;
            return this;
        }
        public EnemyStruct SetSpeed(float _speed)
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
    }public struct TalentStruct
    {
        public int level;
        public Dictionary<Language, string> name;
        public Dictionary<Language, string> explain;
        public List<T_Effect> effects;
        public TalentStruct(Dictionary<Language, string> _name, int _level, Dictionary<Language, string> _explain, List<T_Effect> _effects)
        {
            effects = _effects;
            name = _name;
            level = _level;
            explain = _explain;
        }
    }
    public struct T_Effect
    {
        public float value;
        public T_Type type;
        public T_Effect(float _value, string _type)
        {
            value = _value;
            switch (_type)
            {
                case "AttAscend":
                    type = T_Type.AttAscend;
                    break;
                case "DefAscend":
                    type = T_Type.DefAscend;
                    break;
                case "AttDescend":
                    type = T_Type.AttDescend;
                    break;
                case "DefDescend":
                    type = T_Type.DefDescend;
                    break;
                case "FameAscend":
                    type = T_Type.FameAscend;
                    break;
                case "GoldAscend":
                    type = T_Type.GoldAscend;
                    break;
                case "Critical":
                    type = T_Type.Critical;
                    break;
                case "HealAscend":
                    type = T_Type.HealAscend;
                    break;
                case "BuffAscend":
                    type = T_Type.BuffAscend;
                    break;
                case "DebuffAscend":
                    type = T_Type.DebuffAscend;
                    break;
                default:
                    type = T_Type.None;
                    break;
            }
        }
    }
}