Shader "Custom/URP/Flame"
{
    Properties
    {
        [Header(Main Texture)]
        _MainTex ("Flame Texture (RGBA)", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1, 1, 1, 1)
        _Brightness ("Brightness", Range(0.5, 5)) = 1.5

        [Header(Scroll)]
        _ScrollSpeedX ("Scroll Speed X", Float) = 0.0
        _ScrollSpeedY ("Scroll Speed Y (upward flicker)", Float) = 0.8

        [Header(Distortion)]
        _DistortionStrength ("Distortion Strength", Range(0, 0.3)) = 0.05
        _DistortionSpeed ("Distortion Speed", Float) = 1.0
        _DistortionTiling ("Distortion Tiling", Float) = 2.0

        [Header(Shape)]
        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.02
        _EdgeSoftness ("Edge Softness", Range(0.001, 0.5)) = 0.15
        _FadeTop ("Fade Near Top (0=off)", Range(0, 1)) = 0.3

        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 5 // SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 1 // One (additive-ish glow)
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
            Name "Flame"
            Tags { "LightMode" = "UniversalForward" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _Brightness;
                float _ScrollSpeedX;
                float _ScrollSpeedY;
                float _DistortionStrength;
                float _DistortionSpeed;
                float _DistortionTiling;
                float _AlphaCutoff;
                float _EdgeSoftness;
                float _FadeTop;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color; // supports vertex color alpha (e.g. from particle systems)
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float t = _Time.y;

                // Base scrolling UVs (flame rising upward)
                float2 scrollUV = IN.uv + float2(_ScrollSpeedX, _ScrollSpeedY) * t;

                // Use the same texture, tiled/offset differently and scrolled at another speed,
                // as a cheap distortion/noise source so the flame edges flicker.
                float2 noiseUV = IN.uv * _DistortionTiling + float2(0.13, -0.7) * t * _DistortionSpeed;
                float noiseSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, noiseUV).r;

                // Offset the main UV using the noise, stronger near the top for a "waver" look
                float distortAmount = _DistortionStrength * (0.5 + 0.5 * IN.uv.y);
                float2 distortedUV = scrollUV + (noiseSample - 0.5) * distortAmount;

                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);

                // Soft alpha falloff based on the texture's own alpha/brightness
                half alpha = smoothstep(_AlphaCutoff, _AlphaCutoff + _EdgeSoftness, tex.a);

                // Optional fade near the top of the mesh/UV so flame tips taper off
                if (_FadeTop > 0)
                {
                    float topFade = 1.0 - smoothstep(1.0 - _FadeTop, 1.0, IN.uv.y);
                    alpha *= topFade;
                }

                half3 color = tex.rgb * _Color.rgb * _Brightness;
                alpha *= _Color.a * IN.color.a;

                return half4(color * alpha, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
