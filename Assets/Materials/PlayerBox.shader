// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PlayerBox" {
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 300

        Cull Front
		
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
        ZTest Equal

        Pass {
            ZTest Less
            ZWrite On
            ColorMask 0
        }

		
		Pass {
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
        
            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDirection: TEXCOORD1;
                float2 uv_MainTex : TEXCOORD2;
            };
        
            float4 _MainTex_ST;
        
            v2f vert(appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDirection = normalize(UnityWorldSpaceViewDir(worldPos));
                o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
        
            sampler2D _MainTex;
        
            float4 frag(v2f i) : COLOR {
                float fresnel = pow((1.0 - saturate(dot(-i.worldNormal, i.viewDirection))), 1.0f);
                half4 c = half4(fresnel, fresnel, fresnel, 0.5); 
                return c;
            }
			ENDCG
		}
	}
}
