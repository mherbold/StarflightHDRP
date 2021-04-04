
using UnityEngine;

public class DoorPanelController : MonoBehaviour
{
	[SerializeField] GameObject[] m_pages;
	[SerializeField] PanelButton[] m_buttons;

	protected int m_currentPageIndex;
	protected int m_currentButtonIndex;

	private bool m_initialized = false;

	void OnEnable()
	{
		Debug.Log( "DoorPanelController OnEnable" );

		if ( !m_initialized )
		{
			Initialize();

			m_initialized = true;
		}

		Restart();
	}

	public void SetCurrentDoorPanel()
	{
		DoorPanelManager.m_instance.SetCurrentDoorPanelController( this );
	}

	public void UnsetCurrentDoorPanel()
	{
		DoorPanelManager.m_instance.SetCurrentDoorPanelController( null );
	}

	public void SetCurrentPage( int pageIndex, bool makeNoise = true, bool playDeactivateSoundInstead = false )
	{
		foreach ( var page in m_pages )
		{
			page.SetActive( false );
		}

		m_currentPageIndex = pageIndex;

		m_pages[ m_currentPageIndex ].SetActive( true );

		if ( makeNoise )
		{
			if ( playDeactivateSoundInstead )
			{
				Sounds.m_instance.PlayDeactivate();
			}
			else
			{
				Sounds.m_instance.PlayActivate();
			}
		}
	}

	public void SetCurrentButton( int buttonIndex, bool makeNoise = true )
	{
		Debug.Log( "SetCurrentButton( " + buttonIndex + " )" );

		if ( m_buttons[ m_currentButtonIndex ].m_enabled )
		{
			if ( !m_buttons[ m_currentButtonIndex ].m_fireOnSelect )
			{
				m_buttons[ m_currentButtonIndex ].m_buttonImage.color = Colors.LightBlue;

				if ( m_buttons[ m_currentButtonIndex ].m_buttonTMP != null )
				{
					m_buttons[ m_currentButtonIndex ].m_buttonTMP.color = Colors.Black;
				}
			}
		}

		m_currentButtonIndex = buttonIndex;

		m_buttons[ m_currentButtonIndex ].m_enabled = true;

		if ( m_buttons[ m_currentButtonIndex ].m_fireOnSelect )
		{
			OnFire();
		}
		else
		{
			m_buttons[ m_currentButtonIndex ].m_buttonImage.color = Colors.DarkGreen;

			if ( m_buttons[ m_currentButtonIndex ].m_buttonTMP != null )
			{
				m_buttons[ m_currentButtonIndex ].m_buttonTMP.color = Colors.LightCyan;
			}
		}

		if ( makeNoise )
		{
			Sounds.m_instance.PlayClick();
		}
	}

	public void ShowButton( int buttonIndex, bool show )
	{
		Debug.Log( "ShowButton( " + buttonIndex + ", " + show + " )" );

		m_buttons[ buttonIndex ].m_buttonImage.gameObject.SetActive( show );

		EnableButton( buttonIndex, show );
	}

	public void EnableButton( int buttonIndex, bool enabled )
	{
		Debug.Log( "EnableButton( " + buttonIndex + ", " + enabled + " )" );

		if ( m_buttons[ buttonIndex ].m_enabled != enabled )
		{
			m_buttons[ buttonIndex ].m_enabled = enabled;

			if ( !m_buttons[ buttonIndex ].m_fireOnSelect )
			{
				if ( enabled )
				{
					m_buttons[ buttonIndex ].m_buttonImage.color = Colors.LightBlue;

					if ( m_buttons[ buttonIndex ].m_buttonTMP != null )
					{
						m_buttons[ buttonIndex ].m_buttonTMP.color = Colors.Black;
					}
				}
				else
				{
					m_buttons[ buttonIndex ].m_buttonImage.color = Colors.DarkGray;

					if ( m_buttons[ buttonIndex ].m_buttonTMP != null )
					{
						m_buttons[ buttonIndex ].m_buttonTMP.color = Colors.Black;
					}
				}
			}
		}
	}

	public bool ButtonIsEnbled( int buttonIndex )
	{
		return m_buttons[ buttonIndex ].m_enabled;
	}

	protected void SetInputFocus( bool inputHasFocus )
	{
		DoorPanelManager.m_instance.EnableTextInput( inputHasFocus );
	}

	protected void Exit()
	{
		DoorPanelManager.m_instance.ClosePanel();
	}

	protected virtual void Initialize()
	{
	}

	protected virtual void Restart()
	{
	}

	public virtual void OnFire()
	{
		Debug.Log( "OnFire; m_currentButtonIndex = " + m_currentButtonIndex );
	}

	public virtual void OnCancel()
	{
		Debug.Log( "OnCancel" );
	}

	public virtual void OnTextInput( char ch )
	{
	}

	public virtual void OnLeft()
	{
		var buttonIndex = m_currentButtonIndex;

		do
		{
			buttonIndex = m_buttons[ buttonIndex ].m_leftButtonIndex;

			if ( buttonIndex == -1 )
			{
				break;
			}
		}
		while ( !m_buttons[ buttonIndex ].m_enabled );

		if ( buttonIndex == -1 )
		{
			Sounds.m_instance.PlayError();
		}
		else
		{
			SetCurrentButton( buttonIndex );
		}
	}

	public virtual void OnRight()
	{
		var buttonIndex = m_currentButtonIndex;

		do
		{
			buttonIndex = m_buttons[ buttonIndex ].m_rightButtonIndex;

			if ( buttonIndex == -1 )
			{
				break;
			}
		}
		while ( !m_buttons[ buttonIndex ].m_enabled );

		if ( buttonIndex == -1 )
		{
			Sounds.m_instance.PlayError();
		}
		else
		{
			SetCurrentButton( buttonIndex );
		}
	}

	public virtual void OnUp()
	{
		var buttonIndex = m_currentButtonIndex;

		do
		{
			buttonIndex = m_buttons[ buttonIndex ].m_upButtonIndex;

			if ( buttonIndex == -1 )
			{
				break;
			}
		}
		while ( !m_buttons[ buttonIndex ].m_enabled );

		if ( buttonIndex == -1 )
		{
			Sounds.m_instance.PlayError();
		}
		else
		{
			SetCurrentButton( buttonIndex );
		}
	}

	public virtual void OnDown()
	{
		var buttonIndex = m_currentButtonIndex;

		do
		{
			buttonIndex = m_buttons[ buttonIndex ].m_downButtonIndex;

			if ( buttonIndex == -1 )
			{
				break;
			}
		}
		while ( !m_buttons[ buttonIndex ].m_enabled );

		if ( buttonIndex == -1 )
		{
			Sounds.m_instance.PlayError();
		}
		else
		{
			SetCurrentButton( buttonIndex );
		}
	}
}
