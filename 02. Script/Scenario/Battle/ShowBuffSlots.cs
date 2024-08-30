using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBuffSlots : MonoBehaviour
{
    public GameObject buffSlotPrefab;
    public Dictionary<EffectType, BuffSlot> buffDict = new();
    public List<BuffSlot> slotPool = new();
    public Transform parentBuffSlot;
    public EffectType testType;
    private void Awake()
    {
        testType = EffectType.AttAscend;
        parentBuffSlot.gameObject.SetActive(false);
    }
    public BuffSlot SetBuff(EffectType _effectType)
    {
        BuffSlot buffSlot;
        if (buffDict.TryGetValue(_effectType, out buffSlot))
        {
            buffSlot.ChangeBuffSlotOne(true);
        }
        else
        {
            if (slotPool.Count == 0)
            {
                buffSlot = Instantiate(buffSlotPrefab, parentBuffSlot).GetComponent<BuffSlot>();
            }
            else
            {
                buffSlot = slotPool[0];
                slotPool.Remove(buffSlot);
                buffSlot.transform.SetSiblingIndex(parentBuffSlot.childCount - 1);
                buffSlot.gameObject.SetActive(true);
            }
            buffSlot.SetBuffSlot(_effectType);
            buffDict.Add(_effectType, buffSlot);
        }
        return buffSlot;
    }
    public void RemoveBuff(EffectType _effectType)
    {
        BuffSlot buffSlot;
        if (buffDict.TryGetValue(_effectType, out buffSlot))
        {
            if (!buffSlot.ChangeBuffSlotOne(false))
            {
                slotPool.Add(buffDict[_effectType]);
                buffDict.Remove(_effectType);
            }
        }
    }
    public void RemoveAllBuff()
    {
        foreach (var buffSlot in buffDict.Values)
        {
            buffSlot.ChangeBuffSlotOne(false);
            slotPool.Add(buffSlot);
            buffSlot.gameObject.SetActive(false);
        }
        buffDict.Clear();
    }
    [ContextMenu("AddTest")]
    public void AddTest()
    {
        SetBuff(testType);
    }
    [ContextMenu("RemoveTest")]
    public void RemoveTest()
    {
        RemoveBuff(testType);
    }
}
