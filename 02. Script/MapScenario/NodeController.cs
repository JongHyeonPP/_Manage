using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public SpriteRenderer EventNode;
    public Color Active,TurnOff;
    public List<GameObject> objList_Silhouette;
    public void ActiveSilhouette()
    {
        //transform.name = "sad";
        for (int i = 0; i < objList_Silhouette.Count; i++)
        {
            objList_Silhouette[i].SetActive(true);
        }
    }

    public List<GameObject> objList_Detailed;
    public void ActiveDetailed()
    {
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
        transform.name = "sad";
        EventNode.color = TurnOff;


        for (int i = 0; i < objList_Detailed.Count; i++)
        {
            objList_Detailed[i].SetActive(false);
        }

        for (int i = 0; i < objList_Silhouette.Count; i++)
        {
            objList_Silhouette[i].SetActive(true);
        }
    }
}
