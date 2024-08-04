using DefaultCollection;
using EnumCollection;
using HardLight2DUtil;
using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleCollection
{
    [Serializable]
    public class SkillInBattle
    {
        public static float defaultAttackCooltime = 3f;
        public float cooltime;
        public List<SkillEffect> effects = new();
        public bool isAnim;
        public VisualEffect visualEffect { get; set; }
        public bool isPre;
        public SkillCategori skillCategori;
        public SkillInBattle(float _coolTime, List<SkillEffect> _effects, bool _isAnim, VisualEffect _visualEffect, bool _isPre, SkillCategori _skillCategori)
        {
            cooltime = _coolTime;
            effects = _effects;
            isAnim = _isAnim;
            visualEffect = _visualEffect;
            isPre = _isPre;
            skillCategori = _skillCategori;
        }


        public List<PassiveEffect> GetPassiveEffects(BaseInBattle _caster)
        {
            List<PassiveEffect> passiveEffects = new();
            foreach (var effect in effects)
            {
                if (!effect.isPassive)
                    continue;
                PassiveEffect passiveEffect = (PassiveEffect)effect;
                passiveEffect.value = _caster.CalcEffectValueByType(passiveEffect.value, effect.effectType);
                passiveEffect.SetTargets(_caster);
                passiveEffect.ApplyToTarget();
                passiveEffects.Add(passiveEffect);
            }
            return passiveEffects;
        }
    }

    public abstract class SkillEffect
    {
        public int count;
        public bool isPassive;
        public float value;
        public EffectType effectType;
        public EffectRange range = EffectRange.Dot;
        public ValueBase valueBase;
        public bool isTargetEnemy;
        public float vamp;//설명에 띄워야해서 여기 있어야함.
        public SkillEffect(int _count ,bool _isPassive, float _value, EffectType _type, EffectRange _range, ValueBase _valueBase, bool _isTargetEnemy, float _vamp)
        {
            count = _count;
            isPassive = _isPassive;
            value = _value;
            effectType = _type;
            range = _range;
            valueBase = _valueBase;
            isTargetEnemy = _isTargetEnemy;
            vamp = _vamp;
        }

    }
    [Serializable]

    public class PassiveEffect:SkillEffect
    {
        public bool byAtt = false;

        public List<BaseInBattle> targets = new();

        public PassiveEffect(int _count, bool _isPassive, float _value, EffectType _type, EffectRange _range, ValueBase _valueBase, bool _isTargetEnemy, float _vamp, bool _byAtt) : base(_count, _isPassive, _value, _type, _range, _valueBase, _isTargetEnemy, _vamp)
        {
            byAtt = _byAtt;
        }

        public void SetTargets(BaseInBattle _caster)
        {
            BaseInBattle target;
            if (isTargetEnemy)//가장 가까운 적
            {
                target = _caster.targetOpponent;
            }
            else
            {
                target = _caster;
            }

            targets = BattleScenario.GetTargetsByRange(range, target);
        }
        internal void ApplyToTarget()
        {
            foreach (var x in targets)
            {
                x.ApplyValue(value, effectType);
            }
        }

        internal void RemoveToTarget()
        {
            foreach (var x in targets)
            {
                x.ApplyValue(-value, effectType);
            }
        }
    }
    public class ActiveEffect : SkillEffect
    {
        public float delay;

        public BaseInBattle caster;

        public ActiveEffect(int _count, bool _isPassive, float _value, EffectType _type, EffectRange _range, ValueBase _valueBase, bool _isTargetEnemy, float _vamp, float _delay) : base(_count, _isPassive, _value, _type, _range, _valueBase, _isTargetEnemy, _vamp)
        {
            delay = _delay;
        }

        public ActiveEffect SetCaster(BaseInBattle _caster)
        {
            caster = _caster;
            return this;
        }
        public void ActiveEffect0nTarget(BaseInBattle target)
        {
            float calcValue = value;
            switch (valueBase)
            {
                case ValueBase.Ability:
                    calcValue *= caster.abilityInBattle;
                    break;
                case ValueBase.Armor:
                    calcValue *= caster.armor;
                    break;
                case ValueBase.HpMax_Enemy:
                    calcValue *= target.maxHp;
                    break;
                case ValueBase.HpMax_Caster:
                    calcValue *= caster.maxHp;
                    break;
            }
            calcValue = caster.CalcEffectValueByType(calcValue, effectType);
            float calcTemp = calcValue;

            calcValue = calcTemp;
            switch (effectType)
            {
                case EffectType.Damage://타겟에 대한 보정값
                    calcValue *= BattleScenario.CalcResist(target.resistInBattle);
                    calcValue -= target.GetRegularValue(EffectType.Reduce);
                    calcValue = Mathf.Max(calcValue, 0);
                    foreach (KeyValuePair<EffectType, float> kvp in caster.EffectsByAtt)
                    {
                        target.ApplyValue(kvp.Value, kvp.Key);
                    }
                    break;
                case EffectType.Curse:
                    calcValue *= target.Hp;
                    break;
            }
            caster.StartCoroutine(RoopEffect(calcValue, target));
        }

        private IEnumerator RoopEffect(float calcValue, BaseInBattle _target)
        {
            for (int i = 0; i < count; i++)
            {
                //능력치 흡수
                if (effectType == EffectType.AbilityVamp)
                {
                    caster.abilityInBattle += _target.abilityInBattle * calcValue;
                }
                _target.ApplyValue(calcValue, effectType);//////핵심

                //대미지 후속작업
                if (effectType == EffectType.Damage)
                {
                    float vampValue = vamp + caster.GetRegularValue(EffectType.Vamp);
                    vampValue *= 1 + caster.GetRegularValue(EffectType.ResilienceAscend);
                    //흡수하는 채력
                    if (vampValue > 0)
                    {
                        caster.Hp += vampValue * calcValue;
                    }
                    //흡수되는 능력치
                    if (_target.GetRegularValue(EffectType.ResistByDamage) > 0)
                    {
                        float resistValue = caster.resistInBattle * _target.GetRegularValue(EffectType.ResistByDamage);
                        caster.resistInBattle -= resistValue;
                        _target.resistInBattle += resistValue;
                    }
                    if (_target.GetRegularValue(EffectType.Reflect) > 0)
                    {
                        caster.Hp -= calcValue * _target.GetRegularValue(EffectType.Reflect);
                    }
                }
                yield return new WaitForSeconds(delay);
            }
        }


    }
    public class JobClass
    {
        public Dictionary<Language, string> name = null;
        public Skill jobSkill;
        public Dictionary<ClothesPart, Sprite> spriteDict;
        public string jobId;
        public Sprite jobIcon;
        public JobClass(Dictionary<Language, string> _name, Skill _jobSkill, Dictionary<ClothesPart, Sprite> _spriteDict, string _jobId, Sprite _jobIcon)
        {
            name = _name;
            jobSkill = _jobSkill;
            spriteDict = _spriteDict;
            jobId = _jobId;
            jobIcon = _jobIcon;
        }
    }
    public class EnemyClass
    {
        public Dictionary<Language, string> name;
        public float ability;
        public float hp;
        public float resist;
        public List<SkillInBattle> skills;
        public float speed;
        public string type { get; set; }
        public int enemyLevel;

        public EnemyClass(Dictionary<Language, string> _name, float _ability, float _hp, float _resist, List<SkillInBattle> _skills, float _speed, string _type, int _enemyLevel)
        {
            name = _name;
            ability = _ability;
            hp = _hp;
            resist = _resist;
            skills = _skills;
            speed = _speed;
            type = _type;
            enemyLevel = _enemyLevel;
        }
    }
    public class EnemyCase
    {
        public List<EnemyPieceForm> pieces;//id, index
        public EnemyCase(List<EnemyPieceForm> _pieces)
        {
            pieces = _pieces;
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
        public EnemyPieceForm SetLevel(int _enemyLevel)
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
    [Serializable]
    public class SkillActiveForm
    {
        readonly float skillCastTime = 1f;

        public BaseInBattle caster;
        public List<ActiveEffect> effectActiveForms = new();
        public SkillInBattle skillInBattle;
        public CooldownSlot cooldownSlot; 
        public SkillActiveForm(BaseInBattle _caster, SkillInBattle _skillInBattle , CooldownSlot _cooldownSlot)
        {
            cooldownSlot = _cooldownSlot;
            skillInBattle = _skillInBattle;
            caster = _caster;
            foreach (SkillEffect effect in skillInBattle.effects)
            {
                if (!effect.isPassive)
                {
                    ActiveEffect activeEffect = ((ActiveEffect)effect).SetCaster(_caster);
                    effectActiveForms.Add(activeEffect);
                }
            }
        }
        //Default Attack
        public SkillActiveForm(BaseInBattle _caster)
        {
            caster = _caster;
            List<SkillEffect> skillEffects = new List<SkillEffect>() { new ActiveEffect(1, false, 1f, EffectType.Damage, EffectRange.Dot, ValueBase.Ability, true, 0f, 0f) };
            skillInBattle = new SkillInBattle(SkillInBattle.defaultAttackCooltime, skillEffects, true, LoadManager.loadManager.skillVisualEffectDict["Default"], false, SkillCategori.Default);
            effectActiveForms.Add(((ActiveEffect)skillInBattle.effects[0]).SetCaster(caster));
        }

        public IEnumerator ActiveSkill()
        {
            BaseInBattle confusedTarget = null;
            bool isParalyze = false;
            float confuseProb = caster.GetRegularValue(EffectType.Confuse);
            //if (GameManager.CalculateProbability(confuseProb))
            //{
            //    List<BaseInBattle> confusedTargetsBase = (caster.IsEnemy ^ skill.isTargetEnemy ? BattleScenario.characters : BattleScenario.enemies).Where(item => !item.isDead).ToList();
            //    confusedTarget = confusedTargetsBase[Random.Range(0, confusedTargetsBase.Count)];
            //}
            //else
            //{
            float paralyzeProb = caster.GetRegularValue(EffectType.Paralyze);
            isParalyze = GameManager.CalculateProbability(paralyzeProb);
            //}

            if (!caster.isDead)
            {
                if (isParalyze)
                {
                    //마비마비맨
                }
                else
                {
                    List<GridObject> targetGrids = new();
                    foreach (ActiveEffect effectForm in effectActiveForms)
                    {
                        BaseInBattle effectTarget;
                        effectTarget = GetTarget(effectForm);
                        List<GridObject> tempGrids = BattleScenario.GetTargetGridsByRange(effectForm.range, effectTarget.grid);
                        foreach (GridObject grid in tempGrids)
                        {
                            if (!targetGrids.Contains(grid))
                            {
                                targetGrids.Add(grid);
                            }
                        }
                    }
                    if (skillInBattle.isPre)
                    {
                        caster.fireObj.SetActive(true);
                        foreach (var x in targetGrids)
                        {
                            x.PreActive();
                        }
                    }
                    float repeatValue = caster.GetRegularValue(EffectType.Repeat);
                    for (int i = 0; i < ((repeatValue > 0) ? 2 : 1); i++)
                    {

                        foreach (ActiveEffect effectForm in effectActiveForms)
                        {
                            BaseInBattle effectTarget;
                            effectTarget = GetTarget(effectForm);
                            List<BaseInBattle> targets = BattleScenario.GetTargetsByRange(effectForm.range, effectTarget);
                            if (skillInBattle.isPre)
                                yield return new WaitForSeconds(3f);
                            foreach (BaseInBattle target in targets)
                            {
                                if (target.gameObject.activeSelf)
                                    effectForm.ActiveEffect0nTarget(target);/////핵심
                                if (skillInBattle.visualEffect != null)
                                {
                                    GameManager.battleScenario.CreateVisualEffect(skillInBattle.visualEffect, target, true);
                                }
                            }
                        }
                    }
                    for (int i = 0; i < ((repeatValue > 0) ? 2 : 1); i++)
                    {

                        //Weapon
                        if (caster.weapon != null)
                            caster.StartCoroutine(WeaponVisualEffect());
                        if (skillInBattle.isAnim)
                        {

                            SkillAnim();
                            yield return new WaitForSeconds(skillCastTime);
                        }
                    }
                    if (skillInBattle.isPre)
                        foreach (GridObject x in targetGrids)
                        {
                            if (caster.fireObj)
                                caster.fireObj.SetActive(false);
                            x.PreInactive();
                        }
                }
            }


            void SkillAnim()
            {
                float minValue = SkillInBattle.defaultAttackCooltime;
                float maxValue = 8;
                if (skillInBattle.isAnim)
                    caster.animator.speed = 0.5f;
                else
                    caster.animator.speed = 1f;
                if (!caster.isMonster)
                    caster.animator.SetFloat("AttackState", (skillInBattle.cooltime - minValue) / (maxValue - minValue));
                //caster.animator.SetFloat("NormalState", 0.5f);
                //caster.animator.SetFloat("SkillState", 0.5f);
                bool isTagetEnemy = skillInBattle.effects.Where(item => item.isPassive == false).FirstOrDefault().isTargetEnemy;
                if (isTagetEnemy)
                    caster.animator.SetTrigger("Attack");
                else
                    caster.animator.SetTrigger("Buff");
            }

            IEnumerator WeaponVisualEffect()
            {
                switch (caster.weapon.weaponType)
                {
                    case WeaponType.Bow:
                        yield return new WaitForSeconds(0.3f);
                        break;
                }
                if (skillInBattle.skillCategori == SkillCategori.Default)
                    GameManager.battleScenario.CreateVisualEffect(caster.weapon.defaultVisualEffect, caster, false);
                else
                    GameManager.battleScenario.CreateVisualEffect(caster.weapon.skillVisualEffect, caster, false);
            }


    }
        BaseInBattle GetTarget(ActiveEffect effectForm)
        {
            BaseInBattle effectTarget;
            switch (effectForm.isTargetEnemy)
            {
                case true:
                    switch (effectForm.range)
                    {
                        default:
                            effectTarget = caster.targetOpponent;
                            break;
                    }
                    break;
                case false:
                    switch (effectForm.range)
                    {
                        default:
                            effectTarget = caster;
                            break;
                        case EffectRange.Dot:
                            effectTarget = caster.targetAlly;
                            break;
                    }
                    break;
            }

            return effectTarget;
        }
    }
    
}