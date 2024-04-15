using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _MyDEBUG : MonoBehaviour
{
    public Transform _transform;
    public Image _prefab;
    public List<Sprite> sprList = new();

    [ContextMenu("Testing")]
    public void _test(){
        Transform temp = Instantiate(_transform,transform);
        for (int i = 0; i < sprList.Count; i++)
        {
            Image _image = Instantiate(_prefab, temp);
            _image.transform.position = _prefab.transform.position + new Vector3(((i % 10) + 1) * 50, ((i / 10) + 1) *50, 0);
            _image.sprite = sprList[i];
            _image.transform.name = sprList[i].name;
        }
    }
}
