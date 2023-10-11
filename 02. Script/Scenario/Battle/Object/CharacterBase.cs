using System.Collections.Generic;
using UnityEngine;
using EnumCollection;
using StructCollection;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;

abstract public class CharacterBase : ObjectThing
{
    public float maxHp;
    public float hp;
    public float ability;
    public float resist;
    public bool isMoved = false;
    public float armor = 0f;
    public bool isDead { get; protected set; }
    public SkillCor[] skillCors = new SkillCor[2];
    private SkillCor defaultAttack;
    public static readonly Color DEFAULTCOLOR = new(1f, 1f, 1f, 100f / 255f);
    public GridPatern gridPatern = GridPatern.Deactive;
    public GameObject hpObject;
    public Image hpBar;
    public Image armorBar;
    public Dictionary<EffectType, float> TotalEffects { get; private set; } = new();//적용되는 효과들의 합계
    internal bool isCasting = false;
    
    public bool IsEnemy { get; protected set; }
    protected void InitCharacter( List<Skill> _skills, float _maxHp, float _hp, float _ability, float _resist, float _speed)
    {
        hpObject = Instantiate(GameManager.gameManager.objectHpBar, transform);
        hpObject.transform.localScale = Vector3.one;
        hpObject.transform.GetComponent<RectTransform>().localPosition = new(0f, -15f, 0f);

        hpBar = transform.GetChild(1).GetChild(1).GetComponent<Image>();
        armorBar = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        hp = maxHp = _hp;
        ability = _ability;
        hp = _hp;
        maxHp = _maxHp;
        resist = _resist;
        defaultAttack = new(this, new(_speed));
        if (_skills != null)
        {
            for (int i = 0; i < _skills.Count; i++)
            {
                skillCors[i] = new(this, _skills[i]);
            }
        }
        CalculateHpImage();
    }

    public abstract void OnDead();
    public void CalculateHpImage()
    {
        float hpUpper = (hp + armor > maxHp)? hp + armor:maxHp;
        hpBar.fillAmount = hp / hpUpper;
        armorBar.fillAmount = (hp + armor) / hpUpper;
    }
    public void OnEffected(float _value, int _count, EffectType _effectType, float _delay = 0f)
    {
        float value = _value;
        float incrementValue = 0f;
        switch (_effectType)//보정값 산출
        {
            case EffectType.Damage:
                if (TotalEffects.ContainsKey(EffectType.DefBuff))
                    incrementValue -= TotalEffects[EffectType.DefBuff];
                if (TotalEffects.ContainsKey(EffectType.DefDebuff))
                    incrementValue += TotalEffects[EffectType.DefDebuff];
                break;
        }
        value *= 1 + incrementValue;
        StartCoroutine(ApplyValue(value, _count, _effectType, _delay));

    }
    public void ActiveRegularEffect()
    {
        if (TotalEffects.ContainsKey(EffectType.Bleed))
        {
            hp -= TotalEffects[EffectType.Bleed];
        }
        hp = Mathf.Clamp(hp, 0, maxHp);
        if (hp == 0)
        {
            OnDead();
        }
        CalculateHpImage();
    }
    IEnumerator ApplyValue(float _value, int _count, EffectType _effectType, float _delay)
    {

        for (int i = 0; i < _count; i++)
        {
            switch (_effectType)
            {
                default:
                    if (!TotalEffects.ContainsKey(_effectType))
                        TotalEffects.Add(_effectType, new());
                    TotalEffects[_effectType] += _value;
                    break;
                case EffectType.Damage:
                    float temp = armor - _value;
                    if (temp < 0)
                    {
                        armor = 0f;
                        hp = Mathf.Max(hp + temp, 0);
                    }
                    else
                    {
                        armor = temp;
                    }

                    if (hp == 0)
                    {
                        OnDead();
                    }
                    break;
                case EffectType.Heal:
                    hp = Mathf.Min(hp + _value, maxHp);
                    break;
                case EffectType.Armor:
                    armor += _value;
                    break;
            }
            CalculateHpImage();
            yield return new WaitForSeconds(_delay);
        }
    }
    protected void OnDead_Base()
    {
        Debug.Log(gameObject.name + " is Dead");
        isDead = true;
        GameManager.battleScenario.regularEffect -= ActiveRegularEffect;
        StopAllCoroutines();
    }



    public void StartSkillCor()
    {

        StartCoroutine(defaultAttack.ActiveSkillOnTarget());
        foreach (var x in skillCors)
        {
            if (x != null)
                StartCoroutine(x.ActiveSkillOnTarget());
        }
    }









    /// Define SkillTargetCor
    public class SkillCor
    {
        public CharacterBase caster;
        public Skill skill;
        public bool isTargetEnemy;
        public Image imageSkill = null;
        readonly float skillCastTime = 1f;
        public bool isReady = false;
        public int targetRow;
        public SkillCor(CharacterBase _caster, Skill _skill)
        {
            caster = _caster;
            skill = _skill;
        }
        
