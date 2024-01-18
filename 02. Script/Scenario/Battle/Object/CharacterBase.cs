using System.Collections.Generic;
using UnityEngine;
using EnumCollection;
using StructCollection;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;

abstract public class CharacterBase : MonoBehaviour
{
    public JobClass job;
    public float maxHp;
    [SerializeField] private float hp;
    public float Hp {
        get { return hp; }
        set
        {
            hp = value;
            float hpUpper = (hp + armor > maxHp) ? hp + armor : maxHp;
            hpBar.fillAmount = hp / hpUpper;
            armorBar.fillAmount = (hp + armor) / hpUpper;
        }
    }
    public float ability;
    public float resist;
    public float speed;
    public bool isMoved = false;
    public float armor = 0f;
    public bool isDead { get; protected set; }
    public List<Skill> skills;
    private Skill defaultAttack;
    public static readonly Color DEFAULTCOLOR = new(1f, 1f, 1f, 100f / 255f);
    public GameObject hpObject;
    public Image hpBar;
    public Image armorBar;
    public Dictionary<EffectType, float> TempEffects { get; private set; } = new();//전투동안 지속되는 효과
    public Dictionary<EffectType, float> PermEffects { get; private set; } = new();//전투가 끝나도 유지되는 효과
    public bool isBleedTransfer = false;
    public CharacterBase targetOpponent;
    public CharacterBase targetAlly;
    public bool IsEnemy { get; protected set; }
    //스킬 정보
    private List<IEnumerator> skillQueue = new();
    private List<EffectPassiveForm> passiveEffectsAtGrid = new();
    private List<EffectPassiveFormDot> passiveAtDotOpponent = new();
    private List<EffectPassiveFormDot> passiveAtDotAlly = new();
    protected Animator animator;
    protected void InitCharacter(List<Skill> _skills, float _maxHp, float _hp, float _ability, float _resist, float _speed)
    {
        hpObject = Instantiate(GameManager.gameManager.objectHpBar, transform);
        hpObject.transform.localScale = Vector3.one;
        hpObject.transform.GetComponent<RectTransform>().localPosition = new(0f, -15f, 0f);

        hpBar = transform.GetChild(1).GetChild(1).GetComponent<Image>();
        armorBar = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        ability = _ability;
        maxHp = _maxHp;
        Hp = _hp;
        resist = _resist;
        speed = _speed;
        if (_skills != null)
        {
            skills = _skills;
        }
        animator = transform.GetChild(0).GetComponent<Animator>();
        defaultAttack = new Skill(speed);
    }
    public abstract void SetAnimParam();
    public ObjectGrid grid;
    private Coroutine moveCoroutine;
    private readonly float ARRIVAL_TIME = 2f;
    public void MoveToTargetGrid(ObjectGrid _grid)
    {
        bool isInstant = (grid == _grid) || GameManager.battleScenario.battlePatern == BattlePatern.OnReady;
        float distance = Mathf.Abs(grid.index / 3 - _grid.index / 3) + Mathf.Abs(grid.index % 3 - _grid.index % 3);
        
        if (!grid.Equals(_grid))
        {
            if (grid.ExitOnGrid != null)
            {
                grid.ExitOnGrid(this);
            }
            foreach (EffectPassiveForm passive in passiveEffectsAtGrid)
            {
                passive.UnsetPassiveEffect();
            }

            grid = _grid;
            _grid.owner = this;

            FindNewTargetAlly();
            FindNewTargetOpponent();
            
            if (grid.EnterOnGrid != null)
            {
                grid.EnterOnGrid(this);
            }
            foreach (EffectPassiveForm passive in passiveEffectsAtGrid)
            {
                passive.SetPassiveEffect();
            }
            foreach (CharacterBase x in IsEnemy ? GameManager.Friendlies : GameManager.Enemies)
            {
                x.FindNewTargetOpponent();
            }
            foreach (CharacterBase x in IsEnemy ? GameManager.Enemies : GameManager.Friendlies)
            {
                x.FindNewTargetAlly();
            }


        }

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        if (isInstant)
        {
            transform.SetParent(_grid.transform);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            moveCoroutine = StartCoroutine(MoveCharacterCoroutine(_grid.transform, distance / 8));
        }
    }
    private IEnumerator MoveCharacterCoroutine(Transform _targetTransform, float _speed)
    {
        animator.SetFloat("RunState", _speed);
        transform.localPosition = Vector3.zero;
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
        animator.SetFloat("RunState", 0f);
    }
    public abstract void OnDead();
    public float GetRegularValue(EffectType _type)
    {
        float returnValue = 0f;
        if (TempEffects.TryGetValue(_type, out float value0))
        {
            returnValue += value0;
        }
        if (PermEffects.TryGetValue(_type, out float value1))
        {
            returnValue += value1;
        }
        return returnValue;
    }
    public void ActiveRegularEffect()
    {
        Hp -= GetRegularValue(EffectType.Bleed);
        Hp += GetRegularValue(EffectType.Heal);
        Hp = Mathf.Clamp(Hp, 0, maxHp);
        if (hp == 0)
        {
            OnDead();
        }
    }
    void ApplyValue(float _value, EffectType _effectType)
    {
        if (this is EnemyScript)
            Debug.Log(_effectType + " : " + _value);
        switch (_effectType)
        {
            default:
                if (!TempEffects.ContainsKey(_effectType))
                {
                    TempEffects.Add(_effectType, new());
                }
                TempEffects[_effectType] += _value;
                break;
            case EffectType.Damage:
                float temp = armor - _value;
                if (temp < 0)
                {
                    armor = 0f;
                    Hp = Mathf.Max(Hp + temp, 0);
                }
                else
                {
                    armor = temp;
                }

                if (Hp == 0)
                {
                    OnDead();
                }
                break;
            case EffectType.Heal:
                Hp = Mathf.Min(Hp + _value, maxHp);
                break;
            case EffectType.Armor:
                armor += _value;
                break;
            case EffectType.AbilityVamp:
                ability -= _value * ability;
                Debug.Log("피격자 : " + ability);
                break;
            case EffectType.Restoration:
                Debug.Log("Restoration : " + (maxHp - hp) * _value);
                Hp += (maxHp - hp) * _value;
                break;
            case EffectType.BleedTransfer:
                isBleedTransfer = true;
                break;
        }
    }
    protected void OnDead_Base()
    {
        if (isBleedTransfer)
        {
            List<CharacterBase> characters = new();
            foreach (var character in IsEnemy ? GameManager.Enemies : GameManager.Friendlies)
            {
                if (character != this)
                    characters.Add(character);
            }
            CharacterBase target = characters[Random.Range(0, characters.Count)];
            target.ApplyValue(GetRegularValue(EffectType.Bleed), EffectType.Bleed);
            Debug.Log("BleedTransfer : " + target.grid.index);
        }
        Debug.Log(gameObject.name + " is Dead");
        isDead = true;
        GameManager.battleScenario.regularEffect -= ActiveRegularEffect;
        foreach (EffectPassiveForm passive in passiveEffectsAtGrid)
        {
            passive.UnsetPassiveEffect();
        }
        StopAllCoroutines();
        StartCoroutine(DieCor());

        IEnumerator DieCor()
        {
            animator.SetTrigger("Die");
            yield return new WaitForSeconds(2f);
            gameObject.SetActive(false);
        }
        foreach (CharacterBase x in IsEnemy ? GameManager.Enemies : GameManager.Friendlies)
        {
            if (x != this)
                x.FindNewTargetAlly();
        }
        foreach (CharacterBase x in IsEnemy ? GameManager.Friendlies : GameManager.Enemies)
        {
            x.FindNewTargetOpponent();
        }
    }



