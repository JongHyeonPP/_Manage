using System;
using UnityEngine;
using UnityEngine.VFX;

public class SlashCtrl : MonoBehaviour
{
    public Transform _src, _dst;

    [SerializeField]
    internal Values_SlashCtrl myVFX;

    // Start is called before the first frame update Bolder_thickness
    [ContextMenu("TestPlay")]
    public void test()
    {
        transform.position = _src.position;
        transform.right = (_dst.position- _src.position);
        myVFX.SetEffect();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            myVFX.vfx.Play();
        }    
    }
}

[Serializable]
internal class Values_SlashCtrl
{
    [SerializeField] internal Transform joint1, joint2;
    [SerializeField] internal float _angle, _trans;

    [SerializeField] internal VisualEffect vfx;
    [SerializeField] internal float lifeTime;
    [SerializeField] internal float ratio;
    [SerializeField] internal float size;
    [SerializeField] internal Color Outline;
    [SerializeField] internal Color Main;
    [SerializeField] internal AnimationCurve Curv;

    public void SetEffect()
    {
        joint1.localRotation = Quaternion.Euler(_angle * _trans, 0, 0);
        joint2.localRotation = Quaternion.Euler(0, _angle, 0);

        vfx.SetFloat(Shader.PropertyToID("_lifeTime"), lifeTime);
        vfx.SetFloat(Shader.PropertyToID("_thickness"), ratio);
        vfx.SetFloat(Shader.PropertyToID("_size"), size);
        vfx.SetVector4(Shader.PropertyToID("_outlineColor"), Outline);
        vfx.SetVector4(Shader.PropertyToID("_mainColor"), Main);
        vfx.SetAnimationCurve(Shader.PropertyToID("_curv"), Curv);
    }
}