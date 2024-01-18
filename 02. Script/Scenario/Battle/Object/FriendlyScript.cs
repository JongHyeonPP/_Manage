using EnumCollection;
using BattleCollection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LobbyCollection;

public class FriendlyScript : CharacterBase
{
    public JobClass job;
    public static readonly Color TARGET_COLOR = new(0f, 0f, 1f, 0.5f);
    public bool isAct = false;
    public new string name;
    public List<TalentStruct> talents;

    public void InitFriendly(string _documentId)
    {
        InitCharacter();
        IsEnemy = false;
        documentId = _documentId;
    }
    [ContextMenu("Dead")]
    public override void OnDead()
    {
        StartCoroutine(OnDead_Base());
        bool gameOverFlag = false;
        foreach (var x in BattleScenario.friendlies)
            if (!x.isDead)
                gameOverFlag = true;
        if (!gameOverFlag)
        {
            GameManager.gameManager.GameOver();
        }

    }

    public override void SetAnimParam()
    {
        animator.SetFloat("NormalState", 0.0f);
        animator.SetFloat("SkillState", 0.0f);
    }
}