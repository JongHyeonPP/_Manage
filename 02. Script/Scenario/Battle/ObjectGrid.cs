using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class ObjectGrid : MonoBehaviour
{
    public CharacterBase owner = null;
    public int index;
    public bool isEnemy;
    EventTrigger eventTrigger;
    public delegate void PassiveEffectHandler(CharacterBase _target);
    public PassiveEffectHandler EnterOnGrid;
    public PassiveEffectHandler ExitOnGrid;
    public ObjectGrid SetClickEvent()
    {
        Entry downEntry = new();
        // Button 이벤트 추가
        gameObject.AddComponent<Button>().onClick.AddListener(() =>
        {
            OnGridClicked();
        });
        
        return this;
    }
    public ObjectGrid SetDownEvent()
    {
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        Entry downEntry = new();
        eventTrigger.triggers.Add(downEntry);
        downEntry.eventID = EventTriggerType.PointerDown;
        downEntry.callback.AddListener((data) =>
        {
            OnGridPointerDown();
        });
        return this;
    }
    public ObjectGrid SetEnterEvent()
    {
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        Entry enterEntry = new();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) =>
        {
            OnGridPointerEnter(this);
        });
        eventTrigger.triggers.Add(enterEntry);
        return this;
    }
    public ObjectGrid SetDragEvent()
    {
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        Entry dragEntry = new();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) =>
        {
            // 현재 드래그되고 있는 포인트의 위치 가져오기
            PointerEventData pointerEventData = (PointerEventData)data;
            Vector2 dragPosition = pointerEventData.position;

            OnGridPointerDrag(dragPosition);
        });
        eventTrigger.triggers.Add(dragEntry);
        return this;
    }
    public ObjectGrid SetExitEvent()
    {
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        Entry exitEntry = new();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) =>
        {
            OnGridPointerExit();
        });
        eventTrigger.triggers.Add(exitEntry);
        return this;
    }
    public ObjectGrid SetUpEvent()
    {
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        Entry upEntry = new();
        upEntry.eventID = EventTriggerType.PointerUp;
        upEntry.callback.AddListener((data) =>
        {
            OnGridPointerUp();
        });
        eventTrigger.triggers.Add(upEntry);
        return this;
    }
    internal void OnGridClicked()
    {
    }

    internal void OnGridPointerDown()
    {
        if (!owner) return;
        
        GameManager.battleScenario.OnGridPointerDown(this);
    }

    internal void OnGridPointerDrag(Vector2 _dragPosition)
    {
        if (!GameManager.battleScenario.isInFriendly) return;
        if (!owner) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
    GetComponent<RectTransform>(),
    _dragPosition,
    GameManager.gameManager.canvasGrid.GetComponent<Canvas>().worldCamera,
    out var localPoint
);
        owner.transform.localPosition = localPoint;
    }

    internal void OnGridPointerEnter(ObjectGrid _grid)
    {
        if (!GameManager.battleScenario.isDragging) return;
        GameManager.battleScenario.gridOnPointer = _grid;
    }

    internal void OnGridPointerExit() => GameManager.battleScenario.gridOnPointer = null;
    internal void OnGridPointerUp()
    {
        if (!owner) return;
        if (!GameManager.battleScenario.isDragging) return;
        if (!GameManager.battleScenario.gridOnPointer || isEnemy != GameManager.battleScenario.gridOnPointer.isEnemy)
            GameManager.battleScenario.MoveCharacterByGrid(this, this);
        else
        {

            GameManager.battleScenario.MoveCharacterByGrid(this, GameManager.battleScenario.gridOnPointer);
        }
        GameManager.battleScenario.gridOnPointer = null;
        GameManager.battleScenario.isDragging = false;
        Time.timeScale = 1f;
    }
}
