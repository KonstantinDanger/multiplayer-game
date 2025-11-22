Shader "Custom/URP/ZoneSphereShader"
{
    Properties
    {
        [Header(Zone Settings)]
        _MainColor ("Main Color", Color) = (0.2, 0.6, 1.0, 0.6)
        _EdgeColor ("Edge Color", Color) = (0.0, 0.8, 1.0, 0.9)
        _CutoffHeight ("Cutoff Height", Range(-1, 1)) = 0.5
        _CutoffSmoothness ("Cutoff Smoothness", Range(0.01, 0.5)) = 0.15
        
        [Header(Wave Effect)]
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1.0
        _WaveScale ("Wave Scale", Range(0.1, 10)) = 2.0
        _WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 0.3
        _WaveFrequency ("Wave Frequency", Range(1, 10)) = 3.0
        
        [Header(Intersection Effect)]
        _IntersectionPower ("Intersection Power", Range(0.1, 5)) = 1.5
        _IntersectionThickness ("Intersection Thickness", Range(0.1, 5)) = 1.0
        _IntersectionColor ("Intersection Color", Color) = (0.3, 1.0, 0.8, 1.0)
        _IntersectionIntensity ("Intersection Intensity", Range(0, 3)) = 2.0
        
        [Header(Noise Settings)]
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1.5
        _NoiseSpeed ("Noise Speed", Range(0, 3)) = 0.5
        _VoronoiScale ("Voronoi Scale", Range(0.1, 5)) = 1.0
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.4
        
        [Header(Fresnel)]
        _FresnelPower ("Fresnel Power", Range(0.5, 8)) = 3.0
        _FresnelIntensity ("Fresnel Intensity", Range(0, 3)) = 1.5
        
        [Header(Animation)]
        _FlowSpeed ("Flow Speed", Range(0, 2)) = 0.3
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.2
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "IgnoreProjector"="True"
        }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        Cull Off // Two-sided rendering
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                float fogFactor : TEXCOORD5;
                float3 positionOS : TEXCOORD6;
                float3 sphereScale : TEXCOORD7;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainColor;
                float4 _EdgeColor;
                float4 _IntersectionColor;
                float _CutoffHeight;
                float _CutoffSmoothness;
                float _WaveSpeed;
                float _WaveScale;
                float _WaveAmplitude;
                float _WaveFrequency;
                float _IntersectionPower;
                float _IntersectionThickness;
                float _IntersectionIntensity;
                float _NoiseScale;
                float _NoiseSpeed;
                float _VoronoiScale;
                float _NoiseStrength;
                float _FresnelPower;
                float _FresnelIntensity;
                float _FlowSpeed;
                float _DistortionStrength;
            CBUFFER_END
            
            // Voronoi noise function
            float2 voronoiHash(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
            }
            
            float voronoi(float2 uv, float time)
            {
                float2 p = floor(uv);
                float2 f = frac(uv);
                
                float minDist = 1.0;
                
                for (int j = -1; j <= 1; j++)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        float2 neighbor = float2(i, j);
                        float2 voroPoint = voronoiHash(p + neighbor);
                        voroPoint = 0.5 + 0.5 * sin(time + 6.2831 * voroPoint);
                        
                        float2 diff = neighbor + voroPoint - f;
                        float dist = length(diff);
                        minDist = min(minDist, dist);
                    }
                }
                
                return minDist;
            }
            
            // Simplex-like noise
            float noise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float n = i.x + i.y * 57.0 + 113.0 * i.z;
                
                return lerp(
                    lerp(lerp(frac(sin(n) * 43758.5), frac(sin(n + 1.0) * 43758.5), f.x),
                        lerp(frac(sin(n + 57.0) * 43758.5), frac(sin(n + 58.0) * 43758.5), f.x), f.y),
                    lerp(lerp(frac(sin(n + 113.0) * 43758.5), frac(sin(n + 114.0) * 43758.5), f.x),
                        lerp(frac(sin(n + 170.0) * 43758.5), frac(sin(n + 171.0) * 43758.5), f.x), f.y), f.z);
            }
            
            // Turbulent noise with multiple octaves
            float turbulence(float3 p, float time)
            {
                float t = 0.0;
                float amplitude = 1.0;
                float frequency = 1.0;
                
                for (int i = 0; i < 3; i++)
                {
                    t += amplitude * noise(p * frequency + float3(time, time * 0.5, time * 0.3));
                    amplitude *= 0.5;
                    frequency *= 2.0;
                }
                
                return t;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.uv = input.uv;
                output.screenPos = ComputeScreenPos(positionInputs.positionCS);
                output.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                output.positionOS = input.positionOS.xyz;
                
                // Calculate sphere scale from object to world transformation
                float3 worldScale = float3(
                    length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)),
                    length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y)),
                    length(float3(UNITY_MATRIX_M[0].z, UNITY_MATRIX_M[1].z, UNITY_MATRIX_M[2].z))
                );
                output.sphereScale = worldScale;
                
                return output;
            }
            
            half4 frag(Varyings input, half facing : VFACE) : SV_Target
            {
                float time = _Time.y;
                
                // Normalize vectors
                float3 normalWS = normalize(input.normalWS) * sign(facing);
                float3 viewDirWS = normalize(input.viewDirWS);
                
                // Use object space position for scale-independent effects
                float3 posOS = input.positionOS;
                float avgScale = (input.sphereScale.x + input.sphereScale.y + input.sphereScale.z) / 3.0;
                
                // Height-based cutoff with smooth transition (cuts from top) - use object space Y
                float heightFactor = posOS.y;
                float cutoffMask = smoothstep(_CutoffHeight + _CutoffSmoothness, 
                                             _CutoffHeight - _CutoffSmoothness, 
                                             heightFactor);
                
                // Depth-based intersection detection (scale-independent)
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float sceneDepth = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams);
                float surfaceDepth = input.screenPos.w;
                
                // Scale the intersection thickness by sphere size
                float scaledThickness = _IntersectionThickness * avgScale;
                
                // Calculate depth difference with scaled thickness
                float rawDepthDiff = sceneDepth - surfaceDepth;
                float depthDiff = (sceneDepth > 0.0001) ? saturate(rawDepthDiff / scaledThickness) : 1.0;
                
                // Wave effect at intersection - scale independent using object space
                float2 waveUV = posOS.xz * _WaveScale + time * _WaveSpeed;
                float waves = sin(waveUV.x * _WaveFrequency + time * 2.0) * 
                             cos(waveUV.y * _WaveFrequency + time * 1.5) * _WaveAmplitude;
                
                // Voronoi noise for magical effect - use object space
                float2 voronoiUV = posOS.xz * _VoronoiScale;
                float voronoiNoise = voronoi(voronoiUV, time * _NoiseSpeed);
                
                // Turbulent noise for fluid effect - use object space
                float3 noisePos = posOS * _NoiseScale;
                float turbulentNoise = turbulence(noisePos, time * _FlowSpeed);
                
                // Combine noises
                float combinedNoise = lerp(voronoiNoise, turbulentNoise, 0.5) * _NoiseStrength;
                combinedNoise = combinedNoise * 0.5 + 0.5; // Remap to 0-1
                
                // Distort intersection with noise
                float intersectionMask = pow(1.0 - depthDiff, _IntersectionPower);
                intersectionMask = saturate(intersectionMask + waves * 0.5 + combinedNoise * _DistortionStrength);
                
                // Boost intersection visibility significantly
                intersectionMask = saturate(intersectionMask * _IntersectionIntensity);
                
                // Fresnel effect
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _FresnelPower);
                fresnel *= _FresnelIntensity;
                
                // Animated color variation
                float colorVariation = turbulentNoise * 0.3 + 0.7;
                
                // Combine colors with stronger intersection influence
                float4 baseColor = lerp(_MainColor, _EdgeColor, fresnel);
                baseColor.rgb *= colorVariation;
                
                // Ensure minimum visibility
                baseColor.a = max(baseColor.a, 0.3);
                
                float4 finalColor = baseColor;
                
                // Apply intersection color with much stronger blending
                finalColor = lerp(finalColor, _IntersectionColor, intersectionMask);
                
                // Add noise-based brightness variation
                finalColor.rgb += combinedNoise * 0.2;
                
                // Brighten intersection areas significantly
                finalColor.rgb += intersectionMask * _IntersectionColor.rgb * 0.5;
                
                // Apply cutoff mask
                finalColor.a *= cutoffMask;
                
                // Strong boost to base alpha for visibility
                finalColor.a = saturate(finalColor.a + 0.4);
                
                // Enhance edges more
                finalColor.a = saturate(finalColor.a + fresnel * 0.5);
                
                // Apply fog
                finalColor.rgb = MixFog(finalColor.rgb, input.fogFactor);
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}