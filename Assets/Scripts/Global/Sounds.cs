
using UnityEngine;

public class Sounds : MonoBehaviour
{
	[SerializeField] AudioSource m_clickAudioSource;
	[SerializeField] AudioSource m_activateAudioSource;
	[SerializeField] AudioSource m_deactivateAudioSource;
	[SerializeField] AudioSource m_errorAudioSource;
	[SerializeField] AudioSource m_updateAudioSource;

	public static Sounds m_instance;

	void Awake()
	{
		Debug.Log( "Sounds Awake" );

		m_instance = this;
	}

	public void PlayClick()
	{
		Debug.Log( "** click **" );

		m_clickAudioSource.Play();
	}

	public void PlayActivate()
	{
		Debug.Log( "** activate **" );

		m_activateAudioSource.Play();
	}

	public void PlayDeactivate()
	{
		Debug.Log( "** deactivate **" );

		m_deactivateAudioSource.Play();
	}

	public void PlayError()
	{
		Debug.Log( "** error **" );

		m_errorAudioSource.Play();
	}

	public void PlayUpdate()
	{
		Debug.Log( "** update **" );

		m_updateAudioSource.Play();
	}
}
