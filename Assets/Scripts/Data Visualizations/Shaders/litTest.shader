Shader "Terminal Eden/Data Visualization/Vegetation"
{
    Properties {
    _BaseMap ("Base Texture", 2D) = "white" {}
    _BaseColor ("Example Colour", Color) = (0, 0.66, 0.73, 1)
    _Smoothness ("Smoothness", Float) = 0.5
 
    [Toggle(_ALPHATEST_ON)] _EnableAlphaTest("Enable Alpha Cutoff", Float) = 0.0
    _Cutoff ("Alpha Cutoff", Float) = 0.5
 
    [Toggle(_NORMALMAP)] _EnableBumpMap("Enable Normal/Bump Map", Float) = 0.0
    _BumpMap ("Normal/Bump Texture", 2D) = "bump" {}
    _BumpScale ("Bump Scale", Float) = 1
 
    [Toggle(_EMISSION)] _EnableEmission("Enable Emission", Float) = 0.0
    _EmissionMap ("Emission Texture", 2D) = "white" {}
    _EmissionColor ("Emission Colour", Color) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // int textureSize;
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

        // bool colorsMatch(fixed4 a, fixed4 b, float tolerance = 0.01) {
        //     half3 delta = abs(a.rgb - b.rgb);
        //     return length(delta) < tolerance ? true : false;
        // }

        // bool inMask(float2 uv) {
        //     return (uv.x < 0.5 + (percentageVisible / 2) && uv.x > 0.5 - (percentageVisible / 2) && uv.y < 0.5 + (percentageVisible / 2) && uv.y > 0.5 - (percentageVisible / 2)) ? true : false;
        // }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // float2 uv = IN.uv_MainTex;

            // // Selected Cells
            // float2 coords = float2(ceil(uv.x * textureSize), ceil(uv.y * textureSize));                
            // fixed4 h = fixed4(_HighlightColor.r, _HighlightColor.g, _HighlightColor.b , 1);
            
            // // Unrolled Loop for selected cells
            // float t = 0;
            // t += (abs(_SelectedCells[0].x - coords.x) < 0.001 && abs(_SelectedCells[0].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[1].x - coords.x) < 0.001 && abs(_SelectedCells[1].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[2].x - coords.x) < 0.001 && abs(_SelectedCells[2].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[3].x - coords.x) < 0.001 && abs(_SelectedCells[3].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[4].x - coords.x) < 0.001 && abs(_SelectedCells[4].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[5].x - coords.x) < 0.001 && abs(_SelectedCells[5].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[6].x - coords.x) < 0.001 && abs(_SelectedCells[6].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[7].x - coords.x) < 0.001 && abs(_SelectedCells[7].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[8].x - coords.x) < 0.001 && abs(_SelectedCells[8].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[9].x - coords.x) < 0.001 && abs(_SelectedCells[9].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[10].x - coords.x) < 0.001 && abs(_SelectedCells[10].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[11].x - coords.x) < 0.001 && abs(_SelectedCells[11].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[12].x - coords.x) < 0.001 && abs(_SelectedCells[12].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[13].x - coords.x) < 0.001 && abs(_SelectedCells[13].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[14].x - coords.x) < 0.001 && abs(_SelectedCells[14].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[15].x - coords.x) < 0.001 && abs(_SelectedCells[15].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[16].x - coords.x) < 0.001 && abs(_SelectedCells[16].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[17].x - coords.x) < 0.001 && abs(_SelectedCells[17].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[18].x - coords.x) < 0.001 && abs(_SelectedCells[18].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[19].x - coords.x) < 0.001 && abs(_SelectedCells[19].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;
            // t += (abs(_SelectedCells[20].x - coords.x) < 0.001 && abs(_SelectedCells[20].y - coords.y) < 0.001 ) ? _HighlightColor.a : 0;

            // if (inMask(uv)) {
            //         if (colorsMatch(c, _Grown)) return lerp(_GrownDisplay, _HighlightColor, t);
            //         if (colorsMatch(c, _Overgrown)) return lerp(_OvergrownDisplay, _HighlightColor, t);
            //         if (colorsMatch(c, _Fire)) return lerp(_FireDisplay, _HighlightColor, t);
            //         if (colorsMatch(c, _Burned)) return lerp(_BurnedDisplay, _HighlightColor, t);
            //     } else {
            //         return fixed4(0,0,0,0);
            //     }

            // fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            // o.Alpha = inMask(uv) ? 1 : 0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
