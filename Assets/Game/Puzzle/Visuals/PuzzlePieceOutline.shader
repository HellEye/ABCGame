Shader "Custom/JigsawPieceOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Float) = 0.02
        _EnableOutline ("Enable Outline", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalRenderPipeline"
        }

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
                // x = distance from the piece boundary, in local mesh units.
                // 0 at the boundary ring, growing towards the piece center.
                float2 uv2 : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float edgeDistance : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _OutlineColor;
            float _OutlineWidth;
            float _EnableOutline;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // Vertex positions are NEVER modified - the piece silhouette and
                // its UVs stay exactly as generated, regardless of outline width.
                // This guarantees the texture never shifts/distorts and pieces
                // always keep fitting together.
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.edgeDistance = IN.uv2.x;

                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // Hard cutoff with a tiny screen-space anti-alias band so the edge
                // doesn't look jagged, without producing a visible gradient.
                float aa = max(fwidth(IN.edgeDistance), 1e-5);
                float outlineAmount = 1.0 - smoothstep(_OutlineWidth - aa, _OutlineWidth + aa, IN.edgeDistance);
                outlineAmount *= _EnableOutline;

                float4 finalColor = lerp(texColor, _OutlineColor, outlineAmount);

                return finalColor;
            }
            ENDHLSL
        }
    }
}