Shader "Custom/Monochrome" {
    Properties{
    _MainTex("Texture", 2D) = "white" {}
    _Transparency("Transparency", Range(0, 1)) = 1
    }

        SubShader{
            Tags { "Queue" = "Transparent" }
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                v2f vert(appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                half4 frag(v2f i) : SV_Target{
    half4 col = tex2D(_MainTex, i.uv);
    half gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
    return half4(gray, gray, gray, col.a * _Transparency);
                }
                ENDCG
            }
    }
}