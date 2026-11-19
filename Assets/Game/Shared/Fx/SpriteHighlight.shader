Shader "Sprites/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [Toggle(_OUTLINE_ON)] _Outline ("Outline", Float) = 0
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size (px)", Float) = 1
    }

    // inside Pass, after #include "UnityCG.cginc"
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_local __ _OUTLINE_ON

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            half4 _Color;
            half4 _OutlineColor;
            float _OutlineSize;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
                o.uv = v.texcoord;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 c = tex2D(_MainTex, i.uv) * i.color;

                #if defined(_OUTLINE_ON)
                if (c.a < 0.999)
                {
                    float2 o = _MainTex_TexelSize.xy * _OutlineSize;
                    // Max alpha in a ring around this texel, smooth, not thresholded
                    half m = 0;
                    m = max(m, tex2D(_MainTex, i.uv + float2(o.x, 0)).a);
                    m = max(m, tex2D(_MainTex, i.uv + float2(-o.x, 0)).a);
                    m = max(m, tex2D(_MainTex, i.uv + float2(0, o.y)).a);
                    m = max(m, tex2D(_MainTex, i.uv + float2(0, -o.y)).a);
                    m = max(m, tex2D(_MainTex, i.uv + float2(o.x, o.y) * 0.7071).a);
                    m = max(m, tex2D(_MainTex, i.uv + float2(-o.x, o.y) * 0.7071).a);
                    m = max(m, tex2D(_MainTex, i.uv + float2(o.x, -o.y) * 0.7071).a);
                    m = max(m, tex2D(_MainTex, i.uv + float2(-o.x, -o.y) * 0.7071).a);

                    half outA = m * _OutlineColor.a;
                    // Premultiplied "over": sprite on top of outline
                    half3 rgb = c.rgb + _OutlineColor.rgb * outA * (1 - c.a);
                    half a = c.a + outA * (1 - c.a);
                    return half4(rgb, a);
                }
                #endif

                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}