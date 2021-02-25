
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent( typeof( PlayableDirector ) )]
public class ActivateAtEndOfTimeline : MonoBehaviour
{
	PlayableDirector m_playableDirector;

	[SerializeField] GameObject[] m_gameObjectsToDisable;
	[SerializeField] GameObject[] m_gameObjectsToEnable;

	void OnEnable()
	{
		m_playableDirector = GetComponent<PlayableDirector>();

		m_playableDirector.stopped += OnPlayableDirectorStopped;
	}

	void OnPlayableDirectorStopped( PlayableDirector playableDirector )
	{
		if ( m_playableDirector == playableDirector )
		{
			foreach ( var gameObject in m_gameObjectsToDisable )
			{
				gameObject.SetActive( false );
			}

			foreach ( var gameObject in m_gameObjectsToEnable )
			{
				gameObject.SetActive( true );
			}
		}
	}

	void OnDisable()
	{
		m_playableDirector.stopped -= OnPlayableDirectorStopped;
	}
}
