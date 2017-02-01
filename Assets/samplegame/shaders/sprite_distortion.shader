// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Sprites/Distortion"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_DistortionTex ("Distortion Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Speed ("Speed", Float) = 1
		_TimeOffset ("TimeOffset", Float) = 0
		_Scale ("Scale", Float) = 1
		_Amount ("Amount", Float) = 1
		_Offset ("Offset", Vector) = (0, 0, 0, 0)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half4 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
			float4 _Offset;
			half _Speed;
			half _TimeOffset;
			half _Scale;
			half _Amount;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord.xy = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

			    half t = frac((_Time + _TimeOffset) * _Speed);
				OUT.texcoord.zw = (mul(unity_ObjectToWorld, IN.vertex).xy - _Offset.xy) * _Scale / 768 + float2(t, t);
				return OUT;
			}

			sampler2D _MainTex;
 			sampler2D _DistortionTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half2 distortion = tex2D (_DistortionTex, IN.texcoord.zw).xy - half2(0.5, 0.5);
			    half2 uv0 = IN.texcoord.xy + _Amount * distortion;
				fixed4 c = SampleSpriteTexture (uv0) * IN.color;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