    public void SetSkillsWithBattle()
    {
        List<Skill> skillsAndDa = new(skills);
        skillsAndDa.Add(defaultAttack);
        foreach (Skill skill in skillsAndDa)
        {
            SkillActiveForm skillActiveForm = null;
            foreach (SkillEffect effect in skill.effects)
            {
                if (effect.isPassive)//패시브 스킬
                {
                    float value = effect.value;
                    if (!effect.isConst)
                        value *= ability;

                    switch (effect.range)
                    {
                        case EffectRange.Dot:
                            if (skill.isTargetEnemy)//가장 가까운 적
                            {
                                EffectPassiveFormDot formDot = new(effect.type, value);
                                passiveAtDotOpponent.Add(formDot);
                                formDot.ApplyEffect(targetOpponent);
                            }
                            else//가장 가까운 아군
                            {
                                EffectPassiveFormDot formDot = new(effect.type, value);
                                passiveAtDotAlly.Add(formDot);
                                formDot.ApplyEffect(targetAlly);
                            }
                            break;
                        case EffectRange.Self:
                            {
                                ApplyValue(value, effect.type);
                            }
                            break;
                        default:
                            EffectPassiveForm form = new(effect, skill.isTargetEnemy, this, value);
                            passiveEffectsAtGrid.Add(form);
                            form.SetPassiveEffect();
                            break;
                    }
                }
                else//액티브 스킬
                {
                    if(skillActiveForm == null)
                    {
                        skillActiveForm = new SkillActiveForm(this, skill);
                    }
                    skillActiveForm.actvieEffects.Add(new EffectActiveForm(effect, skill.isTargetEnemy, this));
                }
            }
            if (skillActiveForm != null)
            {
                StartCoroutine(skillActiveForm.StartQueueCycle());
            }
        }
        
    }
    public void FindNewTargetOpponent()
    {
        foreach (EffectPassiveFormDot passiveDot in passiveAtDotOpponent)
        {
            passiveDot.DeapplyEffect(targetOpponent);
        }
        List<CharacterBase> targetByColumn;
        int targetColumn;
        List<CharacterBase> targetsBase = (IsEnemy ? GameManager.Friendlies : GameManager.Enemies).Where(item => !item.isDead).ToList();
        if (targetsBase.Count == 0)
            return;
        if (IsEnemy)
        {
            targetColumn = targetsBase.Max(item => item.grid.index % 3);
        }
        else
        {
            targetColumn = targetsBase.Min(item => item.grid.index % 3);
        }
        targetByColumn = targetsBase.Where(item => item.grid.index % 3 == targetColumn).ToList();
        if (targetByColumn.Count == 1)
        {
            targetOpponent = targetByColumn[0];
        }
        else
        {
            int RowDist = targetByColumn.Min(item => Mathf.Abs(item.grid.index / 3 - grid.index / 3));
            var temp = targetByColumn.Where(item => Mathf.Abs(item.grid.index / 3 - grid.index / 3) == RowDist).ToList();
            targetOpponent = temp[Random.Range(0, temp.Count)];
        }
        foreach (EffectPassiveFormDot passiveDot in passiveAtDotOpponent)
        {
            passiveDot.ApplyEffect(targetOpponent);
        }
    }
    public void FindNewTargetAlly()
    {
        foreach (EffectPassiveFormDot passiveDot in passiveAtDotAlly)
        {
            passiveDot.DeapplyEffect(targetAlly);
        }
        List<CharacterBase> targetsBase = (IsEnemy ? GameManager.Enemies : GameManager.Friendlies).Where(item => !item.isDead).ToList();
        if (targetsBase.Count == 1)
            return;
        targetsBase.Remove(this);
        int minDist = targetsBase.Min(item => GetDistance(grid.index, item.grid.index));
        List<CharacterBase> targetCandi = targetsBase.Where(item => GetDistance(grid.index, item.grid.index) == minDist).ToList();
        targetAlly = targetCandi[Random.Range(0, targetCandi.Count)];
        int GetDistance(int _index0, int _index1)
        {
            int rowDist = Mathf.Abs(_index0 % 3 - _index1 % 3);
            int ColumnDist = Mathf.Abs(_index0 / 3 - _index1 / 3);
            return rowDist + ColumnDist;
        }
    }












