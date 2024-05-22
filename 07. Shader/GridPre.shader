Shader "Custom/GridPre"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (0,0,0,1)
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _Speed ("Speed", float) = 1
        _Color2Width ("Color 2 Width", float) = 0.7
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Speed;
            float4 _Color1;
            float4 _Color2;
            float _Color2Width;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // ���� ����
                float t = _Time.y * _Speed;
                float2 uv = i.uv;
                float timeMod = sin(t); // sin �Լ��� ����Ͽ� ����2�� �� �Ϸ��̵��� ����
                uv.x *= 1.0 + timeMod; // �����ʿ������� �帣����
                uv.x = 1.0 - uv.x; // �������� ���ʿ������� �帣���� ����

                // color1�� color2�� �ͽ��Ͽ� ������ ����
                float4 color = lerp(_Color2, _Color1, saturate(uv.x * _Color2Width));
                return color;
            }
            ENDCG
        }
    }
}
