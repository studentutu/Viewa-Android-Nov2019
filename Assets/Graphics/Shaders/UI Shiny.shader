Shader "ACP/UI Shiny" {

Properties
{
	_Color ("Color", Color) = (1,1,1,1)
	_MainTex("Texture", 2D) = "" {}
	_MaskTex("Texture Mask", 2D) = "" { TexGen SphereMap }
}

SubShader 
{
	Cull Off
	Zwrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	Tags {Queue=Transparent}

	Pass
	{
		SetTexture[_MainTex] {
			ConstantColor[_Color] 
			Combine Texture * constant
		}
		SetTexture[_MaskTex] { Combine texture + previous, previous alpha}
	}
}

}