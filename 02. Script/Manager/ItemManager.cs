using BattleCollection;
using DefaultCollection;
using EnumCollection;
using Firebase.Firestore;
using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager itemManager;
    public static readonly int inventorySize = 25;//5*5
    public Material itemMat_Normal;
    public Material itemMat_Rare;
    public Material itemMat_Unique;
 
    public Material nameMat_Normal;
    public Material nameMat_Rare;
    public Material nameMat_Unique;
    public InventoryUi inventoryUi;

    public Sprite book_P;
    public Sprite book_S;
    public Sprite book_U;

    private List<Item> notReievedItem;
    private void Awake()
    {
        if (!itemManager)
        {
            itemManager = this;
            inventoryUi.gameObject.SetActive(false);
        }
    }

    public void LoadInventory(object _invenData)
    {
        List<object> objList = _invenData as List<object>;
        for (int i = 0; i < objList.Count; i++)
        {
            object obj = objList[i];
            if (obj == null)
                continue;
            Dictionary<string, object> objDict = obj as Dictionary<string, object>;
            ItemType itemType;
            string itemId;
            int amount;
            switch (objDict["ItemType"])
            {
                default:
                    itemType = ItemType.Weapon;
                    break;
                case "Skill":
                    itemType = ItemType.Skill;
                    break;
                case "Food":
                    itemType = ItemType.Food;
                    break;
                case "Ingredient":
                    itemType = ItemType.Ingredient;
                    break;
            }
            itemId = (string)objDict["ItemId"];
            amount = (int)(long)objDict["Amount"];
            Item item = GetItemClass(itemType, itemId);
            CountableItem ci = new(item, amount);
            inventoryUi.SetSlot(ci, i);
        }
    }

    public async void SetLootAsync()
    {
        int nodeNum = GameManager.gameManager.nodeNum;
        List<CountableItem> main = new();
        List<CountableItem> sub = new();
        Dictionary<string, object> documentDict = new();
        //전리품 생성
        if (nodeNum == 7 || nodeNum == 14)
        {

        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                int lootCase = GameManager.AllocateProbability(new float[] { 0.5f, 0.5f });
                string id;
                Item item = null;
                switch (lootCase)
                {
                    case 0://Weapon
                        id = GetRandomWeaponIdByGrade(ItemGrade.Normal);
                        item = GetItemClass(ItemType.Weapon, id);

                        break;
                    case 1://Skill
                        id = GetRandomSkillIdByGrade(ItemGrade.Normal);
                        item = GetItemClass(ItemType.Skill, id);
                        break;
                }
                CountableItem ci = new(item);
                //main.Add(ci);
            }
            for (int i = 0; i < Random.Range(3, 5); i++)//재료는 3개나 4개
            {
                string ingredientId = GetRandomIngredientId();
                Item ingredientItem = GetItemClass(ItemType.Ingredient, ingredientId);
                CountableItem ci = new(ingredientItem);
                AddCiToArr(sub, ci);
            }
        }
        int gold = Random.Range(10, 13);
        GameManager.gameManager.gold = gold;
        List<CountableItem> addMainSub = new(main);
        addMainSub.AddRange(sub);
        foreach (CountableItem ci in addMainSub)//
        {
            Debug.Log(inventoryUi.slots == null);
            InventorySlot existingSlot = inventoryUi.slots.Where(data => data.ci!=null).Where(data => data.ci.item.itemId == ci.item.itemId).FirstOrDefault();
            if (existingSlot == null)
            {
                int ableIndex = GetAbleIndex();
                if (ableIndex == -1)
                {
                    continue;
                }
                inventoryUi.SetSlot(ci, ableIndex);
            }
            else
            {
                existingSlot.ci.amount += ci.amount;
            }

        }
        //전리품 Set
        SetInventoryAtDb();
        if (GameManager.battleScenario)
        {
            GameManager.battleScenario.LootUi.SetLootAtUi(main, sub, gold);
        }

        int GetAbleIndex()
        {
            int ableIndex;
            for (ableIndex = 0; ableIndex < inventorySize; ableIndex++)
            {
                if (inventoryUi.slots[ableIndex].ci == null)
                    break;
            }
            if (ableIndex >= inventorySize)
                ableIndex = -1;
            return ableIndex;
        }
    }


    private void SetInventoryAtDb()
    {
        object[] setDict = new object[24];
        for (int i = 0; i < inventorySize; i++)
        {
            CountableItem ci = inventoryUi.slots[i].ci;
            if (ci == null)
                continue;
            string typeStr;
            switch (ci.item.itemType)
            {
                default:
                    typeStr = "Weapon";
                    break;
                case ItemType.Skill:
                    typeStr = "Skill";
                    break;
                case ItemType.Food:
                    typeStr = "Food";
                    break;
                case ItemType.Ingredient:
                    typeStr = "Ingredient";
                    break;
            }
            Dictionary<string, object> itemDict = new()
            {
                { "ItemType", typeStr },
                { "ItemId", ci.item.itemId },
                { "Amount", ci.amount}
            };
            setDict[i] = itemDict;
        }
        DataManager.dataManager.SetDocumentData("Inventory", setDict, "Progress", GameManager.gameManager.Uid);
    }

    private string GetRandomWeaponIdByGrade(ItemGrade _grade)
    {
        int caseNum = GameManager.AllocateProbability(0.25f, 0.25f, 0.25f, 0.25f);
        WeaponType weaponType;
        string weaponTypeStr;
        switch (caseNum)
        {
            default:
                weaponType = WeaponType.Sword;
                weaponTypeStr = "Sword";
                break;
            case 1:
                weaponType = WeaponType.Bow;
                weaponTypeStr = "Bow";
                break;
            case 2:
                weaponType = WeaponType.Magic;
                weaponTypeStr = "Magic";
                break;
            case 3:
                weaponType = WeaponType.Club;
                weaponTypeStr = "Club";
                break;
        }
        Dictionary<string, WeaponClass> targetWeaponDict = LoadManager.loadManager.weaponDict[weaponType];
        List<string> targetKeyList = targetWeaponDict.Where(item => item.Value.itemGrade == _grade).Select(kvp => kvp.Key).ToList();
        string name = targetKeyList[Random.Range(0, targetKeyList.Count)];
        return $"{weaponTypeStr}:::{name}" ;
    }
    private string GetRandomSkillIdByGrade(ItemGrade _grade)
    {
        string gradeStr;
        switch (_grade)
        {
            default:
                gradeStr = "Normal";
                break;
            case ItemGrade.Rare:
                gradeStr = "Rare";
                break;
            case ItemGrade.Unique:
                gradeStr = "Unique";
                break;

        }
        List<string> skillList = LoadManager.loadManager.skillsDict
     .Where(item => item.Value.categori != SkillCategori.Enemy)
     .Select(item => item.Key)
     .ToList();
        string skillId = skillList[Random.Range(0, skillList.Count)];
        return $"{skillId}:::{gradeStr}";
    }
    private string GetRandomIngredientId()
    {
        List<string> skillList = LoadManager.loadManager.ingredientDict.Keys.ToList();
        string ingredientId = skillList[Random.Range(0, skillList.Count)];
        return ingredientId;
    }
    private Item GetItemClass(ItemType _type, string _id)
    {
        Item returnValue = null;
        switch (_type)
        {
            case ItemType.Weapon:
                string[] splittedId0 = _id.Split(":::");
                WeaponType weaponType;
                switch (splittedId0[0])
                {
                    default:
                        weaponType = WeaponType.Sword;
                        break;
                    case "Bow":
                        weaponType = WeaponType.Bow;
                        break;
                    case "Magic":
                        weaponType = WeaponType.Magic;
                        break;
                    case "Club":
                        weaponType = WeaponType.Club;
                        break;
                }
                WeaponClass weapon = LoadManager.loadManager.weaponDict[weaponType][splittedId0[1]];
                returnValue = weapon;
                break;
            case ItemType.Skill:
                string[] spllitedId1 = _id.Split(":::");
                ItemGrade grade;
                switch (spllitedId1[1])
                {
                    default:
                        grade = ItemGrade.Normal;
                        break;
                    case "Rare":
                        grade = ItemGrade.Rare;
                        break;
                    case "Unique":
                        grade = ItemGrade.Unique;
                        break;
                }
                returnValue = LoadManager.loadManager.skillsDict[spllitedId1[0]].LocalizeSkill(grade);
                break;
            case ItemType.Food:
                returnValue = LoadManager.loadManager.foodDict[_id];
                break;
            case ItemType.Ingredient:
                returnValue = LoadManager.loadManager.ingredientDict[_id];
                break;
        }
        return returnValue;
    }
    private void AddCiToArr(List<CountableItem> _list, CountableItem ci)
    {
        CountableItem existingCi = _list.Where(data => data.item.itemId == ci.item.itemId).FirstOrDefault();
        if (existingCi != null)
            existingCi.amount++;
        else
            _list.Add(ci);

    }
}
