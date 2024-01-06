using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class GUI_SelectedNode : MonoBehaviour
{
    public CreateMap adress;
    public List<NodeScriptPerLevel> _eventObjectList = new();
    public GUI_MapScenario values;
    public GUI_MapNodeInfo NodeInfoGUI;
    [SerializeField] internal List<HighLightGUI> myList;
    [SerializeField] internal State_NodeBtn state = new();

    public Material HighLightMaterial, subHighLightMaterial;
    public MapScenario.OnClickFunc _decideSelected_Func; // Progress 
    public float power; public float timer;
    
    public void _InitSelectedFunc(MapScenario.OnClickFunc target)
    {
        _decideSelected_Func = target;
    }

    public void OnLoadedInit()
    {
        this.SaveHighlight();
        Clear();
        state.StartGetInput();
    }

    public void _SelectEvent(int value)
    {
        if (!state.isTimeToInput)
            return;

        // progress?
        if (value == state.selectedNode)
        {
            _DecideSelected();
            return;
        }
        else if(-1 != state.selectedNode)
        {
            Clear();
            return;
        }

        Select(value);
    }

    public void CancelSelected()
    {
        if (!state.isTimeToInput)
        {
            Debug.Log("Not Now");
            return;
        }
        Clear();
    }

    public void _DecideSelected()
    {
        state.StopGetInput();

        if (state.selectedNode < 0)
        {
            Debug.Log("Tlqkf");
            return;
        }

        _decideSelected_Func(state.selectedNode);
    }

    public void Clear()
    {
        state.selectedNode = -1;
        for (int i = 0; i < myList.Count; i++)
        {
            myList[i].SetDefault();
        }
        myList.Clear();

        NodeInfoGUI.SetGUI_toDefault();
        return;
    }

    public void Select(int input)
    {
        List<SpriteRenderer> materials =  this.getDataToGUI_ByIndex(input);
     
        for (int i = 0; i < myList.Count; i++)
        {
            myList[i].SetDefault();
        }
        myList.Clear();
        StartCoroutine(HighLightWave());

        if (true)
        {
            Debug.Log("Select Event");
            this.getNodeData_ByIndex(input);
        }



        return;

        IEnumerator HighLightWave()
        {
            state.StopGetInput();
            for (int i = 0; i < materials.Count; i++)
            {
                myList.Add(new HighLightGUI(materials[i], HighLightMaterial));
            }

            state.selectedNode = input;

            float time = 0;
            while (false)
            {
                time += Time.deltaTime;
                if (time > timer)
                    break;

                HighLightMaterial.SetFloat("_Lighting", (time/timer));

                yield return new WaitForSeconds(0.01f);
            }
            time = timer;
            HighLightMaterial.SetFloat("_Lighting", 1);

            state.StartGetInput();
            yield return null;
        }
    }
}

[Serializable]
internal class HighLightGUI
{
    [SerializeField] SpriteRenderer changedSpriteRenderer;
    [SerializeField] Material originMaterial;

    internal HighLightGUI(SpriteRenderer render,Material highlightMaterial)
    {
        changedSpriteRenderer = render;
        originMaterial = changedSpriteRenderer.material;
        changedSpriteRenderer.material = highlightMaterial;
    }

    internal void SetDefault()
    {
        changedSpriteRenderer.material = originMaterial;
    }

    internal void SetMaterial(Material input)
    {
        changedSpriteRenderer.material = input;
    }
}

[Serializable]
internal class State_NodeBtn
{
    [SerializeField] internal bool isTimeToInput = true;
    [SerializeField] internal int selectedNode;

    internal void StopGetInput()
    {
        isTimeToInput = false;
    }
    internal void StartGetInput()
    {
        isTimeToInput = true;
    }
}