
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent( typeof( PlayableDirector ) )]
public class TimelineController : MonoBehaviour
{
	[SerializeField] bool m_loop = true;
	[SerializeField] float m_loopPoint = 3.0f;

	PlayableDirector m_playableDirector;

	void Awake()
	{
		Debug.Log( "TimelineController Awake" );

		m_playableDirector = GetComponent<PlayableDirector>();
	}

	void Update()
	{
		if ( m_playableDirector.state != PlayState.Playing )
		{
			if ( m_loop )
			{
				m_playableDirector.time = m_loopPoint;

				m_playableDirector.Play();
			}
		}
	}

	public void Stop()
	{
		m_loop = false;

		m_playableDirector.Stop();
	}
}
