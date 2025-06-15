// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FX/Gem2Halves"
{
	Properties {
		_ColorLeft("Color Left", Color) = (1, 0, 0, 1)
		_ColorRight("Color Right", Color) = (0, 1, 0, 1)
		_ReflectionStrength ("Reflection Strength", Range(0.0,2.0)) = 1.0
		_EnvironmentLight ("Environment Light", Range(0.0,2.0)) = 1.0
		_Speed("Speed", Range(0.0,100.0)) = 1.0
		_Amplitude("Amplitude", Range(0.0,2.0)) = 1.0
		_Emission ("Emission", Range(0.0,2.0)) = 0.0
		[NoScaleOffset] _RefractTex ("Refraction Texture", Cube) = "" {}

	}
	SubShader {
		Tags {
			"Queue" = "Transparent"
		}
		// First pass - here we render the backfaces of the diamonds. Since those diamonds are more-or-less
		// convex objects, this is effectively rendering the inside of them.
		Pass {

			Cull Front
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
        
			struct v2f {
				float4 pos : SV_POSITION;
				float3 uv : TEXCOORD0;
				float3 objPos : TEXCOORD1;
			};
			half _Speed;
			half _Amplitude;

			v2f vert (float4 v : POSITION, float3 n : NORMAL)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v);

				// TexGen CubeReflect:
				// reflect view direction along the normal, in view space.
				float3 viewDir = normalize(ObjSpaceViewDir(v));
				o.uv = -reflect(viewDir, n);
				o.uv = mul(unity_ObjectToWorld, float4(o.uv,0)) + sin(_Time * _Speed) * _Amplitude;
				// Calculate the object's center in world space
				float4 centerWS = UnityObjectToClipPos(float4(0, 0, 0, 1));
				o.objPos = UnityObjectToClipPos(v) - centerWS;
				return o;
			}

			fixed4 _ColorLeft;
			fixed4 _ColorRight;
			samplerCUBE _RefractTex;
			half _EnvironmentLight;
			half _Emission;
			half4 frag (v2f i) : SV_Target
			{
				half3 refraction = texCUBE(_RefractTex, i.uv).rgb * ((i.objPos.x < 0) ? _ColorLeft.rgb : _ColorRight.rgb);
				half4 reflection = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.uv);
				reflection.rgb = DecodeHDR(reflection, unity_SpecCube0_HDR);
				half3 multiplier = reflection.rgb + _EnvironmentLight + _Emission;
				return half4(refraction.rgb * multiplier.rgb, 1.0f);
			}
			ENDCG 
		}

		// Second pass - here we render the front faces of the diamonds.
		Pass {
			ZWrite On
			Blend One One
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
        
			struct v2f {
				float4 pos : SV_POSITION;
				float3 uv : TEXCOORD0;
				float objPos : TEXCOORD1;
				half fresnel : TEXCOORD2;
			};
			half _Speed;
			half _Amplitude;

			v2f vert (float4 v : POSITION, float3 n : NORMAL)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v);

				// TexGen CubeReflect:
				// reflect view direction along the normal, in view space.
				float3 viewDir = normalize(ObjSpaceViewDir(v));
				o.uv = -reflect(viewDir, n);
				o.uv = mul(unity_ObjectToWorld, float4(o.uv,0)) + sin(_Time * _Speed) * _Amplitude;
				o.fresnel = 1.0 - saturate(dot(n,viewDir));
				float4 centerWS = UnityObjectToClipPos(float4(0, 0, 0, 1));
				o.objPos = UnityObjectToClipPos(v) - centerWS;
				return o;
			}
			
			fixed4 _ColorLeft;
			fixed4 _ColorRight;
			samplerCUBE _RefractTex;
			half _ReflectionStrength;
			half _EnvironmentLight;
			half _Emission;
			half4 frag (v2f i) : SV_Target
			{
				half3 refraction = texCUBE(_RefractTex, i.uv).rgb * ((i.objPos.x < 0) ? _ColorLeft.rgb : _ColorRight.rgb);
				half4 reflection = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.uv);
				reflection.rgb = DecodeHDR (reflection, unity_SpecCube0_HDR);
				half3 reflection2 = reflection * _ReflectionStrength * i.fresnel;
				half3 multiplier = reflection.rgb + _EnvironmentLight + _Emission;
				return fixed4(reflection2 + refraction.rgb * multiplier, 1.0f);
			}
			ENDCG
		}
	}
}
