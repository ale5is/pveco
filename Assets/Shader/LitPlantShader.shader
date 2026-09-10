Shader "LitPlantShader" {
	Properties {
		_MainTex ("Diffuse", 2D) = "white" {}
		_MaskTex ("Mask", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "bump" {}
		_EyeTex ("EyeTex", 2D) = "black" {}
		_GrayScale ("GrayScale", Float) = 1
		_Brightness ("Brightness", Float) = 1
		[Toggle] _OpenGray ("OpenGray", Float) = 1
		[Toggle] _OpenSolid ("OpenSolid", Float) = 1
		_SolideColor ("_SolideColor", Vector) = (1,1,1,1)
		[HideInInspector] _Color ("Tint", Vector) = (1,1,1,1)
		[HideInInspector] _RendererColor ("RendererColor", Vector) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
		[HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
		_AngleX ("AngleX", Float) = 0
		_AngleY ("AngleY", Float) = 0
		_ScaleX ("ScaleX", Float) = 1
		_ScaleY ("ScaleY", Float) = 1
		_Alpha ("Alpha", Float) = 1
		_IsVisible ("IsVisible", Range(-1, 0)) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy) * _Color;
			}

			ENDHLSL
		}
	}
	Fallback "Sprites/Default"
}