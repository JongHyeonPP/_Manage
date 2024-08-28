using DefaultCollection;
using EnumCollection;
using HardLight2DUtil;
using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

namespace BattleCollection
{
    [Serializable]
    public class SkillInBattle
    {
        public static float defaultAttackCooltime = 3f;
        public float cooltime;
        public List<SkillEffect> effects = new();
        public bool isPre;
        public SkillCategori skillCategori;
        public SkillInBattle(float _coolTime, List<SkillEffect> _effects, bool _isPre, SkillCategori _skillCategori)
        {
            cooltime = _coolTime;
            effects = _effects;
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
                passiveEffect.SetCaster(_caster);
                passiveEffect.ApplyTempEffectToTarget();
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
        public float vamp;
        public BaseInBattle caster;
        public float duration;
        public float probability;
        public SkillEffect(int _count ,bool _isPassive, float _value, EffectType _type, EffectRange _range, ValueBase _valueBase, bool _isTargetEnemy, float _vamp, float _duration, float _probability)
        {
            count = _count;
            isPassive = _isPassive;
            value = _value;
            effectType = _type;
            range = _range;
            valueBase = _valueBase;
            isTargetEnemy = _isTargetEnemy;
            vamp = _vamp;
            duration = _duration;
            probability = _probability;
        }

        public SkillEffect SetCaster(BaseInBattle _caster)
        {
            caster = _caster;
            return this;
        }
    }
    [Serializable]

    public class PassiveEffect:SkillEffect
    {
        public bool byAtt = false;
        public float calcValue;
        public List<TempEffect> tempEffects;


        public PassiveEffect(int _count, bool _isPassive, float _value, EffectType _type, EffectRange _range, ValueBase _valueBase, bool _isTargetEnemy, float _vamp, float _duration, float _probability) : base(_count, _isPassive, _value, _type, _range, _valueBase, _isTargetEnemy, _vamp, _duration, _probability)
        {
        }

        public PassiveEffect SetByAtt(bool _byAtt)
        {
            byAtt = _byAtt;
            return this;
        } 
        internal void ApplyTempEffectToTarget()
        {
            tempEffects = new();
            BaseInBattle effectTarget = GameManager.battleScenario.GetTarget(this, caster);
            List<BaseInBattle> targets = BattleScenario.GetTargetsByRange(range, effectTarget);
            switch (effectType)
            {
                default:
                    float calcedValue = caster.CalcEffectValueByType(value, effectType);
                    foreach (BaseInBattle target in targets)
                    {
                        TempEffect tempEffect = target.ApplyPassiveEffect(calcedValue, effectType, valueBase, caster, duration);
                        tempEffects.Add(tempEffect);
                    }
                    break;
                case EffectType.RewardAscend:
                    BattleScenario.rewardAscend += value;
                    break;
            }
        }

        internal void RemoveTempEffectToTarget()
        {
            foreach (TempEffect tempEffect in tempEffects)
            {
                tempEffect.RemoveFromList();
            }
        }
    }
    public class ActiveEffect : SkillEffect
    {
        public float delay;
        public bool isAnim;
        public VisualEffect visualEffect;

        public ActiveEffect(int _count, bool _isPassive, float _value, EffectType _type, EffectRange _range, ValueBase _valueBase, bool _isTargetEnemy, float _vamp, float _duration, float _probability) 
            : base(_count, _isPassive, _value, _type, _range, _valueBase, _isTargetEnemy, _vamp, _duration, _probability)
        {
        }

        public ActiveEffect SetDelay(float _delay)
        {
            delay = _delay;
            return this;
        }
        public ActiveEffect SetIsAnim(bool _isAnim)
        {
            isAnim = _isAnim;
            return this;
        }
        public ActiveEffect SetVisualEffect(VisualEffect _visualEffect)
        {
            visualEffect = _visualEffect;
            return this;
        }
        public ActiveEffect SetProbability(float _probability)
        {
            probability = _probability;
            return this;
        }
        public void ActiveEffect0nTarget(BaseInBattle target, float _valueRate)
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
            calcValue *= _valueRate;
            caster.StartCoroutine(RoopEffect(calcValue, target, valueBase, caster));
        }

