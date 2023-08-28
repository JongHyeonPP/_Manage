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
        public SkillTarget target;
        public short targetColumn = -1;
        public float coolTime;
        public bool isSelf;
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
        public SkillForm SetTarget(SkillTarget _target)
        {
            target = _target;
            return this;
        }
        public SkillForm SetCoolTime(float _coolTime)
        {
            coolTime = _coolTime;
            return this;
        }
        public SkillForm SetIsSelf(bool _isSelf)
        {
            isSelf = _isSelf;
            return this;
        }
        public SkillForm SetEffects(List<List<SkillEffect>> _effects)
        {
            effects = _effects;
            return this;
        }
        public SkillForm SetTargetColumn(short _targetColumn)
        {
            targetColumn = _targetColumn;
            return this;
        }
    }
    public class Skill
    {
        public Dictionary<Language, string> name;
        public Dictionary<Language, string> explain;
        public SkillTarget target;
        public short targetColumn;
        public SkillCategori categori;
        public float coolTime;
        public bool isSelf;
        public List<SkillEffect> effects;
        public Skill(SkillForm _skillForm, byte _level)//Skill
        {
            name = _skillForm.name;
            explain = _skillForm.explain[_level];
            target = _skillForm.target;
            categori = _skillForm.categori;
            coolTime = _skillForm.coolTime;
            isSelf = _skillForm.isSelf;
            targetColumn = _skillForm.targetColumn;
            effects = new();
            foreach (var x in _skillForm.effects[_level])
            {
                effects.Add(new SkillEffect().
                    SetCount(x.count).
                    SetCycle(x.cycle).
                    SetIsConst(x.isConst).
                    SetRange(x.range).
                    SetType(x.type).
                    SetValue(x.value).
                    SetDelay(x.delay)
                    );
            }
        }
        public Skill()//Default Attack
        {
            name = new() { {Language.En, "Default Attack" },{ Language.Ko, "기본 공격"} };
            explain = new() { { Language.En, "Default Attack" }, { Language.Ko, "기본 공격" } };
            target = SkillTarget.Nontarget;
            coolTime = 3f;
            isSelf = false;
            targetColumn = -1;
            effects = new() { new SkillEffect().
                SetType(EffectType.Damage).
                SetRange(EffectRange.Dot).
                SetCount(1).
                SetIsConst(false).
                SetValue(1f).
                SetDelay(0f).
                SetCycle(1)};
        }
    }
    public class SkillEffect
    {
        public float value;
        public int count;
        public int cycle;
        public int currentCycle;
        public EffectType type;
        public EffectRange range;
        public bool isConst;
        public float delay;
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
        public SkillEffect SetRange(EffectRange _range)
        {
            range = _range;
            return this;
        }
        public SkillEffect SetIsConst(bool _isConst)
        {
            isConst = _isConst;
            return this;
        }
        public SkillEffect SetCycle(int _cycle)
        {
            cycle = _cycle;
            currentCycle = 0;
            return this;
        }
        public SkillEffect SetDelay(float _delay)
        {
            delay = _delay;
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
        public EnemyStruct(string _name, float _ability, float _hp, float _resist, List<Skill> _skills)
        {
            name = _name;
            ability = _ability;
            hp = _hp;
            skills = _skills;
            resist = _resist;
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
        public CandidateInfoStruct(string _name, float _ability, float _hp ,float _resist, List<TalentStruct> _talent)
        {
            name = _name;
            ability = _ability;
            hp = _hp;
            resist = _resist;
            talents = _talent;
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