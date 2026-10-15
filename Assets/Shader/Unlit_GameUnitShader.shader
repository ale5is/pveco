Shader "Unlit/GameUnitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor ("Renderer Color", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        _AngleX ("Angle X", Float) = 0
        _AngleY ("Angle Y", Float) = 0
        _ScaleX ("Scale X", Float) = 1
        _ScaleY ("Scale Y", Float) = 1
        _Alpha ("Alpha", Range(0,1)) = 1
        _IsVisible ("Is Visible", Range(-1,0)) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Name "GameUnitSprite"
            Tags { "LightMode"="Universal2D" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_AlphaTex); SAMPLER(sampler_AlphaTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST, _Color, _RendererColor, _Flip;
                float _AngleX, _AngleY, _ScaleX, _ScaleY, _Alpha, _IsVisible, _EnableExternalAlpha;
            CBUFFER_END
            struct Attributes { float4 positionOS : POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; float2 alphaUV : TEXCOORD1; };
            Varyings vert(Attributes input)
            {
                Varyings o;
                float3 p = input.positionOS.xyz;
                p.x *= _ScaleX; p.y *= _ScaleY;
                float ax = radians(_AngleX), ay = radians(_AngleY);
                float sy = sin(ax), cy = cos(ax);
                p = float3(p.x, p.y * cy - p.z * sy, p.y * sy + p.z * cy);
                float sx = sin(ay), cx = cos(ay);
                p = float3(p.x * cx + p.z * sx, p.y, -p.x * sx + p.z * cx);
                p.xy *= _Flip.xy;
                o.positionCS = TransformObjectToHClip(p);
                o.uv = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                o.alphaUV = input.uv;
                o.color = input.color * _Color * _RendererColor;
                return o;
            }
            half4 frag(Varyings i) : SV_Target
            {
                clip(_IsVisible + 0.5);
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;
                if (_EnableExternalAlpha > 0.5) c.a *= SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, i.alphaUV).r;
                c.a *= saturate(_Alpha);
                return c;
            }
            ENDHLSL
        }
    }
    FallBack "Sprites/Default"
}
