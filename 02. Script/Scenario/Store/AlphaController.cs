using UnityEngine;
using UnityEngine.UI;

public class AlphaController : MonoBehaviour
{
    private Image imageComponent;
    private Material material;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        material = imageComponent.material;
    }

    void Update()
    {
        // Image ������Ʈ�� ���İ� ��������
        float alpha = imageComponent.color.a;

        // ���̴��� ���İ� ������Ʈ
        material.SetFloat("_Alpha", alpha);
    }
}
