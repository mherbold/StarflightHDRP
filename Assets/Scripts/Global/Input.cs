
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent( typeof( PlayerInput ) )]
public class Input : MonoBehaviour
{
	private static PlayerInput m_playerInput;

	public static Input m_instance;

	void Awake()
	{
		Debug.Log( "Input Awake" );

		m_playerInput = GetComponent<PlayerInput>();

		m_instance = this;
	}

	public void SwitchToActionMap( string actionMapName )
	{
		Debug.Log( "Switching to action map " + actionMapName );

		m_playerInput.currentActionMap = m_playerInput.actions.FindActionMap( actionMapName, true );
	}
}
