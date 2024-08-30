using DefaultCollection;
using EnumCollection;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class ApplicantSlot : SlotBase
{
    private RecruitUi recruitUi;
    public float Hp { get; private set; }
    public float Ability { get; private set; }
    public float Speed { get; private set; }
    public float Resist { get; private set; }
    public GameObject objectSelect;
    public GameObject imageNum;
    public TMP_Text textNum;
    private TMP_Text textSelect;
    private bool isActived;
    private bool isSelected;
    public Animator templateAnimator;
    private readonly Color Black = Color.black;
    private readonly Color GrayColor = new Color(45f / 255f, 45f / 255f, 45f / 255f);
    private readonly Color BrownColor = new Color(80f / 255f, 45f / 255f, 0f);
    private readonly Color BlueColor = new Color(28f / 255f, 58f / 255f, 180f / 255f);
    private readonly Color RedColor = new Color(204f / 255f, 18f / 255f, 23f / 255f);
    private readonly Color YellowColor = new Color(1f, 1f, 0f);
    private readonly Color WhiteColor = new Color(1f, 1f, 1f);
    private readonly Color GreenColor = new Color(0f, 190f / 255f, 0f);
    public GameObject templateObject;
    public Dictionary<string, object> bodyDict = new();

    public List<TalentClass> talents { get; private set; } = new();

    private new void Awake()
    {
        imageNum.gameObject.SetActive(false);
        IsActived = false;
        isSelected = false;
        textSelect = objectSelect.transform.GetChild(0).GetComponent<TMP_Text>();
        OnLanguageChange();
        SettingManager.LanguageChangeEvent += OnLanguageChange;

    }
    private void OnLanguageChange()
    {
        if (isSelected)
            textSelect.text = GameManager.language == Language.Ko ? "해제" : "Deselect";
        else
            textSelect.text = GameManager.language == Language.Ko ? "선택" : "Select";
    }
    private void Start()
    {
        if (GameManager.lobbyScenario == null)
            return;
        recruitUi = GameManager.lobbyScenario.recruitUi;
    }
    public bool IsActived {
        get {
            return isActived;
        }
        set {
            isActived = value;
            objectSelect.SetActive(isActived);
            if (templateAnimator)
                templateAnimator.speed = isActived ? 1f : 0f;
        }
    }
    public void InitApplicantSlot()
    {
        InitStatusInRange();
        InitTalent();
        InitCharacterTemplate();
    }

    private void InitTalent()
    {
        float talentLevel = 0f;
        if (GameManager.gameManager.upgradeValueDict.ContainsKey(UpgradeEffectType.TalentLevelUp))
            talentLevel = GameManager.gameManager.upgradeValueDict[UpgradeEffectType.TalentLevelUp];
        int talentNum = GameManager.AllocateProbability(0.1f, 0.6f, 0.25f, 0.05f);//0, 1, 2, 3개
        //int talentNum = GameManager.AllocateProbability(0.5f, 0.5f);
        List<TalentClass> ableTalents = LoadManager.loadManager.talentDict.Where(item => item.Value.ableLevel>=0).Where(item => item.Value.ableLevel <= talentLevel).Select(item =>item.Value).ToList();
        for (int i = 0; i < talentNum; i++)
        {
            TalentClass selectedTalent = ableTalents[Random.Range(0, ableTalents.Count)];
            selectedTalent.SetEffectLevel(Random.Range(0, selectedTalent.effects.Count));
            talents.Add(selectedTalent);
            ableTalents.Remove(selectedTalent);
        }
        talents = talents.OrderBy(item => item.ableLevel).ThenBy(item => item.ableLevel).ToList();
        if (talents.Count == 0)
        {
            talents.Add(LoadManager.loadManager.talentDict["NoTalent"]);
        }
    }

    private void InitStatusInRange()
    {
        isActived = false;
        objectSelect.SetActive(isActived);
        Hp = GetStatus(LobbyScenario.defaultHp, LobbyScenario.hpSd);
        Ability = GetStatus(LobbyScenario.defaultAbility, LobbyScenario.abilitySd);
        Speed = GetStatus(LobbyScenario.defaultSpeed, LobbyScenario.speedSd);
        Resist = GetStatus(LobbyScenario.defaultResist, LobbyScenario.resistSd);
    }
    private void InitCharacterTemplate()
    {
        templateObject = Instantiate(GameManager.gameManager.CharacterTemplate, transform);
        foreach (Transform child in templateObject.transform)
        {
            if (null == child)
            {
                continue;
            }
            child.gameObject.layer = LayerMask.NameToLayer("Lobby");
        }
        templateObject.transform.localPosition = new Vector3(0f, -45f, 0f);
        templateObject.transform.localScale = Vector3.one * 120f;
        templateAnimator = templateObject.transform.GetChild(0).GetComponent<Animator>();
        templateAnimator.speed = 0f;
        CharacterHierarchy characterHierarchy = templateObject.transform.GetChild(0).GetComponent<CharacterHierarchy>();
        InitTemplateSprite(characterHierarchy);

        void InitTemplateSprite(CharacterHierarchy characterHierarchy)
        {
            characterHierarchy.transform.localScale = Vector3.one;
            Sprite hair;
            Sprite faceHair;
            Sprite head;
            Sprite eyeFront;
            Sprite eyeBack;
            Sprite armL;
            Sprite armR;
            Color hairColor = Color.black;
            Color eyeColor=Color.black;
            hair = faceHair =  null;
            //Species
            Species species;
            //Human, Elf, Devil, Skelton, Orc
            switch (GameManager.AllocateProbability(0.65f,0.1f, 0.05f, 0.1f, 0.1f ))
            {
                default:
                    species = Species.Human;
                    bodyDict.Add("Species", "Human");
                    break;
                case 1:
                    species = Species.Elf;
                    bodyDict.Add("Species", "Elf");
                    break;
                case 2:
                    species = Species.Devil;
                    bodyDict.Add("Species", "Devil");
                    break;
                case 3:
                    species = Species.Skelton;
                    bodyDict.Add("Species", "Skelton");
                    break;
                case 4:
                    species = Species.Orc;
                    bodyDict.Add("Species", "Orc");
                    break;
            }
            //Eye
            List<KeyValuePair<string, EyeClass>> eyeKvps = new(LoadManager.loadManager.eyeDict[species]);
            int eyeNum = Random.Range(0, eyeKvps.Count);
            KeyValuePair<string, EyeClass> eyeKvp = eyeKvps[eyeNum];
            eyeFront = eyeKvp.Value.front;
            eyeBack = eyeKvp.Value.back;
            bodyDict.Add("Eye", eyeKvp.Key);
            //BodyPart
            List<KeyValuePair<string, BodyPartClass>> bodyPartValues = new(LoadManager.loadManager.BodyPartDict[species]);
            int bodyPartNum = Random.Range(0, bodyPartValues.Count);
            KeyValuePair<string, BodyPartClass> bodyPartKvp = bodyPartValues[bodyPartNum];
            head = bodyPartKvp.Value.head;
            armL = bodyPartKvp.Value.armL;
            armR = bodyPartKvp.Value.armR;
            bodyDict.Add("Body", bodyPartKvp.Key);
            if (species == Species.Human || species == Species.Elf)
            {
                //Hair
                List<KeyValuePair<string, Sprite>> hairKvps = new(LoadManager.loadManager.hairDict);
                int hairNum = Random.Range(0, hairKvps.Count + 1);
                if (hairNum != hairKvps.Count)
                {
                    hair = hairKvps[hairNum].Value;
                    bodyDict.Add("Hair", hairKvps[hairNum].Key);
                }
                else
                {
                    hair = null;
                    bodyDict.Add("Hair", string.Empty);
                }
                //FaceHair
                if (GameManager.CalculateProbability(0.3f))
                {
                    List<KeyValuePair<string, Sprite>> faceHairKvps = new(LoadManager.loadManager.faceHairDict);
                    int faceHairNum = Random.Range(0, faceHairKvps.Count);
                    faceHair = faceHairKvps[faceHairNum].Value;
                    bodyDict.Add("FaceHair", faceHairKvps[faceHairNum].Key);
                }
                else
                {
                    faceHair = null;
                    bodyDict.Add("FaceHair", string.Empty);
                }
                
                //Haircolor
                switch (GameManager.AllocateProbability(0.5f, 0.2f, 0.05f, 0.05f, 0.05f, 0.1f, 0.05f))
                {
                    default:
                        hairColor = GrayColor;
                        break;
                    case 1:
                        hairColor = BrownColor;
                        break;
                    case 2:
                        hairColor = BlueColor;
                        break;
                    case 3:
                        hairColor = RedColor;
                        break;
                    case 4:
                        hairColor = YellowColor;
                        break;
                    case 5:
                        hairColor = WhiteColor;
                        break;
                    case 6:
                        hairColor = GreenColor;
                        break;

                }
                //Eye
                switch (GameManager.AllocateProbability(0.4f, 0.4f, 0.1f, 0.1f))
                {
                    default:
                        eyeColor = BrownColor;
                        break;
                    case 1:
                        eyeColor = Black;
                        break;
                    case 2:
                        eyeColor = BlueColor;
                        break;
                    case 3:
                        eyeColor = RedColor;
                        break;

                }
                Dictionary<string, float> colorDict = new();
                colorDict.Add("R", hairColor.r);
                colorDict.Add("G", hairColor.g);
                colorDict.Add("B", hairColor.b);
                bodyDict.Add("EyeColor", colorDict);
            }
            characterHierarchy.SetBodySprite(hair, faceHair, eyeFront, eyeBack, head, armL, armR,null, hairColor, eyeColor);
        }
    }
    public void SlotBtnClicked()
    {
        SoundManager.SfxPlay("Hover");

        switch (isActived)
        {
            case true://활성화 돼있었다면 비활성화
                recruitUi.InitStatusText();
                recruitUi.InitTalent();
                IsActived = false;
                break;
            case false://비활성화 돼있었다면 활성화
                recruitUi.SetStatusText(Hp, Ability, Speed, Resist);
                recruitUi.InactiveEnterBtns();
                recruitUi.SetTalents(talents);
                IsActived = true;
                break;
        }

    }
    public void SelectBtnClicked()
    {
        SoundManager.SfxPlay("ScifiThin");
        recruitUi.InitStatusText();
        recruitUi.InitTalent();
        IsActived = false;
        if (!isSelected)
        {
            int currentSelectedNum = recruitUi.AddSelectedSlot(this);
            if (currentSelectedNum != -1)//성공 여부
            {
                isSelected = true;
                textSelect.text = GameManager.language == Language.Ko ? "해제" : "Deselect";
                imageNum.gameObject.SetActive(true);
                textNum.text = currentSelectedNum.ToString(); ;
            }
        }
        else
        {
            recruitUi.RemoveSelectedSlot(this);
            isSelected = false;
            textSelect.text = GameManager.language == Language.Ko ? "선택" : "Select";
            imageNum.gameObject.SetActive(false);
        }

    }
    private float GetStatus(float _defaultStatus, float _standardDeviation)
    {
        float returnValue = GetRandomNumber(_defaultStatus, _standardDeviation);
        if (GameManager.gameManager.upgradeValueDict.TryGetValue(UpgradeEffectType.StatusUp, out float statusUp))
        {
            returnValue *= 1f + statusUp;
        }
        return returnValue;
    }
    public float GetRandomNumber(float _mean, float _standardDeviation)
    {
        System.Random random = new();
        double u1 = 1.0 - random.NextDouble(); // 난수 생성
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); // 정규 분포를 따르는 값 생성
        float randNormal = _mean + _standardDeviation * (float)randStdNormal; // 평균과 표준 편차 적용

        // 평균 값의 최소 50%, 최대 200%로 값을 제한
        randNormal = Mathf.Clamp(randNormal, _mean * 0.5f, _mean * 2f);

        return randNormal;
    }

}
