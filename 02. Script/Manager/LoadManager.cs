using BattleCollection;
using DefaultCollection;
using EnumCollection;
using Firebase.Firestore;
using ItemCollection;
using LobbyCollection;
using StageCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
public class LoadManager : MonoBehaviour//Firestore에 있는 기초 데이터들 로딩해서 저장하고 있는 스크립트
{
    public static LoadManager loadManager;
    public float dbProgress = 0f;
    public bool isInit = false;
    public Dictionary<string, Skill> skillsDict = new();//Key는 Document의 ID
    public Dictionary<string, JobClass> jobsDict = new();//Key는 스킬의 Index. Ex)200, 101
    public Dictionary<string, EnemyClass> enemyiesDict = new();//Key는 Document의 ID를 int로 parse
    public Dictionary<string, UpgradeClass> upgradeDict = new();
    public Dictionary<string, TalentClass> talentDict = new();
    public Dictionary<string, EnemyCase> enemyCaseDict = new();
    public Dictionary<string, NodeType> nodeTypesDict = new();
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
    private void Awake()
    {
        if (!loadManager)
        {
            isInit = false;
            loadManager = this;
        }
    }

    public async Task LoadDbBaseData()
    {
        float totalSteps = 4; // 총 단계 수
        dbProgress = 0f;

        // Visual Effect 초기화
        await LoadVisualEffect();
        dbProgress += 1f / totalSteps; // 진행률 업데이트

        // Skill 및 Upgrade 초기화
        await Task.WhenAll(InitSkill(), InitUpgrade());
        dbProgress += 1f / totalSteps; // 진행률 업데이트

        // 나머지 초기화 작업
        await Task.WhenAll(InitUserDoc(), InitJob(), InitEnemy(), InitTalent(),
                           InitEnemyCase(), InitWeapon(), InitIngredient(), InitFood(),
                           InitNodeType());
        dbProgress += 1f / totalSteps; // 진행률 업데이트

        // 나머지 초기화 작업 (빠른 작업)
        InitBodyPart();
        InitEye();
        InitFaceHair();
        InitHair();
        dbProgress = 1f; // 진행률 업데이트

        isInit = true;
        Debug.Log("DB Load Complete");
    }


