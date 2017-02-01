Shader "Custom/vignetting" {
	Properties {
		_Inner ("Inner", Color) = (1.0, 1.0, 1.0, 1.0)
        _Outer ("Outer", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
        Blend One OneMinusSrcAlpha
        
		Pass {
			CGPROGRAM
			#pragma vertex vert
	        #pragma fragment frag

	        #include "UnityCG.cginc"
	        
	        float4 _Inner;
            float4 _Outer;
	
	        struct v2f {
	            float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
	        };
	        
	        // Needed for TRANSFORM_TEX to work
	        float4 _MainTex_ST;
	
	        v2f vert (appdata_base v) {
	            v2f o;
			    o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
			    o.uv = (v.texcoord - float2(0.5, 0.5)) * 1.414;
			    return o;
	        }
	
	        fixed4 frag (v2f i) : COLOR0 {
                float dd = dot(i.uv, i.uv);
                return lerp(_Inner, _Outer, dd);
	        }
			ENDCG
		}
	} 
}
