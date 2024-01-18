using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public SpriteRenderer EventNode, myPoint;
    public Sprite sillhouette, detail,point;
    public List<Sprite> myDetailData;
    public Color Active,TurnOff;
    public List<GameObject> objList_Silhouette;
    public List<GameObject> objList_MyRoad;


    public void SetSprite()
    {
        EventNode.sprite = detail;
        myPoint.sprite = sillhouette;
        transform.name = "Silhouette";
        for (int i = 0; i < objList_Silhouette.Count; i++)
        {
            objList_Silhouette[i].SetActive(false);
        }
    }

    public List<GameObject> objList_Detailed;


    public void ActiveDetailed()
    {
        myPoint.sprite = point;
        transform.name = "Detail";

        EventNode.sprite = detail;
        for (int i = 0; i < objList_Silhouette.Count; i++)
        {
            objList_Silhouette[i].SetActive(false);
        }

        for (int i = 0; i < objList_Detailed.Count; i++)
        {
            objList_Detailed[i].SetActive(true);
        }
    }

    public void TurnOffFunc()
    {
        transform.name = "Cut";
        EventNode.color = TurnOff;

        for (int i = 0; i < objList_Detailed.Count; i++)
        {
            objList_Detailed[i].SetActive(false);
        }

        for (int i = 0; i < objList_MyRoad.Count; i++)
        {
            objList_MyRoad[i].SetActive(false);
        }

        for (int i = 0; i < objList_Silhouette.Count; i++)
        {
            objList_Silhouette[i].SetActive(true);
        }
    }
}
