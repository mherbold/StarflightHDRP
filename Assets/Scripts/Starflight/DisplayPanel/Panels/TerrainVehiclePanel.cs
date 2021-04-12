
using UnityEngine;

public class TerrainVehiclePanel : MonoBehaviour
{
	public static TerrainVehiclePanel m_instance;

	// unity awake
	void Awake()
	{
		Debug.Log( "TerrainVehiclePanel Awake" );

		// global access to the panel
		m_instance = this;
	}
}
