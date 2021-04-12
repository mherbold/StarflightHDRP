
using UnityEngine;

public class SystemPanel : MonoBehaviour
{
	public static SystemPanel m_instance;

	// unity awake
	void Awake()
	{
		Debug.Log( "SystemPanel Awake" );

		// global access to the panel
		m_instance = this;
	}
}
