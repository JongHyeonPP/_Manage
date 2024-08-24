using EnumCollection;
using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientScrollView : CustomScrollRect
{
    public new RectTransform content;
    public GameObject ingredientSlotObj;
    private GridLayoutGroup gridLayoutGroup;
    public List<IngredientSlot> slots = new();
    private new void Start()
    {
        base.Start();
        gridLayoutGroup = content.GetComponent<GridLayoutGroup>();
        content.sizeDelta = new Vector2(gridLayoutGroup.spacing.x, content.sizeDelta.y);
        for (int i = 0; i < content.childCount; i++)
        {
            content.sizeDelta += new Vector2(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x, 0);
        }
    }
    public void AddIngredientCi(CountableItem _ci)
    {
        GameObject slotObj = Instantiate(ingredientSlotObj, content);
        IngredientSlot ingredientSlot = slotObj.GetComponent<IngredientSlot>();
        ingredientSlot.parentScrollView = this;
        slots.Add(ingredientSlot);
        ingredientSlot.SetCountableItem(_ci);
    }
    public void ModifySizeDelta(bool _isIncrease)
    {
        if (_isIncrease)
            content.sizeDelta += new Vector2(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x, 0);
        else
            content.sizeDelta -= new Vector2(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x, 0);
    }
}
