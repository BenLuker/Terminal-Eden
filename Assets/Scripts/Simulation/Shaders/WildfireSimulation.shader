Shader "Terminal Eden/Simulation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Wind ("Wind Direction and Speed", Vector) = (0,1,2,0)

        _Grown ("Grown Color", Color) = (0, 1, 0, 1)
        _Overgrown ("Overgrown Color", Color) = (0, 0, 1, 1)
        _Fire ("Fire Color", Color) = (1, 0, 0, 1)
        _Burned ("Burned Color", Color) = (0, 0, 0, 1)

        _GrowSpawn ("Grow Spawn Rate", Float) = 0.000005
        _OvergrowSpawn ("Overgrow Spawn Rate", Float) = 0.0000005
        _FireSpawn ("Fire Spawn Rate", Float) = 0.00001

        _GrowSpread ("Grow Spread", Range (0, 1)) = 0.001
        _OvergrowSpread ("Overgrow Spread", Range (0, 1)) = 0.001
        _FireSpread ("Fire Spread", Range (0, 1)) = 0.2
        _FireWithFuelSpread ("Fire with Fuel Spread", Range (0, 1)) = 0.3
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

            sampler2D _MainTex;
            int textureSize;

            fixed4 _Grown;
            fixed4 _Overgrown;
            fixed4 _Fire;
            fixed4 _Burned;

            float4 _Wind;
            
            float _GrowSpawn;
            float _OvergrowSpawn;
            float _FireSpawn;
            
            float _GrowSpread;
            float _OvergrowSpread;
            float _FireSpread;
            float _FireWithFuelSpread;

            float random(float2 Seed, float Min = 0, float Max = 1)
            {
                float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
                return lerp(Min, Max, randomno);
            }

            float randomTime(float2 Seed, float Min = 0, float Max = 1) {
                return random(_Time.xy + Seed, Min, Max);
            }
            
            float randomHighPrecision(float2 Seed, int precision, float iterator = 0) {
                float n = 0;
                for (int i = 0; i < precision; i++) {
                    n += randomTime(Seed + float2(iterator, iterator))/pow(100,i);
                    iterator += 0.01;
                }                
                return n;
            }

            bool colorsMatch(fixed4 a, fixed4 b, float tolerance = 0.01) {
                half3 delta = abs(a.rgb - b.rgb);
                return length(delta) < tolerance ? true : false;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                _Wind = float4(1,0,1,0);

                float2 uv = i.uv;
                float step = 1 / (float)textureSize;

                fixed4 neighborData = fixed4(0,0,0,1);

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

                // Calculate Extended Neighbors (From Wind)
                for (int i = 1; i <= _Wind.z; i++) {
                    neighborData += tex2D(_MainTex, uv + float2(-_Wind.x * (i + 1) * step, -_Wind.y * (i + 1) * step));
                }

                // Grab Current Pixel
                fixed4 c = tex2D(_MainTex, uv);

                // Determine state
                // If Burned
                if (colorsMatch(c, _Burned)) {
                    if (randomTime(uv) / (neighborData.g + neighborData.b) < _GrowSpread) {
                        c = _Grown;
                    } else if (randomHighPrecision(uv, 5) < _GrowSpawn) {
                        c = _Grown;
                    } else {
                        c = _Burned;
                    }
                }

                // If Grown
                else if (colorsMatch(c, _Grown)) {
                    if (randomTime(uv) / neighborData.r < _FireSpread) {
                        c = _Fire;
                    } else if (randomTime(uv) / neighborData.b < _OvergrowSpread) {
                        c = _Overgrown;
                    } else if (randomHighPrecision(uv, 5) < _OvergrowSpawn) {
                        c = _Overgrown;
                    } else if (randomHighPrecision(uv, 5) < _FireSpawn) {
                        c = _Fire;
                    } else {
                        c = _Grown;
                    }
                }

                // If Overgrown
                else if (colorsMatch(c, _Overgrown)) {
                    if (randomTime(uv) / neighborData.r < _FireWithFuelSpread) {
                        c = _Fire;
                    } else if (randomHighPrecision(uv, 5) < _FireSpawn) {
                        c = _Fire;
                    } else {
                        c = _Overgrown;
                    }
                }

                // If Fire
                else if (colorsMatch(c, _Fire)) {
                    c = _Burned;
                }

                return c;

            }
            ENDCG
        }
    }
}
