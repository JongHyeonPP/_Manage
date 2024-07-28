using EnumCollection;
using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientScrollView : MonoBehaviour
{
    public RectTransform content;
    public GameObject ingredientSlotObj;
    private GridLayoutGroup gridLayoutGroup;
    private List<IngredientSlot> slots = new();
    private void Start()
    {
        gridLayoutGroup = content.GetComponent<GridLayoutGroup>();
        content.sizeDelta = new Vector2(gridLayoutGroup.spacing.x, content.sizeDelta.y);
        for (int i = 0; i < content.childCount; i++)
        {
            content.sizeDelta += new Vector2(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x, 0);
        }
    }
    [ContextMenu("AddTest")]
    public void AddTest()
    {
        Instantiate(ingredientSlotObj, content);
        content.sizeDelta += new Vector2(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x, 0);
    }
    public void AddIngredientCi(CountableItem _ci)
    {
        GameObject slotObj = Instantiate(ingredientSlotObj, content);
        IngredientSlot ingredientSlot = slotObj.GetComponent<IngredientSlot>();
        slots.Add(ingredientSlot);
        ingredientSlot.SetCountableItem(_ci);
    }
}
