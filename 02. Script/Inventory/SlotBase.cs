using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SlotBase : MonoBehaviour//Highlight에 관련된 기능
{
    public Image imageHighlight;
    protected Coroutine highlightCoroutine;
    private readonly float _highlightAlpha = 0.5f;
    private readonly float _highlightFadeDuration = 0.2f;
    private float _currentHLAlpha = 0f;
    private void Awake()
    {
        imageHighlight.color = new Color(
    imageHighlight.color.r,
    imageHighlight.color.g,
    imageHighlight.color.b,
    0f
);

    }
    private IEnumerator HighlightFadeInRoutine()
    {
        StopCoroutine(nameof(HighlightFadeOutRoutine));
        //imageHighlight.gameObject.SetActive(true);

        float unit = _highlightAlpha / _highlightFadeDuration;

        for (; _currentHLAlpha <= _highlightAlpha; _currentHLAlpha += unit * Time.deltaTime)
        {
            imageHighlight.color = new Color(
                imageHighlight.color.r,
                imageHighlight.color.g,
                imageHighlight.color.b,
                _currentHLAlpha
            );

            yield return null;
        }
    }

    private IEnumerator HighlightFadeOutRoutine()
    {
        StopCoroutine(nameof(HighlightFadeInRoutine));

        float unit = _highlightAlpha / _highlightFadeDuration;

        for (; _currentHLAlpha >= 0f; _currentHLAlpha -= unit * Time.deltaTime)
        {
            imageHighlight.color = new Color(
                imageHighlight.color.r,
                imageHighlight.color.g,
                imageHighlight.color.b,
                _currentHLAlpha
            );

            yield return null;
        }

        //imageHighlight.gameObject.SetActive(false);
    }


    public void HightlightOn()
    {
        if (ItemManager.itemManager.inventoryUi.draggingSlot != null || this)
            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
            }
        imageHighlight.gameObject.SetActive(true);
        highlightCoroutine = StartCoroutine(HighlightFadeInRoutine());
    }

    public void HightlightOff()
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }
        highlightCoroutine = StartCoroutine(HighlightFadeOutRoutine());
        ItemManager.itemManager.inventoryUi.targetInventorySlot = null;
    }
}