        public IEnumerator ActiveSkillOnTarget()
        {
            while (true)
            {
                while (caster.isCasting)
                {
                    yield return null;
                }
                caster.isCasting = true;
                yield return new WaitForSeconds(skillCastTime);
                caster.isCasting = false;
                yield return new WaitForSeconds(skill.coolTime);

                float attBuffSum = 0f;
                float abilitySum = caster.ability;
                if (caster.TotalEffects.ContainsKey(EffectType.AttBuff))
                    attBuffSum = caster.TotalEffects[EffectType.AttBuff];
                if (caster.TotalEffects.ContainsKey(EffectType.Ability))
                { abilitySum += caster.ability * caster.TotalEffects[EffectType.Ability]; }

                foreach (SkillEffect effect in skill.effects)
                {
                    List<CharacterBase> target = GetTargetOnRange(effect.range);
                    float value = effect.value;
                    switch (effect.type)//Value 기반값 결정
                    {
                        default:
                            value *= abilitySum;
                            break;
                        case EffectType.ArmorAtt:
                            value *= caster.armor;
                            break;
                        case EffectType.BleedTransfer:
                            List<CharacterBase> enemies = GameManager.Enemies;
                            if (enemies.Count == 1)
                                break;
                            foreach (var x in target)
                            {
                                if (!x.TotalEffects.TryGetValue(EffectType.Bleed, out value))
                                    continue;
                                CharacterBase enemy;
                                while (true)
                                {
                                    enemy = enemies[Random.Range(0, enemies.Count)];
                                    if (!(enemy == x))
                                        break;
                                }
                                enemy.OnEffected(value, effect.count, EffectType.Bleed, 0f);
                            }
                            break;
                    }
                    FriendlyScript friendlyCaster = caster as FriendlyScript; ;
                    switch (effect.type)//Value 보정값 설정
                    {
                        case EffectType.ArmorAtt:
                        case EffectType.Damage:
                        case EffectType.Bleed:
                            if (friendlyCaster)//아군이 발동한 경우
                            {
                                value *= friendlyCaster.talentEffects[T_Type.AttAscend] + attBuffSum + 1;
                                if (GameManager.CalculateProbability(friendlyCaster.talentEffects[T_Type.Critical]))
                                {
                                    //치명타 판정
                                    value *= 2;
                                }
                            }
                            if (caster.TotalEffects.TryGetValue(EffectType.Enchant, out float enchantValue))
                                value += enchantValue;
                            break;
                        case EffectType.Heal:
                                if (friendlyCaster)
                                {
                                    value *= friendlyCaster.talentEffects[T_Type.HealAscend] + 1;
                                }
                            break;

                        case EffectType.AttBuff:
                        case EffectType.DefBuff:
                        case EffectType.Enchant:
                            value = effect.isConst ? effect.value : effect.value * abilitySum;
                            if (friendlyCaster)
                            {
                                value *= friendlyCaster.talentEffects[T_Type.BuffAscend] + 1;
                            }
                            break;

                        case EffectType.AttDebuff:
                        case EffectType.DefDebuff:
                            value = effect.isConst ? effect.value : effect.value * abilitySum;
                            if (friendlyCaster)
                            {
                                value *= friendlyCaster.talentEffects[T_Type.DebuffAscend] + 1;
                            }
                            break;
                        default:
                            value = effect.isConst ? effect.value : effect.value * abilitySum;
                            break;
                    }
                    foreach (var x in target)//타겟에게 스킬 적용
                    {
                        x.OnEffected(value, effect.count, effect.type, effect.delay);//핵심
                        yield return new WaitForSeconds(effect.delay);
                    }
                    yield return new WaitForSeconds(effect.delay);
                }
            }
        }
        internal List<CharacterBase> GetTargetOnRange(EffectRange _effectRange)
        {
            List<CharacterBase> targets = new();
            List<CharacterBase> characters = characters = caster.IsEnemy ^ skill.isTargetEnemy ? GameManager.Enemies : GameManager.Friendlies;
            int targetRow;
            switch (_effectRange)//타겟 정의
            {
                case EffectRange.Dot:
                    targetRow = GetTargetRow(characters);
                    foreach (CharacterBase x in characters)
                    {
                        if (x.isDead) continue;
                        if (x.grid.index / 3 == targetRow)
                        {
                            targets.Add(x);
                        }
                    }
                    targets = targets.OrderBy(data => data.grid.index).ToList();
                    break;
                case EffectRange.Row:
                    targetRow = GetTargetRow(characters);
                    foreach (CharacterBase x in characters)
                    {
                        if (x.grid.index / 3 == targetRow)
                        {
                            targets.Add(x);
                        }
                    }
                    targets = new() { targets.OrderBy(data => data.grid.index).First() };
                    break;
                case EffectRange.All:
                    targets = characters;
                    break;
            }


            return targets;


            int GetTargetRow(List<CharacterBase> characters)
            {
                int targetRow = -1;
                foreach (var x in characters)
                {
                    if (x.grid.index / 3 == caster.grid.index / 3)
                    {
                        targetRow = x.grid.index / 3;
                        break;
                    }
                }
                if (targetRow == -1)
                {
                    targetRow = characters[Random.Range(0, characters.Count)].grid.index / 3;
                }

                return targetRow;
            }
        }
    }
}