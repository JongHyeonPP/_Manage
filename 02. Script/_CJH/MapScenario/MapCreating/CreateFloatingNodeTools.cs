using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal static class CreateFloatingNodeTools
{
    internal delegate GameObject InstantiateFunc(GameObject _prefab);

    internal static int getRandomIndex(this CreateNodeValues values, int[] targetTable)
    {
        int index = Random.Range(0, 100);
        int a = 0;

        for (; a < targetTable.Length; a++)
        {
            if (targetTable[a] > index)
                break;
            else
                index -= targetTable[a];
        }

        return a;
    }

    internal static Vector3 getRnDmV3(this CreateNodeValues values)
    {
        return new Vector3(Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100)).normalized;
    }

    internal static int getChildCountIndex(this CreateNodeValues values)
    {
        return values.getRandomIndex(values.childCountTable);
    }

    internal static int getRandomEventIndex(this CreateNodeValues values)
    {
        return values.getRandomIndex(values.eventCoefTable);
    }

    internal static void ConnectRefToChild(List<Node> childs, List<Node> refNodes, int level)
    {
        int createLevel = childs[0].gridPos.x + 1;

        //Node focusNode = values.focusNode;

    }

    internal static List<int> GetCombination(int childs, int clone, int totalCount)
    {
      
        List<int> combination = new List<int>();

        for (int i = 0; i < childs; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < clone; j++)
                {
                    combination.Add(i);
                }
            }
            else if (i == childs-1)
            {
                for (int j = 0; j < clone; j++)
                {
                    combination.Add(i);
                }
            }else
                combination.Add(i);
        }

        while (combination.Count > totalCount)
        {
            int randomIndex = Random.Range(0, combination.Count);
            combination.RemoveAt(randomIndex);
        }

        List<int> result = new List<int>();

        for (int i = 0; i < childs; i++)
        {
            result.Add(0);
        }

        for (int i = 0; i < combination.Count; i++)
        {
            result[combination[i]]++;
        }

        //Debug.Log("TlqkfTlqkf - " + childs + " / " + totalCount);
        return result;
    }

    internal static void ShareChildNode(Node node, List<Node> addTarget)
    {
        if (addTarget.Count != node.myChilds.Count - 1)
            return;

        for (int i = 0; i < node.myChilds.Count - 1; i++)
        {
            ShareChildNode(node.myChilds[i], node.myChilds[i+1], addTarget[i]);
        }
    }

    internal static void ShareChildNode(Node node_L, Node node_R, Node addTarget)
    {
        node_L.shared_R = addTarget;
        node_L.myChilds.Insert(node_L.myChilds.Count, addTarget);

        node_R.shared_L = addTarget;
        node_R.myChilds.Insert(0, addTarget);
    }

    internal static void InitNodeSetting(this CreateNodeValues values)
    {
        values.instance = Object.Instantiate;

        //is nodeList is null?
        if (true)
        {
            if (values.levelList == null)
                values.levelList = new();
            else
                values.levelList.Clear();
        }

        //is rootnode is exist?
        if (true)
        {
            NodeSetPerLevel curr = new NodeSetPerLevel();
            
            if(curr.nodeList == null)
                curr.nodeList = new();

            curr.nodeList.Add(new Node(new Vector2Int(0, 0), -1)
            {
                terrainV3 = values.initValues.INIT_terrainV3.normalized,
                myEvent = -1
            });

            values.levelList.Add(curr);
        }

        for (int i = 1; i < values.maxLevel; i++)
        {
            values.AddLevelNodes();
        }

        return;
    }

    static void AddLevelNodes(this CreateNodeValues values)
    {
        int nextLevel = values.levelList.Count;

        if (values.levelList[nextLevel - 1].nodeList.Count == 1)
            AddLevelNodes_1(values);
        else
            AddLevelNodes_2(values);

        return; 

        void AddLevelNodes_1(CreateNodeValues values)
        {
            NodeSetPerLevel prev = values.levelList[values.levelList.Count - 1];
            NodeSetPerLevel curr = new(); curr.nodeList = new();
            curr.level = prev.level + 1;
            values.levelList.Add(curr);

            for (int i = 0; i < values.getRandomIndex(values.childCountTable); i++)
            {
                curr.nodeList.Add(new Node(new Vector2Int(curr.level, i))
                {
                    terrainV3 = values.initValues.INIT_terrainV3.normalized,
                    myEvent = 1
                });
            }
        }

        void AddLevelNodes_2(CreateNodeValues values)
        {
            NodeSetPerLevel prev = values.levelList[values.levelList.Count - 1];
            NodeSetPerLevel curr = new(); curr.nodeList = new();
            curr.level = prev.level + 1;
            values.levelList.Add(curr);

            for (int i = 0; i < values.getRandomIndex(values.nodesPerLevel); i++)
            {
                curr.nodeList.Add(new Node(new Vector2Int(curr.level, i))
                {
                    terrainV3 = values.getRnDmV3(),
                    myEvent = values.getRandomEventIndex()
                });
            }
        }
    }

    internal static void buildTree_Leaf(this CreateNodeValues values, Node focusingNode)
    {
        int createLevel = values.GetFocusNode().gridPos.x + 2;

        if (createLevel >= values.levelList.Count)
            return;

        NodeSetPerLevel nodeSet = values.levelList[createLevel];
        List<Node> targetChilds = focusingNode.myChilds;

        List<int> rtnList = GetCombination(targetChilds.Count, values.childCountTable.Length-1,
            nodeSet.nodeList.Count - (targetChilds.Count - 1));

        int childIndex = 0;
        for (int i = 0; i < targetChilds.Count; i++)
        {
            for (int j = 0; j < rtnList[i]; j++)
            {
                targetChilds[i].myChilds.Add(values.levelList[createLevel].nodeList[childIndex]);
                childIndex++;
            }

            if (i < targetChilds.Count - 1)
            {
                ShareChildNode(targetChilds[i], targetChilds[i + 1], values.levelList[createLevel].nodeList[childIndex]);
                childIndex++;
            }
        }
        focusingNode.connData_focus = rtnList;

        return;
    }

    internal static void buildTree_Stem(this CreateNodeValues values, Node focusingNode)
    {
        int createLevel = focusingNode.gridPos.x + 1;

        if (createLevel >= values.levelList.Count)
            return;

        for (int i = 0; i < values.levelList[createLevel].nodeList.Count; i++)
        {
            focusingNode.myChilds.Add(values.levelList[createLevel].nodeList[i]);
        }

        focusingNode.connData_focus = new()
        {
            focusingNode.myChilds.Count
        };
    }

    internal static Node GetFocusNode(this CreateNodeValues values)
    {
        return values.levelList[values.focusNodeGridPos.x].nodeList[values.focusNodeGridPos.y];
    }

    internal static void SetFocusNode(this CreateNodeValues values,Node targetNode)
    {
        Debug.Log(targetNode.gridPos + " - target");
        values.focusNodeGridPos = new Vector2Int(targetNode.gridPos.x, targetNode.gridPos.y);
        Debug.Log(values.focusNodeGridPos + " - after");
    }

    internal static Node GetRootNode(this CreateNodeValues values)
    {
        return values.levelList[0].nodeList[0];
    }
}
