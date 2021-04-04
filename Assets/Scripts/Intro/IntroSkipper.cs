
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

[RequireComponent( typeof( PlayableDirector ) )]
public class IntroSkipper : MonoBehaviour
{
	PlayableDirector m_playableDirector;

	void Awake()
	{
		Debug.Log( "IntroSkipper Awake" );

		m_playableDirector = GetComponent<PlayableDirector>();
	}

	public void OnFire( InputAction.CallbackContext context )
	{
		if ( !context.canceled && context.action.triggered )
		{
			var time = m_playableDirector.time;

			if ( time < 15.0f )
			{
				m_playableDirector.time = 15.0f;
			}
			else if ( time < 32.0f )
			{
				m_playableDirector.time = 32.0f;
			}
			else if ( time < 41.0f )
			{
				m_playableDirector.time = 41.0f;
			}
			else if ( time < 62.0f )
			{
				m_playableDirector.time = 62.0f;
			}
		}
	}
}
