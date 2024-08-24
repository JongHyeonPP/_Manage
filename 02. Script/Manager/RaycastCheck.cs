using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class RaycastCheck : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckUIRaycast();
        }
    }

    void CheckUIRaycast()
    {
        // ���콺 ��ġ���� Raycast�� �����մϴ�.
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        // ���� ������ ��� GraphicRaycaster ã��
        GraphicRaycaster[] raycasters = FindObjectsOfType<GraphicRaycaster>();

        RaycastResult? closestResult = null;

        // ��� Canvas�� GraphicRaycaster�� ���� Raycast ����
        foreach (var raycaster in raycasters)
        {
            raycaster.Raycast(pointerData, results);

            foreach (var result in results)
            {
                if (closestResult == null || result.depth < closestResult.Value.depth)
                {
                    closestResult = result;
                }
            }
        }

        if (closestResult.HasValue)
        {
            Debug.Log("Raycast hit closest UI element: " + closestResult.Value.gameObject.name);
        }
        else
        {
            Debug.Log("No UI element was hit by the Raycast.");
        }
    }
}
