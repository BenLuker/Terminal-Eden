Shader "Hidden/WildfireSimulation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            int textureSize;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float step = 1 / (float)textureSize;

                float neighbors = 0;

                // Top Row
                neighbors += tex2D(_MainTex, uv + float2(-step, step)).r; 
                neighbors += tex2D(_MainTex, uv + float2(0, step)).r; 
                neighbors += tex2D(_MainTex, uv + float2(step, step)).r; 

                // Middle Row
                neighbors += tex2D(_MainTex, uv + float2(-step, 0)).r; 
                neighbors += tex2D(_MainTex, uv + float2(step, 0)).r; 

                // Bottom Row
                neighbors += tex2D(_MainTex, uv + float2(-step, -step)).r; 
                neighbors += tex2D(_MainTex, uv + float2(0, -step)).r; 
                neighbors += tex2D(_MainTex, uv + float2(step, -step)).r;

                if (tex2D(_MainTex, uv).r > 0)
                {
                    return (neighbors == 2 || neighbors == 3) ? fixed4(1,1,1,1) : fixed4(0,0,0,1);
                }
                else 
                {
                    return (neighbors == 3) ? fixed4(1,1,1,1) : fixed4(0,0,0,1);
                }
            }
            ENDCG
        }
    }
}
