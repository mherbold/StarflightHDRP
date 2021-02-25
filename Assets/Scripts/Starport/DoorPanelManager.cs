
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class DoorPanelManager : MonoBehaviour
{
	[SerializeField] PlayableDirector m_panelSlideInTimeline;
	[SerializeField] PlayableDirector m_panelSlideOutTimeline;
	[SerializeField] Astronaut m_astronaut;

	[SerializeField] AudioSource m_clickAudioSource;
	[SerializeField] AudioSource m_activateAudioSource;
	[SerializeField] AudioSource m_deactivateAudioSource;
	[SerializeField] AudioSource m_errorAudioSource;
	[SerializeField] AudioSource m_updateAudioSource;

	[SerializeField] float m_debounceTime = 0.25f;

	DoorPanelController m_currentDoorPanel;

	bool m_panelIsVisible = false;
	bool m_inputHasFocus = false;
	bool m_ignoreInputThisFrame = false;

	Vector2 m_moveVector;

	float m_nextMoveTime;

	void OnEnable()
	{
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
					m_currentDoorPanel.OnLeft();
				}
				else if ( x > 0 )
				{
					m_currentDoorPanel.OnRight();
				}
				else if ( y > 0 )
				{
					m_currentDoorPanel.OnUp();
				}
				else if ( y < 0 )
				{
					m_currentDoorPanel.OnDown();
				}
			}
		}

		m_ignoreInputThisFrame = false;
	}

	public void SetCurrentDoorPanel( DoorPanelController doorPanelController )
	{
		if ( m_currentDoorPanel )
		{
			m_currentDoorPanel.gameObject.SetActive( false );
		}

		m_currentDoorPanel = doorPanelController;

		if ( m_panelIsVisible && ( m_currentDoorPanel == null ) )
		{
			m_panelIsVisible = false;

			m_astronaut.SetInputActive( true );
		}
	}

	public void ClosePanel()
	{
		m_panelSlideInTimeline.Stop();
		m_panelSlideOutTimeline.Stop();
		m_panelSlideOutTimeline.Play();

		m_panelIsVisible = false;

		m_astronaut.SetInputActive( true );

		DataController.m_instance.SaveActiveGame();
	}

	public void SetInputFocus( bool inputHasFocus )
	{
		m_inputHasFocus = inputHasFocus;
		m_ignoreInputThisFrame = true;

		if ( m_inputHasFocus )
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
		if ( !m_inputHasFocus )
		{
			m_moveVector = context.ReadValue<Vector2>();
		}
	}

	public void OnFire( InputAction.CallbackContext context )
	{
		if ( !m_inputHasFocus )
		{
			if ( context.action.triggered )
			{
				if ( m_currentDoorPanel )
				{
					if ( m_panelIsVisible )
					{
						m_currentDoorPanel.OnFire();
					}
					else
					{
						m_currentDoorPanel.gameObject.SetActive( true );

						m_panelSlideOutTimeline.Stop();
						m_panelSlideInTimeline.Stop();
						m_panelSlideInTimeline.Play();

						m_panelIsVisible = true;

						m_astronaut.SetInputActive( false );
					}
				}
			}
		}
	}

	public void OnCancel( InputAction.CallbackContext context )
	{
		if ( !m_inputHasFocus )
		{
			if ( context.action.triggered )
			{
				if ( m_panelIsVisible )
				{
					m_currentDoorPanel.OnCancel();
				}
			}
		}
	}

	public void OnTextInput( char ch )
	{
		if ( m_inputHasFocus && !m_ignoreInputThisFrame )
		{
			m_currentDoorPanel.OnTextInput( ch );
		}
	}

	public void PlayClickSound()
	{
		Debug.Log( "** click **" );

		m_clickAudioSource.Play();
	}

	public void PlayActivateSound()
	{
		Debug.Log( "** activate **" );

		m_activateAudioSource.Play();
	}

	public void PlayDeactivateSound()
	{
		Debug.Log( "** deactivate **" );

		m_deactivateAudioSource.Play();
	}

	public void PlayErrorSound()
	{
		Debug.Log( "** error **" );

		m_errorAudioSource.Play();
	}

	public void PlayUpdateSound()
	{
		Debug.Log( "** update **" );

		m_updateAudioSource.Play();
	}
}
