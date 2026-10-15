Shader "Shader Graphs/CausticEffect"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Caustic Texture A", 2D) = "white" {}
        [ToggleUI] _WorldSpace ("Use World Space UVs", Float) = 0
        _FogSize ("World UV Size", Float) = 8.44
        _FogColor ("Caustic Color", Color) = (1,1,1,0)
        _Fog_Speed ("Texture A Speed", Vector) = (0,0,0,0)
        _Speed2 ("Texture B Speed", Vector) = (0,0,0,0)
        _Scale ("UV Scale", Vector) = (1,1,0,0)
        [NoScaleOffset] _SampleTexture2D_d8d34466f5714a48a7a8cb8d46a4e209_Texture_1 ("Caustic Texture B", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Name "Caustics"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_SampleTexture2D_d8d34466f5714a48a7a8cb8d46a4e209_Texture_1); SAMPLER(sampler_SampleTexture2D_d8d34466f5714a48a7a8cb8d46a4e209_Texture_1);
            CBUFFER_START(UnityPerMaterial)
                float4 _FogColor, _Fog_Speed, _Speed2, _Scale;
                float _WorldSpace, _FogSize;
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
                float2 uvA = uv * scale + _Time.y * _Fog_Speed.xy;
                float2 uvB = uv * scale * 1.73 + _Time.y * _Speed2.xy;
                half a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvA).r;
                half b = SAMPLE_TEXTURE2D(_SampleTexture2D_d8d34466f5714a48a7a8cb8d46a4e209_Texture_1, sampler_SampleTexture2D_d8d34466f5714a48a7a8cb8d46a4e209_Texture_1, uvB).r;
                half caustic = saturate(a * b * 2.0h);
                return half4(_FogColor.rgb * caustic, saturate(_FogColor.a * caustic));
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
