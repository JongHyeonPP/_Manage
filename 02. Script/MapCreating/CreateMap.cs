using System;
using System.Collections.Generic;
using UnityEngine;
using static CreateMapTools;


public class CreateMap : MonoBehaviour
{
    public Material NodeActive, NodeDisable;
    [SerializeField] internal CreateMapBackgroundValues createMapBackgroundValues;
    [SerializeField] private CreateMapEventValues createMapEventValues;
    [SerializeField] private CreateRoadValues createRoadValues;
    //[SerializeField] private CaptureScreenshotValue captureScreenshotValues;
    public List<NodeScriptPerLevel> eventObjectList = new();
    public Vector2Int focusingNode;

    public void InitSettingEventPos_Root(EventNodeDataToPlace treeData)
    {
        NodeScriptPerLevel rootLevel = eventObjectList.tryAddLevel(0);
        rootLevel.spriteList = createMapEventValues.setEventArea_Root(rootLevel.transformList);
        rootLevel.nodeTerrainData.Add(treeData.nodeTerrainData[0]);
        return;
    }

    public void InitSettingEventPos(EventNodeDataToPlace treeData, int index)
    {
        int aryIndex;
        if (index == -1)
            aryIndex = 0;
        else
            aryIndex = index;

        // TODO : 지도 폭 입력받기
        int level = treeData.targetLevel + treeData.focusGridPos.x;
        int std = (int)(eventObjectList.getChildRangeByGridPos(new Vector2Int(treeData.focusGridPos.x, aryIndex)).x);
        
        focusingNode = treeData.focusGridPos;
        eventObjectList[focusingNode.x].childStd = std;

        Vector2 sum = GetFocusTransform().position;
        sum.x = createMapEventValues.GetChildSumOfAxis(eventObjectList, focusingNode, aryIndex);

        List<Vector2> createList = createMapEventValues.GetEventPointRow(treeData.nodeTreeData, level, 
            sum + createMapEventValues.stdPos);

        NodeScriptPerLevel currLevel = eventObjectList[focusingNode.x];
        NodeScriptPerLevel nextLevel = eventObjectList[level - 1];
        NodeScriptPerLevel createdLevel = eventObjectList.tryAddLevel(level);

        createdLevel.spriteList = createMapEventValues.setEventArea(createList, createdLevel.transformList, treeData.nodeEventData, index == -1);
        createdLevel.nodeTerrainData = treeData.nodeTerrainData;
        nextLevel.connectingData.setConnectedData(treeData.nodeTreeData);

        // build Road, ( need connectData => nextLevel.connData )
        createRoadValues.setRoadAreaSetBySet(nextLevel, createdLevel, currLevel.childStd);
        createMapBackgroundValues.setRoadSpriteByData(createdLevel);

        return;
    }

    public void InitSettingEventPos(EventNodeDataToPlace treeData, int index, ref GUI_MapScenario.ProgressMap_preInput task)
    {
        int aryIndex;
        if (index == -1)
            aryIndex = 0;
        else
            aryIndex = index;

        // TODO : 지도 폭 입력받기
        int level = treeData.targetLevel + treeData.focusGridPos.x;
        int std = (int)(eventObjectList.getChildRangeByGridPos(new Vector2Int(treeData.focusGridPos.x, aryIndex)).x);

        focusingNode = treeData.focusGridPos;
        eventObjectList[focusingNode.x].childStd = std;

        Vector2 sum = GetFocusTransform().position;
        sum.x = createMapEventValues.GetChildSumOfAxis(eventObjectList, focusingNode, aryIndex);

        List<Vector2> createList = createMapEventValues.GetEventPointRow(treeData.nodeTreeData, level,
            sum + createMapEventValues.stdPos);

        NodeScriptPerLevel currLevel = eventObjectList[focusingNode.x];
        NodeScriptPerLevel nextLevel = eventObjectList[level - 1];
        NodeScriptPerLevel createdLevel = eventObjectList.tryAddLevel(level);

        createdLevel.spriteList = createMapEventValues.setEventArea(createList, createdLevel.transformList, treeData.nodeEventData, index == -1);
        {
            foreach (var item in createdLevel.spriteList)
            {
                item.gameObject.SetActive(false);
            }
        };

        createdLevel.nodeTerrainData = treeData.nodeTerrainData;
        nextLevel.connectingData.setConnectedData(treeData.nodeTreeData);
        task += () =>
        {
            foreach (var item in createdLevel.spriteList)
            {
                item.gameObject.SetActive(true);
            }  
        };

        // build Road, ( need connectData => nextLevel.connData )
        task += () => createRoadValues.setRoadAreaSetBySet(nextLevel, createdLevel, currLevel.childStd);
        task += () => createMapBackgroundValues.setRoadSpriteByData(createdLevel);

        return;
    }


