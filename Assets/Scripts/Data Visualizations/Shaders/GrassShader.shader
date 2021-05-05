Shader "Unlit/GrassShader"
{
    Properties
    {
        [MainTexture] _BaseMap("Grass Texture", 2D) = "white" {}
        _Height ("Height", Float) = 1.0
        _Base ("Base", Float) = 1.0
        _Tint ("Tint", Color) = (0.5, 0.5, 0.5, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        LOD 100

        Pass
        {
            Name "Geometry Pass"
            Tags {"LightMode"="UniversalForward"}

            HLSLPROGRAM

            // #pragma multicompile 

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            ENDHLSL
        }
    }
}
