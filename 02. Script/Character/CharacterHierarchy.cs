using BattleCollection;
using EnumCollection;
using ItemCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHierarchy : MonoBehaviour
{
    private Color hairColor;
    [Header("Body")]
    [SerializeField] private SpriteRenderer hairRenderer;
    [SerializeField] private SpriteRenderer faceHairRenderer;
    [SerializeField] private SpriteRenderer headRenderer;
    [SerializeField] private SpriteRenderer[] eyesFrontRenderer;
    [SerializeField] private SpriteRenderer[] eyesBackRenderer;
    [SerializeField] private SpriteRenderer armLRenderer;
    [SerializeField] private SpriteRenderer armRRenderer;
    [Header("Weapon")]
    [SerializeField] private SpriteRenderer weaponRenderer;
    [Header("Clothes")]
    [SerializeField] private SpriteRenderer backRenderer;
    [SerializeField] private SpriteRenderer clothBodyRenderer;
    [SerializeField] private SpriteRenderer clothLeftRenderer;
    [SerializeField] private SpriteRenderer clothRightRenderer;
    [SerializeField] private SpriteRenderer armorBodyRenderer;
    [SerializeField] private SpriteRenderer armorRightRenderer;
    [SerializeField] private SpriteRenderer armorLeftRenderer;
    [SerializeField] private SpriteRenderer helmetRenderer;
    [SerializeField] private SpriteRenderer footRightRenderer;
    [SerializeField] private SpriteRenderer footLeftRenderer;

    public void SetBodySprite(Sprite _hair, Sprite _faceHair, Sprite _eyeFront, Sprite _eyeBack, Sprite _head, Sprite _armL, Sprite _armR,  Sprite _weapon,Color _hairColor)
    {
        hairColor = _hairColor;
        hairRenderer.sprite = _hair;
        hairRenderer.color = hairColor;
        faceHairRenderer.sprite = _faceHair;
        faceHairRenderer.color = hairColor;
        if (_head != null)
            headRenderer.sprite = _head;
        foreach (SpriteRenderer renderer in eyesFrontRenderer)
        {
            if (_eyeFront != null)
                renderer.sprite = _eyeFront;
        }
        foreach (SpriteRenderer renderer in eyesBackRenderer)
        {
            if (_eyeBack != null)
                renderer.sprite = _eyeBack;
        }
        if (_armL != null)
            armLRenderer.sprite = _armL;
        if (_armR != null)
            armRRenderer.sprite = _armR;
        weaponRenderer.sprite = _weapon;
    }

    public void SetWeaponSprite(WeaponClass _weapon)
    {
        weaponRenderer.sprite = _weapon.sprite;
    }
    public void SetJob(string _jobId)
    {
        hairRenderer.enabled = false;
        Dictionary<ClothesPart, Sprite> clothesDictPiece = LoadManager.loadManager.jobsDict[_jobId].spriteDict;
        ClothesSet(backRenderer,  ClothesPart.Back);
        ClothesSet(clothBodyRenderer, ClothesPart.ClothBody);
        ClothesSet(clothLeftRenderer,  ClothesPart.ClothLeft);
        ClothesSet(clothRightRenderer,  ClothesPart.ClothRight);
        ClothesSet(armorBodyRenderer,  ClothesPart.ArmorBody);
        ClothesSet(armorRightRenderer,  ClothesPart.ArmorRight);
        ClothesSet(armorLeftRenderer,  ClothesPart.ArmorLeft);
        ClothesSet(helmetRenderer,  ClothesPart.Helmet);
        ClothesSet(footRightRenderer,  ClothesPart.FootRight);
        ClothesSet(footLeftRenderer,  ClothesPart.FootLeft);
        void ClothesSet(SpriteRenderer _spriteRenderer, ClothesPart _clothesPart)
        {
            if (clothesDictPiece.ContainsKey(_clothesPart))
                _spriteRenderer.sprite = clothesDictPiece[_clothesPart];
            else
                _spriteRenderer.sprite = null;
        }
    }
    public void CopyHierarchySprite(CharacterHierarchy _origin)
    {
        hairRenderer.enabled = _origin.hairRenderer.enabled;
        List<Sprite> sprites = _origin.GetHierarchySprite(out Color _hairColor);
        hairColor = _hairColor;
        hairRenderer.sprite = sprites[0];
        faceHairRenderer.sprite = sprites[1];
        headRenderer.sprite = sprites[2];
        foreach (var x in eyesFrontRenderer)
        {
            x.sprite = sprites[3];
        }
        foreach (var x in eyesBackRenderer)
        {
            x.sprite = sprites[4];
        }
        armLRenderer.sprite = sprites[5];
        armLRenderer.sprite = sprites[6];
        weaponRenderer.sprite = sprites[7];
        backRenderer.sprite = sprites[8];
        clothBodyRenderer.sprite = sprites[9];
        clothLeftRenderer.sprite = sprites[10];
        clothRightRenderer.sprite = sprites[11];
        armorBodyRenderer.sprite = sprites[12];
        armorRightRenderer.sprite = sprites[13];
        armorLeftRenderer.sprite = sprites[14];
        helmetRenderer.sprite = sprites[15];
        footRightRenderer.sprite = sprites[16];
        footLeftRenderer.sprite = sprites[17];
        hairRenderer.color = hairColor;
        faceHairRenderer.color = hairColor;
    }
    public List<Sprite> GetHierarchySprite(out Color _hairColor)
    {
        List<Sprite> returnValue = new();
        returnValue.AddRange(new Sprite[] {
        hairRenderer.sprite,
        faceHairRenderer.sprite,
        headRenderer.sprite,
        eyesFrontRenderer[0].sprite,
        eyesBackRenderer[0].sprite,
        armLRenderer.sprite,
        armRRenderer.sprite,
        weaponRenderer.sprite,
        backRenderer.sprite,
        clothBodyRenderer.sprite,
        clothLeftRenderer.sprite,
        clothRightRenderer.sprite,
        armorBodyRenderer.sprite,
        armorRightRenderer.sprite,
        armorLeftRenderer.sprite,
        helmetRenderer.sprite,
        footRightRenderer.sprite,
        footLeftRenderer.sprite
        });
        _hairColor = hairColor;
        return returnValue;
    }
}