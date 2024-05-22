using ItemCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUi : MonoBehaviour
{
    public Transform parentSlot;
    public List<InventorySlot> slots { get; private set; }
    private void Awake()
    {
        slots = new();
        for (int i = 0; i < ItemManager.inventorySize; i++)
        {
            slots.Add(parentSlot.GetChild(i).GetComponent<InventorySlot>());
        }
    }
    public void SetSlot(CountableItem _ci, int _index)
    {
        slots[_index].SetSlot(_ci);
    }
}
