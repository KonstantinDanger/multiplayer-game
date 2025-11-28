Shader "Hidden/DamageVignette"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

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
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _VignetteIntensity;
            float _VignetteSoftness;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                float2 center = input.uv - 0.5;
                float dist = length(center);
                
                float vignette = smoothstep(_VignetteIntensity, _VignetteIntensity - _VignetteSoftness, dist);
                
                half3 redTint = half3(1.0, 0.1, 0.1);
                color.rgb = lerp(color.rgb * redTint, color.rgb, vignette);
                
                return color;
            }
            ENDHLSL
        }
    }
}