Shader "Unlit/PBR_Test"
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
        LOD 100

        Pass
        {
            CGPROGRAM
            // #pragma vertex vert
            // #pragma fragment frag
            // // make fog work
            // #pragma multi_compile_fog

            // #include "UnityCG.cginc"

// Material Keywords
#pragma shader_feature _NORMALMAP
#pragma shader_feature _ALPHATEST_ON
#pragma shader_feature _ALPHAPREMULTIPLY_ON
#pragma shader_feature _EMISSION
//#pragma shader_feature _METALLICSPECGLOSSMAP
//#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
//#pragma shader_feature _OCCLUSIONMAP
 
//#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
//#pragma shader_feature _ENVIRONMENTREFLECTIONS_OFF
//#pragma shader_feature _SPECULAR_SETUP
#pragma shader_feature _RECEIVE_SHADOWS_OFF
 
// URP Keywords
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
#pragma multi_compile _ _SHADOWS_SOFT
#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
 
// Unity defined keywords
#pragma multi_compile _ DIRLIGHTMAP_COMBINED
#pragma multi_compile _ LIGHTMAP_ON
#pragma multi_compile_fog
 
// Some added includes, required to use the Lighting functions
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
// And this one for the SurfaceData struct and albedo/normal/emission sampling functions.
// Note : It also defines the _BaseMap, _BumpMap and _EmissionMap textures for us, so we should use these as Shaderlab Properties too.
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
 
struct Attributes {
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float4 color        : COLOR;
    float2 uv           : TEXCOORD0;
    float2 lightmapUV   : TEXCOORD1;
};
 
struct Varyings {
    float4 positionCS               : SV_POSITION;
    float4 color                    : COLOR;
    float2 uv                       : TEXCOORD0;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
    // Note this macro is using TEXCOORD1
#ifdef REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
    float3 positionWS               : TEXCOORD2;
#endif
    float3 normalWS                 : TEXCOORD3;
#ifdef _NORMALMAP
    float4 tangentWS                : TEXCOORD4;
#endif
    float3 viewDirWS                : TEXCOORD5;
    half4 fogFactorAndVertexLight   : TEXCOORD6;
    // x: fogFactor, yzw: vertex light
#ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
    float4 shadowCoord              : TEXCOORD7;
#endif
};
 
//TEXTURE2D(_BaseMap);
//SAMPLER(sampler_BaseMap);
// Removed, since SurfaceInput.hlsl now defines the _BaseMap for us

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
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
