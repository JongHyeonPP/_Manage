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
        // Image 컴포넌트의 알파값 가져오기
        float alpha = imageComponent.color.a;

        // 셰이더의 알파값 업데이트
        material.SetFloat("_Alpha", alpha);
    }
}
