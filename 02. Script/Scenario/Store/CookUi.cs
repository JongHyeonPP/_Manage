using HardLight2DUtil;
using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookUi : MonoBehaviour
{
    public IngredientScrollView meatView;
    public IngredientScrollView breadView;
    public IngredientScrollView fruitView;
    public IngredientScrollView vegetableView;
    //Meat, Bread, Fruit, Vegetable
    private void Start()
    {

        IEnumerable<CountableItem> cies = ItemManager.itemManager.inventoryUi.inventorySlots
    .Where(data => data.ci != null)
    .Where(data =>data.ci.item.itemType == EnumCollection.ItemType.Ingredient)
    .Select(data => data.ci);
        List<CountableItem> meatCies = cies.Where(data => ((IngredientClass)data.item).ingredientType == EnumCollection.IngredientType.Meat).ToList();
        List<CountableItem> breadCies = cies.Where(data => ((IngredientClass)data.item).ingredientType == EnumCollection.IngredientType.Bread).ToList();
        List<CountableItem> fruitCies = cies.Where(data => ((IngredientClass)data.item).ingredientType == EnumCollection.IngredientType.Fruit).ToList();
        List<CountableItem> vegetableCies = cies.Where(data => ((IngredientClass)data.item).ingredientType == EnumCollection.IngredientType.Vegetable).ToList();
        SetCiesToScrollView(meatCies, meatView);
        SetCiesToScrollView(breadCies, breadView);
        SetCiesToScrollView(fruitCies, fruitView);
        SetCiesToScrollView(vegetableCies, vegetableView);
    }
    private void SetCiesToScrollView(List<CountableItem> _cies, IngredientScrollView _view)
    {
        foreach (var ci in _cies)
        {
            _view.AddIngredientCi(ci);
        }
    }
}
