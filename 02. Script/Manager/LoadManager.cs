using BattleCollection;
using DefaultCollection;
using EnumCollection;
using Firebase.Firestore;
using ItemCollection;
using LobbyCollection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LoadManager : MonoBehaviour//Firestore에 있는 기초 데이터들 로딩해서 저장하고 있는 스크립트
{
    public static LoadManager loadManager;
    bool isInit = false;
    public Dictionary<string, SkillForm> skillsDict = new();//Key는 Document의 ID
    public Dictionary<string, JobClass> jobsDict = new();//Key는 스킬의 Index. Ex)200, 101
    public Dictionary<string, EnemyClass> enemyiesDict = new();//Key는 Document의 ID를 int로 parse
    public Dictionary<string, UpgradeClass> upgradeDict = new();
    public Dictionary<string, TalentFormStruct> talentDict = new();
    public Dictionary<string, EnemyCase> enemyCaseDict = new();
    public Dictionary<WeaponType, Dictionary<string, WeaponClass>> weaponDict = new();
    public Dictionary<string, IngredientClass> ingredientDict = new();
    public Dictionary<string, FoodClass> foodDict = new();
    private readonly string visualEffectPath = "Prefab/VisualEffect";
    public Dictionary<string, VisualEffect> skillVisualEffectDict = new();
    public Dictionary<string, VisualEffect> weaponVisualEffectDict = new();
    public Dictionary<Species, Dictionary<string, BodyPartClass>> BodyPartDict = new();
    public Dictionary<Species, Dictionary<string, EyeClass>> eyeDict = new();
    public Dictionary<string, Sprite> hairDict = new();
    public Dictionary<string, Sprite> faceHairDict = new();
    public Dictionary<string, Dictionary<ClothesPart, Sprite >> clothesDict = new();
    private void Awake()
    {
        if (!loadManager)
        {
            loadManager = this;
        }
    }
    public void Start()
    {
        StartAsync();
    }
    async void StartAsync()
    {
        if (!isInit)
        {
            await LoadVisualEffect();
            await InitSkill();
            await Task.WhenAll(InitJob(), InitEnemy(), InitUpgrade(), InitTalent(),
                InitUserDoc(), InitEnemyCase(), InitWeapon(), InitIngredient(), InitFood());
            InitBodyPart(); InitEye(); InitFaceHair(); InitHair(); InitClothes();
            Debug.Log("LoadComplete");
            isInit = true;
        }
    }
    async Task LoadVisualEffect()
    {
        
        LoadVisualEffectType("Weapon", weaponVisualEffectDict);
        LoadVisualEffectType("Skill", skillVisualEffectDict);
        async void LoadVisualEffectType(string _type, Dictionary<string, VisualEffect> _dict)
        {
            Dictionary<string, object> durationData = await DataManager.dataManager.GetField("VisualEffect", _type);
            GameObject[] visualEffects = Resources.LoadAll<GameObject>(visualEffectPath + "/" + _type);
            
            foreach (GameObject visualEffect in visualEffects)
            {
                float duration = 1f;
                string sound = string.Empty;
                bool fromRoot = false;
                if (durationData.TryGetValue(visualEffect.name, out object obj0))
                {
                    object obj1;
                    Dictionary<string, object> ve = obj0 as Dictionary<string, object>;
                    if (ve.TryGetValue("Duration", out obj1))
                    {
                        duration = GetFloatValue(obj1);
                    }
                    if (ve.TryGetValue("Sound", out obj1))
                    {
                        sound = (string)obj1;
                    }
                    if (ve.TryGetValue("FromRoot", out obj1))
                    {
                        fromRoot = (bool)obj1;
                    }
                }
                _dict.Add(visualEffect.name, new VisualEffect(visualEffect, duration, sound, fromRoot));
            }
        }

    }
    private async Task InitSkill()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Skill");
        Sprite[] sprites = Resources.LoadAll<Sprite>("Texture/SkillIcon");
        foreach (DocumentSnapshot doc in documents)
        {

            Dictionary<string, object> skillDict = doc.ToDictionary();//도큐먼트
            if ((string)skillDict["Categori"] == "Withhold")
                continue;

            List<Dictionary<Language, string>> explains;

            SkillCategori categori;
            List<List<SkillEffect>> effects;
            Dictionary<Language, string> name = new();
            explains = new();
            float cooltime;
            bool isTargetEnemy;
            switch ((string)skillDict["Categori"])
            {
                default:
                    categori = SkillCategori.Power;
                    break;
                case "Sustain":
                    categori = SkillCategori.Sustain;
                    break;
                case "Util":
                    categori = SkillCategori.Util;
                    break;
                case "Enemy":
                    categori = SkillCategori.Enemy;
                    break;
            }




            if (categori == SkillCategori.Enemy)//적의 스킬
            {
                List<SkillEffect> effect = InitEffect("Effect", skillDict);
                effects = new() { effect };
                explains = new() { new(),new(),new()};
            }
            else//아군 캐릭터가 사용하는 스킬
            {

                if (skillDict.TryGetValue("Name", out object nameField))
                {
                    foreach (var x in nameField as Dictionary<string, object>)
                    {
                        switch (x.Key)
                        {
                            default:
                                name.Add(Language.Ko, (string)x.Value);
                                break;
                            case "En":
                                name.Add(Language.En, (string)x.Value);
                                break;
                        }
                    }
                }
                List<SkillEffect> effect0 = InitEffect("Effect_0", skillDict);
                List<SkillEffect> effect1 = InitEffect("Effect_1", skillDict);
                List<SkillEffect> effect2 = InitEffect("Effect_2", skillDict);
                effects = new() { effect0, effect1, effect2 };

                foreach (var x0 in skillDict["Explain"] as List<object>)
                {
                    Dictionary<Language, string> temp = new();
                    foreach (var x1 in x0 as Dictionary<string, object>)
                    {
                        switch (x1.Key)
                        {
                            default:
                                temp.Add(Language.Ko, (string)x1.Value);
                                break;
                            case "En":
                                temp.Add(Language.En, (string)x1.Value);
                                break;
                        }
                    }
                    explains.Add(temp);
                }

            }
            object obj;
            //CoolTime
            if (skillDict.TryGetValue("Cooltime", out obj))
                cooltime = GetFloatValue(obj);
            else
                cooltime = 8f;
            //IsTargetEnemy

            if (skillDict.TryGetValue("IsTargetEnemy", out obj))
                isTargetEnemy = (bool)obj;
            else
                isTargetEnemy = true;

            //IsAnim
            bool isAnim;
            if (skillDict.TryGetValue("IsAnim", out obj))
                isAnim = (bool)obj;
            else
                isAnim = true;

            //SkillEffect
            List<string> skillEffect = new();
            if (skillDict.TryGetValue("VisualEffect", out obj))
            {
                foreach (object x in (List<object>)obj)
                {
                    skillEffect.Add((string)x);
                }
            }
            else
                skillEffect = null;

            //IsPre
            bool isPre;
            if (skillDict.TryGetValue("IsPre", out obj))
                isPre = (bool)obj;
            else
                isPre = false;

            //Sprite
            Sprite sprite;
            sprite = sprites.Where(item => item.name == doc.Id).FirstOrDefault();

            //Scale
            Vector2 scale;
            if (skillDict.ContainsKey("Scale"))
            {
                Dictionary<string, object> scaleDict = skillDict["Scale"] as Dictionary<string, object>;
                scale = new(GetFloatValue(scaleDict["X"]), GetFloatValue(scaleDict["Y"]));
            }
            else
            {
                scale = Vector2.one;
            }
            //Position
            Vector2 position;
            if (skillDict.ContainsKey("Position"))
            {
                Dictionary<string, object> positionDict = skillDict["Position"] as Dictionary<string, object>;
                position = new(GetFloatValue(positionDict["X"]), GetFloatValue(positionDict["Y"]));
            }
            else
            {
                position = Vector2.zero;
            }
            //Set
            SkillForm skillForm = new SkillForm().
                SetCooltime(cooltime).
            SetExplain(explains).
            SetCategori(categori).
            SetIsTargetEnemy(isTargetEnemy).
            SetIsAnim(isAnim).
            SetSkillEffect(skillEffect).
            SetIsPre(isPre).
            SetEffects(effects).
            SetCooltime(cooltime).
            SetScale(scale).
            SetPosition(position).
            SetName(name).
            SetSprite(sprite).
            SetId(doc.Id);
            skillsDict.Add(doc.Id, skillForm);
        }
    }
    List<SkillEffect> InitEffect(string _effectStr, Dictionary<string, object> _skillDict)
    {
        List<SkillEffect> effects = new();
        if (!(_skillDict.TryGetValue(_effectStr, out object listObj)))
            return null;
        foreach (object docDict in listObj as List<object>)
        {
            SkillEffect effect = new();
            Dictionary<string, object> effectDict = docDict as Dictionary<string, object>;
            //Value
            object obj;
            if (effectDict.TryGetValue("Value", out obj))
                effect.SetValue(GetFloatValue(obj));
            //Count
            if (effectDict.TryGetValue("Count", out obj))
                effect.SetCount((int)(long)obj);
            //Range
            EffectRange range;
            if (effectDict.TryGetValue("Range", out obj))
            {
                switch (obj)
                {
                    default:
                        range = EffectRange.Dot;
                        break;
                    case "Self":
                        range = EffectRange.Self;
                        break;
                    case "Row":
                        range = EffectRange.Row;
                        break;
                    case "Column":
                        range = EffectRange.Column;
                        break;
                    case "Behind":
                        range = EffectRange.Behind;
                        break;
                    case "Front":
                        range = EffectRange.Front;
                        break;
                }
                effect.SetRange(range);
            }
            //ValueBase
            ValueBase valueBase;
            if (effectDict.TryGetValue("ValueBase", out obj))
            {
                switch ((string)obj)
                {
                    default:
                        valueBase = ValueBase.Const;
                        break;
                    case "Ability":
                        valueBase = ValueBase.Ability;
                        break;
                    case "Armor":
                        valueBase = ValueBase.Armor;
                        break;
                }
            }
            else
            {
                valueBase = ValueBase.Ability;
            }
            effect.SetValueBase(valueBase);
            //Type
            EffectType type;
            type = ParseEffectType((string)effectDict["Type"]);
            effect.SetType(type);
            //Delay
            if (effectDict.TryGetValue("Delay", out obj))
                effect.SetDelay(GetFloatValue(obj));
            //IsPassive
            if (effectDict.TryGetValue("IsPassive", out obj))
                effect.SetIsPassive((bool)obj);
            //Vamp
            if (effectDict.TryGetValue("Vamp", out obj))
                effect.SetVamp(GetFloatValue(obj));
            //ByAtt
            if (effectDict.TryGetValue("ByAtt", out obj))
                effect.SetByAtt(((bool)obj));
            //적용
            effects.Add(effect);
        }
        return effects;
    }
    private static EffectType ParseEffectType(string _typeName)
    {
        EffectType type;
        switch (_typeName)
        {
            case "AttAscend":
                type = EffectType.AttAscend;
                break;
            case "ResistAscend":
                type = EffectType.ResistAscend;
                break;
            case "AttDescend":
                type = EffectType.AttDescend;
                break;
            case "ResistDescend":
                type = EffectType.ResistDescend;
                break;
            case "SpeedDescend":
                type = EffectType.SpeedDescend;
                break;
            case "Bleed":
                type = EffectType.Bleed;
                break;
            case "Reflect":
                type = EffectType.Reflect;
                break;
            case "Paralyze":
                type = EffectType.Paralyze;
                break;
            case "Enchant":
                type = EffectType.Enchant;
                break;
            case "Repeat":
                type = EffectType.Repeat;
                break;
            case "Heal":
                type = EffectType.Heal;
                break;
            case "Restoration":
                type = EffectType.Restoration;
                break;
            case "Armor":
                type = EffectType.Armor;
                break;
            case "AbilityAscend":
                type = EffectType.AbilityAscend;
                break;
            case "BleedTransfer":
                type = EffectType.BleedTransfer;
                break;
            case "AbilityVamp":
                type = EffectType.AbilityVamp;
                break;
            case "AbilityByDamage":
                type = EffectType.ResistByDamage;
                break;
            case "Vamp":
                type = EffectType.Vamp;
                break;
            case "Confuse":
                type = EffectType.Confuse;
                break;
            case "Damage":
                type = EffectType.Damage;
                break;
            case "Reduce":
                type = EffectType.Reduce;
                break;
            case "Curse":
                type = EffectType.Curse;
                break;
            case "Revive":
                type = EffectType.Revive;
                break;
            case "ResistByDamage":
                type = EffectType.ResistByDamage;
                break;
            case "Necro":
                type = EffectType.Necro;
                break;
            case "GoldAscend":
                type = EffectType.GoldAscend;
                break;
            case "FameAscend":
                type = EffectType.FameAscend;
                break;
            case "Critical":
                type = EffectType.Critical;
                break;
            case "DebuffAscend":
                type = EffectType.DebuffAscend;
                break;
            case "HealAscend":
                type = EffectType.HealAscend;
                break;
            case "CorpseExplo":
                type = EffectType.CorpseExplo;
                break;
            case "ResistAscend_P":
                type = EffectType.ResistAscend_P;
                break;
            case "BuffAscend":
                type = EffectType.BuffAscend;
                break;
            case "RewardAscend":
                type = EffectType.RewardAscend;
                break;
            case "AttAscend_Torment":
                type = EffectType.AttAscend_Torment;
                break;
            case "ResilienceAscend":
                type = EffectType.ResilienceAscend;
                break;
            default:
                Debug.LogError("Init Effect By Default..." + _typeName);
                type = EffectType.Damage;
                break;
        }

        return type;
    }

    private async Task InitJob()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Job");
        foreach (DocumentSnapshot doc in documents)
        {
            object obj;
            Dictionary<string, object> dict = doc.ToDictionary();
            JobClass jobClass = new();
            if (dict.TryGetValue("Effect", out obj))
            {
                List<object> objList = obj as List<object>;
                List<SkillEffect> effects = new();
                foreach (object x in objList)
                {
                    Dictionary<string, object> effectDict = x as Dictionary<string, object>;
                    string typeStr = (string)effectDict["Type"];
                    bool byAtt;
                    if (effectDict.TryGetValue("ByAtt", out obj))
                    {
                        byAtt = (bool)obj;
                    }
                    else
                        byAtt = false;
                    EffectType effectType = ParseEffectType(typeStr);
                    float value;
                    if (effectDict.TryGetValue("Value", out obj))
                    {
                        value = GetFloatValue(obj);
                    }
                    else
                        value = 0f;
                    SkillEffect skillEffect = new SkillEffect().SetType(effectType).SetValue(value).SetByAtt(byAtt);
                    effects.Add(skillEffect);
                }
                jobClass.SetEffects(effects);
            }
            string jobId = (string)dict["JobId"];
            //Name
            Dictionary<Language, string> name = new();
            Dictionary<string, object> nameTemp = dict["Name"] as Dictionary<string, object>;
            name.Add(Language.Ko, (string)nameTemp["Ko"]);
            name.Add(Language.En, (string)nameTemp["En"]);
            jobClass.SetName(name);

            jobsDict.Add(jobId, jobClass);
        }
    }
    private async Task InitEnemy()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Enemy");
        foreach (DocumentSnapshot doc in documents)
        {
                EnemyClass enemyClass = new EnemyClass();
                Dictionary<string, object> dict = doc.ToDictionary();
                object obj;
                //Name
                Dictionary<Language, string> name = new();
                Dictionary<string, object> nameTemp = dict["Name"] as Dictionary<string, object>;
                name.Add(Language.Ko, (string)nameTemp["Ko"]);
                name.Add(Language.En, (string)nameTemp["En"]);
                enemyClass.SetName(name);

                //Skill
                List<Skill> skills = new();
                foreach (object skillObj in dict["Skills"] as List<object>)
                {
                    SkillForm skillForm = skillsDict[(string)skillObj];
                    skills.Add(skillForm.LocalizeSkill(0));
                }
                enemyClass.SetSkills(skills);
                //Ability
                if (dict.TryGetValue("Ability", out obj))
                {
                    enemyClass.SetAbility(GetFloatValue(obj));
                }
                //Hp
                if (dict.TryGetValue("Hp", out obj))
                {
                    enemyClass.SetHp(GetFloatValue(obj));
                }
                //Resist
                if (dict.TryGetValue("Resist", out obj))
                {
                    enemyClass.SetResist(GetFloatValue(obj));
                }
                //Speed
                if (dict.TryGetValue("Speed", out obj))
                {
                    enemyClass.SetSpeed(GetFloatValue(obj));
                }
                //IsMonster
                if (dict.TryGetValue("IsMonster", out obj))
                {
                    enemyClass.SetIsMonster((bool)obj);
                }
                //Type
                if (dict.TryGetValue("Type", out obj))
                {
                    enemyClass.SetType((string)obj);
                }
                //EnemyLevel
                if (dict.TryGetValue("EnemyLevel", out obj))
                {
                    enemyClass.SetEnemyLevel((int)(long)(obj));
                }
                enemyiesDict.Add(doc.Id, enemyClass);
            }
    }
    private async Task InitUpgrade()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Upgrade");
        foreach (DocumentSnapshot doc in documents)
        {
            Dictionary<string, object> dict = doc.ToDictionary();
            List<UpgradeContent> guildContents = new();
            foreach (object contentObj in dict["Contents"] as List<object>)
            {
                Dictionary<string, object> contentDict = contentObj as Dictionary<string, object>;
                float value = GetFloatValue(contentDict["Value"]);
                int price = (int)(long)contentDict["Price"];
                guildContents.Add(new(value, price));
            }
            Dictionary<Language, string> name = new();
            foreach (var x in dict["Name"] as Dictionary<string, object>)
            {
                switch (x.Key)
                {
                    default:
                        name.Add(Language.Ko, (string)x.Value);
                        break;
                    case "En":
                        name.Add(Language.En, (string)x.Value);
                        break;
                }
            }
            Dictionary<Language, string> explain = new();
            foreach (var x in dict["Explain"] as Dictionary<string, object>)
            {
                string value = (string)x.Value;
                value = value.Replace("<E>", "<color=#4C4CFF><size=0.3><b>");
                value = value.Replace("</E>", "</b></size></color>");
                value = value.Replace("\\n", "\n");
                switch (x.Key)
                {
                    default:
                        explain.Add(Language.Ko, value);
                        break;
                    case "En":
                        explain.Add(Language.En, value);
                        break;
                }
            }
            Dictionary<Language, string> info = new();
            foreach (var x in dict["Info"] as Dictionary<string, object>)
            {
                string value = (string)x.Value;
                switch (x.Key)
                {
                    default:
                        info.Add(Language.Ko, value);
                        break;
                    case "En":
                        info.Add(Language.En, value);
                        break;
                }
            }
            UpgradeEffectType type;
            switch (dict["Type"])
            {
                default:
                    Debug.LogError("InitDefault... " + dict["Type"]);
                    type = UpgradeEffectType.AllocateNumberUp;
                    break;
                case "AllocateNumberUp":
                    type = UpgradeEffectType.AllocateNumberUp;
                    break;
                case "TalentEffectUp":
                    type = UpgradeEffectType.TalentEffectUp;
                    break;
                case "TalentLevelUp":
                    type = UpgradeEffectType.TalentLevelUp;
                    break;
                case "StatusUp":
                    type = UpgradeEffectType.StatusUp;
                    break;
                case "FameUp":
                    type = UpgradeEffectType.FameUp;
                    break;
                case "GoldUp":
                    type = UpgradeEffectType.GoldUp;
                    break;
            }
            upgradeDict.Add(doc.Id, new UpgradeClass(name, (int)(long)dict["Index"], guildContents, explain, info, type, (string)dict["LobbyCase"]));
        }
    }
    private async Task InitTalent()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Talent");
        foreach (DocumentSnapshot doc in documents)
        {
            Dictionary<string, object> dict = doc.ToDictionary();
            Dictionary<Language, string> name = new();
            Dictionary<Language, string> explain = new();
            foreach (KeyValuePair<string, object> keyPair in dict["Name"] as Dictionary<string, object>)
            {
                switch (keyPair.Key)
                {
                    default:
                        name.Add(Language.Ko, (string)keyPair.Value);
                        break;
                    case "En":
                        name.Add(Language.En, (string)keyPair.Value);
                        break;
                }
            }
            foreach (KeyValuePair<string, object> keyPair in dict["Explain"] as Dictionary<string, object>)
            {
                switch (keyPair.Key)
                {
                    default:
                        explain.Add(Language.Ko, (string)keyPair.Value);
                        break;
                    case "En":
                        explain.Add(Language.En, (string)keyPair.Value);
                        break;
                }
            }
            List<TalentEffectForm> effects = new();
            {
                List<object> effectList = dict["Effect"] as List<object>;
                foreach (object x in effectList)
                {
                    Dictionary<string, object> effectDict = x as Dictionary<string, object>;
                    EffectType effectType = ParseEffectType((string)effectDict["Type"]);
                    effects.Add(new((string)effectDict["Value"], effectType));
                }
            }
            talentDict.Add(doc.Id, new TalentFormStruct(name, (int)(long)dict["Level"], explain, effects, (int)(long)dict["Order"]));
        }
    }

    private async Task InitUserDoc()
    {
        DocumentReference userRef = FirebaseFirestore.DefaultInstance.Collection("User").Document(GameManager.gameManager.Uid);
        DocumentSnapshot snapshot = await userRef.GetSnapshotAsync();

        await FirebaseFirestore.DefaultInstance.RunTransactionAsync(async transaction =>
        {
            if (snapshot.Exists)
            {
                await GameManager.gameManager.LoadUserDoc();
            }
            else
            {
                DataManager.dataManager.SetDocumentData("Fame", 0, "User", GameManager.gameManager.Uid);
                GameManager.gameManager.fame = 0;
            }
        });
    }
    private async Task InitEnemyCase()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("EnemyCase");
        foreach (DocumentSnapshot doc in documents)
        {
            EnemyCase enemyCase = new EnemyCase();
            Dictionary<string, object> dict = doc.ToDictionary();
            //Enemies
            List<EnemyPieceForm> enemyPieces = new();//id, grid
            List<object> enemies = dict["Enemies"] as List<object>;
            foreach (object enemyObj in enemies)
            {
                Dictionary<string, object> pieceObj = enemyObj as Dictionary<string, object>;
                object obj;
                var piece = new EnemyPieceForm();
                if (pieceObj.TryGetValue("Id", out obj))
                {
                    piece.SetId((string)obj);
                }
                else if (pieceObj.TryGetValue("Type", out obj))
                {
                    piece.SetType((string)obj);
                }
                else if (pieceObj.TryGetValue("EnemyLevel", out obj))
                {
                    piece.SetLevel((int)(long)obj);
                }
                piece.SetIndex((int)(long)pieceObj["GridIndex"]);
                enemyPieces.Add(piece);
            }
            enemyCase.SetEnemies(enemyPieces);
            //LevelRange
            List<int> levelRange = new();
            foreach (var x in dict["LevelRange"] as List<object>)
            {
                levelRange.Add((int)(long)x);
            }
            enemyCase.SetLevelRange(levelRange);
            enemyCaseDict.Add(doc.Id, enemyCase);
        }
    }
    private async Task InitWeapon()
    {
        await Task.WhenAll(InitWeaponOnType("Bow"),
            InitWeaponOnType("Magic"),
            InitWeaponOnType("Sword"),
            InitWeaponOnType("Club"));

        async Task InitWeaponOnType(string _weaponTypeStr)
        {
            List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots(string.Format("{0}/{1}/{2}", "Weapon", "Data", _weaponTypeStr));
            //Type
            WeaponType type;
            switch (_weaponTypeStr)
            {
                default:
                    type = WeaponType.Sword;
                    break;
                case "Bow":
                    type = WeaponType.Bow;
                    break;
                case "Magic":
                    type = WeaponType.Magic;
                    break;
                case "Club":
                    type = WeaponType.Club;
                    break;
            }
            weaponDict.Add(type, new());

            foreach (DocumentSnapshot doc in documents)
            {
                
                Dictionary<string, object> dict = doc.ToDictionary();
                object obj;
                //Name
                Dictionary<Language, string> name = new();
                Dictionary<string, object> nameTemp;
                if (dict.ContainsKey("Name"))
                {
                    nameTemp = dict["Name"] as Dictionary<string, object>;
                    name.Add(Language.Ko, (string)nameTemp["Ko"]);
                    name.Add(Language.En, (string)nameTemp["En"]);
                }
                else 
                {
                    name.Add(Language.Ko,string.Empty);
                    name.Add(Language.En, string.Empty);
                }
                //Explain
                Dictionary<Language, string> explain = new();
                if (dict.TryGetValue("Explain", out obj))
                {
                    Dictionary<string, object> explainObjDict = obj as Dictionary<string, object>;
                    explain.Add(Language.Ko, (string)explainObjDict["Ko"]);
                    explain.Add(Language.En, (string)explainObjDict["En"]);
                }
                else
                {
                    explain.Add(Language.Ko, string.Empty);
                    explain.Add(Language.En, string.Empty);
                };
                //Sprite
                Sprite sprite = Resources.Load<Sprite>(string.Format("{0}/{1}/{2}/{3}", "Texture", "Weapon", _weaponTypeStr, doc.Id));
                //Grade
                ItemGrade grade;
                switch (dict["Grade"])
                {
                    default:
                        grade = ItemGrade.None;
                        break;
                    case "Normal":
                        grade = ItemGrade.Normal;
                        break;
                    case "Rare":
                        grade = ItemGrade.Rare;
                        break;
                    case "Unique":
                        grade = ItemGrade.Unique;
                        break;
                }
                //Scale
                Vector2 scale;
                if (dict.ContainsKey("Scale"))
                {
                    Dictionary<string, object> scaleDict = dict["Scale"] as Dictionary<string, object>;
                    scale = new(GetFloatValue(scaleDict["X"]), GetFloatValue(scaleDict["Y"]));
                }
                else
                {
                    scale = Vector2.one;
                }
                //Position
                Vector2 position;
                if (dict.ContainsKey("Position"))
                {
                    Dictionary<string, object> positionDict = dict["Position"] as Dictionary<string, object>;
                    position = new(GetFloatValue(positionDict["X"]), GetFloatValue(positionDict["Y"]));
                }
                else
                {
                    position = Vector2.zero;
                }

                string itemId = $"{_weaponTypeStr}:::{doc.Id}";
                WeaponClass weaponClass = new(ItemType.Weapon, itemId, grade, name,explain, sprite,scale, position);
                //Effects
                List<SkillEffect> effects = InitEffect("Effects", dict);
                if (effects != null)
                    foreach (SkillEffect x in effects)
                    {
                        x.SetIsPassive(true);
                    }
                weaponClass.SetEffects(effects);

                weaponClass.SetGrade(grade);
                //Stat
                if (dict.TryGetValue("Status", out obj))
                {
                    Dictionary<string, object> statusDict = obj as Dictionary<string, object>;
                    object obj1;
                    float ability;
                    if (statusDict.TryGetValue("Ability", out obj1))
                        ability = GetFloatValue(obj1);
                    else
                        ability = 0f;
                    float hp;
                    if (statusDict.TryGetValue("Hp", out obj1))
                        hp = GetFloatValue(obj1);
                    else
                        hp = 0f;
                    float resist;
                    if (statusDict.TryGetValue("Resist", out obj1))
                        resist = GetFloatValue(obj1);
                    else
                        resist = 0f;
                    float speed;
                    if (statusDict.TryGetValue("Speed", out obj1))
                        speed = GetFloatValue(obj1);
                    else
                        speed = 0f;
                    weaponClass.SetStatus(ability, hp, resist, speed);
                }
                weaponClass.SetType(type);
                //VisualEffect
                VisualEffect defaultVisualEffect = null;
                VisualEffect skillVisualEffect = null;
                if (dict.TryGetValue("VisualEffect", out obj))
                {
                    Dictionary<string, object> veDict = obj as Dictionary<string, object>;
                    defaultVisualEffect = weaponVisualEffectDict[(string)veDict["Default"]];
                    skillVisualEffect = weaponVisualEffectDict[(string)veDict["Skill"]];
                }
                weaponClass.SetVisualEffect(defaultVisualEffect, skillVisualEffect);

                weaponDict[type].Add(doc.Id, weaponClass);
            }
        }
    }
    private async Task InitIngredient()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Ingredient");
        Sprite[] sprites = Resources.LoadAll<Sprite>("Texture/Ingredient");
        foreach (DocumentSnapshot doc in documents)
        {
            int num;
            IngredientType ingredientType;
            Sprite sprite;
            Dictionary<string, object> dict = doc.ToDictionary();
            num = (int)(long)dict["Num"];
            switch ((string)dict["IngredientType"])
            {
                default:
                    ingredientType = IngredientType.Meat;
                    break;
                    //케이스 추가 필요
            }
            //Name
            Dictionary<string, object> nameObjDict = dict["Name"] as Dictionary<string, object>;
            Dictionary<Language, string> name = new()
            {
                { Language.Ko, (string)nameObjDict["Ko"] },
                { Language.En, (string)nameObjDict["En"] }
            };
            //Explain
            Dictionary<Language, string> explain = new();
            if (dict.TryGetValue("Explain", out object obj))
            {
                Dictionary<string, object> explainObjDict = obj as Dictionary<string, object>;
                explain.Add(Language.Ko, (string)explainObjDict["Ko"]);
                explain.Add(Language.En, (string)explainObjDict["En"]);
            }
            else
            {
                explain.Add(Language.Ko, string.Empty);
                explain.Add(Language.En, string.Empty);
            };

            sprite = sprites.Where(item => item.name == doc.Id).FirstOrDefault();
            //Scale
            Vector2 scale;
            if (dict.ContainsKey("Scale"))
            {
                Dictionary<string, object> scaleDict = dict["Scale"] as Dictionary<string, object>;
                scale = new(GetFloatValue(scaleDict["X"]), GetFloatValue(scaleDict["Y"]));
            }
            else
            {
                scale = Vector2.one;
            }
            //Position
            Vector2 position;
            if (dict.ContainsKey("Position"))
            {
                Dictionary<string, object> positionDict = dict["Position"] as Dictionary<string, object>;
                position = new(GetFloatValue(positionDict["X"]), GetFloatValue(positionDict["Y"]));
            }
            else
            {
                position = Vector2.zero;
            }
            IngredientClass ingredient = new(ItemType.Ingredient, doc.Id, ItemGrade.None, name, explain, sprite, scale, position);
            ingredient.SetNum(num);
            ingredient.SetIngredientType(ingredientType);
            ingredientDict.Add(doc.Id, ingredient);
        }
    }
    private async Task InitFood()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Food");
        Sprite[] sprites = Resources.LoadAll<Sprite>("Texture/Food");
        foreach (DocumentSnapshot doc in documents)
        {
            Dictionary<string, object> dict = doc.ToDictionary();
            int degree;
            Sprite sprite;
            degree = (int)(long)dict["Degree"];
            sprite = sprites.Where(item => item.name == doc.Id).FirstOrDefault();
            //Name
            Dictionary<string, object> nameObjDict = dict["Name"] as Dictionary<string, object>;
            Dictionary<Language, string> name = new()
            {
                { Language.Ko, (string)nameObjDict["Ko"] },
                { Language.En, (string)nameObjDict["En"] }
            };
            //Explain
            Dictionary<Language, string> explain = new();
            if (dict.TryGetValue("Explain", out object obj))
            {
                Dictionary<string, object> explainObjDict = obj as Dictionary<string, object>;
                explain.Add(Language.Ko, (string)explainObjDict["Ko"]);
                explain.Add(Language.En, (string)explainObjDict["En"]);
            }
            else
            {
                explain.Add(Language.Ko, string.Empty);
                explain.Add(Language.En, string.Empty);
            };
            //Scale
            Vector2 scale;
            if (dict.ContainsKey("Scale"))
            {
                Dictionary<string, object> scaleDict = dict["Scale"] as Dictionary<string, object>;
                scale = new(GetFloatValue(scaleDict["X"]), GetFloatValue(scaleDict["Y"]));
            }
            else
            {
                scale = Vector2.one;
            }
            //Position
            Vector2 position;
            if (dict.ContainsKey("Position"))
            {
                Dictionary<string, object> positionDict = dict["Position"] as Dictionary<string, object>;
                position = new(GetFloatValue(positionDict["X"]), GetFloatValue(positionDict["Y"]));
            }
            else
            {
                position = Vector2.zero;
            }
            FoodClass foodClass = new(ItemType.Food, doc.Id, ItemGrade.None, name,explain, sprite, scale, position);
            foodClass.SetDegree(degree);
            foodDict.Add(doc.Id, foodClass);
        }
    }
    private void InitBodyPart()
    {
        InitBySpecies(Species.Human);
        InitBySpecies(Species.Elf);
        InitBySpecies(Species.Devil);
        InitBySpecies(Species.Skelton);
        InitBySpecies(Species.Orc);

        void InitBySpecies(Species _species)
        {
            string speciesStr = string.Empty;
            BodyPartDict.Add(_species, new());
            switch (_species)
            {
                case Species.Human:
                    speciesStr = "0_Human";
                    break;
                case Species.Elf:
                    speciesStr = "1_Elf";
                    break;
                case Species.Devil:
                    speciesStr = "2_Devil";
                    break;
                case Species.Skelton:
                    speciesStr = "3_Skelton";
                    break;
                case Species.Orc:
                    speciesStr = "4_Orc";
                    break;
            }
            Sprite[] sprites = Resources.LoadAll<Sprite>("Texture/Character/Body/" + speciesStr);

            foreach (Sprite sprite in sprites)
            {
                if (!BodyPartDict[_species].ContainsKey(sprite.texture.name))
                {
                    BodyPartDict[_species].Add(sprite.texture.name, new());
                }
                switch (sprite.name)
                {
                    case "Arm_L":
                        BodyPartDict[_species][sprite.texture.name].armL = sprite;
                        break;
                    case "Arm_R":
                        BodyPartDict[_species][sprite.texture.name].armR = sprite;
                        break;
                    case "Head":
                        BodyPartDict[_species][sprite.texture.name].head = sprite;
                        break;
                    default:
                        continue;
                }
            }
        }
    }
    private void InitEye()
    {
        InitBySpecies(Species.Human);
        InitBySpecies(Species.Elf);
        InitBySpecies(Species.Devil);
        InitBySpecies(Species.Skelton);
        InitBySpecies(Species.Orc);

        void InitBySpecies(Species _species)
        {
            string speciesStr = string.Empty;
            eyeDict.Add(_species, new());
            switch (_species)
            {
                case Species.Human:
                    speciesStr = "0_Human";
                    break;
                case Species.Elf:
                    speciesStr = "1_Elf";
                    break;
                case Species.Devil:
                    speciesStr = "2_Devil";
                    break;
                case Species.Skelton:
                    speciesStr = "3_Skelton";
                    break;
                case Species.Orc:
                    speciesStr = "4_Orc";
                    break;
            }
            Sprite[] sprites = Resources.LoadAll<Sprite>("Texture/Character/Eye/" + speciesStr);

            foreach (Sprite sprite in sprites)
            {
                if (!eyeDict[_species].ContainsKey(sprite.texture.name))
                {
                    eyeDict[_species].Add(sprite.texture.name, new());
                }
                switch (sprite.name)
                {
                    case "Front":
                        eyeDict[_species][sprite.texture.name].front = sprite;
                        break;
                    case "Back":
                        eyeDict[_species][sprite.texture.name].back = sprite;
                        break;
                }
            }
        }
    }
    private void InitHair()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Texture/Character/Hair");
        foreach (Sprite sprite in sprites)
            hairDict.Add(sprite.name, sprite);
    }
    private void InitFaceHair()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Texture/Character/FaceHair");
        foreach (Sprite sprite in sprites)
            faceHairDict.Add(sprite.name, sprite);
    }
    float GetFloatValue(object _obj)
    {
        if (_obj is long)
            return (int)(long)_obj;
        else
            return (float)(double)_obj;
    }
    private void InitClothes()
    {
        InitClothesPiece("002");
        InitClothesPiece("011");
        InitClothesPiece("020");
        InitClothesPiece("101");
        InitClothesPiece("110");
        InitClothesPiece("200");
        void InitClothesPiece(string _job)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>($"Texture/Clothes/{_job}");
            Dictionary<ClothesPart, Sprite> tempDict = new();
            foreach (Sprite sprite in sprites)
            {
                ClothesPart clothesPart;
                switch (sprite.name)
                {
                    default:
                        clothesPart = ClothesPart.Back;
                        break;
                    case "ClothBody":
                        clothesPart = ClothesPart.ClothBody;
                        break;
                    case "ClothRight":
                        clothesPart = ClothesPart.ClothRight;
                        break;
                    case "ClothLeft":
                        clothesPart = ClothesPart.ClothLeft;
                        break;
                    case "ArmorBody":
                        clothesPart = ClothesPart.ArmorBody;
                        break;
                    case "ArmorRight":
                        clothesPart = ClothesPart.ArmorRight;
                        break;
                    case "ArmorLeft":
                        clothesPart = ClothesPart.ArmorLeft;
                        break;
                    case "Helmet":
                        clothesPart = ClothesPart.Helmet;
                        break;
                    case "FootRight":
                        clothesPart = ClothesPart.FootRight;
                        break;
                    case "FootLeft":
                        clothesPart = ClothesPart.FootLeft;
                        break;
                }
                tempDict.Add(clothesPart, sprite);
            }
            clothesDict.Add(_job, tempDict);
        }
    }
    [ContextMenu("SetDoc")]
    public async void SetDoc()
    {

        List<DocumentSnapshot> docs = await DataManager.dataManager.GetDocumentSnapshots($"Skill");
        foreach (DocumentSnapshot doc in docs)
        {
            Dictionary<string, object> dict = doc.ToDictionary();
            for (int i = 0; i <= 3; i++)
            {
                List<object> effectArr = dict[$"Effect_{i}"] as List<object>;
                foreach (object x in effectArr)
                {
                    Dictionary<string, object> dict0 = x as Dictionary<string, object>;
                    dict0.Add("ValueBase", "Const");
                }
            }
        }
        Debug.Log("Fin");
    }

}