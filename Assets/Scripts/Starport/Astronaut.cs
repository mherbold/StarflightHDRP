
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent( typeof( Animator ) )]
[RequireComponent( typeof( AudioSource ) )]
public class Astronaut : MonoBehaviour
{
	[SerializeField] StarportCamera m_starportCamera;

	[SerializeField] float m_floorRadius = 13.25f;

	[SerializeField] float m_padRadius = 5.0f;
	[SerializeField] float m_padHeight = 0.15f;

	[SerializeField] float m_angleLerpFactor = 5.0f;
	[SerializeField] float m_heightLerpFactor = 10.0f;

	Animator m_animator;
	AudioSource m_audioSource;

	Vector2 m_moveVector;

	float m_currentAngle = 180.0f;
	float m_targetAngle = 180.0f;

	bool m_inputActive = true;

	void OnEnable()
	{
		m_animator = GetComponent<Animator>();
		m_audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		var x = Mathf.RoundToInt( m_moveVector.x );
		var y = Mathf.RoundToInt( m_moveVector.y );

		if ( !m_inputActive )
		{
			x = 0;
			y = 0;
		}

		if ( ( x != 0 ) || ( y != 0 ) )
		{
			if ( ( x == 0 ) && ( y == -1 ) )
			{
				m_targetAngle = 0.0f;
			}
			else if ( ( x == -1 ) && ( y == -1 ) )
			{
				m_targetAngle = 45.0f;
			}
			else if ( ( x == -1 ) && ( y == 0 ) )
			{
				m_targetAngle = 90.0f;
			}
			else if ( ( x == -1 ) && ( y == 1 ) )
			{
				m_targetAngle = 135.0f;
			}
			else if ( ( x == 0 ) && ( y == 1 ) )
			{
				m_targetAngle = 180.0f;
			}
			else if ( ( x == 1 ) && ( y == 1 ) )
			{
				m_targetAngle = 225.0f;
			}
			else if ( ( x == 1 ) && ( y == 0 ) )
			{
				m_targetAngle = 270.0f;
			}
			else if ( ( x == 1 ) && ( y == -1 ) )
			{
				m_targetAngle = 315.0f;
			}

			m_targetAngle += 180.0f;

			m_targetAngle += m_starportCamera.GetCurrentAngle();
		}

		while ( ( m_targetAngle - m_currentAngle ) > 180.0f )
		{
			m_currentAngle += 360.0f;
		}

		while ( ( m_currentAngle - m_targetAngle ) > 180.0f )
		{
			m_currentAngle -= 360.0f;
		}

		m_currentAngle = Mathf.Lerp( m_currentAngle, m_targetAngle, Time.deltaTime * m_angleLerpFactor );

		transform.rotation = Quaternion.AngleAxis( m_currentAngle, Vector3.up );

		if ( ( x != 0 ) || ( y != 0 ) )
		{
			m_animator.SetBool( "Run", true );
		}
		else
		{
			m_animator.SetBool( "Run", false );
		}

		var position = transform.position;

		position.y = 0.0f;

		if ( position.magnitude < m_padRadius )
		{
			position.y = m_padHeight;
		}

		if ( position.magnitude > m_floorRadius )
		{
			position = position.normalized * m_floorRadius;
		}

		transform.position = Vector3.Lerp( transform.position, position, Time.deltaTime * m_heightLerpFactor );
	}

	public void OnMove( InputAction.CallbackContext context )
	{
		m_moveVector = context.ReadValue<Vector2>();
	}

	public void Footstep()
	{
		m_audioSource.Play();
	}

	public void SetInputActive( bool inputActive )
	{
		m_inputActive = inputActive;
	}
}
