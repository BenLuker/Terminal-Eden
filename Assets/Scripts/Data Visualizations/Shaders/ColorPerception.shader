Shader "Terminal Eden/Data Visualization/Color"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Grown ("Grown Color", Color) = (0, 1, 0, 1)
        _Overgrown ("Overgrown Color", Color) = (0, 0, 1, 1)
        _Fire ("Fire Color", Color) = (1, 0, 0, 1)
        _Burned ("Burned Color", Color) = (0, 0, 0, 1)

        _GrownDisplay ("Grown Color Display", Color) = (0.04, 0.4, 0.14, 1)
        _OvergrownDisplay ("Overgrown Color Display", Color) = (0, 0.3, 0.22, 1)
        _FireDisplay ("Fire Color Display", Color) = (1, 0, 0, 1)
        _BurnedDisplay ("Burned Color Display", Color) = (0.24, 0.16, 0.07, 1)

        _HighlightColor ("Highlight Color", Color) = (1, 1, 0, 0.2)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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
            float percentageVisible;

            fixed4 _Grown;
            fixed4 _Overgrown;
            fixed4 _Fire;
            fixed4 _Burned;

            fixed4 _GrownDisplay;
            fixed4 _OvergrownDisplay;
            fixed4 _FireDisplay;
            fixed4 _BurnedDisplay;
            fixed4 _HighlightColor;

            bool colorsMatch(fixed4 a, fixed4 b, float tolerance = 0.01) {
                half3 delta = abs(a.rgb - b.rgb);
                return length(delta) < tolerance ? true : false;
            }

            bool inMask(float2 uv) {
                return (uv.x < 0.5 + (percentageVisible / 2) && uv.x > 0.5 - (percentageVisible / 2) && uv.y < 0.5 + (percentageVisible / 2) && uv.y > 0.5 - (percentageVisible / 2)) ? true : false;
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
                fixed4 c = tex2D(_MainTex, uv);

                float2 coords = float2(ceil(uv.x * textureSize), ceil(uv.y * textureSize));                
                fixed4 h = fixed4(_HighlightColor.r, _HighlightColor.g, _HighlightColor.b , 1);
                float t = 0;
                for (int j = 0; j < _SelectedCellsSize; j++) {
                    if (abs(_SelectedCells[j].x - coords.x) < 0.001 && abs(_SelectedCells[j].y - coords.y) < 0.001 ) {
                        t = _HighlightColor.a;
                        break;
                    }
                }

                if (inMask(uv)) {
                    if (colorsMatch(c, _Grown)) return lerp(_GrownDisplay, _HighlightColor, t);
                    if (colorsMatch(c, _Overgrown)) return lerp(_OvergrownDisplay, _HighlightColor, t);
                    if (colorsMatch(c, _Fire)) return lerp(_FireDisplay, _HighlightColor, t);
                    if (colorsMatch(c, _Burned)) return lerp(_BurnedDisplay, _HighlightColor, t);
                } else {
                    return fixed4(0,0,0,0);
                }

                return c;

            }
            ENDCG
        }
    }
}
