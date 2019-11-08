Shader "ACP/UI Mask" {

Properties
{
	_Color ("Color", Color) = (1,1,1,1)
	_MainTex("Texture", 2D) = "" {} //{ TexGen SphereMap}
	_MaskTex("Texture Mask", 2D) = "" {} //{ TexGen SphereMap}
}

SubShader 
{
	Cull Off
	Zwrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	Tags {Queue=Transparent}

	Pass
	{
		SetTexture[_MaskTex] {
			ConstantColor[_Color] 
			Combine Texture alpha * constant alpha
		}
		SetTexture[_MainTex] { Combine previous * texture }
	}
}

}