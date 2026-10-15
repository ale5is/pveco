Shader "MyBaseShader"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Vector) = (1,1,1,1)
		_GrayScale ("GrayScale", Float) = 1
		_Brightness ("Brightness", Float) = 1
		[Toggle] _OpenGray ("OpenGray", Float) = 1
		[Toggle] _OpenBrightness ("OpenBrightness", Float) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"RenderType"="Transparent"
			"IgnoreProjector"="True"
			"CanUseSpriteAtlas"="True"
			"PreviewType"="Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct VertexInput
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;

			float _GrayScale;
			float _Brightness;
			float _OpenGray;
			float _OpenBrightness;

			VertexOutput vert(VertexInput input)
			{
				VertexOutput output;

				output.vertex = UnityObjectToClipPos(input.vertex);
				output.uv = TRANSFORM_TEX(input.uv, _MainTex);
				output.color = input.color * _Color;

				return output;
			}

			float4 frag(VertexOutput input) : SV_Target
			{
				float4 color = tex2D(_MainTex, input.uv) * input.color;

				if (_OpenGray > 0.5)
				{
					float gray = dot(color.rgb, float3(0.299, 0.587, 0.114));
					color.rgb = lerp(color.rgb, gray.xxx, saturate(_GrayScale));
				}

				if (_OpenBrightness > 0.5)
				{
					color.rgb *= _Brightness;
				}

				return color;
			}

			ENDHLSL
		}
	}

	Fallback "Sprites/Default"
}