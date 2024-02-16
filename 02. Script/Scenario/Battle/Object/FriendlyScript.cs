using EnumCollection;
using BattleCollection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LobbyCollection;

public class FriendlyScript : CharacterBase
{
    public static readonly Color TARGET_COLOR = new(0f, 0f, 1f, 0.5f);
    public bool isAct = false;
    public new string name;
    public List<TalentStruct> talents;

    public void InitFriendly(string _documentId)
    {
        InitCharacter();
        IsEnemy = false;
        documentId = _documentId;
        Transform armSet = transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(3);
        weaponRenderer = armSet.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
        shieldRenderer = armSet.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetComponent<SpriteRenderer>();
    }
    [ContextMenu("DeadTest")]
    public override void OnDead()
    {
        GameManager.battleScenario.StartCoroutine(OnDead_Base());
        bool gameOverFlag = false;
        foreach (var x in BattleScenario.friendlies)
            if (!x.isDead)
                gameOverFlag = true;
        if (!gameOverFlag)
        {
            //GameManager.gameManager.GameOver();
        }

    }

    public override void SetAnimParam()
    {

    }
}