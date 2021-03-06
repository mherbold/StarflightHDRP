
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent( typeof( VisualEffect ) )]
public class SetDustFieldVisibility : MonoBehaviour
{
	[SerializeField] Camera m_camera;
	[SerializeField] float m_minSpeed;
	[SerializeField] float m_maxSpeed;
	[SerializeField] float m_minAlpha;
	[SerializeField] float m_maxAlpha;
	[SerializeField] float m_speedLerpFactor;

	VisualEffect m_visualEffect;
	Vector3 m_lastCameraPosition;
	float m_cameraSpeed;

	void OnEnable()
	{
		m_visualEffect = GetComponent<VisualEffect>();

		m_lastCameraPosition = m_camera.transform.position;
		m_cameraSpeed = 0.0f;
	}

	void Update()
	{
		var cameraSpeed = ( m_camera.transform.position - m_lastCameraPosition ).magnitude / Time.deltaTime;

		m_cameraSpeed = Mathf.Lerp( m_cameraSpeed, cameraSpeed, m_speedLerpFactor * Time.deltaTime );

		m_lastCameraPosition = m_camera.transform.position;

		var dustFieldVisibility = Mathf.Lerp( m_minAlpha, m_maxAlpha, ( m_cameraSpeed - m_minSpeed ) / ( m_maxSpeed - m_minSpeed ) );

		m_visualEffect.SetFloat( "Dust Field Visibility", dustFieldVisibility );
	}
}
