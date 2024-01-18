using BattleCollection;
using EnumCollection;
using Firebase.Firestore;
using LobbyCollection;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadManager : MonoBehaviour//Firestore에 있는 기초 데이터들 로딩해서 저장하고 있는 스크립트
{
    public static LoadManager loadManager;
    bool isInit = false;
    public Dictionary<string, SkillForm> skillsDict = new();//Key는 Document의 ID
    public Dictionary<string, JobClass> jobsDict = new();//Key는 스킬의 Index. Ex)200, 101
    public Dictionary<string, EnemyClass> enemyiesDict = new();//Key는 Document의 ID를 int로 parse
    public Dictionary<string, GuildClass> guildDict = new();
    public Dictionary<string, TalentFormStruct> talentDict = new();
    public Dictionary<string, EnemyCase> enemyCaseDict = new();
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
            await InitSkill();
            await Task.WhenAll(InitJob(), InitEnemy(), InitGuild(), InitTalent(), InitUserDoc(), InitEnemyCase());
            Debug.Log("FireStoreLoaded");
            isInit = true;
        }
    }
    private async Task InitSkill()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Skill");
        foreach (DocumentSnapshot doc in documents)
        {
            SkillForm skillForm = new();
            Dictionary<string, object> skillDict = doc.ToDictionary();//도큐먼트
            if ((string)skillDict["Categori"] == "Withhold")
                continue;

            List<Dictionary<Language, string>> explains;

            SkillCategori categori;
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
            skillForm.SetCategori(categori);
                List<List<SkillEffect>> effects;
            if (categori == SkillCategori.Enemy)//적의 스킬
            {
                List<SkillEffect> effect = InitEffect("Effect", skillDict);
                effects = new() { effect };
                skillForm.SetEffects(effects);
               explains = null;
            }
            else//아군 캐릭터가 사용하는 스킬
            {
                Dictionary<Language, string> name = new();
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
                skillForm.SetName(name);
                //Debug.Log("Skill Init : " + doc.Id + ", " + "Effect_0");
                List<SkillEffect> effect0 = InitEffect("Effect_0", skillDict);
                //Debug.Log("Skill Init : " + doc.Id + ", " + "Effect_1");
                List<SkillEffect> effect1 = InitEffect("Effect_1", skillDict);
                //Debug.Log("Skill Init : " + doc.Id + ", " + "Effect_2");
                List<SkillEffect> effect2 = InitEffect("Effect_2", skillDict);
                skillForm.SetEffects(new() { effect0, effect1, effect2 });
                explains = new();
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
                skillForm.SetExplain(explains);    
            }
            object obj;
            //CoolTime
            if (skillDict.TryGetValue("Cooltime", out obj))
                skillForm.SetCooltime(GetFloatValue(obj));
            //IsTargetEnemy
            bool isTargetEnemy;
            if (skillDict.TryGetValue("IsTargetEnemy", out obj))
                isTargetEnemy = (bool)obj;
            else
                isTargetEnemy = true;
            skillForm.SetIsTargetEnemy(isTargetEnemy);
            //IsAnim
            bool isAnim;
            if (skillDict.TryGetValue("IsAnim", out obj))
                isAnim = (bool)obj;
            else
                isAnim = true;
            skillForm.SetIsAnim(isAnim);


            skillsDict.Add(doc.Id, skillForm);
            
            List<SkillEffect> InitEffect(string effectStr, Dictionary<string, object> _skillDict)
            {
                List<SkillEffect> effects = new();
                foreach (object docDict in _skillDict[effectStr] as List<object>)
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
                                range = EffectRange.Back;
                                break;
                            case "Front":
                                range = EffectRange.Front;
                                break;
                        }
                        effect.SetRange(range);
                    }
                    //IsConst
                    if (effectDict.TryGetValue("IsConst", out obj))
                        effect.SetIsConst((bool)effectDict["IsConst"]);
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
                    //적용
                    effects.Add(effect);
                }
                return effects;
            }
        }
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
            case "BuffAscend":
                type = EffectType.BuffAscend;
                break;
            case "DebuffAscend":
                type = EffectType.DebuffAscend;
                break;
            case "HealAscend":
                type = EffectType.HealAscend;
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
            JobClass jobStruct = new();
            if (dict.TryGetValue("HpCoef", out obj))
            {
                jobStruct.SetHpCoef(GetFloatValue(obj));
            }
            if (dict.TryGetValue("AbilityCoef", out obj))
            {
                jobStruct.SetAbilityCoef(GetFloatValue(obj));
            }
            if (dict.TryGetValue("SpeedCoef", out obj))
            {
                jobStruct.SetSpeedCoef(GetFloatValue(obj));
            }
            string jobId = (string)dict["JobId"];
            JobType jobType;
            switch (jobId)
            {
                default:
                    jobType = JobType.None;
                    break;
                case "020":
                    jobType = JobType.Tanker;
                    break;
                case "110":
                    jobType = JobType.Warrior;
                    break;
                case "200":
                    jobType = JobType.Ranger;
                    break;
                case "011":
                    jobType = JobType.Crusader;
                    break;
                case "101":
                    jobType = JobType.Thief;
                    break;
                case "002":
                    jobType = JobType.Witch;
                    break;
            }
            jobStruct.SetJobType(jobType);
            //Name
            Dictionary<Language, string> name = new();
            Dictionary<string, object> nameTemp = dict["Name"] as Dictionary<string, object>;
            name.Add(Language.Ko, (string)nameTemp["Ko"]);
            name.Add(Language.En, (string)nameTemp["En"]);
            jobStruct.SetName(name);

            jobsDict.Add(jobId, jobStruct);
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
            foreach (object x in dict["Skills"] as List<object>)
            {
                skills.Add(GameManager.LocalizeSkill((string)x));
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
            enemyiesDict.Add(doc.Id, enemyClass);
        }
    }
    private async Task InitGuild()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Guild");
        foreach (DocumentSnapshot doc in documents)
        {
            Dictionary<string, object> dict = doc.ToDictionary();
            List<GuildContent> guildContents = new();
            foreach (object contentObj in dict["Content"] as List<object>)
            {
                Dictionary<string, object> contentDict = contentObj as Dictionary<string, object>;
                guildContents.Add(new((int)(long)contentDict["Price"], (int)(long)contentDict["Value"]));
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
                switch (x.Key)
                {
                    default:
                        explain.Add(Language.Ko, (string)x.Value);
                        break;
                    case "En":
                        explain.Add(Language.En, (string)x.Value);
                        break;
                }
            }
            GuildEffectType type;
            switch (dict["Type"])
            {
                default:
                    Debug.LogError("InitDefault... " + dict["Type"]);
                    type = GuildEffectType.AllocateNumberUp;
                    break;
                case "AllocateNumberUp":
                    type = GuildEffectType.AllocateNumberUp;
                    break;
                case "AbilityUp":
                    type = GuildEffectType.AbilityUp;
                    break;
                case "HpUp":
                    type = GuildEffectType.HpUp;
                    break;
                case "ResistUp":
                    type = GuildEffectType.ResistUp;
                    break;
                case "TalentLevelUp":
                    type = GuildEffectType.TalentLevelUp;
                    break;
                case "TalentNumUp":
                    type = GuildEffectType.TalentNumUp;
                    break;
                case "SpeedUp":
                    type = GuildEffectType.SpeedUp;
                    break;
            }
            guildDict.Add(doc.Id, new GuildClass(name, (int)(long)dict["Index"], guildContents, explain[GameManager.language], type));
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
                for (int i = 0; i < guildDict.Count; i++)
                {
                    DataManager.dataManager.SetDocumentData("Guild_" + i, 0, "User", GameManager.gameManager.Uid);
                }
                DataManager .dataManager.SetDocumentData( "Fame", 0, "User", GameManager.gameManager.Uid);
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
            List<System.Tuple<string, int>> enemyTuples = new();
            List<object> enemies = dict["Enemies"] as List<object>;
            foreach (object enemyObj in enemies)
            {
                Dictionary<string, object> enemy = enemyObj as Dictionary<string, object>;
                string id = (string)enemy["Id"];
                int index = (int)(long)enemy["Index"];
                enemyTuples.Add(new(id, index));
            }
            enemyCase.SetEnemies(enemyTuples);
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
    float GetFloatValue(object _obj)
    {
        if (_obj is long)
            return (int)(long)_obj;
        else
            return (float)(double)_obj;
    }
}