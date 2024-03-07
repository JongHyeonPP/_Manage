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
        // ���� ���콺�� ȭ�� ��ǥ�� �����ɴϴ�.
        Vector3 mousePosition = Input.mousePosition;

        // ���콺�� ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ�մϴ�.
        Vector3 worldPosition = cursorCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));

        // Ŀ�� ������Ʈ�� ��ġ�� ���콺 ��ġ�� ������Ʈ�մϴ�.
        transform.position = worldPosition;
    }
}