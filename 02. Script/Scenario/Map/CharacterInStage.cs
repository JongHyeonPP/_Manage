using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInStage : MonoBehaviour
{
    public float speed = 1.0f;
    public CharacterHierarchy characterHierarchy;
    public Animator animator;

    public IEnumerator MoveToNewNode(RectTransform[] _edges, RectTransform _destination)
    {
        animator.SetFloat("RunState", 0.5f);

        // 월드 좌표계를 사용하여 모든 위치를 배열에 저장
        Vector3[] points = new Vector3[_edges.Length + 1];
        for (int i = 0; i < _edges.Length; i++)
        {
            points[i] = _edges[i].position; // 각 _edges의 월드 좌표를 배열에 저장
        }
        points[points.Length - 1] = _destination.position; // _destination의 월드 좌표를 배열에 저장

        // 이동 루프
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = points[i];
            float distance = Vector3.Distance(startPosition, endPosition);
            float journeyTime = distance / speed;
            float startTime = Time.time;

            while (Vector3.Distance(transform.position, endPosition) > 0.01f)
            {
                float fraction = (Time.time - startTime) / journeyTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, fraction);
                yield return null;
            }

            // 최종 위치를 정확하게 설정
            transform.position = endPosition;
        }

        // 애니메이터 상태를 원래대로 되돌림
        animator.SetFloat("RunState", 0f);
        Debug.Log("On Moved");
    }
}
