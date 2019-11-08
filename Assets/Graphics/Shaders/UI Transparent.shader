Shader "ACP/UI Transparent" {

Properties
{
	_Color ("Color", Color) = (1,1,1,0)
	_MainTex("Texture", 2D) = "" {}
}

SubShader 
{
	Tags {Queue=Transparent}
	Cull Off
	Zwrite Off
	//ZTest Less
	Blend SrcAlpha OneMinusSrcAlpha

	Pass
	{
		SetTexture[_MainTex] {
			ConstantColor[_Color] 
			Combine Texture * constant
		}
	}
}


FallBack "Transparent/Diffuse"

}