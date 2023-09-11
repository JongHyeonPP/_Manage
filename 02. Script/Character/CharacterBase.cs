using System.Collections.Generic;
using UnityEngine;
using EnumCollection;
using StructCollection;
using UnityEngine.UI;
using TMPro;
using System.Collections;

abstract public class CharacterBase : MonoBehaviour
{
    public float maxHp;
    public float hp;
    public float ability;
    public float resist;
    public bool isMoved = false;
    public float armor = 0f;
    public bool isDead { get; protected set; }
    public GameObject grid;
    public SkillTargetCor[] skillTargetCor;
    public int gridIndex;
    public static readonly Color DEFAULTCOLOR = new(1f, 1f, 1f, 100f / 255f);
    public GridPatern gridPatern = GridPatern.Deactive;
    public GameObject hpObject;
    public Image hpBar;
    public Image armorBar;
    public Dictionary<EffectType, float> TotalEffects { get; private set; } = new();//적용되는 효과들의 합계
    internal bool isCasting = false;
    private readonly float ARRIVAL_TIME = 3f;
    private Coroutine moveCoroutine;
    public bool IsEnemy { get; protected set; }
    protected void InitCharacter(List<Skill> _skills, GameObject _grid, int _gridIndex, float _maxHp, float _hp, float _ability, float _resist)
    {
        hpObject = Instantiate(GameManager.gameManager.objectHpBar, transform);
        hpObject.transform.localScale = Vector3.one;
        hpObject.transform.GetComponent<RectTransform>().localPosition = new(0f, -15f, 0f);

        hpBar = transform.GetChild(1).GetChild(1).GetComponent<Image>();
        armorBar = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        grid = _grid;
        hp = maxHp = _hp;
        ability = _ability;
        gridIndex = _gridIndex;
        hp = _hp;
        maxHp = _maxHp;
        resist = _resist;
        if (_skills != null)
        {
            skillTargetCor = new SkillTargetCor[2];
            for (int i = 0; i < 2; i++)
            {
                if (i < _skills.Count)
                    skillTargetCor[i] = new(this, _skills[i]);
                else
                    skillTargetCor[i] = new(this, new Skill());//기본 공격
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
    public void MoveCharacter(int _targetGridIndex, bool _isEnemyGrid)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        gridIndex = _targetGridIndex;
        grid = (_isEnemyGrid ? GameManager.gameManager.EnemyGrids : GameManager.gameManager.FriendlyGrids)[_targetGridIndex];
        moveCoroutine = StartCoroutine(MoveCharacterCoroutine(grid.transform));
    }
    private IEnumerator MoveCharacterCoroutine(Transform _targetTransform)
    {
        transform.SetParent(_targetTransform);
        Vector3 initialPosition = transform.localPosition;
        Vector3 targetPosition = Vector3.zero;
        float distanceToTarget = Vector3.Distance(initialPosition, targetPosition);
        float moveSpeed = distanceToTarget / ARRIVAL_TIME; // ARRIVAL_TIME은 도착하는 데 걸리는 시간을 나타내는 상수로 설정

        float startTime = Time.time;
        while (Time.time - startTime < ARRIVAL_TIME)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfDistance = distanceCovered / distanceToTarget;
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, fractionOfDistance);
            yield return null;
        }

        transform.localPosition = targetPosition;
        moveCoroutine = null;
    }










