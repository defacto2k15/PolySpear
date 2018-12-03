// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/BlurryHexShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_GridColor ("GridColor", Color) = (0,0,0,0)
		_Size ("Size", float) = 1
		_Thickness ("Thickness", Range(0,1)) = 0.075
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		float _Size;
		float _Thickness;
		fixed4 _Color;
		fixed4 _GridColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float smoothBounded(float mn, float mx, float t) {
			float tt = (t-mn)/(mx-mn);
			if(tt <= 0)
				return 0;
			if(tt >= 1)
				return 1;
			return 3*tt*tt - 2*tt*tt*tt;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			//o.Albedo = IN.worldPos.xyz;
			//return;

			float sqrt32 = 0.86602540378443864676372317075294;
			float yy = 1 / sqrt32 * IN.worldPos.z / _Size + 1;
			float xx = IN.worldPos.x / _Size + yy / 2 + 0.5;
			float uu = (xx + yy)/3;
			float vv = (xx - yy + uu + 1) / 2;
			float u = floor ((floor (xx) + floor (yy)) / 3);
			float v = floor ((xx - yy + u + 1) / 2);
			float du = uu - u;
			float dv = vv - v;
			float dist = max(abs(du+dv-1.5), max(abs(2*du-dv - 0.5), abs(2*dv-du-1)));

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			float s = smoothBounded(1,1-_Thickness,dist);
			o.Albedo = s*c.rgb + (1-s)*_GridColor;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
