Shader "Unlit/InitWildfireSimulation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _Grown ("Grown Color", Color) = (0, 1, 0, 1)
        _Overgrown ("Overgrown Color", Color) = (0, 0, 1, 1)
        _Fire ("Fire Color", Color) = (1, 0, 0, 1)
        _Burned ("Burned Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Grown;
            fixed4 _Overgrown;
            fixed4 _Fire;
            fixed4 _Burned;

            float2 gradientNoise_dir(float2 p)
            {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float gradientNoise(float2 UV, float Scale)
            {
                float2 p = UV * Scale;
                
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(gradientNoise_dir(ip), fp);
                float d01 = dot(gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                
                return (lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x)) + 0.5;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                if (gradientNoise(uv, 10) > 0.45) {
                    if (gradientNoise(uv, 10) < 0.7) {
                        return _Grown;
                    } else {
                        return _Overgrown;
                    }
                } else {
                    return _Burned;
                }
            }
            ENDCG
        }
    }
}
