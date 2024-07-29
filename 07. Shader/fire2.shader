Shader "Example/URPUnlitShaderBasic"
{
    Properties
    {
        _BaseMap("BaseMap",2D) = "white"{}
        _BaseMap2("BaseMap2",2D) = "white"{}
        _Alpha("Alpha", Range(0, 1)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            TEXTURE2D(_BaseMap2);
            SAMPLER(sampler_BaseMap);
            SAMPLER(sampler_BaseMap2);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseMap2_ST;
                float _Alpha;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //노이즈를 흘러가게 해 줍니다.
                half4 color2 = SAMPLE_TEXTURE2D(_BaseMap2, sampler_BaseMap2, TRANSFORM_TEX(IN.uv, _BaseMap2) + float2(0, -_Time.y));
                //두 번째 텍스쳐의 r 채널을 UV에 더해서 UV를 구겨줍니다. 
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, TRANSFORM_TEX(IN.uv, _BaseMap) + (color2.r - 0.5) * 0.1);
                
                // 알파 값을 적용합니다.
                color.a *= _Alpha;
                
                return color;
            }
            ENDHLSL
        }
    }
}
