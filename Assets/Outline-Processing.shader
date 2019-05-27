Shader "WallStudio/Outline/Processing"
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 _Direction;
            static const int radius = 1;
            fixed4 frag (v2f i) : SV_Target
            {
                float depth = tex2D(_MainTex, i.uv);

                // 24近傍（5x5）で最小値（最前面）を探索
                for(int j = -radius; j <= radius; j++)
                for(int k = -radius; k <= radius; k++)
                {
                    fixed nearDepth = tex2D(_MainTex, i.uv + _Direction.xy * half2(j, k));

                    fixed condition = step(nearDepth, depth);
                    depth = depth * (1-condition) + nearDepth * condition;
                }

                return fixed4(depth, depth, depth, 1);
            }
            ENDCG
        }
    }
}