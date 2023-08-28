using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponedByDrop1 : MonoBehaviour
{
    public virtual void OnDragDrop(DragDropObj temp)
    {
        Debug.Log("1. Responed object : " + gameObject.name);
        Debug.Log("2. Dropped object : " + temp.gameObject.name);
    }
}
