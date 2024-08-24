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
        // 마우스 위치에서 Raycast를 수행합니다.
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        // 현재 씬에서 모든 GraphicRaycaster 찾기
        GraphicRaycaster[] raycasters = FindObjectsOfType<GraphicRaycaster>();

        RaycastResult? closestResult = null;

        // 모든 Canvas의 GraphicRaycaster에 대해 Raycast 수행
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
