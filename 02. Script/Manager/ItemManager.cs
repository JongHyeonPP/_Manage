using BattleCollection;
using EnumCollection;
using Firebase.Firestore;
using ItemCollection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ItemManager : MonoBehaviour
{
    public static ItemManager itemManager;
    public static readonly int inventorySize = 36;//6*6

    public Sprite item_None;
    public Sprite item_Normal;
    public Sprite item_Rare;
    public Sprite item_Unique;

    public Sprite name_Normal;
    public Sprite name_Rare;
    public Sprite name_Unique;

    public Sprite ingredient_Fish;
    public Sprite ingredient_Meat;
    public Sprite ingredient_Fruit;
    public Sprite ingredient_Vegetable;
    public Sprite ingredient_Random;

    public InventoryUi inventoryUi;
    public GetJobUi getJobUi;
    public UpgradeSkillUi upgradeSkillUi;

    public Sprite book_Default;
    public Sprite book_P;
    public Sprite book_S;
    public Sprite book_U;

    private List<Item> notReievedItem;

    public static readonly Color powerColor = new(1f, 0.12f, 0.12f);
    public static readonly Color sustainColor = new(1f, 1f, 0.34f);
    public static readonly Color utilColor = new(0.2003195f, 1f, 0.0235849f);

    public GameObject backgroundInventoryAdd;
    public CharacterData selectedCharacter { get; set; }

    public bool isUpgradeCase;
    public static readonly int[] needExp = new int[2] { 5, 10 };

    private void Awake()
    {
        if (!itemManager)
        {
            itemManager = this;
            inventoryUi.gameObject.SetActive(false);
            getJobUi.gameObject.SetActive(false);
            upgradeSkillUi.gameObject.SetActive(false);
            backgroundInventoryAdd.gameObject.SetActive(false);
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
            inventoryUi.inventorySlots[i].SetSlot(ci);
        }
    }

    public async Task SetLootAsync()
    {
        List<CountableItem> main = new();
        List<CountableItem> sub = new();
        //전리품 생성
        IngredientType ingredientType;
        SkillCategori skillCategori;
        ItemGrade skillGrade;
        GetLootTypesByBackgroundType(StageScenarioBase.stageCanvas.currentNode.nodeType.backgroundType, out skillCategori, out skillGrade, out ingredientType);
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
                    id = GetRandomSkillIdByGradeCategori(ItemGrade.Normal, skillCategori);
                    item = GetItemClass(ItemType.Skill, id);
                    break;
            }
            CountableItem ci = new(item);
            main.Add(ci);
        }

        int ingredientNum = Mathf.RoundToInt(Random.Range(3, 5) * (1 + BattleScenario.rewardAscend));
        for (int i = 0; i < ingredientNum; i++)//재료는 3개나 4개
        {
            string ingredientId = GetRandomIngredientId(ingredientType);
            Item ingredientItem = GetItemClass(ItemType.Ingredient, ingredientId);
            CountableItem ci = new(ingredientItem);
            AddCiToArr(sub, ci);
        }
        main = main.OrderBy(data => data.item.itemType).ToList();
        sub = sub.OrderBy(data => ((IngredientClass)data.item).pokerNum).ToList();
        int gold = Random.Range(10, 13);
        int goldAscend = Mathf.RoundToInt(gold * BattleScenario.rewardAscend);
        GameManager.gameManager.ChangeGold(gold + goldAscend);
        List<CountableItem> addMainSub = new(main);
        addMainSub.AddRange(sub);
        AddCiesToInventory(addMainSub);
        await SetInventoryAtDb();
        if (GameManager.battleScenario)
        {
            GameManager.battleScenario.lootUi.SetLootAtUi(main, sub, gold, goldAscend);
        }
    }

    public void AddCiesToInventory(List<CountableItem> addMainSub)
    {
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

    }

    public InventorySlot GetExistingSlot(Item _item)
    {
        return inventoryUi.inventorySlots
            .Where(data => data.ci != null && data.ci.item.itemId == _item.itemId)
            .FirstOrDefault();
    }

    public void SetItemToAbleIndex(CountableItem _ci)
    {
        int ableIndex = GetAbleIndex();
        if (ableIndex != -1)
            inventoryUi.inventorySlots[ableIndex].SetSlot(_ci);
        else
        {
            string popUpMessage = (GameManager.language == Language.Ko) ? "가방이 가득차 아이템을\n수령하지 못했습니다." : "Bag is full, unable to receive the item.";
            GameManager.gameManager.SetPopUp(popUpMessage);
        }
            

    }
    public int GetAbleIndex()
    {
        int ableIndex;
        for (ableIndex = 0; ableIndex < inventorySize; ableIndex++)
        {
            if (inventoryUi.inventorySlots[ableIndex].ci == null || inventoryUi.inventorySlots[ableIndex].ci.amount ==0)
                break;
        }
        if (ableIndex >= inventorySize)
            ableIndex = -1;
        return ableIndex;
    }

    public async Task SetInventoryAtDb()
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
            string itemId;
            switch (ci.item.itemType)
            {
                default:
                    itemId = ci.item.itemId;
                    break;
                case ItemType.Skill:
                    itemId = ci.item.itemId +":::"+ ci.item.itemGrade.ToString();
                    typeStr = "Skill";
                    break;
            }
            Dictionary<string, object> itemDict = new()
            {
                { "ItemType", typeStr },
                { "ItemId", itemId },
                { "Amount", ci.amount}
            };
            setArr[i] = itemDict;
        }
        await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
        {
            DataManager.dataManager.SetDocumentData("Inventory", setArr, "Progress", GameManager.gameManager.Uid);
            DataManager.dataManager.SetDocumentData("Gold", GameManager.gameManager.gold, "Progress", GameManager.gameManager.Uid);
            return Task.CompletedTask;
        });


    }
    public Task SetCharacterAtDb()
    {
        foreach (CharacterData data in GameManager.gameManager.characterList)
        {
            if (data)
                data.SetCharacterAtDbAsync();
        }

        return Task.CompletedTask;
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
    private string GetRandomSkillIdByGradeCategori(ItemGrade _grade, SkillCategori _skillCategori)
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
        List<string> skillList;
        if (_skillCategori == default)
        {
            skillList = LoadManager.loadManager.skillsDict
            .Where(item => item.Value.categori != SkillCategori.Enemy)
            .Select(item => item.Key)
            .ToList();
        }
        else
        {
            skillList = LoadManager.loadManager.skillsDict
         .Where(item => item.Value.categori == _skillCategori)
         .Select(item => item.Key)
         .ToList();
        }
        string skillId = skillList[Random.Range(0, skillList.Count)];
        return $"{skillId}:::{gradeStr}";
    }
    private string GetRandomIngredientId(IngredientType _ingredientType)
    {
        List<string> ingredientList;
        if (_ingredientType == IngredientType.All)
        {
            ingredientList = LoadManager.loadManager.ingredientDict
            .Select(item => item.Key)
            .ToList();
        }
        else
        {
            ingredientList = LoadManager.loadManager.ingredientDict
            .Where(item => item.Value.ingredientType == _ingredientType)
            .Select(item => item.Key)
            .ToList();
        }
        string ingredientId = ingredientList[Random.Range(0, ingredientList.Count)];
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
                string[] splittedSkillId = _id.Split(":::");
                Skill skill = LoadManager.loadManager.skillsDict[splittedSkillId[0]];
                int index;
                switch (splittedSkillId[1])
                {
                    default:
                        index = 0;
                        break;
                    case "Rare":
                        index = 1;
                        break;
                    case "Unique":
                        index = 2;
                        break;
                }
                SkillAsItem skillAsItem = skill.GetAsItem(index);
                returnValue = skillAsItem;
                break;
            case ItemType.Ingredient:
                returnValue = LoadManager.loadManager.ingredientDict[_id];
                break;
            case ItemType.Food:
                returnValue = LoadManager.loadManager.foodDict[_id];
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

    public void ActiveCharacter(bool _isActive)
    {
        inventoryUi.ch.gameObject.SetActive(_isActive);
    }

    public void LoadEquip()
    {
        CharacterData character = GameManager.gameManager.characterList.Where(data => data != null).FirstOrDefault();
        
        SkillAsItem[] skills = character.skillAsItems;
        WeaponClass weapon = character.weapon;
        inventoryUi.SetEquipSlot(skills[0], 0);
        inventoryUi.SetEquipSlot(skills[1], 1);
        inventoryUi.SetEquipSlot(weapon, 2);
    }
    public void SetJobAtSelectedCharacter()
    {
        JobClass job = GameManager.gameManager.GetJob(selectedCharacter.skillAsItems[0].itemId, selectedCharacter.skillAsItems[1].itemId);
        selectedCharacter.jobClass = job;
        selectedCharacter.characterHierarchy.SetJobSprite(job);
        if (StageScenarioBase.stageCanvas)
            if (GameManager.gameManager.characterList.IndexOf(selectedCharacter) == 0)
                StageScenarioBase.stageCanvas.characterInStage.characterHierarchy.SetJobSprite(job);
        inventoryUi.ch.SetJobSprite(job);
        inventoryUi.jobSlot.SetJobIcon(job);
        for (int i = 0; i < 2; i++)
        {
            inventoryUi.equipSlots[i].expBar.SetActive(true);
            inventoryUi.equipSlots[i].SetExp(0f);
        }
    }
    public void SetGetJobUi()
    {
        getJobUi.gameObject.SetActive(true);
        getJobUi.SetInfo(selectedCharacter);
    }
    public int GetNeedExp(ItemGrade _skillGrade)
    {
        switch (_skillGrade)
        {
            default:
                return needExp[0];
            case ItemGrade.Rare:
                return needExp[1];
        }
    }
    public static void GetLootTypesByBackgroundType(BackgroundType _backgroundType, out SkillCategori _skillCategori, out ItemGrade _skillGrade, out IngredientType _ingredientType)
    {
        switch (_backgroundType)
        {
            default://Plains
                _ingredientType = IngredientType.Vegetable;
                _skillCategori = SkillCategori.Power;
                _skillGrade = ItemGrade.Normal;
                break;
            case BackgroundType.Forest:
                _ingredientType = IngredientType.Meat;
                _skillCategori = SkillCategori.Sustain;
                _skillGrade = ItemGrade.Normal;
                break;
            case BackgroundType.Beach:
                _ingredientType = IngredientType.Fish;
                _skillCategori = SkillCategori.Util;
                _skillGrade = ItemGrade.Normal;
                break;
            case BackgroundType.Ruins:
                _ingredientType = IngredientType.Fruit;
                _skillCategori = SkillCategori.Power;
                _skillGrade = ItemGrade.Normal;
                break;
            case BackgroundType.ElfCity:
                _ingredientType = IngredientType.All;
                _skillCategori = SkillCategori.Default;
                _skillGrade = ItemGrade.Rare;
                break;
            case BackgroundType.MysteriousForest:
                _ingredientType = IngredientType.Vegetable;
                _skillCategori = SkillCategori.Sustain;
                _skillGrade = ItemGrade.Normal;
                break;
            case BackgroundType.WinterForest:
                _ingredientType = IngredientType.Meat;
                _skillCategori = SkillCategori.Util;
                _skillGrade = ItemGrade.Normal;
                break;
            case BackgroundType.VineForest:
                _ingredientType = IngredientType.Fish;
                _skillCategori = SkillCategori.Power;
                _skillGrade = ItemGrade.Normal;
                break;
            case BackgroundType.Swamp:
                _ingredientType = IngredientType.Fruit;
                _skillCategori = SkillCategori.Sustain;
                _skillGrade = ItemGrade.Normal;
                break;
            case BackgroundType.IceField:
                _ingredientType = IngredientType.All;
                _skillCategori = SkillCategori.Default;
                _skillGrade = ItemGrade.Rare;
                break;
            case BackgroundType.DesertRuins:
                _ingredientType = IngredientType.Vegetable;
                _skillCategori = SkillCategori.Util;
                _skillGrade = ItemGrade.Normal;
                break;
            case BackgroundType.Cave:
                _ingredientType = IngredientType.Meat;
                _skillCategori = SkillCategori.Power;
                _skillGrade = ItemGrade.Normal;
                break;
            case BackgroundType.Desert:
                _ingredientType = IngredientType.Fish;
                _skillCategori = SkillCategori.Sustain;
                _skillGrade = ItemGrade.Normal;
                break;
            case BackgroundType.RedRock:
                _ingredientType = IngredientType.Fruit;
                _skillCategori = SkillCategori.Util;
                _skillGrade = ItemGrade.Normal;
                break;
        }
    }
}
