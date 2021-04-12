
using UnityEngine;

public class StatusPanel : MonoBehaviour
{
	public static StatusPanel m_instance;

	// unity awake
	void Awake()
	{
		Debug.Log( "StatusPanel Awake" );

		// global access to the panel
		m_instance = this;
	}
}
