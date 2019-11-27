// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Skybox" {
    Properties {
		_NoiseTex ("Noise", 2D) = "white" {}
		_TextTex ("Text Map", 2D) = "white" {}
        _SkyColor ("Sky Color", Color) = (0.05, 0.05, 0.1, 1)
        _CloudColor ("Cloud Color", Color) = (0.1, 0.1, 0.2, 1)
        _DigitColor ("Digit Color", Color) = (1, 0, 0, 1)
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    struct appdata {
        float4 position : POSITION;
        float3 texcoord : TEXCOORD0;
    };
    
    struct v2f {
        float4 position : SV_POSITION;
        float3 texcoord : TEXCOORD0;
    };
    
    half4 _Color1;
    half4 _Color2;
    half4 _UpVector;
    half _Intensity;
    half _Exponent;
    
    v2f vert (appdata v) {
        v2f o;
        o.position = UnityObjectToClipPos (v.position);
        o.texcoord = v.texcoord;
        return o;
    }

    sampler2D _NoiseTex;
    sampler2D _TextTex;
    half4 _SkyColor;
    half4 _CloudColor;
    half4 _DigitColor;

    float sampleText(float x, float z) {
        if (abs(x) < 1.0 && abs(z) < 1.0) {
            float2 coord = float2(x / 2 + 0.5, z / 2 + 0.5);
            coord -= coord % (1.0 / 128.0) - (1.0 / 256.0);
            return tex2Dlod(_TextTex, float4(coord.x, coord.y, 0, 0)).r;
        } else {
            return 0;
        }
    }

    float sampleWeakBlurText(float x, float z) {
        if (abs(x) < 1.0 && abs(z) < 1.0) {
            return tex2Dlod(_TextTex, float4(x / 2 + 0.5, z / 2 + 0.5, 0, 1)).r;
        } else {
            return 0;
        }
    }

    float sampleBlurText(float x, float z) {
        if (abs(x) < 1.0 && abs(z) < 1.0) {
            return tex2Dlod(_TextTex, float4(x / 2 + 0.5, z / 2 + 0.5, 0, 2)).r;
        } else {
            return 0;
        }
    }

    float getCloudiness(half3 texcoord, float realy) {
        float cloudiness = tex2D(
            _NoiseTex, 
            float2(texcoord.x, texcoord.z)
        ).r;
        float diminisher = pow(1.0 - realy, 2.0);
        cloudiness = saturate(cloudiness - diminisher - 0.2) / (1 - diminisher * 0.7);
        return cloudiness;
    }

    float getText(half3 texcoord) {
        return sampleText(texcoord.x, texcoord.z);
    }

    float getWeakBlurredText(half3 texcoord) {
        return sampleWeakBlurText(texcoord.x, texcoord.z);
    }

    float getStrongBlurredText(half3 texcoord) {
        float tx = texcoord.x, tz = texcoord.z, d = 0.03, d2 = d * 2.0, d3 = d * 3.0;
        float blurred = 
            sampleBlurText(tx, tz)
            + sampleBlurText(tx + d, tz + d)
            + sampleBlurText(tx + d, tz - d)
            + sampleBlurText(tx - d, tz + d)
            + sampleBlurText(tx - d, tz - d)
            + sampleBlurText(tx + d2, tz + d2)
            + sampleBlurText(tx + d2, tz - d2)
            + sampleBlurText(tx - d2, tz + d2)
            + sampleBlurText(tx - d2, tz - d2)
            + sampleBlurText(tx + d3, tz)
            + sampleBlurText(tx - d3, tz)
            + sampleBlurText(tx, tz + d3)
            + sampleBlurText(tx, tz - d3);
        blurred /= 13.0;
        return blurred;
    }
    
    fixed4 frag (v2f i) : COLOR {
        if (i.texcoord.y > 0.2) {
            half3 texcoord = i.texcoord;
            texcoord /= i.texcoord.y;
            float cloudiness = getCloudiness(texcoord, i.texcoord.y);
            texcoord /= 2;
            float textValue = getText(texcoord);
            float weakBlur = getWeakBlurredText(texcoord);
            float blurred = pow(getStrongBlurredText(texcoord), 0.5);

            half4 cloudColor = _CloudColor;
            cloudColor += lerp(0, lerp(_DigitColor, 0, 1 - cloudiness), blurred);
            float textGlow = lerp(textValue, weakBlur, saturate(cloudiness * 2.0));
            half4 skyColor = lerp(_SkyColor, _DigitColor, textGlow);
            return lerp(skyColor, cloudColor, pow(saturate(cloudiness * 1.5), 0.7));
        } else {
            return _SkyColor;
        }
    }

    ENDCG

    SubShader {
        Tags { "RenderType"="Background" "Queue"="Background" }
        Pass {
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
