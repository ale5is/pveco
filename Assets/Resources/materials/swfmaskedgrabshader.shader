Shader "FlashTools/SwfMaskedGrab" {
	Properties {
		[PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
		[PerRendererData] _AlphaTex ("Alpha Texture", 2D) = "white" {}
		[PerRendererData] _ExternalAlpha ("External Alpha", Float) = 0
		[PerRendererData] _Tint ("Tint", Vector) = (1,1,1,1)
		_StencilID ("Stencil ID", Float) = 0
		[Enum(UnityEngine.Rendering.BlendOp  )] _BlendOp ("BlendOp", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Float) = 10
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

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy);
			}

			ENDHLSL
		}
	}
}