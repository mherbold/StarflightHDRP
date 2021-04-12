
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent( typeof( PlayableDirector ) )]
public class TimelineForceEvaluate : MonoBehaviour
{
	private PlayableDirector m_playableDirector;

	void Awake()
	{
		Debug.Log( "TimelineForceEvaluate Awake" );

		m_playableDirector = GetComponent<PlayableDirector>();
	}

	void OnEnable()
	{
		Debug.Log( "TimelineForceEvaluate OnEnable" );

		m_playableDirector.Evaluate();
	}
}
