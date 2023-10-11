using EnumCollection;
using Firebase.Firestore;
using StructCollection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadManager : MonoBehaviour//Firestore에 있는 기초 데이터들 로딩해서 저장하고 있는 스크립트
{
    public static LoadManager loadManager;
    bool isInit = false;
    public Dictionary<string, SkillForm> skillsDict = new();//Key는 Document의 ID
    public Dictionary<string, JobStruct> jobsDict = new();//Key는 스킬의 Index. Ex)200, 101
    public Dictionary<string, EnemyStruct> enemyiesDict = new();//Key는 Document의 ID를 int로 parse
    public Dictionary<string, GuildStruct> guildDict = new();
    public Dictionary<string, TalentFormStruct> talentDict = new();
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
            await InitJob();
            await InitEnemy();
            await InitGuild();
            await InitTalent();
            await InitUserDoc();
            Debug.Log("FireStoreLoaded");
            isInit = true;
        }
    }
    private async Task InitSkill()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Skill");
        foreach (DocumentSnapshot doc in documents)
        {
            Dictionary<string, object> skillDict = doc.ToDictionary();//도큐먼트
            if ((string)skillDict["Categori"] == "Withhold")
                continue;
            Dictionary<Language, string> name = new();
            List<Dictionary<Language, string>> explains;
            foreach (var x in skillDict["Name"] as Dictionary<string, object>)
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
            List<List<SkillEffect>> effects;
            if (categori == SkillCategori.Enemy)
            {
                List<SkillEffect> effect = LoadEffect("Effect", skillDict);
                effects = new() { effect };
                explains = null;
            }
            else
            {
                List<SkillEffect> effect0 = LoadEffect("Effect_0", skillDict);
                List<SkillEffect> effect1 = LoadEffect("Effect_1", skillDict);
                List<SkillEffect> effect2 = LoadEffect("Effect_2", skillDict);

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
                effects = new() { effect0, effect1, effect2 };
            }

            //CoolTime
            float coolTime;
            if (skillDict.ContainsKey("CoolTIme"))
                coolTime = GetFloatValue(skillDict, "CoolTime");
            else
                coolTime = 3f;
            //IsSelf
            bool isTargetEnemy;
            if (skillDict.ContainsKey("IsTargetEnemy"))
                isTargetEnemy = (bool)skillDict["IsTargetEnemy"];
            else
                isTargetEnemy = false;
            //ProperDistance
            var skillForm = new SkillForm().
                SetCategori(categori).
                SetCoolTime(coolTime).
                SetEffects(effects).
                SetExplain(explains).
                SetIsSelf(isTargetEnemy).
                SetName(name)
                ;
            skillsDict.Add(doc.Id, skillForm);
        }
    }
    private async Task InitJob()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Job");
        foreach (DocumentSnapshot doc in documents)
        {
            Dictionary<string, object> dict = doc.ToDictionary();
            float hpCoef = GetFloatValue(dict, "HpCoef");
            float abilityCoef = GetFloatValue(dict, "AbilityCoef");
            jobsDict.Add((string)dict["Index"], new JobStruct((string)dict["Name"], hpCoef, abilityCoef));
        }
    }
    private async Task InitEnemy()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Enemy");
        foreach (DocumentSnapshot doc in documents)
        {
            Dictionary<string, object> dict = doc.ToDictionary();
            List<Skill> skills = new();
            foreach (object x in dict["Skills"] as List<object>)
            {
                skills.Add(GameManager.LocalizeSkill((string)x));
            }
            float resist;
            float speed;
            if (dict.ContainsKey("Resist"))
            {
                resist = GetFloatValue(dict, "Resist");
            }
            else
                resist = 0;
            if (dict.ContainsKey("Speed"))
            {
                speed = GetFloatValue(dict, "Speed");
            }
            else
                speed = 0;

            EnemyStruct enemyStruct = new EnemyStruct().SetName((string)dict["Name"]).
                SetAbility(GetFloatValue(dict, "Ability")).
                SetHp(GetFloatValue(dict, "Hp")).
                SetResist(resist).
                SetSkills(skills).
                SetSpeed(speed);
            enemyiesDict.Add(doc.Id, enemyStruct);
        }
    }
    private async Task InitGuild()
    {
        List<DocumentSnapshot> documents = await DataManager.dataManager.GetDocumentSnapshots("Guild");
        foreach (DocumentSnapshot doc in documents)
        {
            Dictionary<string, object> dict = doc.ToDictionary();
            List<int> prices = new();
            foreach (var x in dict["Price"] as List<object>)
            {
                prices.Add((int)(long)x);
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
            guildDict.Add(doc.Id, new GuildStruct(name[GameManager.language], prices, explain[GameManager.language]));
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
            int index = 0;
            while(true)
            {
                if (dict.TryGetValue("Effect_" + index, out object value))
                {
                    Dictionary<string, object> effectDict = value as Dictionary<string, object>;
                    effects.Add(new((string)effectDict["Value"],(string)effectDict["Type"]));
                    index++;
                }
                else
                    break;
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
                    DataManager.dataManager.SetDocumentData("User", GameManager.gameManager.Uid, "Guild_" + i, 0);
                    GameManager.gameManager.guild.Add(0);
                }
                DataManager .dataManager.SetDocumentData("User", GameManager.gameManager.Uid, "Fame", 0);
                GameManager.gameManager.fame = 0;
            }
        });
    }
    float GetFloatValue(Dictionary<string, object> _dict, string _field)
    {
        if (_dict[_field] is long)
            return (long)_dict[_field];
        else
            return (float)(double)_dict[_field];
    }
    List<SkillEffect> LoadEffect(string effectStr, Dictionary<string, object> _skillDict)
    {
        List<SkillEffect> effects = new();
        foreach (object x in _skillDict[effectStr] as List<object>)
        {
            Dictionary<string, object> effectDict = x as Dictionary<string, object>;
            //Value
            float value;
            if (effectDict.ContainsKey("Value"))
                value = GetFloatValue(effectDict, "Value");
            else
                value = 1f;
            //Count
            int count;
            if (effectDict.ContainsKey("Count"))
                count = (int)(long)effectDict["Count"];
            else
                count = 1;
            //Range
            EffectRange range;
            if (effectDict.ContainsKey("Range"))
                switch (effectDict["Range"])
                {
                    default:
                        range = EffectRange.Dot;
                        break;
                    case "Row":
                        range = EffectRange.Row;
                        break;
                    case "All":
                        range = EffectRange.All;
                        break;
                }
            else
                range = EffectRange.Dot;
            //Cycle
            int cycle;
            if (effectDict.ContainsKey("Cycle"))
                cycle = (int)(long)effectDict["Cycle"];
            else
                cycle = 1;
            //IsConst
            bool isConst;
            if (effectDict.ContainsKey("IsConst"))
                isConst = (bool)effectDict["IsConst"];
            else
                isConst = false;
            //Type
            EffectType type;
            if (effectDict.ContainsKey("Type"))
                switch (effectDict["Type"])
                {
                    case "AttBuff":
                        type = EffectType.AttBuff;
                        break;
                    case "DefBuff":
                        type = EffectType.DefBuff;
                        break;
                    case "AttDebuff":
                        type = EffectType.AttDebuff;
                        break;
                    case "DefDebuff":
                        type = EffectType.DefDebuff;
                        break;
                    case "AttUp":
                        type = EffectType.AttUp;
                        break;
                    case "DefUp":
                        type = EffectType.DefUp;
                        break;
                    case "AttDown":
                        type = EffectType.AttDown;
                        break;
                    case "DefDown":
                        type = EffectType.DefDown;
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
                    case "Armor":
                        type = EffectType.Armor;
                        break;
                    case "ArmorAtt":
                        type = EffectType.ArmorAtt;
                        break;
                    case "DamageShare":
                        type = EffectType.DamageShare;
                        break;
                    case "Ability":
                        type = EffectType.Ability;
                        break;
                    case "BleedTransfer":
                        type = EffectType.BleedTransfer;
                        break;
                    default:
                        type = EffectType.Damage;
                        break;
                }
            else
                type = EffectType.Damage;
            float delay;
            if (effectDict.ContainsKey("Delay"))
                delay = GetFloatValue(effectDict, "Delay");
            else
                delay = 0f;
            effects.Add(
                new SkillEffect().
                SetCount(count).
                SetIsConst(isConst).
                SetType(type).
                SetValue(value).
                SetDelay(delay).
                SetRange(range)
                ) ;
        }
        return effects;
    }
}