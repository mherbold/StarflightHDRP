
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[VolumeComponentMenu( "Sky/Starflight Sky" )]
[SkyUniqueID( 99 )]
public class StarflightSky : SkySettings
{
	[Tooltip( "Specify the cubemap HDRP uses to render the sky." )]
	public CubemapParameter m_hdriSky = new CubemapParameter( null );

	public override Type GetSkyRendererType() => typeof( StarflightSkyRenderer );

	public override int GetHashCode()
	{
		int hash = base.GetHashCode();

		unchecked
		{
			hash = m_hdriSky.value != null ? hash * 23 + m_hdriSky.GetHashCode() : hash;
		}

		return hash;
	}
}
