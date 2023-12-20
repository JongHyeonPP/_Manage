using EnumCollection;
using StructCollection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class EnemyScript : CharacterBase
{
    public static readonly Color TARGET_COLOR = new(1f, 0f, 0f, 0.5f);
    public static readonly float DEFAULT_PROB = 0.6f;
    public static readonly float INCREASE_PROB = 0.1f;
    public List<PriorProb> PriorProbs { get; private set; } = new();
    public void InitEnemy(List<Skill> _skills, ObjectGrid _grid, float _hp, float _ability, float _resist, float _speed)
    {
        PriorProbs.Add(new PriorProb(new Skill(_speed)));
        PriorProbs = PriorProbs.OrderBy(x => x.priority).ToList();
        IsEnemy = true;
        grid = _grid;
        grid.owner = this;
        InitCharacter(_skills, _hp, _hp, _ability, _resist, _speed);
    }

    public override void OnDead()
    {
        OnDead_Base();
        bool gameOverFlag = false;
        foreach (CharacterBase enemy in GameManager.Enemies)
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

    public class PriorProb
    {
        public Skill skill;
        public int priority = 99;//0에 가까운 자연수일 수록 우선 순위가 높다
        public float probability = DEFAULT_PROB;
        public PriorProb(Skill _skill)
        {
            skill = _skill;

            if (_skill.categori == SkillCategori.Default)
            {
                probability = 1f;
                return;
            }
            foreach (SkillEffect effect in _skill.effects)
            {
                if (effect.type >= EffectType.AttAscend)
                {
                    if(priority>1)
                    priority = 1;
                }
                else if (effect.type == EffectType.Damage)
                {
                    if (priority > 0)
                        priority = 0;
                }
            }
            
        }
    }
}