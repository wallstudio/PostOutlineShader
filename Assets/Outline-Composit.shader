Shader "WallStudio/Outline/Composit"
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


            fixed GetEdge(fixed depth, fixed infrated)
            {
                fixed condition = step(depth, infrated);
                return condition;
            }

            sampler2D _Depth;
            sampler2D _InfratedDepth;
            half4 _Direction;
            static const int radius = 0;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 main = tex2D(_MainTex, i.uv);

                for(int j = -radius; j <= radius; j++)
                for(int k = -radius; k <= radius; k++)
                {
                    fixed depth = tex2D(_Depth, i.uv + _Direction.xy * half2(j, k));
                    fixed infrated = tex2D(_InfratedDepth, i.uv + _Direction.xy * half2(j, k));
                    fixed edge = GetEdge(depth, infrated);

                    half condition = step(edge, 0.5);
                    main = main * (1-condition) + fixed4(edge, edge, edge, main.a) * condition;
                }

                return main;
            }
            ENDCG
        }
    }
}