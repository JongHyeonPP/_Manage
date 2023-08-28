using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropObj : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Image myImg;
    public Vector2 DefaultPos;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        DefaultPos = transform.position;
        myImg.raycastTarget = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 currentPos = eventData.position;
        transform.position = currentPos;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        ResponedByDrop temp = UIRaycast(eventData);

        if (temp)
        {
            temp.OnDragDrop(this);            
        }
        else
            Debug.Log("sad");

        transform.position = DefaultPos;
        myImg.raycastTarget = true;

        ResponedByDrop UIRaycast(PointerEventData pointerData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            if (results.Count < 1)
            {
                Debug.Log("Tlqkf");
                return null;
            }
            else
            {
                results[0].gameObject.transform.name = "ttq";
                return results[0].gameObject.GetComponent<ResponedByDrop>();
            }
        }
    }

}