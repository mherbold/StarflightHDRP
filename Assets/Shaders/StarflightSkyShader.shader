Shader "Hidden/HDRP/Sky/StarflightSky"
{
	HLSLINCLUDE

#pragma vertex Vert

#pragma editor_sync_compilation
#pragma target 4.5
#pragma only_renderers d3d11 playstation xboxone vulkan metal switch

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Sky/SkyUtils.hlsl"

	TEXTURECUBE( _Cubemap );
	SAMPLER( sampler_Cubemap );

	float4 _SkyParam;
	matrix _SkyRotation;

#define _Intensity _SkyParam.x
#define _CosSinPhi _SkyParam.zw

	struct Attributes
	{
		uint vertexID : SV_VertexID;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	Varyings Vert( Attributes input )
	{
		Varyings output;

		UNITY_SETUP_INSTANCE_ID( input );

		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

		output.positionCS = GetFullScreenTriangleVertexPosition( input.vertexID, UNITY_RAW_FAR_CLIP_VALUE );

		return output;
	}

	float4 RenderSky( Varyings input, float exposure )
	{
		float3 worldSpaceViewDirection = -GetSkyViewDirWS( input.positionCS.xy );

		float3 rotatedworldSpaceViewDirection = mul( worldSpaceViewDirection, _SkyRotation );

		float3 skyColor = SAMPLE_TEXTURECUBE_LOD( _Cubemap, sampler_Cubemap, rotatedworldSpaceViewDirection, 0 ).rgb * _Intensity * exposure;

		skyColor = ClampToFloat16Max( skyColor );

		return float4( skyColor, 1.0 );
	}

	float4 FragBaking( Varyings input ) : SV_Target
	{
		return RenderSky( input, 1.0 );
	}

	float4 FragRender( Varyings input ) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

		return RenderSky( input, GetCurrentExposureMultiplier() );
	}

	ENDHLSL

	SubShader
	{
		Pass
		{
			ZWrite Off
			ZTest Always
			Blend Off
			Cull Off

			HLSLPROGRAM
#pragma fragment FragBaking
			ENDHLSL
		}

		Pass
		{
			ZWrite Off
			ZTest LEqual
			Blend Off
			Cull Off

			HLSLPROGRAM
#pragma fragment FragRender
			ENDHLSL
		}
	}
	Fallback Off
}
