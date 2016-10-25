Shader "Hidden/Pixel_Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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

			uniform float _pixelDensity;
			uniform float _colorOffset;
			uniform float _height;
			uniform float _width;

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 pos = i.uv;
				pos.x = ceil(pos.x * _width / _pixelDensity) / (_width / _pixelDensity);
				pos.y = ceil(pos.y * _height / _pixelDensity) / (_height / _pixelDensity);
				fixed4 col = tex2D(_MainTex, pos);
				// just invert the colors
				//col = 1 - col;
				col = ceil(col * _colorOffset) / _colorOffset;
				return col;
			}
			ENDCG
		}
	}
}
