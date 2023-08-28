using System;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class CreateStageScenario : MonoBehaviour
{
    public CreateMap createBackGroundSector;
    public CreateFloatingNode createNodeSector;
    public Transform focusingNode;
    public Vector2Int focusingGridPos;

    public Camera MapCamera;

    public List<TouchableNode> currSelectable;
     
    public List<EventNodeDataToPlace> TO_DEBUG;

    internal MapScenario.OnClickFunc onClick;
    public void SetOnClickFunc(MapScenario.OnClickFunc _onClick)
    {
        onClick = _onClick;
    }

    public void NewGame(int seed)
    {
        UnityEngine.Random.InitState(seed);
        this.SetInitData();

        BuildStem();
        BuildLeaf();

        Debug.Log((onClick == null) + " /");
        currSelectable.SettingNextDestination(createBackGroundSector, createNodeSector, onClick);

        this.SetVisualObject();
    }

    public void LoadGame(int seed,int[] history)
    {
        UnityEngine.Random.InitState(seed);
        this.SetInitData();

        BuildStem();
        BuildLeaf();

        for (int i = 0; i < history.Length; i++)
        {
            BuildLeaf(history[i]);
        }

        currSelectable.SettingNextDestination(createBackGroundSector, createNodeSector, onClick);
        this.SetVisualObject();
    }

    //rtn transPos
    public Vector3 ProgressMap(int inputChildIndex, ref GUI_MapScenario.ProgressMap_preInput task)
    {
        if (!createNodeSector.IsInit())
            return Vector3.one*-1;

        // 말단 생성 후, 해당 노드의 트랜스폼 갱신
        BuildLeaf(ref task, inputChildIndex);

        focusingNode = createBackGroundSector.GetFocusTransform();

        Vector3 rtnV3 = createBackGroundSector.getAxisX_CreatedNode();

        focusingGridPos = createNodeSector.getFocusGridPos();
        task += () => currSelectable.SettingNextDestination(createBackGroundSector, createNodeSector, onClick);

        return rtnV3;
    }

    public void BuildStem()
    {
        EventNodeDataToPlace treeData = createNodeSector.buildStem();
        TO_DEBUG.Add(treeData);

        createBackGroundSector.InitSettingEventPos(treeData , -1);
        createBackGroundSector.FillEnv(true);
    }

    public void BuildLeaf(int input = -1)
    {
        EventNodeDataToPlace treeData = createNodeSector.buildTree(input);
        TO_DEBUG.Add(treeData);

        createBackGroundSector.InitSettingEventPos(treeData, input);
        createBackGroundSector.FillEnv();
    }

    public void BuildLeaf(ref GUI_MapScenario.ProgressMap_preInput task, int input = -1)
    {
        EventNodeDataToPlace treeData = createNodeSector.buildTree(input);
        TO_DEBUG.Add(treeData);
        createBackGroundSector.InitSettingEventPos(treeData, input,ref task);
        task += () =>  createBackGroundSector.FillEnv(false);
    }
}

[Serializable]
public class EventNodeDataToPlace
{
    public Vector2Int focusGridPos;
    public int targetLevel;
    public List<int> nodeTreeData;
    public List<int> nodeEventData;
    public List<Vector3> nodeTerrainData;
}

