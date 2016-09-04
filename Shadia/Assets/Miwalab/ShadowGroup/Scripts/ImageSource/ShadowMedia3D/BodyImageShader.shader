Shader "Unlit/BodyImageShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_VertXMin("VertXMin", float) = -5
		_VertXMax("VertXMax", float) = 5
		_VertYMin("VertYMin", float) = -1
		_VertYMax("VertYMax", float) = 2
		_VertZMin("VertZMin", float) = 0
		_VertZMax("VertZMax", float) = 8
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 color : COLOR0;
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float   _VertXMin;
			float	_VertXMax;
			float	_VertYMin;
			float	_VertYMax;
			float	_VertZMin;
			float	_VertZMax;


			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				if (_VertXMin < v.vertex[0] &&
					_VertXMax > v.vertex[0] &&
					_VertYMin < v.vertex[1] &&
					_VertYMax > v.vertex[1] &&
					_VertZMin < v.vertex[2] &&
					_VertZMax > v.vertex[2]) 
				{
					o.color[0] = 1;
					o.color[1] = 1;
					o.color[2] = 1;

				}
				else {
					o.color[0] = 0;
					o.color[1] = 0;
					o.color[2] = 0;
				}
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				return i.color;
			}
			ENDCG
		}
	}
}
