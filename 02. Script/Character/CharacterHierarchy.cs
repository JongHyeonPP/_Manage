using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHierarchy : MonoBehaviour
{
    public GameObject shadow;
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
    //[Header("Clothes")]

    public void SetBodySprite(Sprite _hair, Sprite _faceHair, Sprite _eyeFront, Sprite _eyeBack, Sprite _head, Sprite _armL, Sprite _armR, Color _hairColor)
    {
        hairRenderer.sprite = _hair;
        hairRenderer.color = _hairColor;
        faceHairRenderer.sprite = _faceHair;
        faceHairRenderer.color = _hairColor;
        headRenderer.sprite = _head;
        foreach (SpriteRenderer renderer in eyesFrontRenderer)
        {
            renderer.sprite = _eyeFront;
        }
        foreach (SpriteRenderer renderer in eyesBackRenderer)
        {
            renderer.sprite = _eyeBack;
        }
        armLRenderer.sprite = _armL;
        armRRenderer.sprite = _armR;
    }
}