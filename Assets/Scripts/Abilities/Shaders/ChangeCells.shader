Shader "Terminal Eden/Ability/ChangeCells"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        textureSize("Texture Size", Int)= 1000
        _NewColor ("New Color", Color) = (0, 1, 0, 1)
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

            int textureSize;
            float4 _SelectedCells[100];
            float1 _SelectedCellsSize = 100;

            fixed4 _NewColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 c = tex2D(_MainTex, uv);

                float2 coords = float2(ceil(uv.x * textureSize), ceil(uv.y * textureSize));                
                for (int j = 0; j < _SelectedCellsSize; j++) {
                    if (abs(_SelectedCells[j].x - coords.x) < 0.001 && abs(_SelectedCells[j].y - coords.y) < 0.001 ) {
                        return _NewColor;
                    }
                }
                return c;
            }
            ENDCG
        }
    }
}
