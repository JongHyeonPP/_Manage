using System.Collections;
using UnityEngine;

public abstract class ObjectThing : MonoBehaviour
{
    public ObjectGrid grid;
    private Coroutine moveCoroutine;
    private readonly float ARRIVAL_TIME = 3f;
    public void MoveToTargetGrid(ObjectGrid _grid, bool _isInstant)
    {
        grid = _grid;
        _grid.owner = this;
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        if (_isInstant)
        {
            transform.SetParent(_grid.transform);
            transform.localPosition = Vector3.zero;
        }
        else
            moveCoroutine = StartCoroutine(MoveCharacterCoroutine(_grid.transform));
    }
    private IEnumerator MoveCharacterCoroutine(Transform _targetTransform)
    {
        transform.SetParent(_targetTransform);
        Vector3 initialPosition = transform.localPosition;
        Vector3 targetPosition = Vector3.zero;
        float distanceToTarget = Vector3.Distance(initialPosition, targetPosition);
        float moveSpeed = distanceToTarget / ARRIVAL_TIME; // ARRIVAL_TIME은 도착하는 데 걸리는 시간을 나타내는 상수로 설정

        float startTime = Time.time;
        while (Time.time - startTime < ARRIVAL_TIME)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfDistance = distanceCovered / distanceToTarget;
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, fractionOfDistance);
            yield return null;
        }

        transform.localPosition = targetPosition;
        moveCoroutine = null;
    }
}
