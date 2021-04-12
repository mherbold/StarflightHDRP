
using UnityEngine;

public class SensorsPanel : MonoBehaviour
{
	public static SensorsPanel m_instance;

	// unity awake
	void Awake()
	{
		Debug.Log( "SensorsPanel Awake" );

		// global access to the panel
		m_instance = this;
	}
}
