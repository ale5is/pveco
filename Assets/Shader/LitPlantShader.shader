Shader "LitPlantShader"
{
    Properties
    {
        _MainTex ("Diffuse", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _EyeTex ("Eye Texture", 2D) = "black" {}
        [Toggle] _UseEyeTex ("Use Eye Texture", Float) = 0
        _GrayScale ("GrayScale Amount", Range(0,1)) = 1
        _Brightness ("Brightness", Float) = 1
        [Toggle] _OpenGray ("Enable Grayscale", Float) = 1
        [Toggle] _OpenSolid ("Enable Solid Tint", Float) = 0
        _SolideColor ("Solid Tint Color", Color) = (1,1,1,1)
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
            Name "PlantSprite"
            Tags { "LightMode"="Universal2D" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex); SAMPLER(sampler_MaskTex);
            TEXTURE2D(_EyeTex); SAMPLER(sampler_EyeTex);
            TEXTURE2D(_AlphaTex); SAMPLER(sampler_AlphaTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _RendererColor;
                float4 _Flip;
                float4 _SolideColor;
                float _GrayScale, _Brightness, _OpenGray, _OpenSolid, _UseEyeTex;
                float _AngleX, _AngleY, _ScaleX, _ScaleY, _Alpha, _IsVisible, _EnableExternalAlpha;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; float2 alphaUV : TEXCOORD1; };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 p = input.positionOS.xyz;
                p.x *= _ScaleX; p.y *= _ScaleY;
                float ax = radians(_AngleX), ay = radians(_AngleY);
                float sy = sin(ax), cy = cos(ax);
                p = float3(p.x, p.y * cy - p.z * sy, p.y * sy + p.z * cy);
                float sx = sin(ay), cx = cos(ay);
                p = float3(p.x * cx + p.z * sx, p.y, -p.x * sx + p.z * cx);
                p.xy *= _Flip.xy;
                output.positionCS = TransformObjectToHClip(p);
                output.uv = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                output.alphaUV = input.uv;
                output.color = input.color * _Color * _RendererColor;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                clip(_IsVisible + 0.5);
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;
                half mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv).r;
                if (_OpenSolid > 0.5) c.rgb = lerp(c.rgb, _SolideColor.rgb, saturate(mask * _SolideColor.a));
                if (_OpenGray > 0.5)
                {
                    half gray = dot(c.rgb, half3(0.299, 0.587, 0.114));
                    c.rgb = lerp(c.rgb, gray.xxx, saturate(_GrayScale));
                }
                c.rgb *= max(_Brightness, 0.0);
                if (_UseEyeTex > 0.5)
                {
                    half4 eye = SAMPLE_TEXTURE2D(_EyeTex, sampler_EyeTex, input.uv);
                    c.rgb = lerp(c.rgb, eye.rgb, eye.a);
                }
                if (_EnableExternalAlpha > 0.5) c.a *= SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, input.alphaUV).r;
                c.a *= saturate(_Alpha);
                return c;
            }
            ENDHLSL
        }
    }
    FallBack "Sprites/Default"
}
