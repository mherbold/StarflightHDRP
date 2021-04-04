
using UnityEngine;

public class SwitchInputActionMapOnEnable : MonoBehaviour
{
	[SerializeField] string m_actionMapName;

	void OnEnable()
	{
		Debug.Log( "SwitchInputActionMapOnEnable OnEnable" );

		Input.m_instance.SwitchToActionMap( m_actionMapName );
	}
}