    public class EffectPassiveFormDot
    {
        EffectType type;
        float value;
        public EffectPassiveFormDot(EffectType _type, float _value)
        {
            type = _type;
            value = _value;
        }
        public void ApplyEffect(CharacterBase _target)
        {
            _target.ApplyValue(value, type);
        }
        public void DeapplyEffect(CharacterBase _target)
        {
            _target.ApplyValue(value * -1, type);
        }
    }

    #region EffectPassiveForm
    public class EffectPassiveForm
    {
        SkillEffect effect;
        float value;//최초 Value 고정
        CharacterBase caster;
        public List<ObjectGrid> targetGrids = new();
        bool isTargetEnemy;
        public EffectPassiveForm(SkillEffect _effect, bool _isTargetEnemy, CharacterBase _caster, float _value)
        {
            effect = _effect;
            caster = _caster;
            isTargetEnemy = _isTargetEnemy;
            value = _value;
        }
        public void SetPassiveEffect()
        {
            targetGrids = GetGridsByRange(effect.range, caster, isTargetEnemy);
            foreach (var x in targetGrids)
            {
                x.EnterOnGrid += EnterOnGrid;
                if (x.owner != null)
                {
                    EnterOnGrid(x.owner);
                }
                x.ExitOnGrid += ExitOnGrid;
            }
        }
        public void UnsetPassiveEffect()
        {
            foreach (var x in targetGrids)
            {
                x.EnterOnGrid -= EnterOnGrid;
                if (x.owner != null)
                {
                    ExitOnGrid(x.owner);
                }
                x.ExitOnGrid -= ExitOnGrid;
            }
        }
        void EnterOnGrid(CharacterBase _target)
        {
            _target.ApplyValue(value, effect.type);
        }
        void ExitOnGrid(CharacterBase _target)
        {
            var tempValue = value * -1;
            _target.ApplyValue(tempValue, effect.type);
        }
    }

