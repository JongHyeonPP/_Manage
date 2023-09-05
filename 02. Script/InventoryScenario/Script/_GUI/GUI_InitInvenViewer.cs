using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_InitInvenViewer : MonoBehaviour
{
    public RectTransform rTrans;
    public GridLayoutGroup viewer;
    void Start()
    {
        int count = viewer.constraintCount;
        float width = (rTrans.offsetMax.x - rTrans.offsetMin.x);

        float gap = viewer.spacing.x;
        float item = viewer.cellSize.x;

        float coef = width / (item * count + gap * (count - 1));
        //Debug.Log(width + " / " + ((item * count + gap * (count - 1))));
        viewer.spacing *= coef;
        viewer.cellSize *= coef;
    }
}
