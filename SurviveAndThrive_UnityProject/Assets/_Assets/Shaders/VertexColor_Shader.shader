Shader "FourNomads/VertexColor_Shader"
{
	Properties { 
		_Shininess("Shininess", Range(0, 1)) = 0.078
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert
		half _Shininess;

		struct Input {
			fixed3 vertColors;
		};

		void vert (inout appdata_full v, out Input o) {
			o.vertColors = v.color.rgb;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.vertColors.rgb;
			o.Specular = 1;
			o.Gloss = _Shininess;
		}
		ENDCG
	}
	FallBack "Specular"
}