// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/TransPixelate" {
	Properties{
		_CellSize("_CellSize", Float) = 0.0001
		_SourceTex ("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200


		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		struct v2f {
		float4 pos : SV_POSITION;
		float4 grabUV : TEXCOORD0;
	};

	float _CellSize;

	v2f vert(appdata_base v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.grabUV = ComputeGrabScreenPos(o.pos);
		return o;
	}

	sampler2D _SourceTex;

	float4 frag(v2f IN) : COLOR{
		float2 steppedUV = IN.grabUV.xy / IN.grabUV.w;
		steppedUV /= _CellSize;
		steppedUV = round(steppedUV);
		steppedUV *= _CellSize;
		return tex2D(_SourceTex, steppedUV);
	}
		ENDCG
	}
	}
}