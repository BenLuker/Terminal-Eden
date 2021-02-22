Shader "Terminal Eden/Perception/Vegetation Density"
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
            int textureSize;

            fixed4 _Grown;
            fixed4 _Overgrown;
            fixed4 _Fire;
            fixed4 _Burned;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float step = 1 / (float)textureSize;

                fixed4 neighborData = fixed4(0,0,0,1);
                int textureSize;

                // Calculate Immediate Neighbors
                // Top Row
                neighborData += tex2D(_MainTex, uv + float2(-step, step)); 
                neighborData += tex2D(_MainTex, uv + float2(0, step)); 
                neighborData += tex2D(_MainTex, uv + float2(step, step)); 

                // Middle Row
                neighborData += tex2D(_MainTex, uv + float2(-step, 0)); 
                neighborData += tex2D(_MainTex, uv + float2(step, 0)); 

                // Bottom Row
                neighborData += tex2D(_MainTex, uv + float2(-step, -step)); 
                neighborData += tex2D(_MainTex, uv + float2(0, -step)); 
                neighborData += tex2D(_MainTex, uv + float2(step, -step));

                // Grab Current Pixel
                fixed4 c = tex2D(_MainTex, uv);

                // Calculate vegetation density by remapping the B value
                float d = lerp(1, 0, neighborData.b/8);

                // Return value
                return fixed4(d,d,d,1);
            }
            ENDCG
        }
    }
}
