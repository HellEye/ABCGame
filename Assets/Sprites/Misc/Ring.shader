Shader "Custom/SoftRing"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _OuterRadius ("Outer Radius", Range(0, 0.5)) = 0.5
        _Thickness ("Thickness", Range(0, 0.5)) = 0.1
        _Softness ("Softness", Range(0, 0.1)) = 0.02
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _OuterRadius;
                float _Thickness;
                float _Softness;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = length(IN.uv - center);

                float innerRadius = _OuterRadius - _Thickness;

                float innerEdge = smoothstep(
                    innerRadius - _Softness,
                    innerRadius,
                    dist
                );

                float outerEdge = 1.0 - smoothstep(
                    _OuterRadius,
                    _OuterRadius + _Softness,
                    dist
                );

                float alpha = innerEdge * outerEdge;

                return float4(_Color.rgb, _Color.a * alpha);
            }
            ENDHLSL
        }
    }
}