Shader "Unlit/AddTopography"
{
    Properties
    {
        [MainTexture] _topography ("Topography", 2D) = "white" {}
        _simulation ("Simulation", 2D) = "white" {}
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _topography;
            sampler2D _simulation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 sim = tex2D(_simulation, i.uv);
                fixed4 top = tex2D(_topography, i.uv);

                return fixed4(sim.r, sim.g, top.b, sim.a);
            }
            ENDCG
        }
    }
}
