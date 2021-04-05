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
                if (abs(_SelectedCells[0].x - coords.x) < 0.001 && abs(_SelectedCells[0].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[1].x - coords.x) < 0.001 && abs(_SelectedCells[1].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[2].x - coords.x) < 0.001 && abs(_SelectedCells[2].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[3].x - coords.x) < 0.001 && abs(_SelectedCells[3].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[4].x - coords.x) < 0.001 && abs(_SelectedCells[4].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[5].x - coords.x) < 0.001 && abs(_SelectedCells[5].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[6].x - coords.x) < 0.001 && abs(_SelectedCells[6].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[7].x - coords.x) < 0.001 && abs(_SelectedCells[7].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[8].x - coords.x) < 0.001 && abs(_SelectedCells[8].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[9].x - coords.x) < 0.001 && abs(_SelectedCells[9].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[10].x - coords.x) < 0.001 && abs(_SelectedCells[10].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[11].x - coords.x) < 0.001 && abs(_SelectedCells[11].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[12].x - coords.x) < 0.001 && abs(_SelectedCells[12].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[13].x - coords.x) < 0.001 && abs(_SelectedCells[13].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[14].x - coords.x) < 0.001 && abs(_SelectedCells[14].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[15].x - coords.x) < 0.001 && abs(_SelectedCells[15].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[16].x - coords.x) < 0.001 && abs(_SelectedCells[16].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[17].x - coords.x) < 0.001 && abs(_SelectedCells[17].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[18].x - coords.x) < 0.001 && abs(_SelectedCells[18].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[19].x - coords.x) < 0.001 && abs(_SelectedCells[19].y - coords.y) < 0.001 ) return _NewColor;
                if (abs(_SelectedCells[20].x - coords.x) < 0.001 && abs(_SelectedCells[20].y - coords.y) < 0.001 ) return _NewColor;

                // for (int j = 0; j < _SelectedCellsSize; j++) {
                //     if (abs(_SelectedCells[j].x - coords.x) < 0.001 && abs(_SelectedCells[j].y - coords.y) < 0.001 ) {
                //         return _NewColor;
                //     }
                // }
                return c;
            }
            ENDCG
        }
    }
}
