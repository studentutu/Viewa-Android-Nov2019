Shader "ACP/UI Alpha SBS" {

   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _AlphaOffsetX ("alpha offset x", float) = 0.5
      _AlphaOffsetY ("alpha offset y", float) = 0
      _Cutoff ("Cutoff", Range (0,1)) = .5
   }

   SubShader {
   	Tags {Queue=Transparent}
	Cull Off
	Zwrite Off
	//ZTest Less
	Blend SrcAlpha OneMinusSrcAlpha
//   	AlphaTest Less [_Cutoff]
         CGPROGRAM
	     #pragma surface surf NoLighting
	     //#pragma surface surf Lambert
         sampler2D _MainTex;
         float _AlphaOffsetX;
         float _AlphaOffsetY;

         struct Input {
            float2 uv_MainTex;
         };

         void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);

			if (IN.uv_MainTex.x < 0.5) {
	            IN.uv_MainTex.x += _AlphaOffsetX;
	            IN.uv_MainTex.y += _AlphaOffsetY;
	            half4 d = tex2D (_MainTex, IN.uv_MainTex);
	            o.Albedo = c.rgb;
	            o.Alpha = (d.r*+1);
			}
			else {
	            o.Albedo = c.rgb;
	            o.Alpha = 0;
			}
         }
	   fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	    {
	        fixed4 c;
	        c.rgb = s.Albedo; 
	        c.a = s.Alpha;
	        return c;
	    }
         ENDCG
	} 
   
   
   //FallBack "Transparent"
	FallBack "ACP/UI Transparent"

}
