Shader "Unlit/ButterflyShader2"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Num("Num", Float) = 0
		_MyTime("MyTyme", Float) = 0
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 100

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
				float4 _MainTex_ST;
				fixed4 _Color;
				float _Num;
				float _MyTime;

				v2f vert(appdata v)
				{
					v2f o;
					float time = sin(_MyTime * 200 + _Num - v.vertex.y)-3.14/2 ;
					float4 _vert
						= float4(
							sin(time)*v.vertex.x,
							v.vertex.y,
							- cos(time)*abs(v.vertex.x),
							v.vertex.w);

					o.vertex = mul(UNITY_MATRIX_MVP, _vert);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv);
					col.x = _Color.r;
					col.y = _Color.g;
					col.z = _Color.b;
					col.a = col.a * _Color.a;
					// apply fog
					return col;
				}
				ENDCG
			}
		}
}
