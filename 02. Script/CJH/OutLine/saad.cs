using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saad : MonoBehaviour
{
    public SpriteRenderer sr;
    public Material[] mList;

    private void Start()
    {
        sr.materials = mList;
    }
}
