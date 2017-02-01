Shader "Custom/colorize_vertex" {
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200
        Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
        Blend SrcAlpha OneMinusSrcAlpha 
        Cull Off
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            
            // Needed for TRANSFORM_TEX to work
	        float4 _MainTex_ST;
    
            struct v2f {
                float4 pos : SV_POSITION;
                float4 col : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };
                
            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
                o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                o.col = v.color;
                return o;
            }
    
            fixed4 frag (v2f i) : COLOR0 
            { 
                return tex2D(_MainTex, i.uv) * i.col; // 
            }
            ENDCG
        }
    } 
}