        private IEnumerator RoopEffect(float _calcedValue, BaseInBattle _target, ValueBase _valueBase, BaseInBattle _caster)
        {
            for (int i = 0; i < count; i++)
            {
                float beforeAbilityVamp = _target.abilityInBattle;
                _target.ApplyValue(_calcedValue, effectType, _valueBase, _caster, duration);//////핵심
                //능력치 흡수
                if (effectType == EffectType.AbilityVamp)
                {
                    caster.abilityInBattle += beforeAbilityVamp * _calcedValue;
                }
                //대미지 후속작업
                if (effectType == EffectType.Damage)
                {
                    float vampValue = vamp + caster.GetTempValue_Sum(EffectType.Vamp);
                    //흡수하는 채력
                    if (vampValue > 0)
                    {
                        caster.ApplyValue(vampValue * _calcedValue, EffectType.Heal);
                    }
                    //흡수되는 능력치
                    float resisbyDamage = _target.GetTempValue_Sum(EffectType.ResistByDamage);
                    if (resisbyDamage > 0)
                    {
                        caster.resistInBattle -= resisbyDamage;
                        _target.resistInBattle += resisbyDamage;
                    }
                    if (_target.GetTempValue_Sum(EffectType.Reflect) > 0)
                         _target.StartCoroutine(ReflectCoroutine(_calcedValue, _target));
                }
                yield return new WaitForSeconds(delay);
            }
        }

        private IEnumerator ReflectCoroutine(float calcValue, BaseInBattle _target)
        {
            yield return new WaitForSeconds(0.5f);
                caster.ApplyValue(calcValue * _target.GetTempValue_Max(EffectType.Reflect), EffectType.Damage);
        }
    }
    public class JobClass
    {
        public Dictionary<Language, string> name = null;
        public Dictionary<Language, string> skillName = null;
        public Skill jobSkill;
        public Dictionary<ClothesPart, Sprite> spriteDict;
        public string jobId;
        public Sprite jobIcon;
        public JobClass(string _jobId, Dictionary<Language, string> _name,Dictionary<Language, string> _skillName, Skill _jobSkill, Dictionary<ClothesPart, Sprite> _spriteDict, Sprite _jobIcon)
        {
            name = _name;
            skillName = _skillName;
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
        public float scale;
        public string type { get; set; }
        public int enemyLevel;

        public EnemyClass(Dictionary<Language, string> _name, float _ability, float _hp, float _resist, List<SkillInBattle> _skills, float _speed,float _scale, string _type, int _enemyLevel)
        {
            name = _name;
            ability = _ability;
            hp = _hp;
            resist = _resist;
            skills = _skills;
            speed = _speed;
            scale = _scale;
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
        readonly float skillCastTime = 0.5f;

        public BaseInBattle caster;
        public List<ActiveEffect> activeEffects = new();
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
                    ActiveEffect activeEffect = (ActiveEffect)effect.SetCaster(_caster);
                    activeEffects.Add(activeEffect);
                }
            }
        }
        public SkillActiveForm(BaseInBattle _caster)
        {
            caster = _caster;
            List<SkillEffect> skillEffects = new List<SkillEffect>()
            {
                new ActiveEffect(1, false, 1f, EffectType.Damage, EffectRange.Dot, ValueBase.Ability, true, 0f, -99f, 1f)
            .SetDelay(0f).SetVisualEffect(LoadManager.loadManager.skillVisualEffectDict["Default"]).SetIsAnim(true).SetProbability(1f)
            };

            skillInBattle = new SkillInBattle(SkillInBattle.defaultAttackCooltime, skillEffects, false, SkillCategori.Default);
            activeEffects.Add((ActiveEffect)skillInBattle.effects[0].SetCaster(caster));
        }

