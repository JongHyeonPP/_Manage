using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterCollection;
using UnityEngine.UI;
using TMPro;
public class ApplicantSlot : MonoBehaviour
{
    private float hp;
    private float ability;
    private float speed;
    private float resist;
    public GameObject objectSelect;
    public SpriteRenderer rendererCheck;
    private TMP_Text textSelect;
    private bool isActived;
    private bool isSelected;
    private Animator templateAnimator;
    private readonly Color BlackHair = new Color(45f / 255f, 45f / 255f, 45f / 255f);
    private readonly Color BrownHair = new Color(80f / 255f, 45f / 255f, 0f);
    private readonly Color BlueHair = new Color(28f / 255f, 58f / 255f, 180f / 255f);
    private readonly Color RedHair = new Color(204f / 255f, 18f / 255f, 23f / 255f);
    private readonly Color YellowHair = new Color(1f, 1f, 0f);
    private readonly Color WhiteHair = new Color(1f, 1f, 1f);
    private readonly Color GreenHair = new Color(0f, 190f / 255f, 0f);
    GameObject templateObject;

    private void Awake()
    {
        rendererCheck.enabled = false;
        IsActived = false;
        isSelected = false;
        textSelect = objectSelect.transform.GetChild(0).GetComponent<TMP_Text>();
    }
    public bool IsActived {
        get {
            return isActived;
        }
        set {
            isActived = value;
            objectSelect.SetActive(isActived);
            templateAnimator.speed = isActived ? 1f : 0f;
        }
    }
    public void InitApplicantSlot()
    {
        InitStatusInRange();
        InitCharacterTemplate();
    }
    private void InitStatusInRange()
    {
        isActived = false;
        objectSelect.SetActive(isActived);
        hp = GetStatus(LobbyScenario.defaultHp, LobbyScenario.hpRange);
        ability = GetStatus(LobbyScenario.defaultAbility, LobbyScenario.abilityRange);
        speed = GetStatus(LobbyScenario.defaultSpeed, LobbyScenario.speedRange);
        resist = GetStatus(LobbyScenario.defaultResist, LobbyScenario.resistRange);
    }
    private void InitCharacterTemplate()
    {
        templateObject = Instantiate(GameManager.gameManager.CharacterTemplate, transform);
        templateObject.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        templateObject.transform.localScale = Vector3.one * 1.3f;
        templateAnimator = templateObject.transform.GetChild(0).GetComponent<Animator>();
        templateAnimator.speed = 0f;
        CharacterHierarchy characterHierarchy = templateObject.transform.GetChild(0).GetComponent<CharacterHierarchy>();
        characterHierarchy.shadow.SetActive(false);
        InitTemplateSprite(characterHierarchy);

        void InitTemplateSprite(CharacterHierarchy characterHierarchy)
        {
            Sprite hair;
            Sprite faceHair;
            Sprite head;
            Sprite eyeFront;
            Sprite eyeBack;
            Sprite armL;
            Sprite armR;
            Sprite weapon;
            Color hairColor;
            hair = faceHair = weapon = null;
            hairColor = Color.black;
            //Species
            Species species;
            //Human, Elf, Devil, Skelton, Orc
            switch (GameManager.AllocateProbability(0.65f,0.1f, 0.05f, 0.1f, 0.1f ))
            {
                default:
                    species = Species.Human;
                    break;
                case 1:
                    species = Species.Elf;
                    break;
                case 2:
                    species = Species.Devil;
                    break;
                case 3:
                    species = Species.Skelton;
                    break;
                case 4:
                    species = Species.Orc;
                    break;
            }
            //Eye
            List<EyeClass> eyeValues = new List<EyeClass>(LoadManager.loadManager.EyeDict[species].Values);
            int eyeNum = Random.Range(0, eyeValues.Count);
            EyeClass eyeClass = eyeValues[eyeNum];
            eyeFront = eyeClass.front;
            eyeBack = eyeClass.back;
            //BodyPart
            List<BodyPartClass> bodyPartValues = new List<BodyPartClass>(LoadManager.loadManager.BodyPartDict[species].Values);
            int bodyPartNum = Random.Range(0, bodyPartValues.Count);
            BodyPartClass bodyPartClass = bodyPartValues[bodyPartNum];
            head = bodyPartClass.head;
            armL = bodyPartClass.armL;
            armR = bodyPartClass.armR;
            if (species == Species.Human || species == Species.Elf)
            {
                //Hair
                List<Sprite> hairvalues = new List<Sprite>(LoadManager.loadManager.hairDict.Values);
                int hairNum = Random.Range(0, hairvalues.Count + 1);
                hair = (hairNum == hairvalues.Count) ? null : hairvalues[hairNum];
                //FaceHair
                if (GameManager.CalculateProbability(0.3f))
                {
                    List<Sprite> faceHairvalues = new List<Sprite>(LoadManager.loadManager.faceHairDict.Values);
                    int faceHairNum = Random.Range(0, faceHairvalues.Count);
                    faceHair = faceHairvalues[faceHairNum];
                }
                else
                {
                    faceHair = null;
                }

                //Haircolor
                switch (GameManager.AllocateProbability(0.5f, 0.2f, 0.05f, 0.05f, 0.05f, 0.1f, 0.05f))
                {
                    default:
                        hairColor = BlackHair;
                        break;
                    case 1:
                        hairColor = BrownHair;
                        break;
                    case 2:
                        hairColor = BlueHair;
                        break;
                    case 3:
                        hairColor = RedHair;
                        break;
                    case 4:
                        hairColor = YellowHair;
                        break;
                    case 5:
                        hairColor = WhiteHair;
                        break;
                    case 6:
                        hairColor = GreenHair;
                        break;

                }
            }
            if (GameManager.CalculateProbability(0.5f))
            {
                weapon = LoadManager.loadManager.weaponDict[WeaponType.Sword]["Default"].sprite;
            }
            else
            {
                weapon = LoadManager.loadManager.weaponDict[WeaponType.Club]["Default"].sprite;
            }
            characterHierarchy.SetBodySprite(hair, faceHair, eyeFront, eyeBack, head, armL, armR, hairColor);
        }
    }
    public void SlotBtnClicked()
    {
        switch (isActived)
        {
            case true://활성화 돼있었다면 비활성화
                GameManager.lobbyScenario.InitStatusText();
                IsActived = false;
                break;
            case false://비활성화 돼있었다면 활성화
                GameManager.lobbyScenario.SetStatusText(hp, ability, speed, resist);
                GameManager.lobbyScenario.InactiveEnterBtns();
                IsActived = true;
                break;
        }

    }
    public void SelectBtnClicked()
    {
        GameManager.lobbyScenario.InitStatusText();
        IsActived = false;
        if (!isSelected)
        {
            if (GameManager.lobbyScenario.AddSelectedSlot(this))//성공 여부
            {
                isSelected = true;
                textSelect.text = "해제";
            }
        }
        else
        {
            GameManager.lobbyScenario.RemoveSelectedSlot(this);
            isSelected = false;
            textSelect.text = "선택";
        }
        rendererCheck.enabled = isSelected;
    }
    private float GetStatus(float _defaultStatus, float _statusRange) => (_defaultStatus + Random.Range(0f, _statusRange)) * (1f + GameManager.gameManager.upgradeValueDict[UpgradeEffectType.StatusUp]);
    public void FronApplicantToCharacter()
    {
    
    }
}
