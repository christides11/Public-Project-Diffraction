Shader "Hidden/SepiaShader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (0.191, -0.054, -0.221, 0.0)
	}

		SubShader{
			Pass {
				ZTest Always Cull Off ZWrite Off

		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		half4 _MainTex_ST;
		float4 _Color;

		fixed4 frag(v2f_img i) : SV_Target
		{
			fixed4 original = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));

		// get intensity value (Y part of YIQ color space)
		fixed Y = dot(fixed3(0.299, 0.587, 0.114), original.rgb);

		// Convert to Sepia Tone by adding constant
		fixed4 sepiaConvert = _Color;
		fixed4 output = sepiaConvert + Y;
		output.a = original.a;

		return output;
	}
	ENDCG

		}
	}

		Fallback off

}