Shader "Shader Graphs/RollingFog"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Fog Noise Texture", 2D) = "white" {}
        [ToggleUI] _WorldSpace ("Use World Space UVs", Float) = 0
        _FogSize ("World UV Size", Float) = 8.44
        _FogIntensity ("Fog Intensity", Range(0,1)) = 0
        _FogColor ("Fog Color", Color) = (1,1,1,0)
        _Fog_Speed ("Fog Movement Speed", Vector) = (0,0,0,0)
        _Scale ("UV Scale", Vector) = (1,1,0,0)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Name "RollingFog"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _FogColor, _Fog_Speed, _Scale;
                float _WorldSpace, _FogSize, _FogIntensity;
            CBUFFER_END
            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; float3 positionWS : TEXCOORD1; };
            Varyings vert(Attributes input)
            {
                Varyings o;
                VertexPositionInputs pos = GetVertexPositionInputs(input.positionOS.xyz);
                o.positionCS = pos.positionCS;
                o.positionWS = pos.positionWS;
                o.uv = input.uv;
                return o;
            }
            half4 frag(Varyings i) : SV_Target
            {
                float2 worldUV = i.positionWS.xz / max(abs(_FogSize), 0.001);
                float2 uv = lerp(i.uv, worldUV, saturate(_WorldSpace));
                float2 scale = (abs(_Scale.xy).x + abs(_Scale.xy).y < 0.0001) ? float2(1,1) : _Scale.xy;
                uv = uv * scale + _Time.y * _Fog_Speed.xy;
                half noise = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).r;
                half alpha = saturate(noise * _FogIntensity) * saturate(_FogColor.a);
                return half4(_FogColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