    /// Define SkillTargetCor
    public class SkillTargetCor
    {
        public CharacterBase caster;
        public Skill skill;
        public CharacterBase target;
        public bool isTargetEnemy;
        public Image imageSkill = null;
        readonly float coolUpdateTime = 0.05f;
        readonly float skillCastTime = 1f;
        public bool isReady = false;
        public SkillTargetCor(CharacterBase _caster, Skill _skill)
        {
            caster = _caster;
            skill = _skill;
        }
        public void StartSkillCoroutine()
        {
            caster.StartCoroutine(ActiveSkillOnTarget());
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
                if (caster is FriendlyScript)
                {
                    caster.StartCoroutine(SkillCoolDown());
                }
                yield return new WaitForSeconds(skill.coolTime);

                float attBuffSum = 0f;
                float abilitySum = caster.ability;
                if (caster.TotalEffects.ContainsKey(EffectType.AttBuff))
                    attBuffSum = caster.TotalEffects[EffectType.AttBuff];
                if (caster.TotalEffects.ContainsKey(EffectType.Ability))
                { abilitySum += caster.ability * caster.TotalEffects[EffectType.Ability]; }

                for (int i = 0; i < skill.effects.Count; i++)
                {
                    SkillEffect effect = skill.effects[i];
                    effect.currentCycle = (effect.currentCycle % effect.cycle) + 1;
                    if (effect.currentCycle != effect.cycle) continue;
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
                            List<CharacterBase> enemies = GameManager.gameManager.Enemies;
                            if (enemies.Count == 1)
                                break;
                            foreach (var x in GameManager.battleScenario.GetTargets(target.gridIndex, isTargetEnemy, effect))
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
                    if (target.isDead)
                    {
                        target = FindProperTarget();
                    }
                    foreach (var x in GameManager.battleScenario.GetTargets(target.gridIndex, isTargetEnemy, effect))//타겟에게 스킬 적용
                    {
                        if (skill.targetColumn > 0)
                        {
                            value *= 1 - (Mathf.Abs(skill.targetColumn - (x.gridIndex % 3))*0.1f);
                        }
                        x.OnEffected(value, effect.count, effect.type, effect.delay);//핵심
                        yield return new WaitForSeconds(effect.delay);
                    }
                    yield return new WaitForSeconds(effect.delay);
                }
            }
        }
        public IEnumerator SkillCoolDown()
        {
            Image imageCool = imageSkill.transform.GetChild(0).GetComponent<Image>();
            imageCool.fillAmount = 1f;
            while (true)
            {
                yield return new WaitForSeconds(coolUpdateTime);
                imageCool.fillAmount -= coolUpdateTime / skill.coolTime;
                if (imageCool.fillAmount == 0)
                    break;
            }
        }

        internal CharacterBase FindProperTarget()
        {
            List<CharacterBase> maxCharacters = new();
            Dictionary<CharacterBase, float> targetPriority = new();
            List<CharacterBase> targets = new();
            int targetIndex = BattleScenario.GetTargetIndex(skill);

            var enemyList = GameManager.gameManager.Enemies;
            var friendlyList = GameManager.gameManager.Friendlies;

            void AddValidTargets(List<CharacterBase> targetCollection)
            {
                foreach (var target in targetCollection)
                {
                    if (!target.isDead)
                    {
                        targets.Add(target);
                    }
                }
            }

            switch (targetIndex)
            {
                case 0:
                    AddValidTargets(caster.IsEnemy ? enemyList : friendlyList);
                    break;
                case 1:
                    AddValidTargets(caster.IsEnemy ? friendlyList : enemyList);
                    break;
                case 2:
                    AddValidTargets(enemyList);
                    AddValidTargets(friendlyList);
                    break;
            }
            foreach (var x in targets)
            {
                targetPriority.Add(x, 0f);
                //HP에 대한 우선도 연산
                for (int i = 0; i < skill.effects.Count; i++)
                {
                    SkillEffect effect = skill.effects[i];
                    bool hpProport = !(effect.type == EffectType.Damage
                        || effect.type == EffectType.Heal
                        || effect.type == EffectType.Armor);
                    targetPriority[x] += (hpProport ? x.hp / x.maxHp : 1 - x.hp / x.maxHp)/skill.effects.Count;
                    //Debug.Log("Hp 우선도 : " + targetPriority[x]);
                }
            }
            //TargetColumn에 대한 연산

            float maxValue = 0f;
            foreach (var x in targetPriority.Keys)
            {
                if (targetPriority[x] > maxValue)
                {
                    maxCharacters.Clear();
                    maxCharacters.Add(x);
                    maxValue = targetPriority[x];
                    Debug.Log("Max값 재설정");
                }
                else if(targetPriority[x] == maxValue)
                {
                    maxCharacters.Add(x);
                    Debug.Log("Max값 추가");
                }
            }
            return maxCharacters[Random.Range(0, maxCharacters.Count)];
        }
    }
}