    public static List<CharacterBase> GetTargetsByRange(EffectRange _range, CharacterBase _target, bool _isTargetEnemy)
    {
        List<CharacterBase> targets = null;
        bool orderDir = _isTargetEnemy ^ _target.IsEnemy;
        List<CharacterBase> targetsBase = (orderDir ? GameManager.Enemies : GameManager.Friendlies).Where(item => !item.isDead).ToList();
        switch (_range)
        {
            case EffectRange.Dot://가장 가까운 대상
                targets = new() { _target };
                break;
            case EffectRange.Row:
                targets = targetsBase.Where(item => item.grid.index / 3 == _target.grid.index / 3).ToList();
                break;
            case EffectRange.Column:
                targets = targetsBase.Where(item => item.grid.index % 3 == _target.grid.index % 3).ToList();
                break;
            case EffectRange.Behind:
                if (orderDir)
                    targets = targetsBase.Where(item => item.grid.index % 3 > _target.grid.index % 3).ToList();
                else
                    targets = targetsBase.Where(item => item.grid.index % 3 < _target.grid.index % 3).ToList();
                break;
            case EffectRange.Front:
                if (orderDir)
                    targets = targetsBase.Where(item => item.grid.index % 3 < _target.grid.index % 3).ToList();
                else
                    targets = targetsBase.Where(item => item.grid.index % 3 > _target.grid.index % 3).ToList();
                break;
        }

        return targets;
    }
    public static List<ObjectGrid> GetGridsByRange(EffectRange _range, CharacterBase _target, bool _isTargetEnemy)
    {
        List<ObjectGrid> targetGrids = null;
        bool orderDir = _isTargetEnemy ^ _target.IsEnemy;
        List<ObjectGrid> gridsBase = (orderDir ? GameManager.EnemyGrids : GameManager.FriendlyGrids);
        switch (_range)
        {
            case EffectRange.Row:
                targetGrids = gridsBase.Where(item => item.index / 3 == _target.grid.index / 3).ToList();
                break;
            case EffectRange.Column:
                targetGrids = gridsBase.Where(item => item.index % 3 == _target.grid.index % 3).ToList();
                break;
            case EffectRange.Behind:
                if (orderDir)
                    targetGrids = gridsBase.Where(item => item.index % 3 > _target.grid.index % 3).ToList();
                else
                    targetGrids = gridsBase.Where(item => item.index % 3 < _target.grid.index % 3).ToList();
                break;
            case EffectRange.Front:
                if (orderDir)
                    targetGrids = gridsBase.Where(item => item.index % 3 < _target.grid.index % 3).ToList();
                else
                    targetGrids = gridsBase.Where(item => item.index % 3 > _target.grid.index % 3).ToList();
                break;
        }

        return targetGrids;
    }
    #endregion

