using UnityEngine;
using System.Collections;

public class LobbySky : MonoBehaviour
{
    // ���� ������ ������
    private static float speed = 10.0f; // �ӵ�
    private static float threshold = 2000.0f; // �Ӱ谪
    private static float offsetValue = -5031.0f; // X ��ǥ�� �Ӱ谪�� �Ѿ��� �� ���� ��

    private Coroutine moveCoroutine;

    void Start()
    {
        // �ڷ�ƾ ����
        moveCoroutine = StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        while (true)
        {
            // X ��ǥ ����
            transform.localPosition += new Vector3(speed * Time.deltaTime, 0, 0);

            // X ��ǥ�� threshold �̻��� �� offsetValue ����
            if (transform.localPosition.x > threshold)
            {
                transform.localPosition = new Vector3(transform.localPosition.x + offsetValue, transform.localPosition.y, transform.localPosition.z);
            }

            // �����Ӹ��� ����
            yield return null;
        }
    }

    // �ʿ� �� �ڷ�ƾ�� ������ �� �ִ� �޼���
    public void StopMovement()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
    }
}
