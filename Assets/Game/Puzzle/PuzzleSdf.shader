Shader "Custom/PuzzleSdf"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}

        [Header(Outline)]
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width (fraction of piece size)", Range(0, 0.2)) = 0.03

        [Header(Shape)]
        _CornerRadius("Corner Radius (fraction of piece size)", Range(0, 0.2)) = 0
        _NeckSmoothing("Neck Smoothing (fraction of piece size)", Range(0, 0.2)) = 0.03
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

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

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _OutlineColor;
                float _OutlineWidth;
                float _CornerRadius;
                float _NeckSmoothing;

                // Per-piece values, set through MaterialPropertyBlock (PuzzlePieceData.ApplyTo).
                float2 _CellSize; // Piece size without knobs (world units).
                float _Margin; // Extra quad space around the cell for knobs.
                float4 _UVRect; // Image UV rect of the quad: xy = min, zw = max.
                float4 _EdgeTop; // x = tab (+1 knob, -1 socket, 0 flat), y = centre, z = radius, w = depth
                float4 _EdgeRight;
                float4 _EdgeBottom;
                float4 _EdgeLeft;
            CBUFFER_END

            // Signed distance to a box centred on the origin with half-size b.
            float SdBox(float2 p, float2 b)
            {
                float2 d = abs(p) - b;
                return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
            }

            // Polynomial smooth minimum, k is the blend radius.
            float SmoothMin(float a, float b, float k)
            {
                float h = max(k - abs(a - b), 0.0) / k;
                return min(a, b) - h * h * k * 0.25;
            }

            // Knob shape for one seam, negative inside the knob.
            // The seam is evaluated in a frame relative to the seam itself (q = distance from the seam,
            // t = position along it). Both pieces sharing a seam use the same q/t in world terms, and the
            // same bulge direction, so they get identical values. That is what makes the knob and the
            // socket complement each other exactly.
            float KnobSDF(float2 p, float4 e, float side, bool vertical, float2 cell, float minCell)
            {
                float bulge = e.x * side; // +1: knob points towards +x / +y, -1: towards -x / -y
                float along = vertical ? cell.y : cell.x;
                float depth = e.w;
                float r = max(e.z, 1e-4); // Knob circle radius, from the generator.

                float q = (vertical ? p.x : p.y) - side * (vertical ? cell.x : cell.y) * 0.5;
                float t = (vertical ? p.y : p.x) - (e.y - 0.5) * along;

                // The circle's centre sits (depth - r) outside the seam, so its tip reaches exactly 'depth'.
                float offset = depth - r;
                float circle = length(float2(q - bulge * offset, t)) - r;

                // Half-width of the circle where it meets the seam: this is the neck.
                float chord = sqrt(max(r * r - offset * offset, 0.0));

                // A thin bar along the seam blended into the circle gives the neck fillets.
                float k = max(_NeckSmoothing * minCell, 1e-4);
                float neck = SdBox(float2(q, t), float2(k, chord));
                return SmoothMin(circle, neck, k);
            }

            // Applies one side of the piece. Knobs are unioned, sockets are subtracted.
            float ApplyEdge(float d, float2 p, float4 e, float side, bool vertical, float2 cell, float minCell)
            {
                if (e.x > 0.5)
                    return min(d, KnobSDF(p, e, side, vertical, cell, minCell));
                if (e.x < -0.5)
                    return max(d, -KnobSDF(p, e, side, vertical, cell, minCell));
                return d;
            }

            // Piece silhouette, negative inside. p is relative to the cell centre.
            float PieceSDF(float2 p, float2 cell, float minCell)
            {
                float2 halfSize = cell * 0.5;
                float cr = min(_CornerRadius * minCell, min(halfSize.x, halfSize.y));
                float2 q = abs(p) - (halfSize - cr);
                float d = length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - cr;

                d = ApplyEdge(d, p, _EdgeRight, +1.0, true, cell, minCell);
                d = ApplyEdge(d, p, _EdgeLeft, -1.0, true, cell, minCell);
                d = ApplyEdge(d, p, _EdgeTop, +1.0, false, cell, minCell);
                d = ApplyEdge(d, p, _EdgeBottom, -1.0, false, cell, minCell);
                return d;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 cell = _CellSize;
                float minCell = min(cell.x, cell.y);
                float2 quadSize = cell + 2.0 * _Margin;
                float2 p = (IN.uv - 0.5) * quadSize;

                float d = PieceSDF(p, cell, minCell);

                // Anti-aliasing width in SDF units.
                float aa = max(fwidth(d), 1e-5);
                float coverage = 1.0 - smoothstep(-aa, aa, d);

                // Outline on the inside of the silhouette.
                float outlineWidth = _OutlineWidth * minCell;
                float outline = (1.0 - smoothstep(outlineWidth - aa, outlineWidth + aa, -d))
                    * step(0.0001, _OutlineWidth);

                float2 imageUV = lerp(_UVRect.xy, _UVRect.zw, IN.uv);
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, imageUV) * _BaseColor;

                half3 color = lerp(tex.rgb, _OutlineColor.rgb, outline);
                return half4(color, tex.a * coverage);
            }
            ENDHLSL
        }
    }
}