using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TouchableNode : MonoBehaviour
{
    //public SpriteRenderer spriteRenderer;
    public BoxCollider2D coll;
    //public Vector3 pos3
    MapScenario.OnClickFunc onClick;
    int index;

    public void Setting(int _index, MapScenario.OnClickFunc _onClick)
    {
        coll = transform.AddComponent<BoxCollider2D>();
        //spriteRenderer = transform.GetComponent<SpriteRenderer>();
        onClick = _onClick;
        index = _index;
    }

    private void OnMouseUp()
    {
        Debug.Log("My index = " + index + " / " + onClick == null);
        onClick(index);
    }


    public void Destroy()
    {
        Destroy(coll);
        Destroy(this);
    }
}
