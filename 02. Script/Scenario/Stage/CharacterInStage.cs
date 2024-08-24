using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInStage : MonoBehaviour
{
    float speed = 1.5f;
    public CharacterHierarchy characterHierarchy;
    public IEnumerator MoveToNewNode(List<Vector3> _points)
    {
        characterHierarchy.animator.SetFloat("RunState", 0.5f);

        // 이동 루프
        for (int i = 0; i < _points.Count; i++)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = _points[i];
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
        characterHierarchy.animator.SetFloat("RunState", 0f);
    }
}