        public IEnumerator ApplySkill()
        {
            BaseInBattle confusedTarget = null;
            bool isConfuse = false;
            bool isParalyze = false;
            float confuseProb = caster.GetTempValue_Max(EffectType.Confuse);
            if (GameManager.CalculateProbability(confuseProb))
            {
                isConfuse = true;
            }
            else
            {
                float paralyzeProb = caster.GetTempValue_Max(EffectType.Paralyze);
                isParalyze = GameManager.CalculateProbability(paralyzeProb);
            }

            if (!caster.isDead)
            {
                if (isParalyze)
                {
                    string message = GameManager.language == Language.Ko ? "마비됨" : "Paralyzed";
                    caster.showDamage.StartShowText(message, Color.white);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    List<GridObject> preGrids = new();
                    //Pre
                    if (skillInBattle.isPre)
                    {

                        foreach (ActiveEffect effectForm in activeEffects)
                        {
                            BaseInBattle effectTarget;
                            effectTarget = GameManager.battleScenario.GetTarget(effectForm, caster);

                            List<GridObject> tempGrids = BattleScenario.GetTargetGridsByRange(effectForm.range, effectTarget.grid);
                            foreach (GridObject grid in tempGrids)
                            {
                                if (!preGrids.Contains(grid))
                                {
                                    preGrids.Add(grid);
                                }
                            }
                        }
                        caster.fireObj.SetActive(true);
                        foreach (var x in preGrids)
                        {
                            x.PreActive();
                        }
                        yield return new WaitForSeconds(3f);
                    }
                    //Pre
                    float repeatValue = caster.GetTempValue_Max(EffectType.Repeat);

                    foreach (var x in activeEffects)
                    {
                        if (x.isAnim)
                            CharacterAnim();
                    }
                    if (caster.weapon == null)
                    {
                        yield return new WaitForSeconds(0.75f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(GetPreDelay(caster.weapon.weaponType));//선딜

                    }
                    if (caster.weapon != null)
                    {
                        foreach (var x in activeEffects)
                            if (x.isAnim)
                                WeaponVisualEffect();
                    }
                    foreach (ActiveEffect activeEffect in activeEffects)
                    {
                        BaseInBattle effectTarget;

                        effectTarget = GameManager.battleScenario.GetTarget(activeEffect, caster);
                        if (effectTarget == null)
                        {
                            yield break;
                        }
                        if (isConfuse && effectTarget == caster.targetOpponent)
                        {
                            effectTarget = caster.targetAlly;
                            string message = GameManager.language == Language.Ko ? "혼란!" : "Confused!";
                            caster.showDamage.StartShowText(message, Color.white);
                        }
                        List<BaseInBattle> targets = BattleScenario.GetTargetsByRange(activeEffect.range, effectTarget);

                        if (GameManager.CalculateProbability(activeEffect.probability))
                            foreach (BaseInBattle target in targets)
                            {

                                if (target.gameObject.activeSelf)
                                {
                                    int count = ((repeatValue > 0) ? 2 : 1);
                                    for (int i = 0; i < count; i++)
                                    {
                                        float valueRate;
                                        {
                                            if (i == 0)
                                            {
                                                valueRate = 1f;
                                            }
                                            else
                                            {
                                                valueRate = repeatValue;
                                            }
                                        }
                                        activeEffect.ActiveEffect0nTarget(target, valueRate);/////핵심
                                    }
                                }
                                if (activeEffect.visualEffect != null)
                                    GameManager.battleScenario.CreateVisualEffect(activeEffect.visualEffect, target, true);
                            }
                    }
                    if (caster.weapon == null)
                    {
                        yield return new WaitForSeconds(0.75f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(GetPostDelay(caster.weapon.weaponType));//후딜
                    }


                    if (skillInBattle.isPre)
                        foreach (GridObject x in preGrids)
                        {
                            if (caster.fireObj)
                                caster.fireObj.SetActive(false);
                            x.PreInactive();
                        }
                }
            }


            void CharacterAnim()
            {

                float minValue = SkillInBattle.defaultAttackCooltime;
                float maxValue = 8;
                if (!caster.isMonster)
                    caster.animator.SetFloat("AttackState",(skillInBattle.cooltime - minValue) / (maxValue - minValue));
                float stateNum;
                if (caster.weapon != null)
                {
                    switch (caster.weapon.weaponType)
                    {
                        default:
                            stateNum = 0f;
                            break;
                        case WeaponType.Bow:
                            stateNum = 0.5f;
                            break;
                        case WeaponType.Magic:
                            stateNum = 1f;
                            break;
                    }
                    caster.animator.SetFloat("NormalState", stateNum);
                    caster.animator.SetFloat("SkillState", stateNum);
                }
                bool isTargetEnemy = skillInBattle.effects.Any(item => item.isTargetEnemy);
                caster.animator.speed = caster.speedInBattle;
                if (isTargetEnemy)
                    caster.animator.SetTrigger("Attack");
                else
                    caster.animator.SetTrigger("Buff");

                caster.animator.speed = 1f;
            }
            void WeaponVisualEffect()
            {
                if (skillInBattle.skillCategori == SkillCategori.Default)
                    GameManager.battleScenario.CreateVisualEffect(caster.weapon.defaultVisualEffect, caster, false);
                else
                    GameManager.battleScenario.CreateVisualEffect(caster.weapon.skillVisualEffect, caster, false);
            }

        }

        private float GetPreDelay(WeaponType _weaponType)
        {
            float returnValue = caster.animator.GetFloat("AttackState")*0.5f;
            
            switch (_weaponType)
            {
                default:
                    returnValue += 0.5f;
                    break;
                case WeaponType.Bow:
                    returnValue +=0.5f;
                    break;
                case WeaponType.Magic:
                    returnValue += 0.5f;
                    break;
            }
            returnValue /= caster.speedInBattle;
            return returnValue;
        }
        private float GetPostDelay(WeaponType _weaponType)
        {
            float returnValue;
            switch (_weaponType)
            {
                default:
                    returnValue =  1f;
                    break;
                case WeaponType.Bow:
                    returnValue =  1f;
                    break;
                case WeaponType.Magic:
                    returnValue =  1f;
                    break;
            }
            returnValue /= caster.speedInBattle;
            return returnValue;
        }
    }
    public class TempEffect
    {
        public float value;
        public float duration;
        public ValueBase valueBase;
        public BaseInBattle caster;
        public BaseInBattle target;
        public EffectType effectType;

        public TempEffect(float _value, float _duration, ValueBase _valueBase, BaseInBattle _caster, BaseInBattle _target, EffectType _effectType)
        {
            value = _value;
            duration = _duration;
            valueBase = _valueBase;
            caster = _caster;
            target = _target;
            effectType = _effectType;
            switch (_effectType)
            {
                case EffectType.Bleed:
                case EffectType.AttAscend:
                case EffectType.DefAscend:
                case EffectType.ResistAscend:
                case EffectType.AttDescend:
                case EffectType.DefDescend:
                case EffectType.ResistDescend:
                case EffectType.SpeedAscend:
                case EffectType.SpeedDescend:
                case EffectType.Confuse:
                case EffectType.Paralyze:
                case EffectType.Enchant:
                case EffectType.Critical:
                case EffectType.Restore:
                case EffectType.BuffAscend:
                    _target.showBuffSlots.SetBuff(_effectType);
                    break;
            }

        }
        public void RemoveFromList()
        {
            target.tempEffectsDict[effectType].Remove(this);
            target.showBuffSlots.RemoveBuff(effectType);
        }
    }
}