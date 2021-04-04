
using UnityEngine;

public class Radar : MonoBehaviour
{
	public float m_maxPlayerDetectionDistanceInHyperspace;
	public float m_maxPlayerDetectionDistanceInStarSystem;
	public float m_maxAlienDetectionDistanceInHyperspace;
	public float m_maxAlienDetectionDistanceInStarSystem;

	public static Radar m_instance;

	void Awake()
	{
		m_instance = this;
	}

}
