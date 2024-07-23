using EnumCollection;
using BattleCollection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class EnemyInBattle : BaseInBattle
{
    public static readonly Color TARGET_COLOR = new(1f, 0f, 0f, 0.5f);
    public static readonly float DEFAULT_PROB = 0.6f;
    public static readonly float INCREASE_PROB = 0.1f;

    public void InitEnemy(EnemyClass _enemyClass, GridObject _grid)
    {
        IsEnemy = true;
        InitBase(_grid);
        
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        //transform.GetChild(0).localScale = Vector3.one * 1.3f;
        skillTargetTransform = transform.GetChild(0).GetChild(0);
        rootTargetTransform.localScale = skillTargetTransform.localScale = new Vector3(1f, 1f, 0f);
        skillTargetTransform.localScale = skillTargetTransform.localScale = new Vector3(1f, 1f, 0f);
        maxHp = Hp = maxHpInBattle =  _enemyClass.hp;
        ability = abilityInBattle = _enemyClass.ability;
        resist = resistInBattle = _enemyClass.resist;
        speed = speedInBattle = _enemyClass.speed;
        skillInBattles = _enemyClass.skills;
        grid = _grid;
        grid.owner = this;
        fireObj = Instantiate(GameManager.gameManager.prefabFire0, transform);
        fireObj.SetActive(false);
    }
    public override void OnDead()
    {
        GameManager.battleScenario.StartCoroutine(OnDead_Base());
        bool gameOverFlag = false;
        foreach (BaseInBattle enemy in BattleScenario.enemies)
        {
            if (!enemy.isDead)
                gameOverFlag = true;
        }
        if (!gameOverFlag)
            StartCoroutine(GameManager.battleScenario.StageClearCoroutine());
    }

    public override void SetAnimParam()
    {
    }
}