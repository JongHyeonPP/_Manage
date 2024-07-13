using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnumCollection;

public class RecruitUi : LobbyUiBase
{
    public TMP_Text textStatusHp;
    public TMP_Text textStatusAbility;
    public TMP_Text textStatusSpeed;
    public TMP_Text textStatusResist;

    public void SetStatusText(float _hp, float _ability, float _speed, float _resist)
    {
        textStatusHp.text = _hp.ToString("F0");
        textStatusAbility.text = _ability.ToString("F0");
        textStatusSpeed.text = _speed.ToString("F1");
        textStatusResist.text = _resist.ToString("F0");
    }
    public void InitStatusText()
    {
        textStatusHp.text =
        textStatusAbility.text =
        textStatusSpeed.text =
        textStatusResist.text =
        "-";
    }
}