    async Task LoadVisualEffect()
    {
        await LoadVisualEffectType("Weapon", weaponVisualEffectDict);
        await LoadVisualEffectType("Skill", skillVisualEffectDict);
        async Task LoadVisualEffectType(string _type, Dictionary<string, VisualEffect> _dict)
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
            List<List<SkillEffect>> effectsList;
            Dictionary<Language, string> name = new();
            explains = new();
            float cooltime;
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
                List<SkillEffect> effect = GetSkillEffect(skillDict["Effect"]);
                effectsList = new() { effect };
                explains = null;
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
                List<SkillEffect> effect0 = GetSkillEffect(skillDict["Effect_0"]);
                List<SkillEffect> effect1 = GetSkillEffect(skillDict["Effect_1"]);
                List<SkillEffect> effect2 = GetSkillEffect(skillDict["Effect_2"]);
                effectsList = new() { effect0, effect1, effect2 };

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
                cooltime = 0f;

            //IsAnim
            bool isAnim;
            if (skillDict.TryGetValue("IsAnim", out obj))
                isAnim = (bool)obj;
            else
                isAnim = true;

            //SkillEffect
            List<VisualEffect> visualEffect = new();
            if (skillDict.TryGetValue("VisualEffect", out obj))
            {
                foreach (object x in (List<object>)obj)
                {
                    string visualEffectStr = (string)x;
                    if (skillVisualEffectDict.ContainsKey(visualEffectStr))
                        visualEffect.Add(skillVisualEffectDict[visualEffectStr]);
                }
            }

            //IsPre
            bool isPre;
            if (skillDict.TryGetValue("IsPre", out obj))
                isPre = (bool)obj;
            else
                isPre = false;

            //Sprite
            Sprite sprite;
            sprite = sprites.Where(item => item.name == doc.Id).FirstOrDefault();


            Skill skill = new Skill(doc.Id, categori, cooltime, effectsList, isAnim, isPre, name, explains, sprite);

            skillsDict.Add(doc.Id, skill);
        }
    }
    List<SkillEffect> GetSkillEffect(object _effectListObj)
    {
        List<object> objList = (List<object>)_effectListObj;
        List<SkillEffect> effects = new();

        foreach (object docDict in objList)
        {
            bool success;
            //Field
            float value;
            int count;
            EffectRange effectRange;
            ValueBase valueBase;
            Dictionary<string, object> effectDict = docDict as Dictionary<string, object>;
            EffectType effectType;
            bool isPassive;
            bool isTargetEnemy;
            float vamp;
            float duration;
            float probability;
            VisualEffect visualEffect;
            //Value
            object obj;
            if (effectDict.TryGetValue("Value", out obj))
                value = GetFloatValue(obj);
            else
            {
                value = 0f;
                Debug.LogError("No Value");
            }
            //Count
            if (effectDict.TryGetValue("Count", out obj))
                count = (int)(long)obj;
            else
                count = 1;
            //Range
            effectRange = EffectRange.Dot;
            if (effectDict.TryGetValue("Range", out obj))
            {
                success = Enum.TryParse((string)obj, out effectRange);
                if (!success)
                {
                    Debug.LogError("Invalid Parse");
                }
            }
            //ValueBase
            valueBase = ValueBase.Ability;
            if (effectDict.TryGetValue("ValueBase", out obj))
            {
                success = Enum.TryParse((string)obj, out valueBase);
                if (!success)
                {
                    Debug.LogError("Invalid Parse");
                }
            }
            //EffectType
            effectType = EffectType.Damage;
            if (effectDict.TryGetValue("Type", out obj))
            {
                success = Enum.TryParse((string)obj, out effectType);
                if (!success)
                {
                    Debug.LogError("Invalid Parse" + (string)obj);
                }
            }
            //IsTargetEnemy
            if (effectDict.TryGetValue("IsTargetEnemy", out obj))
                isTargetEnemy = (bool)obj;
            else
            {
                isTargetEnemy = true;
            }
            //IsPassive
            if (effectDict.TryGetValue("IsPassive", out obj))
                isPassive = (bool)obj;
            else
                isPassive = false;
            //Vamp
            if (effectDict.TryGetValue("Vamp", out obj))
                vamp = GetFloatValue(obj);
            else
                vamp = 0f;
            //Duration
            if (effectDict.TryGetValue("Duration", out obj))
                duration = GetFloatValue(obj);
            else
                duration = -99f;

            //Probability
            if (effectDict.TryGetValue("Probability", out obj))
                probability = GetFloatValue(obj);
            else
                probability = 1f;


            SkillEffect effect;
            switch (isPassive)
            {
                case true:
                    //ByAtt
                    bool byAtt;
                    if (effectDict.TryGetValue("ByAtt", out obj))
                        byAtt = (bool)obj;
                    else
                        byAtt = false;
                    effect = new PassiveEffect(count, true, value, effectType, effectRange, valueBase, isTargetEnemy, vamp, duration, probability).SetByAtt(byAtt);
                    break;
                case false:
                    float delay;
                    bool isAnim;

                    //Delay
                    if (effectDict.TryGetValue("Delay", out obj))
                        delay = GetFloatValue(obj);
                    else
                        delay = 0f;
                    //VisualEffect
                    if (effectDict.TryGetValue("VisualEffect", out obj))
                        visualEffect = skillVisualEffectDict[(string)obj];
                    else
                        visualEffect = null;
                    //IsAnim
                    if (effectDict.TryGetValue("IsAnim", out obj))
                        isAnim = (bool)obj;
                    else
                        isAnim = false;

                    //Set
                    effect = new ActiveEffect(count, false, value, effectType, effectRange, valueBase, isTargetEnemy, vamp, duration, probability)
                        .SetDelay(delay).SetVisualEffect(visualEffect).SetIsAnim(isAnim);
                    break;
            }

            //Set
            effects.Add(effect);
        }
        return effects;
    }

    private async Task InitJob()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Job");
        Sprite[] jobIconSpirtes = Resources.LoadAll<Sprite>($"Texture/Job");
        foreach (DocumentSnapshot doc in documents)
        {
            object obj;
            Dictionary<string, object> dict = doc.ToDictionary();
            JobClass jobClass;
            List<SkillEffect> effects = new();
            Dictionary<Language, string> name = new();
            Dictionary<Language, string> skillName = new();
            Dictionary<Language, string> effectExplain = new();
            Dictionary<ClothesPart, Sprite> spriteDict = new();
            Skill jobSkill = null;
            float duration;
            //Name
            Dictionary<string, object> nameObj = dict["Name"] as Dictionary<string, object>;
            name.Add(Language.Ko, (string)nameObj["Ko"]);
            name.Add(Language.En, (string)nameObj["En"]);
            //SkillName
            Dictionary<string, object> skillNameObj;
            if (dict.TryGetValue("SkillName", out obj))
            {
                skillNameObj = obj as Dictionary<string, object>;
                skillName.Add(Language.Ko, (string)skillNameObj["Ko"]);
                skillName.Add(Language.En, (string)skillNameObj["En"]);
            }
            //EffectExplain
            if (dict.TryGetValue("EffectExplain", out obj))
            {
                Dictionary<string, object> explainDict = obj as Dictionary<string, object>;
                effectExplain.Add(Language.Ko, (string)explainDict["Ko"]);
                effectExplain.Add(Language.En, (string)explainDict["En"]);
            }
            //Effect
            if (dict.TryGetValue("Effect", out obj))
            {
                List<object> objList = obj as List<object>;

                foreach (object x in objList)
                {
                    bool success;
                    Dictionary<string, object> effectDict = x as Dictionary<string, object>;
                    string typeStr = (string)effectDict["Type"];
                    bool byAtt;
                    EffectType effectType;
                    EffectRange effectRange;
                    ValueBase valueBase;
                    float value;
                    bool isTargetEnemy;
                    //ByAtt
                    if (effectDict.TryGetValue("ByAtt", out obj))
                    {
                        byAtt = (bool)obj;
                    }
                    else
                        byAtt = false;
                    //EffectType
                    effectType = EffectType.Damage; // 기본값 설정
                    if (effectDict.TryGetValue("Type", out obj))
                    {
                        success = Enum.TryParse((string)obj, out effectType);
                        if (!success)
                        {
                            Debug.LogError("Invalid Parse" + (string)obj);
                        }
                    }
                    //Value
                    if (effectDict.TryGetValue("Value", out obj))
                    {
                        value = GetFloatValue(obj);
                    }
                    else
                        value = 0f;
                    //Range
                    effectRange = EffectRange.Dot; // 기본값 설정
                    if (effectDict.TryGetValue("Range", out obj))
                    {
                        success = Enum.TryParse((string)obj, out effectRange);
                        if (!success)
                        {
                            Debug.LogError("Invalid Parse");
                        }
                    }
                    //ValueBase
                    valueBase = ValueBase.Const;
                    if (effectDict.TryGetValue("ValueBase", out obj))
                    {
                        success = Enum.TryParse((string)obj, out valueBase);
                        if (!success)
                        {
                            Debug.LogError("Invalid Parse..." + doc.Id);
                        }
                    }
                    //ByAtt
                    if (effectDict.TryGetValue("ByAtt", out obj))
                    {
                        byAtt = (bool)obj;
                    }
                    else
                        byAtt = false;
                    //IsTargetEnemy
                    if (effectDict.TryGetValue("IsTargetEnemy", out obj))
                    {
                        isTargetEnemy = (bool)obj;
                    }
                    else
                        isTargetEnemy = false;
                    //Duration
                    if (effectDict.TryGetValue("Duration", out obj))
                    {
                        duration = GetFloatValue(obj);
                    }
                    else
                        duration = -99f;
                    //Set
                    SkillEffect skillEffect = new PassiveEffect(1, true, value, effectType, effectRange, valueBase, false, 0f, duration, 1f);
                    effects.Add(skillEffect);
                }
                jobSkill = new Skill("Default", SkillCategori.Default, 0f, new() { effects }, false, false, name, new() { effectExplain }, null);
            }
            //Sprite
            Sprite[] clothesSprites = Resources.LoadAll<Sprite>($"Texture/Clothes/{doc.Id}");
            Dictionary<ClothesPart, Sprite> tempDict = new();
            foreach (Sprite sprite in clothesSprites)
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
                spriteDict.Add(clothesPart, sprite);
            }
            Sprite jobIcon = jobIconSpirtes.Where(item => item.name == doc.Id).FirstOrDefault();
            jobClass = new(doc.Id, name, skillName, jobSkill, spriteDict, jobIcon);

            jobsDict.Add(doc.Id, jobClass);
        }
    }
    private async Task InitEnemy()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Enemy");
        foreach (DocumentSnapshot doc in documents)
        {
            Dictionary<string, object> dict = doc.ToDictionary();
            object obj;

            Dictionary<Language, string> name = new();
            List<SkillInBattle> skills = new();
            float ability;
            float hp;
            float resist;
            float speed;
            float scale;
            string type;
            int enemyLevel;

            //Name
            Dictionary<string, object> nameTemp = dict["Name"] as Dictionary<string, object>;
            name.Add(Language.Ko, (string)nameTemp["Ko"]);
            name.Add(Language.En, (string)nameTemp["En"]);

            //Skill
            foreach (object skillObj in dict["Skills"] as List<object>)
            {
                Skill skill = skillsDict[(string)skillObj];
                skills.Add(skill.GetInBattle(0));
            }
            //Ability
            if (dict.TryGetValue("Ability", out obj))
                ability = GetFloatValue(obj);
            else
                ability = 0f;
            //Hp
            if (dict.TryGetValue("Hp", out obj))
                hp = GetFloatValue(obj);
            else
                hp = 1f;
            //Resist
            if (dict.TryGetValue("Resist", out obj))
                resist = GetFloatValue(obj);
            else
                resist = 0f;
            //Speed
            if (dict.TryGetValue("Speed", out obj))
                speed = GetFloatValue(obj);
            else
                speed = 1f;
            //Scale
            if (dict.TryGetValue("Scale", out obj))
                scale = GetFloatValue(obj);
            else
                scale = 1f;
            //Type
            if (dict.TryGetValue("Type", out obj))
                type = (string)obj;
            else
                type = null;
            //EnemyLevel
            if (dict.TryGetValue("EnemyLevel", out obj))
                enemyLevel = (int)(long)(obj);
            else
                enemyLevel = -1;
            EnemyClass enemyClass = new(name, ability, hp, resist, skills, speed,scale, type, enemyLevel);
            enemyiesDict.Add(doc.Id, enemyClass);
        }
    }
    private async Task InitUpgrade()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Upgrade");
        Sprite[] upgradeIconSpirtes = Resources.LoadAll<Sprite>($"Texture/Upgrade");
        foreach (DocumentSnapshot doc in documents)
        {
            Dictionary<string, object> dict = doc.ToDictionary();
            List<UpgradeContent> guildContents = new();
            Dictionary<Language, string> name = new();
            Dictionary<Language, string> explain = new();
            Dictionary<Language, string> info = new();
            UpgradeEffectType type;

            foreach (object contentObj in dict["Contents"] as List<object>)
            {
                Dictionary<string, object> contentDict = contentObj as Dictionary<string, object>;
                float value = GetFloatValue(contentDict["Value"]);
                int price = (int)(long)contentDict["Price"];
                guildContents.Add(new(value, price));
            }
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
            //Explain
            foreach (var x in dict["Explain"] as Dictionary<string, object>)
            {
                string value = (string)x.Value;
                value = value.Replace("<E>", "<color=#4C4CFF><size=120%><b>");
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
            ///Info
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
            //Type
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
                case "RewardUp":
                    type = UpgradeEffectType.RewardUp;
                    break;
            }
            //Upgrade
            Sprite iconSprite = upgradeIconSpirtes.Where(item => item.name == doc.Id).FirstOrDefault();
            upgradeDict.Add(doc.Id, new UpgradeClass(name, (int)(long)dict["Index"], guildContents, explain, info, type, (string)dict["LobbyCase"], iconSprite));
        }
    }
    private async Task InitTalent()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Talent");
        Sprite[] talentSprites = Resources.LoadAll<Sprite>($"Texture/Talent");
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
            List<TalentEffect> effects = new();
            {
                if (dict.ContainsKey("Effect"))
                {
                    List<object> effectList = dict["Effect"] as List<object>;
                    foreach (object effectobj in effectList)
                    {
                        List<float> valueList = new();
                        Dictionary<string, object> effectDict = effectobj as Dictionary<string, object>;
                        EffectType effectType = EffectType.AttAscend;
                        if (effectDict.TryGetValue("Type", out object obj))
                        {
                            bool success = Enum.TryParse((string)obj, out effectType);
                            if (!success)
                            {
                                Debug.LogError("Invalid Parse");
                            }
                        }
                        foreach (object x in (List<object>)effectDict["Value"])
                        {
                            valueList.Add(GetFloatValue(x));
                        }
                        effects.Add(new(valueList, effectType));
                    }
                }
            }
            Sprite sprite = talentSprites.Where(item => item.name == doc.Id).FirstOrDefault();
            talentDict.Add(doc.Id, new TalentClass(doc.Id, name, (int)(long)dict["AbleLevel"], explain, effects, sprite));
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
            EnemyCase enemyCase = new(enemyPieces);
            enemyCaseDict.Add(doc.Id, enemyCase);
        }
    }
    private async Task InitNodeType()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("NodeType");
        BackgroundType[] allTypes = (BackgroundType[])Enum.GetValues(typeof(BackgroundType));
        Sprite[] sprites = Resources.LoadAll<Sprite>("Texture/NodeType");
        foreach (DocumentSnapshot doc in documents)
        {
            Dictionary<string, object> dict = doc.ToDictionary();
            BackgroundType backgroundType;

            Dictionary<string, NodeType> targetDict = nodeTypesDict;
            List<string> casesStr = new();
            if (dict.ContainsKey("EnemyCases"))
                foreach (object obj in (List<object>)dict["EnemyCases"])
                {
                    casesStr.Add((string)obj);
                }
            Dictionary<string, object> nameFromDoc = (Dictionary<string, object>)dict["Name"];
            if (dict.ContainsKey("BackgroundType"))
            {
                backgroundType = (BackgroundType)Enum.Parse(typeof(BackgroundType), (string)dict["BackgroundType"]);
            }
            else
            {
                backgroundType = BackgroundType.Store;
            }
            Dictionary<Language, string> name = new()
            {
                { Language.Ko, (string)nameFromDoc["Ko"] },
                { Language.En, (string)nameFromDoc["En"] }
            };
            Sprite objectSprite = sprites.Where(item => item.name == doc.Id).FirstOrDefault();
            NodeType nodeType = new(casesStr, name, objectSprite, backgroundType);
            targetDict.Add(doc.Id, nodeType);
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
                    name.Add(Language.Ko, string.Empty);
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
                WeaponClass weaponClass = new(ItemType.Weapon, itemId, grade, name, explain, sprite, scale, position);
                //Effects
                if (dict.ContainsKey("Effects"))
                {
                    List<SkillEffect> effects = GetSkillEffect(dict["Effects"]);
                    if (effects != null)
                        foreach (SkillEffect x in effects)
                        {
                            x.isPassive = true;
                        }
                    weaponClass.SetEffects(effects);
                }
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
                    VisualEffect ve;
                    Dictionary<string, object> veDict = obj as Dictionary<string, object>;
                    if (weaponVisualEffectDict.TryGetValue((string)veDict["Default"], out ve))
                        defaultVisualEffect = ve;
                    if (weaponVisualEffectDict.TryGetValue((string)veDict["Skill"], out ve))
                        skillVisualEffect = ve;
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
            int pokerNum;
            IngredientType ingredientType;
            Sprite sprite;
            Dictionary<string, object> dict = doc.ToDictionary();
            pokerNum = (int)(long)dict["PokerNum"];
            switch ((string)dict["IngredientType"])
            {
                default:
                    ingredientType = IngredientType.Meat;
                    break;
                case "Fruit":
                    ingredientType = IngredientType.Fruit;
                    break;
                case "Vegetable":
                    ingredientType = IngredientType.Vegetable;
                    break;
                case "Fish":
                    ingredientType = IngredientType.Fish;
                    break;
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
            if (sprite == null)
                Debug.LogError("No Sprite" + doc.Id);
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
            ingredient.SetPokerNum(pokerNum);
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
            PokerCombination pokerCombination;
            FoodEffect foodEffect = null;
            ItemGrade itemGrade;
            Sprite sprite;
            sprite = sprites.Where(item => item.name == doc.Id).FirstOrDefault();
            if (sprite == null)
                Debug.LogError("No Sprite" + doc.Id);
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
            //PokerCombination
            pokerCombination = PokerCombination.NoCard;
            if (dict.TryGetValue("PokerCombination", out obj))
            {
                bool success = Enum.TryParse((string)obj, out pokerCombination);
                if (!success)
                {
                    Debug.LogError("Invalid Parse");
                }
            }
            if (dict.TryGetValue("Effect", out obj))
            {
                Dictionary<string, object> asDict = obj as Dictionary<string, object>;
                float healAmount;
                FoodTargetRange targetRange;
                Dictionary<StatusType, float> statusValue = null;

                healAmount = GetFloatValue(asDict["HealAmount"]);
                Enum.TryParse((string)asDict["TargetRange"], out targetRange);
                if (asDict.ContainsKey("StatusValue"))
                {
                    statusValue = new();
                    Dictionary<string, object> statusDict = (Dictionary<string, object>)asDict["StatusValue"];
                    statusValue.Add(StatusType.Ability, GetFloatValue(statusDict["Ability"]));
                    statusValue.Add(StatusType.HpMax, GetFloatValue(statusDict["HpMax"]));
                    statusValue.Add(StatusType.Resist, GetFloatValue(statusDict["Resist"]));
                    statusValue.Add(StatusType.Speed, GetFloatValue(statusDict["Speed"]));
                }
                foodEffect = new(healAmount, targetRange, statusValue);
            }
            switch (pokerCombination)
            {
                default://HighCard, OnePair
                    itemGrade = ItemGrade.None;
                    break;
                case PokerCombination.TwoPair:
                case PokerCombination.ThreeOfAKind:
                case PokerCombination.Straight:
                    itemGrade = ItemGrade.Normal;
                    break;
                case PokerCombination.Flush:
                case PokerCombination.FullHouse:
                    itemGrade = ItemGrade.Rare;
                    break;
                case PokerCombination.FourOfAKind:
                case PokerCombination.StraightFlush:
                    itemGrade = ItemGrade.Unique;
                    break;
            }
            FoodClass foodClass = new(ItemType.Food, doc.Id, itemGrade, name, explain, sprite, Vector2.one * 0.6f, Vector2.zero);
            foodClass.SetPokerCombination(pokerCombination);
            foodClass.SetFoodEffect(foodEffect);
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
    [ContextMenu("SetDoc")]
    public async void SetDoc()
    {
        List<DocumentSnapshot> docs = await DataManager.dataManager.GetDocumentSnapshots("EnemyCase");
        foreach (DocumentSnapshot doc in docs)
        {
            List<Dictionary<string, object>> list = new()
            {
                new Dictionary<string, object>() { { "GridIndex", 3 }, { "Type", string.Empty } },
                new Dictionary<string, object>() { { "GridIndex", 3 }, { "Type", string.Empty } },
                new Dictionary<string, object>() { { "GridIndex", 3 }, { "Type", string.Empty } }
            };
            Dictionary<string, object> dict = new()
            {
                { "StatusType", list},
            };
            DataManager.dataManager.SetDocumentData("Enemies", list, "EnemyCase", doc.Id);

        }
        Debug.Log("Fin");

    }

}