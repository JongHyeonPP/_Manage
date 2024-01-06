Shader "Custom/myShaderV2++"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Float) = 2
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Z Test", Float) = 0
        [Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 1
        _Intensity("Intensity", Range(0, 2)) = 1
        _MainTex("Albedo (RGB)", 2D) = "white"{}
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlend Mode", Float) = 5 // SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("DstBlend Mode", Float) = 1 // One
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlendA("SrcBlend Mode Alpha", Float) = 5 // SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlendA("DstBlend Mode Alpha", Float) = 1 // One
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" }
            Blend[_SrcBlend][_DstBlend],[_SrcBlendA][_DstBlendA]
            Ztest[_ZTest]
            Zwrite [_ZWrite]
            Cull[_CullMode]

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float _Intensity;
                //fixed4 _Color; // Modified variable for SpriteRenderer color

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float4 color : COLOR;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.color = v.color; // Apply SpriteRenderer color
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                    return col * _Intensity;
                }

                ENDCG
            }
        }
}
