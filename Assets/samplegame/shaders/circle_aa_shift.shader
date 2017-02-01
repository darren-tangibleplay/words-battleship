Shader "Custom/circle_aa_shift" {
    Properties {
    	_Length ("Length", float) = 0
        _InnerRadius ("InnerRadius", float) = 25
        _OuterRadius ("OuterRadius", float) = 100
        _GradientShift ("GradientShift", float) = 1.0
        _InnerColor ("InnerColor", Color) = (1.0, 1.0, 1.0, 1.0)
        _OuterColor ("OuterColor", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200
        Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
        Blend SrcAlpha OneMinusSrcAlpha 
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            float _PixelToUnit = 0.5;
            float _Length;
            float _InnerRadius;
            float _OuterRadius;
            float _GradientShift;
            float4 _InnerColor;
            float4 _OuterColor;
    
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            // Needed for TRANSFORM_TEX to work
            float4 _MainTex_ST;
    
            v2f vert(appdata_base v) {
                v2f o;
                float4 p = v.vertex;
                float side = p.z;
                float4 pp = mul(UNITY_MATRIX_MVP, float4(1.0, 0, 0, 0));
                pp = ComputeScreenPos(pp);                
               	float aa = v.texcoord.y / (length(pp.xy * _ScreenParams.xy));
                p *= lerp(_InnerRadius - aa, _OuterRadius + aa, v.texcoord.x);
                p.x += side * _Length;
                o.pos = mul( UNITY_MATRIX_MVP, float4(p.xy, 0, 1) );
                o.uv.x = v.texcoord.x;
                o.uv.y = 1 - v.texcoord.y;
                return o;
            }
    
            fixed4 frag(v2f i) : COLOR0 {
                float dd = i.uv.x * i.uv.x;
                //dd *= 2 - dd;
                float4 c = lerp(_InnerColor, _OuterColor, pow(dd, _GradientShift));
                c.a *= i.uv.y;
                return c;
            }
            ENDCG
        }
    } 
}
