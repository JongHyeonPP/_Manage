using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnumCollection;
using System;
using DefaultCollection;
using UnityEngine.UI;

public class RecruitUi : LobbyUiBase
{
    public TMP_Text textStatusHp;
    public TMP_Text textStatusAbility;
    public TMP_Text textStatusSpeed;
    public TMP_Text textStatusResist;
    public Transform parentApplicant;
    public List<ApplicantSlot> applicantSlots;
    public List<ApplicantSlot> selectedSlots = new() { null, null, null };
    public List<TalentSlot> talentSlots;
    public TalentExplainUi talentExplainUi;

    private void Awake()
    {
        InitTalent();
    }
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
    public void InitTalent()
    {
        foreach (TalentSlot x in talentSlots)
        {
            x.gameObject.SetActive(false);
        }
    }
    public void AllocateApplicant()
    {
        float upValue;
        if (GameManager.gameManager.upgradeValueDict.ContainsKey(UpgradeEffectType.AllocateNumberUp))
            upValue = GameManager.gameManager.upgradeValueDict[UpgradeEffectType.AllocateNumberUp];
        else
            upValue = 0f;
        for (int i = 0; i < 6; i++)
        {
            if (i < 3 + upValue)
            {
                applicantSlots[i].gameObject.SetActive(true);
                applicantSlots[i].InitApplicantSlot();
            }
            else
            {
                applicantSlots[i].gameObject.SetActive(false);
            }
        }
    }
    public void InactiveEnterBtns()
    {
        foreach (ApplicantSlot slot in applicantSlots)
        {
            if (slot.gameObject.activeSelf)
                slot.IsActived = false;
        }
    }
    public int AddSelectedSlot(ApplicantSlot _slot)
    {
        for (int i = 0; i < 3; i++)
        {
            if (selectedSlots[i] == null)
            {
                selectedSlots[i] = _slot;
                return i;
            }
        }
        GameManager.gameManager.SetPopUp("최대 3명까지 모집할 수 있습니다.", "3명");
        return -1;
    }
    public void RemoveSelectedSlot(ApplicantSlot _slot)
    {
        int index = selectedSlots.IndexOf(_slot);
        selectedSlots[index] = null;
    }

    internal void SetTalents(List<TalentClass> talents)
    {
        if (talents.Count == 0)
        {
            talentSlots[0].gameObject.SetActive(true);
            talentSlots[1].gameObject.SetActive(false);
            talentSlots[2].gameObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (i < talents.Count)
                {
                    talentSlots[i].gameObject.SetActive(true);
                    talentSlots[i].SetCurrentTalent(talents[i]);
                }
                else
                {
                    talentSlots[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