    public Vector2 getAxisX_CreatedNode()
    {
        NodeScriptPerLevel createdLevel = eventObjectList[eventObjectList.Count - 1];

        float sumX = 0;
        int i = 0;
        for (; i < createdLevel.transformList.Count; i++)
        {
            sumX += createdLevel.transformList[i].position.x;
        }
        sumX /= i;

        return new Vector2(sumX, GetFocusTransform().position.y);
    }

    //List<TouchableNode>
    public Transform GetNodeTransformByGrid(Vector2Int gridPos)
    {
        return eventObjectList.getTransformByGridPos(gridPos);
    }

    public Transform GetFocusTransform()
    {
        return eventObjectList.getTransformByGridPos(focusingNode);
    }

    public void FillEnv(bool isRoot = false)
    {
        float coex_SIZE = 1.6f;

        if (isRoot)
            FillBG_OBJ_Root();
        else
            FillBG_OBJ(eventObjectList.Count - 2);

        List<SpriteRenderer> FillBG_OBJ(int level)
        {
            if (level < 1)
                return null;

            NodeScriptPerLevel currLayer = eventObjectList[level];
            NodeScriptPerLevel nextLayer = eventObjectList[level+1];

            List<SpriteRenderer> events = currLayer.spriteList;
            List<SpriteRenderer> rtn = new();


            Vector2 range_Curr = eventObjectList.getChildRangeByGridPos_FocusStd(focusingNode);

            for (int i = (int)range_Curr.x; i < range_Curr.y + 1; i++)
            {
                List<SpriteRenderer> instantBG = createMapBackgroundValues.setBackgroundSprites(events[i].transform.position, terrainDataToIndex(currLayer.nodeTerrainData[i]));
                List<SpriteRenderer> trashBin = new();
                
                events[i].SetNodeMaterialActive(NodeActive);

                // 이벤트 적용(원형 삭제)
                DisableIntersectingSprites(instantBG, trashBin, events[i], createMapEventValues.ereaseScale);

                //길 적용, focus - next
                DisableIntersectingSprites(instantBG, trashBin, currLayer.FindToDes(i), createRoadValues.ereaseScale);

                //길 적용, next - supers
                for (int j = 0; j < nextLayer.roadSetList.Count; j++)
                {
                    DisableIntersectingSprites(instantBG, trashBin, nextLayer.roadSetList[j].roadSet, createRoadValues.ereaseScale);
                }

                foreach (SpriteRenderer item in trashBin)
                {
                    Destroy(item.gameObject);
                }
            }

            return rtn;
        }

        List<SpriteRenderer> FillBG_OBJ_Root()
        {
            NodeScriptPerLevel currLayer = eventObjectList[0];
            NodeScriptPerLevel nextLayer = eventObjectList[1];

            List<SpriteRenderer> events = currLayer.spriteList;
            List<SpriteRenderer> rtn = new();

            Vector2 range_Curr = eventObjectList.getChildRangeByGridPos(focusingNode);
            //Debug.Log(focusingNode + " child range : " + range_Curr + " / " + currLayer.spriteList.Count);

            Debug.Log(events[0].transform.position);
            List<SpriteRenderer> instantBG = createMapBackgroundValues.setBackgroundSprites(events[0].transform.position,0);
            List<SpriteRenderer> _trashbin = new();

            events[0].SetNodeMaterialActive(NodeActive);

            DisableIntersectingSprites(instantBG, _trashbin, events[0], createMapEventValues.ereaseScale);

            //길 적용, next - super
            for (int j = 0; j < nextLayer.roadSetList.Count; j++)
            {
                DisableIntersectingSprites(instantBG, _trashbin, nextLayer.roadSetList[j].roadSet, createRoadValues.ereaseScale);
            }

            foreach (SpriteRenderer item in _trashbin)
            {
                Destroy(item.gameObject);
            }

            return rtn;
        }
    }

