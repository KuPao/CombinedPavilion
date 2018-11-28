Shader "Tessellation/Clip Opposite" {
        Properties {
            _Tess ("Tessellation", Range(1,32)) = 1
            _maxDist ("Tess Fade Distance", Range(0, 500.0)) = 25.0
            _MainTex ("Base (RGB)", 2D) = "white" {}
            _MOS ("Metallic (R), Occlussion (G), Smoothness (B)", 2D) = "white" {}
            _DispTex ("Disp Texture", 2D) = "gray" {}
            _NormalMap ("Normalmap", 2D) = "bump" {}
            _Displacement ("Displacement", Range(0, 1.0)) = 0.3
            _DispOffset ("Disp Offset", Range(0, 1)) = 0.5
            _Color ("Color", color) = (1,1,1,0)
            _Metallic ("Metallic", Range(0, 1)) = 0.5
            _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        }
        SubShader {
            Tags { "RenderType"="Opaque" }
            LOD 300
            
            CGPROGRAM
            #pragma surface surf Standard addshadow fullforwardshadows
            #pragma target 5.0
            #include "Tessellation.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };

            float _Tess;
            float _maxDist;

            sampler2D _DispTex;
            sampler2D _MOS;
            uniform float4 _DispTex_ST;
            float _Displacement;
            float _DispOffset;

            struct Input {
                float2 uv_MainTex;
                float2 uv_MOS;
				float3 worldPos;
            };

            sampler2D _MainTex;
            sampler2D _NormalMap;
            fixed4 _Color;
            float _Metallic;
            float _Glossiness;

            void surf (Input IN, inout SurfaceOutputStandard o) {
                half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
                half4 mos = tex2D (_MOS, IN.uv_MOS);
				clip (IN.worldPos.x < 0 ? 1 : -1);

                o.Albedo = c.rgb;
                o.Metallic = mos.r * _Metallic;
                o.Smoothness = mos.b *_Glossiness;
                o.Occlusion = mos.g;
                o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
            }
            ENDCG
        }
        FallBack "Standard"
    }