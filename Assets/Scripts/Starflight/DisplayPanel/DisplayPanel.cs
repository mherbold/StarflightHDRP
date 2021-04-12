
using UnityEngine;

public class DisplayPanel : MonoBehaviour
{
	public enum Panel
	{
		Sensors,
		Status,
		System,
		TerrainMap,
		TerrainVehicle,
		Count
	}

	[SerializeField] SensorsPanel m_sensorsPanel;
	[SerializeField] StatusPanel m_statusPanel;
	[SerializeField] SystemPanel m_systemPanel;
	[SerializeField] TerrainMapPanel m_terrainMapPanel;
	[SerializeField] TerrainVehiclePanel m_terrainVehiclePanel;

	public static DisplayPanel m_instance;

	// unity awake
	void Awake()
	{
		Debug.Log( "DisplayPanel Awake" );

		// global access to the display panel
		m_instance = this;
	}

	public void SwitchTo( Panel panel )
	{
		Debug.Log( "DisplayPanel.SwitchTo( " + panel + " )" );

		m_sensorsPanel.gameObject.SetActive( false );
		m_statusPanel.gameObject.SetActive( false );
		m_systemPanel.gameObject.SetActive( false );
		m_terrainMapPanel.gameObject.SetActive( false );
		m_terrainVehiclePanel.gameObject.SetActive( false );

		switch ( panel )
		{
			case Panel.Sensors:
				m_sensorsPanel.gameObject.SetActive( true );
				break;

			case Panel.Status:
				m_statusPanel.gameObject.SetActive( true );
				break;

			case Panel.System:
				m_systemPanel.gameObject.SetActive( true );
				break;

			case Panel.TerrainMap:
				m_terrainMapPanel.gameObject.SetActive( true );
				break;

			case Panel.TerrainVehicle:
				m_terrainVehiclePanel.gameObject.SetActive( true );
				break;
		}
	}
}
