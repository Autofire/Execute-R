// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PlayerBox" {
    Properties {
		_NoiseTex ("Noise", 2D) = "white" {}
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
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDirection: TEXCOORD2;
            };
        
        
            v2f vert(appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDirection = normalize(UnityWorldSpaceViewDir(o.worldPos));
                return o;
            }
        
            sampler2D _NoiseTex;
        
            float4 frag(v2f i) : COLOR {
                float fresnel = pow((1.0 - saturate(dot(-i.worldNormal, i.viewDirection))), 1.0f);
                float noiseValue = tex2D(_NoiseTex, float2(i.worldPos.x / 5.0 + i.worldPos.z / 5.0, i.worldPos.y / 5.0 + _Time.y * 0.1)).r;
                noiseValue *= noiseValue * noiseValue;
                if ((i.worldPos.y - (_Time.y * 0.1 % 0.05) + 1.0) % 0.05 < 0.03) {
                    noiseValue = 0.0;
                }
                half4 c = half4(0, 1, 1, fresnel * noiseValue);
                return c;
            }
			ENDCG
		}
	}
}
