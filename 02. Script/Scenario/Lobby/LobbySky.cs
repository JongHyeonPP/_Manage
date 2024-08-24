using UnityEngine;
using System.Collections;

public class LobbySky : MonoBehaviour
{
    // 조정 가능한 변수들
    private static float speed = 10.0f; // 속도
    private static float threshold = 2000.0f; // 임계값
    private static float offsetValue = -5031.0f; // X 좌표가 임계값을 넘었을 때 더할 값

    private Coroutine moveCoroutine;

    void Start()
    {
        // 코루틴 시작
        moveCoroutine = StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        while (true)
        {
            // X 좌표 증가
            transform.localPosition += new Vector3(speed * Time.deltaTime, 0, 0);

            // X 좌표가 threshold 이상일 때 offsetValue 적용
            if (transform.localPosition.x > threshold)
            {
                transform.localPosition = new Vector3(transform.localPosition.x + offsetValue, transform.localPosition.y, transform.localPosition.z);
            }

            // 프레임마다 실행
            yield return null;
        }
    }

    // 필요 시 코루틴을 정지할 수 있는 메서드
    public void StopMovement()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
    }
}
