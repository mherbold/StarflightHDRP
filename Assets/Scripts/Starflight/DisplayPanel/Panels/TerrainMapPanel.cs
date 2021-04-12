
using UnityEngine;

public class TerrainMapPanel : MonoBehaviour
{
	public static TerrainMapPanel m_instance;

	// unity awake
	void Awake()
	{
		Debug.Log( "TerrainMapPanel Awake" );

		// global access to the panel
		m_instance = this;
	}
}
