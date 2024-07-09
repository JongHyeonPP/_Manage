using EnumCollection;
using Firebase.Firestore;
using ItemCollection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager itemManager;
    public static readonly int inventorySize = 25;//5*5
    public Sprite item_None;
    public Sprite item_Normal;
    public Sprite item_Rare;
    public Sprite item_Unique;

    public Sprite name_Normal;
    public Sprite name_Rare;
    public Sprite name_Unique;
    public InventoryUi inventoryUi;
    public GameObject inventoryButton;
    public Sprite book_P;
    public Sprite book_S;
    public Sprite book_U;

    private List<Item> notReievedItem;

    public static readonly Color powerColor = new(1f, 0.12f, 0.12f);
    public static readonly Color sustainColor = new(1f, 1f, 0.34f);
    public static readonly Color utilColor = new(0.2003195f, 1f, 0.0235849f);

    public InventorySlot targetInventorySlot;
    public EquipSlot targetEquipSlot;
    public InventorySlot draggingSlot;
    public InventorySlot throwSlot;

    public CharacterData selectedCharacter;
    private void Awake()
    {
        if (!itemManager)
        {
            itemManager = this;
            inventoryUi.gameObject.SetActive(false);
            inventoryButton.gameObject.SetActive(true);
            inventoryUi.InitInventory();
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
            inventoryUi.SetInventorySlot(ci, i);
        }
    }

    public async Task SetLootAsync()
    {
        List<CountableItem> main = new();
        List<CountableItem> sub = new();
        Dictionary<string, object> documentDict = new();
        //전리품 생성
        if (false)
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
                main.Add(ci);
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
            InventorySlot existingSlot = null;

            if (ci.item.itemType != ItemType.Weapon)
            {
                existingSlot = GetExistingSlot(ci.item);
            }

            if (existingSlot == null)
            {
                SetItemToAbleIndex(ci);
            }
            else
            {
                existingSlot.ci.amount += ci.amount;
            }
        }
        //전리품 Set
        await SetInventoryAtDb();
        if (GameManager.battleScenario)
        {
            GameManager.battleScenario.lootUi.SetLootAtUi(main, sub, gold);
        }



        
    }

    public InventorySlot GetExistingSlot(Item _item)
    {
        return inventoryUi.inventorySlots
            .Where(data => data.ci != null && data.ci.item.itemId == _item.itemId)
            .FirstOrDefault();
    }

    public void SetItemToAbleIndex(CountableItem ci)
    {
        int ableIndex = GetAbleIndex();
        if (ableIndex != -1)
        {
            inventoryUi.SetInventorySlot(ci, ableIndex);
        }
    }
    int GetAbleIndex()
    {
        int ableIndex;
        for (ableIndex = 0; ableIndex < inventorySize; ableIndex++)
        {
            if (inventoryUi.inventorySlots[ableIndex].ci == null)
                break;
        }
        if (ableIndex >= inventorySize)
            ableIndex = -1;
        return ableIndex;
    }

    private async Task SetInventoryAtDb()
    {
        object[] setArr = new object[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            CountableItem ci = inventoryUi.inventorySlots[i].ci;
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
            setArr[i] = itemDict;
        }


        await DataManager.dataManager.SetDocumentData("Inventory", setArr, "Progress", GameManager.gameManager.Uid);
    }
    private async Task SetEquipAtDb()
    {
        foreach (CharacterData data in GameManager.gameManager.characterList)
        {
            await data.SetEquipAtDbAsync();
        }
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
        return $"{weaponTypeStr}:::{name}";
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

    public async void InventoryActive()
    {
        bool isActive = !inventoryUi.gameObject.activeSelf;
        inventoryUi.gameObject.SetActive(isActive);
        if (!isActive)
        {
            await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
            {
                await SetInventoryAtDb();
                await SetEquipAtDb();
                return Task.CompletedTask;
            });
        }
    }
    public void ActiveCharacter(bool _isActive)
    {
        inventoryUi.ch.gameObject.SetActive(_isActive);
    }

    public void LoadEquip()
    {
        CharacterData character = GameManager.gameManager.characterList[0];

        Skill[] skills = character.skills;
        WeaponClass weapon = character.weapon;
        inventoryUi.SetEquipSlot(skills[0], 0);
        inventoryUi.SetEquipSlot(skills[1], 1);
        inventoryUi.SetEquipSlot(weapon, 2);
    }
    public void SetCategoriCharAtText(SkillCategori _categori, TMP_Text _text)
    {
        switch (_categori)
        {
            default:
                _text.text = "P";
                _text.color = powerColor;
                _text.GetComponent<RectTransform>().anchoredPosition = new Vector3(1.13f, 0.49f, 0f);
                break;
            case SkillCategori.Util:
                _text.text = "U";
                _text.color = utilColor;
                _text.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.4300003f, 0.3599997f, 0f);
                break;
            case SkillCategori.Sustain:
                _text.text = "S";
                _text.color = sustainColor;
                _text.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.02999973f, 0.32f, 0f);
                break;
        }
    }


}
