Shader "Custom/URP_SpriteHighlight"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _NearColor ("Near Color", Color) = (1,1,0,1)
        _FarColor ("Far Color", Color) = (1,0,0,0)
        _OutlineRadius ("Outline Radius", Float) = 5.0
        _AlphaThreshold ("Alpha Threshold", Float) = 0.01
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
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
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize;

            float4 _NearColor;
            float4 _FarColor;
            float _OutlineRadius;
            float _AlphaThreshold;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }


            #define MAX_RADIUS 64
            #define ANGLE_STEPS 16


            float GetMaxNeighborAlpha(float2 uv, float radius)
            {
                float maxAlpha = 0.0;

                [unroll(ANGLE_STEPS)]
                for (int i = 0; i < ANGLE_STEPS; i++)
                {
                    float angle = (6.2831853 / ANGLE_STEPS) * i;
                    float2 dir = float2(cos(angle), sin(angle));

                    float2 offset = dir * radius * _MainTex_TexelSize.xy;
                    float alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset).a;

                    maxAlpha = max(maxAlpha, alpha);
                }

                return maxAlpha;
            }

            float GetDistanceToSprite(float2 uv)
            {
                float minDist = _OutlineRadius;

                for (int r = 1; r <= MAX_RADIUS; r++)
                {
                    if (r > _OutlineRadius)
                        break;

                    float radius = r;

                    [unroll(ANGLE_STEPS)]
                    for (int i = 0; i < ANGLE_STEPS; i++)
                    {
                        float angle = (6.2831853 / ANGLE_STEPS) * i;
                        float2 dir = float2(cos(angle), sin(angle));

                        float2 offset = dir * radius * _MainTex_TexelSize.xy;
                        float alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset).a;

                        if (alpha > _AlphaThreshold)
                        {
                            minDist = min(minDist, radius);
                        }
                    }
                }

                return minDist;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float alpha = tex.a;

                // Основной пиксель
                if (alpha > _AlphaThreshold)
                {
                    // Делаем края непрозрачными и подсвечиваем NearColor
                    float4 col = tex * i.color;

                    float nearBoost = saturate(alpha / _AlphaThreshold);
                    col.rgb = lerp(_NearColor.rgb, col.rgb, nearBoost);
                    col.a = 1.0;

                    return col;
                }

                // ВНЕШНЯЯ ОБВОДКА
                float dist = GetDistanceToSprite(i.uv);

                if (dist < _OutlineRadius)
                {
                    float t = dist / _OutlineRadius;

                    float4 outlineColor = lerp(_NearColor, _FarColor, t);

                    // Альфа плавно затухает
                    outlineColor.a *= (1.0 - t);

                    return outlineColor;
                }

                return float4(0,0,0,0);
            }

            ENDHLSL
        }
    }
}