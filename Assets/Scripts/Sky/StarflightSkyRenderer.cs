
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

class StarflightSkyRenderer : SkyRenderer
{
	public static readonly int m_cubemapPropertyID = Shader.PropertyToID( "_Cubemap" );
	public static readonly int m_skyParamPropertyID = Shader.PropertyToID( "_SkyParam" );
	public static readonly int m_pixelCoordToViewDirWSPropertyID = Shader.PropertyToID( "_PixelCoordToViewDirWS" );
	public static readonly int m_skyRotationPropertyID = Shader.PropertyToID( "_SkyRotation" );

	Material m_starflightSkyMaterial;
	MaterialPropertyBlock m_propertyBlock = new MaterialPropertyBlock();

	static readonly int m_renderCubemapPassID = 0;
	static readonly int m_renderFullscreenSkyPassID = 1;

	static Quaternion currentRotation = Quaternion.identity;

	public override void Build()
	{
		m_starflightSkyMaterial = CoreUtils.CreateEngineMaterial( Shader.Find( "Hidden/HDRP/Sky/StarflightSky" ) );
	}

	public override void Cleanup()
	{
		CoreUtils.Destroy( m_starflightSkyMaterial );
	}

	protected override bool Update( BuiltinSkyParameters builtinParams )
	{
		return base.Update( builtinParams );
	}

	public override void RenderSky( BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk )
	{
		var starflightSky = builtinParams.skySettings as StarflightSky;

		float intensity = GetSkyIntensity( starflightSky, builtinParams.debugSettings );

		m_propertyBlock.SetTexture( m_cubemapPropertyID, starflightSky.m_hdriSky.value );
		m_propertyBlock.SetVector( m_skyParamPropertyID, new Vector4( intensity, 0.0f, 0.0f, 0.0f ) );
		m_propertyBlock.SetMatrix( m_pixelCoordToViewDirWSPropertyID, builtinParams.pixelCoordToViewDirMatrix );
		m_propertyBlock.SetMatrix( m_skyRotationPropertyID, Matrix4x4.TRS( Vector3.zero, currentRotation, Vector3.one ) );

		int passID = renderForCubemap ? m_renderCubemapPassID : m_renderFullscreenSkyPassID;

		CoreUtils.DrawFullScreen( builtinParams.commandBuffer, m_starflightSkyMaterial, m_propertyBlock, passID );
	}

	public static void SetRotation( Quaternion rotation )
	{
		currentRotation = rotation;
	}
}
