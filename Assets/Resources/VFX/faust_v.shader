// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "faust_v"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_dissolve("dissolve", 2D) = "white" {}
		_dissolveTilingxyOffsetzw("dissolve Tiling x y+ Offset z w", Vector) = (1,1,0,0)
		_dissolvetexspeedxy("dissolve tex speed x y", Vector) = (1,0,0,0)
		_MaintexTilingxyOffsetzw("Main tex Tiling x y + Offset z w", Vector) = (0.5,2,0.6,-0.5)
		_maintex("main tex", 2D) = "white" {}
		[HDR]_MaintexcolorHDR("Main tex color HDR", Color) = (0,0,0,0)
		_lightpower("light power", Float) = 1
		_Maintexspeed("Main tex speed", Vector) = (0,0,0,0)
		_mask("mask", 2D) = "white" {}
		_MaskTilingxyOffsetzw("Mask Tiling x y + Offset z w", Vector) = (0.8,1,0.2,1)
		_Maintexcolor("Main tex color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform sampler2D _maintex;
		uniform float2 _Maintexspeed;
		uniform float4 _MaintexTilingxyOffsetzw;
		uniform float4 _MaintexcolorHDR;
		uniform float _lightpower;
		uniform float4 _Maintexcolor;
		uniform sampler2D _mask;
		uniform float4 _MaskTilingxyOffsetzw;
		uniform sampler2D _dissolve;
		uniform float2 _dissolvetexspeedxy;
		uniform float4 _dissolveTilingxyOffsetzw;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult11 = (float2(_MaintexTilingxyOffsetzw.x , _MaintexTilingxyOffsetzw.y));
			float2 appendResult12 = (float2(_MaintexTilingxyOffsetzw.z , _MaintexTilingxyOffsetzw.w));
			float2 uv_TexCoord10 = i.uv_texcoord * appendResult11 + appendResult12;
			float2 panner20 = ( 1.0 * _Time.y * _Maintexspeed + uv_TexCoord10);
			float4 temp_output_78_0 = ( ( ( tex2D( _maintex, panner20 ) + _MaintexcolorHDR ) * _lightpower ) + _Maintexcolor );
			o.Albedo = temp_output_78_0.rgb;
			o.Emission = temp_output_78_0.rgb;
			o.Alpha = 1;
			float2 appendResult50 = (float2(i.uv2_texcoord2.y , _MaskTilingxyOffsetzw.y));
			float2 appendResult58 = (float2(_MaskTilingxyOffsetzw.z , _MaskTilingxyOffsetzw.w));
			float2 uv_TexCoord49 = i.uv_texcoord * appendResult50 + appendResult58;
			float2 appendResult6 = (float2(_dissolveTilingxyOffsetzw.x , _dissolveTilingxyOffsetzw.y));
			float2 appendResult7 = (float2(_dissolveTilingxyOffsetzw.z , _dissolveTilingxyOffsetzw.w));
			float2 uv_TexCoord3 = i.uv_texcoord * appendResult6 + appendResult7;
			float2 panner5 = ( 1.0 * _Time.y * _dissolvetexspeedxy + uv_TexCoord3);
			float4 _Vector2 = float4(0.3,0,1,0);
			float ifLocalVar39 = 0;
			if( tex2D( _dissolve, panner5 ).r >= i.uv2_texcoord2.x )
				ifLocalVar39 = _Vector2.y;
			else
				ifLocalVar39 = _Vector2.z;
			clip( ( ( ( tex2D( _mask, uv_TexCoord49 ).r + 0.0 ) * ifLocalVar39 * 4.0 ) + 0.0 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
167;169;1920;739;927.1987;620.9468;1.3;True;True
Node;AmplifyShaderEditor.Vector4Node;13;-1514.99,178.4028;Inherit;True;Property;_MaintexTilingxyOffsetzw;Main tex Tiling x y + Offset z w;4;0;Create;True;0;0;0;False;0;False;0.5,2,0.6,-0.5;0.5,2,0.6,-0.5;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;4;-1404,-144;Inherit;True;Property;_dissolveTilingxyOffsetzw;dissolve Tiling x y+ Offset z w;2;0;Create;True;0;0;0;False;0;False;1,1,0,0;0.5,4,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;7;-1152,-48;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;11;-1154,190;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;75;-115.6305,-525.6581;Inherit;False;1;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;6;-1152,-144;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;12;-1177.7,344.5;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;57;-597.5768,-267.397;Inherit;False;Property;_MaskTilingxyOffsetzw;Mask Tiling x y + Offset z w;10;0;Create;True;0;0;0;False;0;False;0.8,1,0.2,1;0.8,1,0.2,2.08;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;8;-808,-86;Inherit;False;Property;_dissolvetexspeedxy;dissolve tex speed x y;3;0;Create;True;0;0;0;False;0;False;1,0;0.7,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1008,-160;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;21;-1247.049,533.2345;Inherit;False;Property;_Maintexspeed;Main tex speed;8;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;58;-317.9764,-158.5971;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;50;-319.9379,-257.0122;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1014.1,161;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;49;-161.3379,-246.6122;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;5;-601.2999,-91.99999;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;20;-1018.249,473.4346;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;14;-766,378;Inherit;False;Property;_MaintexcolorHDR;Main tex color HDR;6;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;40;65.95227,-383.2648;Inherit;False;Constant;_Vector2;Vector 2;9;0;Create;True;0;0;0;False;0;False;0.3,0,1,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;-790,146;Inherit;True;Property;_maintex;main tex;5;0;Create;True;0;0;0;False;0;False;-1;d2d212ea9b69a8345bd6b7debdf07fcd;d2d212ea9b69a8345bd6b7debdf07fcd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-326.7971,5.676038;Inherit;True;Property;_dissolve;dissolve;1;0;Create;True;0;0;0;False;0;False;-1;3e788240fc1a09e4894a2a7c99e70249;3e788240fc1a09e4894a2a7c99e70249;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;27;136.8504,-95.8656;Inherit;True;Property;_mask;mask;9;0;Create;True;0;0;0;False;0;False;-1;031e93d3c0708df41aa1ff59a999d8f8;031e93d3c0708df41aa1ff59a999d8f8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-444,216;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-23.83277,158.4444;Inherit;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-396.849,504.6347;Inherit;False;Property;_lightpower;light power;7;0;Create;True;0;0;0;False;0;False;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;67;308.4734,104.3776;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;39;292.1525,-348.1649;Inherit;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;77;-173.4992,526.7054;Inherit;False;Property;_Maintexcolor;Main tex color;11;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.03773582,0.03773582,0.03773582,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-197.5,267;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;522.1672,189.3442;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;78;80.92287,303.5744;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;549.4261,423.6182;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;64;100.8975,119.4431;Inherit;False;Constant;_Vector4;Vector 4;11;0;Create;True;0;0;0;False;0;False;0,-0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;62;773.4998,7.8;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;faust_v;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Geometry;All;16;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;4;3
WireConnection;7;1;4;4
WireConnection;11;0;13;1
WireConnection;11;1;13;2
WireConnection;6;0;4;1
WireConnection;6;1;4;2
WireConnection;12;0;13;3
WireConnection;12;1;13;4
WireConnection;3;0;6;0
WireConnection;3;1;7;0
WireConnection;58;0;57;3
WireConnection;58;1;57;4
WireConnection;50;0;75;2
WireConnection;50;1;57;2
WireConnection;10;0;11;0
WireConnection;10;1;12;0
WireConnection;49;0;50;0
WireConnection;49;1;58;0
WireConnection;5;0;3;0
WireConnection;5;2;8;0
WireConnection;20;0;10;0
WireConnection;20;2;21;0
WireConnection;9;1;20;0
WireConnection;1;1;5;0
WireConnection;27;1;49;0
WireConnection;16;0;9;0
WireConnection;16;1;14;0
WireConnection;67;0;27;1
WireConnection;39;0;1;1
WireConnection;39;1;75;1
WireConnection;39;2;40;2
WireConnection;39;3;40;2
WireConnection;39;4;40;3
WireConnection;17;0;16;0
WireConnection;17;1;18;0
WireConnection;46;0;67;0
WireConnection;46;1;39;0
WireConnection;46;2;47;0
WireConnection;78;0;17;0
WireConnection;78;1;77;0
WireConnection;76;0;46;0
WireConnection;62;0;78;0
WireConnection;62;2;78;0
WireConnection;62;10;76;0
ASEEND*/
//CHKSM=8F54AC6E5C979907C6D4D6CDB7601459F3817FA1