Shader "Terminal Eden/Calculate Levels"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            int fromSize;
            int toSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Smaller Texture Coordinate
                float2 coords = float2(ceil(i.uv.x * toSize), ceil(i.uv.y * toSize));

                // Bigger Texture Step
                float step = 1 / (float)fromSize;

                // From the small coordinate, find all 4 big coordinates
                float2 c1 = float2(coords.x - (step / 2), coords.y);
                float2 c2 = float2(coords.x - ((step * 3) / 2), coords.y);
                float2 c3 = float2(coords.x, coords.y - (step/2));
                float2 c4 = float2(coords.x, coords.y - ((step * 3) / 2));

                // Add all 4 big coordinates together and divide by 4, and return to this pixel
                float c = 0;
                c += tex2D(_MainTex, c1).g;
                c += tex2D(_MainTex, c2).g;
                c += tex2D(_MainTex, c3).g;
                c += tex2D(_MainTex, c4).g;

                return fixed4(0, c / 4, 0, 1);

                // fixed4 c = fixed4(0,0,0,1);
                // c += tex2D(_MainTex, c1);
                // c += tex2D(_MainTex, c2);
                // c += tex2D(_MainTex, c3);
                // c += tex2D(_MainTex, c4);
                // return c / 4;
            }
            ENDCG
        }
    }
}
