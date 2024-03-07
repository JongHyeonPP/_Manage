using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    // Update is called once per frame
    public Camera cursorCamera;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(cursorCamera.gameObject);
        //Cursor.visible = false;
    }
    void Update()
    {
        // 현재 마우스의 화면 좌표를 가져옵니다.
        Vector3 mousePosition = Input.mousePosition;

        // 마우스의 화면 좌표를 월드 좌표로 변환합니다.
        Vector3 worldPosition = cursorCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));

        // 커서 오브젝트의 위치를 마우스 위치로 업데이트합니다.
        transform.position = worldPosition;
    }
}