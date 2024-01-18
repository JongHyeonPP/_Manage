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
    public GUI_ItemUnit _MySrc;

    private Vector2 DefaultPos;
    public Transform _parent;
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        _parent = transform.parent;
        transform.parent = transform.root;

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
        myImg.raycastTarget = true;

        if (temp)
        {
            DefaultPos = Vector3.zero;
            _parent = null;

            temp.OnDragDrop(this);            
        }
        else
        {
            Debug.Log("sad");
            transform.position = DefaultPos;
            transform.parent = _parent;

            DefaultPos = Vector3.zero;
            _parent = null;
        }


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