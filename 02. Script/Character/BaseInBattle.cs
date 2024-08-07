using BattleCollection;
using EnumCollection;
using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static BattleCollection.ActiveEffect;

abstract public class BaseInBattle : MonoBehaviour
{
    public JobClass job;
    public new Dictionary<Language, string> name;
    public float maxHpInBattle;
    public float maxHp;
    [SerializeField] private float hp;
    public float Hp {
        get { return hp; }
        set
        {
            Debug.Log("_Hp Value : " + value);
            hp = Mathf.Clamp(value, 0, maxHpInBattle);
            if (!GameManager.battleScenario)
                return;
            if (hpBarInScene.gameObject)
                hpBarInScene.SetHp(hp, armor, maxHpInBattle);
            if (hpBarInUi)
                hpBarInUi.SetHp(hp, armor, maxHpInBattle);

            if (hp <= 0 && !isDead && BattleScenario.battlePatern == BattlePatern.Battle)
                OnDead();
        }
    }
    public float abilityInBattle;
    public float ability;
    public float resistInBattle;
    public float resist;
    public float speedInBattle;
    public float speed;
    public float armor = 0f;
    public bool isDead = false;
    public List<SkillInBattle> skillInBattles = new();
    public WeaponClass weapon;
    private HpBarBase hpBarInScene;
    public HpBarInUi hpBarInUi;
    public Dictionary<EffectType, float> TempEffects = new();//전투동안 지속되는 효과
    public Dictionary<EffectType, float> EffectsByAtt = new();//대미지를 입히면 적용시키는 효과, 비중첩
    public BaseInBattle targetOpponent;
    public BaseInBattle targetAlly;

    public bool IsEnemy { get; protected set; }
    public bool isMonster { get; protected set; }
    //스킬 정보
    [SerializeField]private List<SkillActiveForm> skillQueue = new();
    [SerializeField] private List<PassiveEffect> passiveEffects;

    public Animator animator;
    public GridObject grid;
    private Coroutine moveCoroutine;
    private readonly float ARRIVAL_TIME = 2f;
    public Transform rootTargetTransform;
    public Transform skillTargetTransform;
    public GameObject fireObj;

    public abstract void SetAnimParam();
    protected void InitBase(GridObject _grid)
    {
        grid = _grid;
        transform.SetParent(_grid.transform);
        rootTargetTransform = new GameObject("RootTarget").transform;
        rootTargetTransform.SetParent(transform.GetChild(0));
        hpBarInScene = Instantiate(GameManager.gameManager.prefabHpBarInScene, transform).GetComponent<HpBarBase>();
        hpBarInScene.transform.localScale = Vector3.one * 0.4f;
        hpBarInScene.transform.GetComponent<RectTransform>().localPosition = new(0f, -18f, 0f);
        if (isMonster)
            animator = GetComponent<Animator>();
        else
            animator = transform.GetComponentInChildren<Animator>();
    }
    public void InBattleFieldZero()
    {
        abilityInBattle = maxHpInBattle = resistInBattle = speedInBattle = 0f;
    }

