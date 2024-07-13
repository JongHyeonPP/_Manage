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

        // ���� ��ǥ�踦 ����Ͽ� ��� ��ġ�� �迭�� ����
        Vector3[] points = new Vector3[_edges.Length + 1];
        for (int i = 0; i < _edges.Length; i++)
        {
            points[i] = _edges[i].position; // �� _edges�� ���� ��ǥ�� �迭�� ����
        }
        points[points.Length - 1] = _destination.position; // _destination�� ���� ��ǥ�� �迭�� ����

        // �̵� ����
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

            // ���� ��ġ�� ��Ȯ�ϰ� ����
            transform.position = endPosition;
        }

        // �ִϸ����� ���¸� ������� �ǵ���
        animator.SetFloat("RunState", 0f);
        Debug.Log("On Moved");
    }
}