    /*
        1. 배경 오브젝트 생성

        2. 전방의 길 제거

        3. 후방의 길 제거

        4. 캡쳐 및 배치

        5. 오브젝트 제거
     */
    private void DisableIntersectingSprites(List<SpriteRenderer> backgroundSprites, List<SpriteRenderer> trashbin, SpriteRenderer emptyZones, float radio)
    {
        if (emptyZones == null)
            return;

        for (int i = 0; i < backgroundSprites.Count; i++)
        {
            float temp = Vector2.Distance(backgroundSprites[i].transform.position, emptyZones.transform.position);
            backgroundSprites[i].transform.name = "sad - " + i + " / " + temp;
            if (temp < radio)
            {
                backgroundSprites[i].gameObject.SetActive(false);

                trashbin.Add(backgroundSprites[i]);
                backgroundSprites.RemoveAt(i);

                i--;
                continue;
            }
        }

        return;
    }

    //Road
    private void DisableIntersectingSprites(List<SpriteRenderer> backgroundSprites, List<SpriteRenderer> trashbin, List<SpriteRenderer> emptyZones, float radio)
    {
        if (emptyZones == null)
            return;

        for (int j = 0; j < backgroundSprites.Count; j++)
        {
            float min = 0;
            for (int i = 0; i < emptyZones.Count; i++)
            {
                float temp = Vector2.Distance(emptyZones[i].transform.position, backgroundSprites[j].transform.position);
                if(min != 0)
                    min = Mathf.Min(temp, min);
                else
                    min = temp;
            }

            backgroundSprites[j].transform.name =  "checked - " + min;

            if (min < radio)
            {
                backgroundSprites[j].transform.name = "sad";
                backgroundSprites[j].gameObject.SetActive(false);
                trashbin.Add(backgroundSprites[j]);
                backgroundSprites.RemoveAt(j);
                j--;
                continue;
            }

        }

        return;
    }
}

[Serializable]
public struct CreateMapBackgroundValues 
{
    public GameObject[] bgPrefabList;
    public Material mapMaterial;
    public Transform parentTransform;
    
    public int countX;
    public int countY;

    public float coefX;
    public float coefY;

    public BoxPointer backgroundArea;
    
}

[Serializable]
public struct CreateMapEventValues
{
    public List<GameObject> eventObject;
    
    public Transform parentTransform;

    public int gapPerLevel;
    public int gapPerEvent;
    public Vector2 radious;

    public float ereaseScale;
    public BoxPointer eventArea;
    public Vector2 stdPos;
}

[Serializable]
public struct CreateRoadValues
{
    public GameObject roadObject;

    public Transform parentTransform;
    
    public float gap;
    public float coef;

    public float ignoreRange;
    public float ereaseScale;
    public bool isDebug_RoadVis;
}

[Serializable]
public class NodeScriptPerLevel
{
    public int level;
    public int childStd = 0;
    public List<Transform> transformList = new();
    public List<Vector2> connectingData = new();
    public List<Vector3> nodeTerrainData = new();
    public List<SpriteRenderer> spriteList = new();
    public List<RoadSet> roadSetList = new();

    public NodeScriptPerLevel(int _level , int childStd = 0)
    {
        level = _level;

        if (transformList == null)
            transformList = new();

        if (spriteList == null)
            spriteList = new();
    }

    public List<SpriteRenderer> FindToDes(int des)
    {
        List<SpriteRenderer> rtnList = new();
        for (int i = 0; i < roadSetList.Count; i++)
        {
            if (roadSetList[i].nameV3.z == des)
                rtnList.AddRange(roadSetList[i].roadSet);
        }

        return rtnList;
    }
}

[Serializable]
public class RoadSet
{
    public Vector3Int nameV3;
    public Vector3 srcTerrain;
    public Vector3 desTerrain;
    public List<SpriteRenderer> roadSet = new();
}


[Serializable]
public class BoxPointer
{
    public float length_X, length_Y; 
    public float getLength_X()
    {
        return length_X;
    }

    public float getLength_Y()
    {
        return length_Y;
    }

    public float getRandomHeight(float coex = 0f)
    {
        float temp = 1 - coex * coex;
        return getLength_Y() * (temp - 0.7f);
    }
}

[Serializable]
public class CaptureScreenshotValue
{
    public SpriteRenderer prefab_Sub;
    public SpriteRenderer prefab;
    public int stdSize;
    public int captureWidth = 1000;
    public int captureHeight = 1000;
    public Transform partentObj;
    public Camera captureCam;
    public Camera mapCam;
}