    #region SkillActiveForm
    public class SkillActiveForm
    {
        public CharacterBase caster;
        public List<EffectActiveForm> actvieEffects = new();
        readonly float skillCastTime = 0.5f;
        public Skill skill;
        public SkillActiveForm(CharacterBase _caster, Skill _skill)
        {
            skill = _skill;
            caster = _caster;


        }
        public IEnumerator StartQueueCycle()
        {
            yield return new WaitForSeconds(skill.cooltime / caster.speed);
            if (caster.skillQueue.Count == 0)
                caster.StartCoroutine(ActiveSkill());
            else
                caster.skillQueue.Add(ActiveSkill());
        }
        public IEnumerator ActiveSkill()
        {
            CharacterBase confusedTarget = null;
            bool isParalyze = false;
            float confuseProb = caster.GetRegularValue(EffectType.Confuse);
            if (GameManager.CalculateProbability(confuseProb))
            {
                List<CharacterBase> confusedTargetsBase = (caster.IsEnemy ^ skill.isTargetEnemy ? GameManager.Friendlies : GameManager.Enemies).Where(item => !item.isDead).ToList();
                confusedTarget = confusedTargetsBase[Random.Range(0, confusedTargetsBase.Count)];
            }
            else
            {
                float paralyzeProb = caster.GetRegularValue(EffectType.Paralyze);
                isParalyze = GameManager.CalculateProbability(paralyzeProb);
            }
            yield return new WaitForSeconds(skillCastTime);
            if (!isParalyze)
            {
                if (skill.isAnim)
                    SkillAnim();
                
                foreach (EffectActiveForm effectForm in actvieEffects)
                {
                    CharacterBase effectTarget;
                    switch (effectForm.effect.range)
                    {
                        case EffectRange.Self:
                            effectTarget = caster;
                            break;
                        case EffectRange.Dot:
                            if (confusedTarget)
                                effectTarget = confusedTarget;
                            else
                                effectTarget = skill.isTargetEnemy ? caster.targetOpponent : caster.targetAlly;
                            break;
                        default:
                            effectTarget = caster;
                            break;
                    }
                    effectForm.ActiveEffect0nTarget(effectTarget);

                }
            }
            //Next Skill
            if (caster.skillQueue.Count > 0)
            {
                caster.skillQueue.RemoveAt(0);
                caster.StartCoroutine(caster.skillQueue[0]);
            }
            yield return new WaitForSeconds(skill.cooltime / caster.speed);
            if (caster.skillQueue.Count == 0)
                caster.StartCoroutine(ActiveSkill());
            else
                caster.skillQueue.Add(ActiveSkill());
            


            void SkillAnim()
            {
                float minValue = 2;
                float maxValue = 5;
                caster.animator.SetFloat("AttackState", (skill.cooltime -minValue ) / (maxValue - minValue));;
                //caster.animator.SetFloat("NormalState", 0.5f);
                //caster.animator.SetFloat("SkillState", 0.5f);
                caster.animator.SetTrigger("Attack");
            }
        }
    }
    #endregion

