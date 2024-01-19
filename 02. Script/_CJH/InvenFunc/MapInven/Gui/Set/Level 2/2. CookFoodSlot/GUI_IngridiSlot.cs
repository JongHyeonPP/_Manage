using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GUI_IngridiSlot : MonoBehaviour
{
    public int value;

    public Image GUI_myImg;
    public Color GUI_DisableColor;
    public Sprite defaultImg;

    public Image GUI_EffectImg;
    public Color GUI_EffectColor;

    public void SetImg(Sprite targetImg, Color targetColor)
    {
        GUI_EffectImg.color = GUI_EffectColor;

        defaultImg = GUI_myImg.sprite;
        GUI_myImg.color = targetColor;
        GUI_myImg.sprite = targetImg;
    }

    public void SetDefault()
    {
        GUI_myImg.color = GUI_DisableColor;
        GUI_EffectImg.color = GUI_DisableColor;
        GUI_myImg.sprite = defaultImg;
        value = -1;
    }
}
