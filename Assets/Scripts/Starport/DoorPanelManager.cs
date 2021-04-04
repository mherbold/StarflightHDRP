
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class DoorPanelManager : MonoBehaviour
{
	[SerializeField] PlayableDirector m_panelSlideInTimeline;
	[SerializeField] PlayableDirector m_panelSlideOutTimeline;

	[SerializeField] float m_debounceTime = 0.25f;

	DoorPanelController m_currentDoorPanelController;

	bool m_panelIsVisible = false;
	bool m_textInputEnabled = false;
	bool m_ignoreTextInputThisFrame = false;

	Vector2 m_moveVector;
	float m_nextMoveTime;

	public static DoorPanelManager m_instance;

	void Awake()
	{
		Debug.Log( "DoorPanelManager Awake" );

		m_instance = this;
	}

	void OnEnable()
	{
		Debug.Log( "DoorPanelManager OnEnable" );

		foreach ( Transform childTransform in transform )
		{
			childTransform.gameObject.SetActive( false );
		}
	}

	void Update()
	{
		if ( m_nextMoveTime > 0.0f )
		{
			m_nextMoveTime -= Time.deltaTime;
		}

		if ( m_panelIsVisible )
		{
			var x = Mathf.RoundToInt( m_moveVector.x );
			var y = Mathf.RoundToInt( m_moveVector.y );

			if ( ( x == 0 ) && ( y == 0 ) )
			{
				m_nextMoveTime = 0.0f;
			}
			else if ( m_nextMoveTime <= 0.0f )
			{
				m_nextMoveTime = m_debounceTime;

				if ( x < 0 )
				{
					m_currentDoorPanelController.OnLeft();
				}
				else if ( x > 0 )
				{
					m_currentDoorPanelController.OnRight();
				}
				else if ( y > 0 )
				{
					m_currentDoorPanelController.OnUp();
				}
				else if ( y < 0 )
				{
					m_currentDoorPanelController.OnDown();
				}
			}
		}

		m_ignoreTextInputThisFrame = false;
	}

	public void SetCurrentDoorPanelController( DoorPanelController doorPanelController )
	{
		if ( m_currentDoorPanelController )
		{
			m_currentDoorPanelController.gameObject.SetActive( false );
		}

		m_currentDoorPanelController = doorPanelController;

		if ( m_currentDoorPanelController == null )
		{
			if ( m_panelIsVisible )
			{
				m_panelIsVisible = false;

				Astronaut.m_instance.Freeze( false );
			}
		}
	}

	public void OpenPanel()
	{
		if ( m_currentDoorPanelController != null )
		{
			m_currentDoorPanelController.gameObject.SetActive( true );

			m_panelSlideOutTimeline.Stop();
			m_panelSlideInTimeline.Stop();
			m_panelSlideInTimeline.Play();

			m_panelIsVisible = true;

			Astronaut.m_instance.Freeze( true );

			Input.m_instance.SwitchToActionMap( "Starport Door Panel" );
		}
	}

	public void ClosePanel()
	{
		m_panelSlideInTimeline.Stop();
		m_panelSlideOutTimeline.Stop();
		m_panelSlideOutTimeline.Play();

		m_panelIsVisible = false;

		Astronaut.m_instance.Freeze( false );

		DataController.m_instance.SaveActiveGame();

		Input.m_instance.SwitchToActionMap( "Starport Astronaut" );
	}

	public void EnableTextInput( bool enabled )
	{
		m_textInputEnabled = enabled;
		m_ignoreTextInputThisFrame = true;

		if ( m_textInputEnabled )
		{
			Keyboard.current.onTextInput += OnTextInput;
		}
		else
		{
			Keyboard.current.onTextInput -= OnTextInput;
		}
	}

	public void OnMove( InputAction.CallbackContext context )
	{
		if ( !m_textInputEnabled )
		{
			m_moveVector = context.ReadValue<Vector2>();
		}
	}

	public void OnFire( InputAction.CallbackContext context )
	{
		if ( !context.canceled && context.action.triggered )
		{
			if ( !m_textInputEnabled )
			{
				if ( m_currentDoorPanelController )
				{
					if ( m_panelIsVisible )
					{
						m_currentDoorPanelController.OnFire();
					}
				}
			}
		}
	}

	public void OnCancel( InputAction.CallbackContext context )
	{
		if ( !context.canceled && context.action.triggered )
		{
			if ( !m_textInputEnabled )
			{
				if ( m_currentDoorPanelController )
				{
					if ( m_panelIsVisible )
					{
						m_currentDoorPanelController.OnCancel();
					}
				}
			}
		}
	}

	public void OnTextInput( char ch )
	{
		if ( m_textInputEnabled && !m_ignoreTextInputThisFrame )
		{
			m_currentDoorPanelController.OnTextInput( ch );
		}
	}
}
