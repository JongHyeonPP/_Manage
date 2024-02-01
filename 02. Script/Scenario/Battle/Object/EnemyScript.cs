using EnumCollection;
using BattleCollection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class EnemyScript : CharacterBase
{
    public static readonly Color TARGET_COLOR = new(1f, 0f, 0f, 0.5f);
    public static readonly float DEFAULT_PROB = 0.6f;
    public static readonly float INCREASE_PROB = 0.1f;
    private void Awake()
    {
        IsEnemy = true;
    }
    public void InitEnemy(EnemyClass _enemyClass, ObjectGrid _grid, bool _isMonster)
    {
        isMonster = _isMonster;
        InitCharacter(true);
        maxHp = Hp = maxHpInBattle =  _enemyClass.hp;
        ability = abilityInBattle = _enemyClass.ability;
        resist = resistInBattle = _enemyClass.resist;
        speed = speedInBattle = _enemyClass.speed;
        skills = _enemyClass.skills;
        grid = _grid;
        grid.owner = this;
    }
    public override void OnDead()
    {
        StartCoroutine(OnDead_Base());
        bool gameOverFlag = false;
        foreach (CharacterBase enemy in BattleScenario.enemies)
        {
            if (!enemy.isDead)
                gameOverFlag = true;
        }
        if (!gameOverFlag)
            GameManager.battleScenario.StageClear();
    }

    public override void SetAnimParam()
    {
    }
}