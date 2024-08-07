using EnumCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class GridObject : MonoBehaviour
{
    public BaseInBattle owner = null;
    public int index;
    public bool isEnemy;
    public Image imageRect { get; private set; }
    public int preStack;
    public Image imagePre;
    public void InitObject()
    {
        imageRect = transform.GetChild(0).GetComponent<Image>();
        preStack = 0;
    }
    public void PreActive()
    {
        preStack++;
        imagePre.enabled = true;
    }
    public void PreInactive()
    {
        preStack--;
        if (preStack <= 0)
            imagePre.enabled = false;
    }
    public void RefreshGrid()
    {
        preStack = 0;
        imagePre.enabled = false;
    }

    public void OnGridPointerDown()
    {
        if (!owner) return;
        if (BattleScenario.battlePatern == BattlePatern.Battle)
        {
            if (GameManager.battleScenario.moveGauge < 10f) return;
        }
        GameManager.battleScenario.isDragging = true;
        GameManager.IsPaused = true;
    }

    public void OnGridPointerDrag()
    {
        if (!owner) return;
        if (!GameManager.battleScenario.isDragging) return;
        Vector2 mousePosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
    GetComponent<RectTransform>(),
    mousePosition,
    GameManager.gameManager.canvasGrid.GetComponent<Canvas>().worldCamera,
    out var worldPoint
);
        if (!isEnemy)
        {
            RectTransform parentCharacter = GameManager.gameManager.parentCharacter.GetComponent<RectTransform>();

            Vector3 worldScale = parentCharacter.lossyScale;
            float halfWidth = (parentCharacter.sizeDelta.x * worldScale.x) / 2;
            float halfHeight = (parentCharacter.sizeDelta.y * worldScale.y) / 2;

            // min/max 범위 계산
            float minX = parentCharacter.position.x - halfWidth;
            float maxX = parentCharacter.position.x + halfWidth;
            float minY = parentCharacter.position.y - halfHeight;
            float maxY = parentCharacter.position.y + halfHeight;

            // worldPoint의 x, y 좌표 클램프
            worldPoint.x = Mathf.Clamp(worldPoint.x, minX, maxX);
            worldPoint.y = Mathf.Clamp(worldPoint.y, minY, maxY);
        }
        owner.transform.position = worldPoint;
    }

    public void OnGridPointerEnter()
    {
        if (GameManager.battleScenario.isDragging&&!isEnemy)
        {
            GameManager.battleScenario.gridOnPointer = this;
            imageRect.enabled = true;
        }
        else
        {
            if (owner && BattleScenario.battlePatern == BattlePatern.OnReady)
            {
                StatusExplain_Battle statusExplain = GameManager.battleScenario.statusExplain;
                statusExplain.gameObject.SetActive(true);
                statusExplain.transform.SetParent(transform);
                statusExplain.transform.localPosition = new Vector3(0f, 145f);
                statusExplain.SetExplain(owner);
            }
        }
    }

    public void OnGridPointerExit()
    {
        GameManager.battleScenario.statusExplain.gameObject.SetActive(false);
        GameManager.battleScenario.gridOnPointer = null;
        imageRect.enabled = false;
    }
    public void OnGridPointerUp()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (!owner || !GameManager.battleScenario.isDragging) return;
        foreach(GridObject x in BattleScenario.CharacterGrids)
            x.imageRect.enabled = false;
        if (GameManager.battleScenario.gridOnPointer == null || GameManager.battleScenario.gridOnPointer == this|| !GameManager.battleScenario.gridOnPointer || isEnemy != GameManager.battleScenario.gridOnPointer.isEnemy)
        {
            GameManager.battleScenario.MoveCharacterByGrid(this, this);//원 위치 복귀
        }
        else
        {
            GameManager.battleScenario.MoveCharacterByGrid(this, GameManager.battleScenario.gridOnPointer);

            if (BattleScenario.battlePatern != BattlePatern.OnReady)
            {
                GameManager.battleScenario.moveGauge = 0f;
                GameManager.battleScenario.PassiveReconnect();
            }
        }

        GameManager.battleScenario.gridOnPointer = null;
        GameManager.battleScenario.isDragging = false;
        GameManager.IsPaused = false;
    }
}