    #region EffectActiveForm
    public class EffectActiveForm
    {
        public SkillEffect effect;
        public CharacterBase caster;
        public bool isTargetEnemy;

        public EffectActiveForm(SkillEffect _effect, bool _isTargetEnemy, CharacterBase _caster)
        {
            effect = _effect;
            caster = _caster;
            isTargetEnemy = _isTargetEnemy;
        }
        public void ActiveEffect0nTarget(CharacterBase _target)
        {
            float calcValue = effect.value;
            calcValue *= 1 + caster.GetRegularValue(EffectType.AbilityAscend);
            if (!effect.isConst)
                calcValue *= caster.ability;
            List<CharacterBase> targets = GetTargetsByRange(effect.range, _target, isTargetEnemy);
            switch (effect.type)//Value 보정값 설정
            {
                case EffectType.Damage:
                    calcValue += caster.GetRegularValue(EffectType.Enchant);
                    calcValue *= 1 + caster.GetRegularValue(EffectType.AttAscend);
                    calcValue *= 1 - caster.GetRegularValue(EffectType.AttDescend);
                    if (GameManager.CalculateProbability(caster.GetRegularValue(EffectType.Critical)))
                    {
                        //치명타 판정
                        calcValue *= 2;
                    }
                    break;
                case EffectType.Bleed:
                    calcValue *= 1 + caster.GetRegularValue(EffectType.AttAscend);
                    calcValue *= 1 - caster.GetRegularValue(EffectType.AttDescend);
                    break;
                case EffectType.Heal:
                    calcValue *= 1 + caster.GetRegularValue(EffectType.HealAscend);
                    break;

                case EffectType.AttAscend:
                case EffectType.DefAscend:
                case EffectType.Enchant:
                    calcValue *= 1 + caster.GetRegularValue(EffectType.BuffAscend);
                    break;
                case EffectType.AttDescend:
                case EffectType.DefDescend:
                    calcValue *= 1 + caster.GetRegularValue(EffectType.DebuffAscend);
                    break;
            }
            if (targets != null)
            {
                foreach (var target in targets)//타겟에게 스킬 적용
                {
                    //여기서 Continue하면 될듯
                    switch (effect.type)
                    {
                        case EffectType.Damage:
                            float incrementValue = 0f;
                            incrementValue -= target.GetRegularValue(EffectType.DefAscend);
                            incrementValue += target.GetRegularValue(EffectType.DefDescend);
                            calcValue *= 1 + incrementValue;
                            calcValue -= target.GetRegularValue(EffectType.Reduce);
                            break;
                    }
                    caster.StartCoroutine(RoopEffect(calcValue, target));
                }
            }
        }

        private IEnumerator RoopEffect(float calcValue, CharacterBase _target)
        {

            for (int i = 0; i < effect.count; i++)
            {
                //능력치 흡수
                if (effect.type == EffectType.AbilityVamp)
                {
                    caster.ability += _target.ability * calcValue;
                }

                _target.ApplyValue(calcValue, effect.type);//핵심

                //대미지 후속작업
                if (effect.type == EffectType.Damage)
                {
                    float vampValue = effect.vamp + caster.GetRegularValue(EffectType.Vamp);
                    //흡수하는 채력
                    if (vampValue > 0)
                    {
                        caster.Hp += vampValue * calcValue;
                    }
                    //흡수되는 능력치
                    if (_target.GetRegularValue(EffectType.AbilityByDamage) > 0)
                    {
                        float abilityValue = caster.ability * _target.GetRegularValue(EffectType.AbilityByDamage);
                        caster.ability -= abilityValue;
                        _target.ability += abilityValue;
                    }
                    if (_target.GetRegularValue(EffectType.Reflect) > 0)
                    {
                        caster.Hp -= calcValue * _target.GetRegularValue(EffectType.Reflect);
                    }
                }
                yield return new WaitForSeconds(effect.delay);
            }
        }
    }
    #endregion
}