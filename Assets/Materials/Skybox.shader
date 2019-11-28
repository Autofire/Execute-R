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

    float sampleChar(float2 st, float n) {
        int digit = 0;
        
        if (n < 1. ) { digit = 9712; }
        else if (n < 2. ) { digit = 21158; }
        else if (n < 3. ) { digit = 25231; }
        else if (n < 4. ) { digit = 23187; }
        else if (n < 5. ) { digit = 23498; }
        else if (n < 6. ) { digit = 31702; }
        else if (n < 7. ) { digit = 25202; }
        else if (n < 8. ) { digit = 30163; }
        else if (n < 9. ) { digit = 18928; }
        else if (n < 10. ) { digit = 23531; }
        else if (n < 11. ) { digit = 29128; }
        else if (n < 12. ) { digit = 17493; }
        else if (n < 13. ) { digit = 7774; }
        else if (n < 14. ) { digit = 31141; }
        else if (n < 15. ) { digit = 29264; }
        else if (n < 16. ) { digit = 3641; }
        else if (n < 17. ) { digit = 31315; }
        else if (n < 18. ) { digit = 31406; }
        else if (n < 19. ) { digit = 30864; }
        else if (n < 20. ) { digit = 31208; }
        else { digit = 1; }

        int pixel = floor(st.x * 3.0) + 3.0 * floor(st.y * 5.0);

        return (digit & (1 << pixel)) > 0 ? 1.0 : 0.0;
    }
 
    float noise(float2 vect) {
        return frac(sin(dot(vect, float2(5372.156, 8452.751))) * 1643.268);
    }

    float sampleText(float x, float z) {
        if (abs(x) < 1.0 && abs(z) < 1.0) {
            float2 coord = float2(x / 2 + 0.5, z / 2 + 0.5);
            float2 charCoord = coord % (1.0 / 128.0) * 128.0;
            charCoord.x /= 0.4;
            charCoord.x -= 0.3;
            charCoord.y /= 0.8;
            charCoord.y -= 0.1;
            coord -= coord % (1.0 / 128.0) - (1.0 / 256.0);
            float pixel = 0.0;
            if (charCoord.x > 0 && charCoord.x < 1 && charCoord.y > 0 && charCoord.y < 1) {
                pixel = sampleChar(charCoord, noise(coord) * 20.0);
            }
            return tex2Dlod(_TextTex, float4(coord.x, coord.y, 0, 0)).r * pixel;
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
        texcoord.x -= _Time.x * 0.3;
        texcoord.z += _Time.x * 0.1;
        half4 channels = tex2D(
            _NoiseTex, 
            float2(texcoord.x, texcoord.z)
        );
        float channelCycle = (_Time.x * 4.0f) % 3.0;
        float subCycle = channelCycle % 1.0;
        float cloudiness;
        if (channelCycle < 1) {
            cloudiness = lerp(channels.r, channels.b, subCycle);
        } else if (channelCycle < 2) {
            cloudiness = lerp(channels.b, channels.g, subCycle);
        } else {
            cloudiness = lerp(channels.g, channels.r, subCycle);
        }
        float diminisher = saturate(1.2 * pow(1.0 - realy, 8.0));
        cloudiness = saturate(cloudiness - diminisher - 0.13) / (1 - diminisher * 0.7);
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
        if (i.texcoord.y > 0.1) {
            half3 texcoord = i.texcoord;
            texcoord /= i.texcoord.y;
            texcoord /= 4;
            float cloudiness = getCloudiness(texcoord, i.texcoord.y);
            float textValue = getText(texcoord);
            float weakBlur = getWeakBlurredText(texcoord);
            float blurred = pow(getStrongBlurredText(texcoord), 0.5);

            half4 cloudColor = _CloudColor;
            cloudColor += lerp(0, lerp(_DigitColor, 0, 1 - cloudiness), blurred);
            float textGlow = lerp(textValue, weakBlur, saturate(cloudiness * 2.0));
            half4 skyColor = lerp(_SkyColor, _DigitColor, textGlow);
            return lerp(skyColor, cloudColor, pow(saturate(cloudiness * 1.8), 1.0));
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
