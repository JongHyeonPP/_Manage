using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

internal static class CreateStageScenarioTools
{
    internal static void SetInitData(this CreateStageScenario createSC)
    {
        EventNodeDataToPlace temp = createSC.createNodeSector.initTree();
        if (temp == null)
        {
            Debug.Log("fucking double init");
            return;
        }
        createSC.createBackGroundSector.InitSettingEventPos_Root(temp);
    }

    internal static void SetVisualObject(this CreateStageScenario createSC)
    {
        //DEBUG
        createSC.focusingNode = createSC.createBackGroundSector.GetFocusTransform();
        createSC.focusingGridPos = createSC.createNodeSector.getFocusGridPos();

        Transform focusingNode = createSC.createBackGroundSector.GetFocusTransform();
        Debug.Log("pos - " + focusingNode.position);

        createSC.MapCamera.transform.position += new Vector3(
                focusingNode.position.x, focusingNode.position.y, focusingNode.position.y);
    }

    internal static void SettingNextDestination(this List<TouchableNode> currSelectable, CreateMap createBackGroundSector, CreateFloatingNode createNodeSector, MapScenario.OnClickFunc onClick)
    {
        for (int i = 0; i < currSelectable.Count; i++)
        {
            Transform targetTrans = currSelectable[i].transform;
            if(targetTrans != createBackGroundSector.GetFocusTransform())
                targetTrans.GetChild(0).GetComponent<NodeController>().TurnOffFunc();


            currSelectable[i].Destroy();
        }

        if (currSelectable.Count > 0)
            currSelectable.Clear();

        List<Vector2Int> temp = createNodeSector.getNextChilds();
        for (int i = 0; i < temp.Count; i++)
        {
            Transform tempTrans = createBackGroundSector.GetNodeTransformByGrid(temp[i]);
            currSelectable.Add(tempTrans.AddComponent<TouchableNode>());
            currSelectable[i].Setting(i, onClick);
        }

        if (onClick == null)
            Debug.Log("DEBUG CST - ");
    }
}