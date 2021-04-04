
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent( typeof( PlayableDirector ) )]
public class TimelineLooper : MonoBehaviour
{
	[SerializeField] float m_loopPoint = 3.0f;

	PlayableDirector m_playableDirector;

	void Awake()
	{
		Debug.Log( "TimelineLooper Awake" );

		m_playableDirector = GetComponent<PlayableDirector>();
	}

	void Update()
	{
		if ( m_playableDirector.state != PlayState.Playing )
		{
			m_playableDirector.time = m_loopPoint;

			m_playableDirector.Play();
		}
	}
}