    public void MoveToTargetGrid(GridObject _grid, bool _isInstant = false)
    {
        bool isInstant;
        if (!_isInstant)
            isInstant = (grid == _grid) || BattleScenario.battlePatern == BattlePatern.OnReady;
        else
            isInstant = true;
        float distance = Mathf.Abs(grid.index / 3 - _grid.index / 3) + Mathf.Abs(grid.index % 3 - _grid.index % 3);

        if (grid != _grid)
        {
            //모든 캐릭터들 패시브 재적용
            //해야함!
            foreach (BaseInBattle x in IsEnemy ? BattleScenario.characters : BattleScenario.enemies)
            {
                if (x)
                    x.FindNewTargetOpponent();
            }
            foreach (BaseInBattle x in IsEnemy ? BattleScenario.enemies : BattleScenario.characters)
            {
                if (x)
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
        if (isMonster)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetFloat("RunState", _speed);
        }


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
        if (isMonster)
        {
            animator.SetBool("Run", false);
        }
        else
        {
            animator.SetFloat("RunState", 0f);
        }

    }
    public abstract void OnDead();
    public float GetRegularValue(EffectType _type)
    {
        float returnValue = 0f;
        if (TempEffects.TryGetValue(_type, out float value))
        {
            returnValue += value;
        }
        return returnValue;
    }
    public void ActiveRegularEffect()
    {
        float bleedValue = GetRegularValue(EffectType.Bleed);
        if (bleedValue > 0)
        {
            Hp -= bleedValue;
        }
    }
    public void ApplyValue(float _value, EffectType _effectType, bool _byAtt = false)
    {
        if (_byAtt)
        {
            if (!EffectsByAtt.ContainsKey(_effectType))
            {
                EffectsByAtt.Add(_effectType, new());
            }
            EffectsByAtt[_effectType] += _value;
        }
        else
        {
            switch (_effectType)
            {
                default:
                    if (!TempEffects.ContainsKey(_effectType))
                    {
                        TempEffects.Add(_effectType, new());
                    }
                    TempEffects[_effectType] += _value;
                    break;
                case EffectType.Necro:
                case EffectType.BleedTransfer:
                case EffectType.CorpseExplo:
                    if (!TempEffects.ContainsKey(_effectType))
                    {
                        TempEffects.Add(_effectType, new());
                    }
                    TempEffects[_effectType] = Mathf.Max(TempEffects[_effectType], _value);
                    break;
                case EffectType.Damage:
                    float temp = armor - _value;
                    if (temp < 0)
                    {
                        armor = 0f;
                        Hp += temp;
                    }
                    else
                    {
                        armor = temp;
                    }
                    break;
                case EffectType.Curse:
                    hp -= _value;
                    break;
                case EffectType.Heal:
                    Hp = Mathf.Min(Hp + _value, maxHpInBattle);
                    break;
                case EffectType.Armor:
                    armor += _value;
                    break;
                case EffectType.AbilityVamp:
                    abilityInBattle -= _value * abilityInBattle;
                    Debug.Log("피격자 : " + abilityInBattle);
                    break;
                case EffectType.Restoration:
                    Hp += (maxHpInBattle - Hp) * _value;
                    break;
                case EffectType.AbilityAscend:
                    abilityInBattle *= 1 + _value;
                    break;

                case EffectType.ResistAscend:
                    resistInBattle += _value * (1 + GetRegularValue(EffectType.ResistAscend));
                    break;
                case EffectType.ResistDescend:
                    resistInBattle -= _value;
                    break;
                case EffectType.SpeedAscend:
                    speedInBattle *= 1 + _value;
                    break;
                case EffectType.SpeedDescend:
                    speedInBattle *= Mathf.Max(1 - _value, 0.1f);
                    break;
                case EffectType.RewardAscend:
                    GameManager.battleScenario.RewardAscend += _value;
                    break;
            }
        }
    }
    protected IEnumerator OnDead_Base()
    {
        StopBattle();
        animator.SetTrigger("Die");
        isDead = true;
        NewTargetForOther();

        StartCoroutine(FadeOutCoroutine());
        yield return new WaitForSeconds(3f);

        foreach (var ally in IsEnemy ? BattleScenario.enemies : BattleScenario.characters)
        {
            if (!ally)
                continue;
            float value = ally.GetRegularValue(EffectType.Revive);
            if (value > 0)
            {
                ally.TempEffects.Remove(EffectType.Revive);
                ReviveMethod(value);
                yield break;
            }
        }
        float bleedTransfer = GetRegularValue(EffectType.BleedTransfer);
        List<BaseInBattle> characters = new();
        if (bleedTransfer > 0)
        {
            foreach (var character in IsEnemy ? BattleScenario.enemies : BattleScenario.characters)
            {
                if (character != this)
                    characters.Add(character);
            }
            BaseInBattle target = characters[Random.Range(0, characters.Count)];
            target.ApplyValue(GetRegularValue(EffectType.Bleed) * bleedTransfer, EffectType.Bleed);
            Debug.Log("BleedTransfer : " + target.grid.index);
        }
        float exploValue = GetRegularValue(EffectType.CorpseExplo);
        characters = new(IsEnemy ? BattleScenario.enemies : BattleScenario.characters);
        if (exploValue > 0)
        {
            foreach (BaseInBattle character in characters)
            {
                if (character != this)
                {
                    character.ApplyValue(maxHpInBattle * exploValue, EffectType.Damage);
                }
            }
        }
        float necro = GetRegularValue(EffectType.Necro);
        if (necro > 0)
        {
            grid.owner = null;
            List<GridObject> gridCandiBase = IsEnemy ? BattleScenario.CharacterGrids : BattleScenario.EnemyGrids;
            List<GridObject> gridCandi = gridCandiBase.Where(item => item.owner == null).ToList();
            if (gridCandi.Count > 0)
            {
                StopAllCoroutines();
                ReviveMethod(necro);
                Vector3 temp = transform.GetChild(0).rotation.eulerAngles;
                temp.y += 180f;
                transform.GetChild(0).rotation = Quaternion.Euler(temp);
                yield break;
            }
        }
        //Dead
        if(passiveEffects!=null)
        foreach (var passive in passiveEffects)
        {
            passive.RemoveToTarget();
        }
        gameObject.SetActive(false);

        ///////////////////////////
        void ReviveMethod(float value)
        {
            animator.SetTrigger("Revive");
            isDead = false;
            maxHpInBattle = Hp = maxHpInBattle * value;
            abilityInBattle = ability * value;
            NewTargetForOther();
            FindNewTargetAlly();
            FindNewTargetOpponent();

        }
        void NewTargetForOther()
        {
            List<BaseInBattle> enemiesBase = BattleScenario.enemies.Where(item =>item&& !item.isDead).ToList();
            List<BaseInBattle> charactersBase = BattleScenario.characters.Where(item => item&& !item.isDead).ToList();
            foreach (BaseInBattle x in IsEnemy ? enemiesBase : charactersBase)
            {
                if (x != this)
                    x.FindNewTargetAlly();
            }
            foreach (BaseInBattle x in IsEnemy ? charactersBase : enemiesBase)
            {
                x.FindNewTargetOpponent();
            }
        }
    }



    public void SetSkillsAndStart()
    {
        abilityInBattle = ability;
        speedInBattle = speed;
        resistInBattle = resist;
        passiveEffects = new();
        List<SkillActiveForm> SkillActiveForms = new()
        {
            new SkillActiveForm(this)
        };
        int cooldownIndex = 0;
        for (int i = 0; i < skillInBattles.Count; i++)
        {
            SkillInBattle skillInBattle = skillInBattles[i];
            if (skillInBattle == null)
                continue;
            CooldownSlot targetCooldownSlot = null;
            if (!IsEnemy)
            {
                if (skillInBattle.skillCategori != SkillCategori.Default)
                {
                    targetCooldownSlot = hpBarInUi.cooldownSlots[cooldownIndex++];
                }
            }
            SkillActiveForm skillActiveForm = new SkillActiveForm(this, skillInBattle, targetCooldownSlot);
            if (skillActiveForm.effectActiveForms.Count > 0)
                SkillActiveForms.Add(skillActiveForm);
            if (!IsEnemy)
            {
                List<PassiveEffect> passiveEffect = skillInBattle.GetPassiveEffects(this);
                if (passiveEffect != null)
                    passiveEffects.AddRange(passiveEffect);
            }
        }
        for (int i = 0; i < SkillActiveForms.Count; i++)
        {
            StartCoroutine(StartQueueCycle(SkillActiveForms[i]));
        }
    }

    IEnumerator StartQueueCycle(SkillActiveForm _skillActiveForm)
    {
        if(_skillActiveForm.cooldownSlot)
        _skillActiveForm.cooldownSlot.StartCooldown(_skillActiveForm.skillInBattle.cooltime / speedInBattle);
        yield return new WaitForSeconds(_skillActiveForm.skillInBattle.cooltime/speedInBattle);
        skillQueue.Add(_skillActiveForm);
        if (skillQueue.Count == 1)
            StartCoroutine(QueueCycle(_skillActiveForm));
    }

    IEnumerator QueueCycle(SkillActiveForm _skillActiveForm)
    {
        yield return StartCoroutine(_skillActiveForm.ActiveSkill());

        // 코루틴이 완료된 후의 작업 수행
        skillQueue.Remove(_skillActiveForm);

        // 다음 스킬 실행
        if (skillQueue.Count > 0)
        {
            StartCoroutine(QueueCycle(skillQueue[0]));
        }

        if (_skillActiveForm.cooldownSlot)
        {
            _skillActiveForm.cooldownSlot.StartCooldown(_skillActiveForm.skillInBattle.cooltime / speedInBattle);
        }

        yield return new WaitForSeconds(_skillActiveForm.skillInBattle.cooltime / speedInBattle);
        skillQueue.Add(_skillActiveForm);

        if (skillQueue.Count == 1)
        {
            StartCoroutine(QueueCycle(_skillActiveForm));
        }
    }
    public void FindNewTargetOpponent()
    {
        List<BaseInBattle> targetByColumn;
        int targetColumn;
        List<BaseInBattle> targetsBase = (IsEnemy ? BattleScenario.characters : BattleScenario.enemies).Where(item => item!=null&& !item.isDead).ToList();
        if (targetsBase.Count == 0)
        {
            targetOpponent = null;
            return;
        }
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

    }
    public void FindNewTargetAlly()
    {

        List<BaseInBattle> targetsBase = (IsEnemy ? BattleScenario.enemies : BattleScenario.characters).Where(item =>item&& !item.isDead).ToList();
        targetsBase.Remove(this);
        if (targetsBase.Count == 0)
        {
            targetAlly = null;
            return;
        }
        int minDist = targetsBase.Min(item => GetDistance(grid.index, item.grid.index));
        List<BaseInBattle> targetCandi = targetsBase.Where(item => GetDistance(grid.index, item.grid.index) == minDist).ToList();
        targetAlly = targetCandi[Random.Range(0, targetCandi.Count)];
        int GetDistance(int _index0, int _index1)
        {
            int rowDist = Mathf.Abs(_index0 % 3 - _index1 % 3);
            int ColumnDist = Mathf.Abs(_index0 / 3 - _index1 / 3);
            return rowDist + ColumnDist;
        }
    }
    public void StopBattle()
    {
        GameManager.battleScenario.regularEffect -= ActiveRegularEffect;
        grid.owner = null;
        skillQueue.Clear();
        //hpBarInScene.StopAllCoroutines();
        //if (hpBarInUi)
        //    hpBarInUi.StopAllCoroutines();
    }
    public void StartBattle()
    {
        FindNewTargetAlly();
        FindNewTargetOpponent();
        SetAnimParam();
        SetSkillsAndStart();
    }
    private IEnumerator FadeOutCoroutine()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Image[] images = hpBarInScene.GetComponentsInChildren<Image>();
        // 3초 동안 반복
        float timer = 0f;
        float coroutineTime = 2f;
        while (timer < coroutineTime)
        {
            // 시간에 따라 투명도 조절
            float alpha = Mathf.Lerp(1f, 0f, timer / coroutineTime);

            // 각 SpriteRenderer의 투명도 조절
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (!spriteRenderer)
                    continue;
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
            foreach (Image image in images)
            {
                if (!image)
                    continue;
                Color color = image.color;
                color.a = alpha;
                image.color = color;
            }
            // 0.05초마다 갱신
            yield return new WaitForSeconds(0.05f);
            timer += 0.05f;
        }

        // 코루틴 종료 후 모든 SpriteRenderer를 비활성화
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer)
                spriteRenderer.gameObject.SetActive(false);
        }
    }


    public static List<BaseInBattle> GetTargetsByRange(EffectRange _range, BaseInBattle _target, bool _isTargetEnemy)
    {
        List<GridObject> targetGrids = null;
        List<BaseInBattle> returnValue;
        bool orderDir = _isTargetEnemy ^ _target.IsEnemy;
        List<GridObject> gridsBase = (orderDir ? BattleScenario.EnemyGrids : BattleScenario.CharacterGrids);
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
        returnValue = targetGrids.Where(item => item.owner != null).Select(item => item.owner).ToList();
        return returnValue;
    }
    public float CalcEffectValueByType(float calcValue, EffectType _type)
    {
        switch (_type)//Value 보정값 설정
        {
            case EffectType.Damage:
                calcValue += GetRegularValue(EffectType.Enchant);
                float incrementValue = GetRegularValue(EffectType.AttAscend) - GetRegularValue(EffectType.AttDescend);
                calcValue *= Mathf.Max(1 + incrementValue, 0);
                if (GameManager.CalculateProbability(GetRegularValue(EffectType.Critical)))
                {
                    //치명타 판정
                    calcValue *= 2;
                }
                break;
            case EffectType.Bleed:
                calcValue *= 1 + GetRegularValue(EffectType.AttAscend);
                calcValue *= 1 - GetRegularValue(EffectType.AttDescend);
                break;
            case EffectType.Heal:
                calcValue *= 1 + GetRegularValue(EffectType.HealAscend);
                calcValue *= 1 + GetRegularValue(EffectType.ResilienceAscend);
                break;

            case EffectType.AttAscend:
            case EffectType.ResistAscend:
            case EffectType.Enchant:
                calcValue *= 1 + GetRegularValue(EffectType.BuffAscend);
                break;
            case EffectType.AttDescend:
            case EffectType.ResistDescend:
                calcValue *= 1 + GetRegularValue(EffectType.DebuffAscend);
                break;
        }

        return calcValue;
    }
    public void PassiveReconnect()
    {
        foreach (var passive in passiveEffects)
        {
            passive.RemoveToTarget();
            passive.SetTargets(this);
            passive.ApplyToTarget();
        }
    }
}