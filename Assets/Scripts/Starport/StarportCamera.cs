
using UnityEngine;

public class StarportCamera : MonoBehaviour
{
	[SerializeField] Transform m_objectToTrack;
	[SerializeField] Camera m_camera;

	[SerializeField] float m_trackMinimumRadius = 5.0f;
	[SerializeField] float m_trackMaximumRadius = 13.5f;

	[SerializeField] float m_normalFieldOfView = 52.3f;
	[SerializeField] float m_zoomedFieldOfView = 15.0f;
	[SerializeField] float m_fieldOfViewTransitionLerpFactor = 2.0f;

	[SerializeField] float m_objecToTrackPositionLerpFactor = 1.0f;

	[SerializeField] float m_angleAccelerationLerpFactor = 1.0f;
	[SerializeField] float m_maximumAngleSpeed = 30.0f;

	float m_currentAngle = 0.0f;
	float m_targetAngle = 0.0f;

	float m_currentAngleSpeed = 0.0f;

	Vector3 m_objectToTrackPosition;

	void OnEnable()
	{
		m_objectToTrackPosition = m_objectToTrack.position;
	}

	void Update()
	{
		float targetFieldOfView = m_normalFieldOfView;

		var position = m_objectToTrack.position;

		position.y = 0.0f;

		if ( position.magnitude >= m_trackMinimumRadius )
		{
			m_targetAngle = Vector3.SignedAngle( position.normalized, Vector3.forward, Vector3.down );

			targetFieldOfView = Mathf.Lerp( m_normalFieldOfView, m_zoomedFieldOfView, ( position.magnitude - m_trackMinimumRadius ) / ( m_trackMaximumRadius - m_trackMinimumRadius ) );
		}

		while ( ( m_targetAngle - m_currentAngle ) > 180.0f )
		{
			m_currentAngle += 360.0f;
		}

		while ( ( m_currentAngle - m_targetAngle ) > 180.0f )
		{
			m_currentAngle -= 360.0f;
		}

		m_currentAngleSpeed = Mathf.Lerp( m_currentAngleSpeed, Mathf.Clamp( m_targetAngle - m_currentAngle, -m_maximumAngleSpeed, m_maximumAngleSpeed ), Time.deltaTime * m_angleAccelerationLerpFactor );

		if ( Mathf.Abs( m_currentAngleSpeed ) > Mathf.Abs( m_targetAngle - m_currentAngle ) )
		{
			m_currentAngleSpeed = m_targetAngle - m_currentAngle;
		}

		m_currentAngle += m_currentAngleSpeed * Time.deltaTime;

		transform.rotation = Quaternion.AngleAxis( m_currentAngle, Vector3.up );

		m_camera.fieldOfView = Mathf.Lerp( m_camera.fieldOfView, targetFieldOfView, Time.deltaTime * m_fieldOfViewTransitionLerpFactor );

		m_objectToTrackPosition = Vector3.Lerp( m_objectToTrackPosition, m_objectToTrack.position, Time.deltaTime * m_objecToTrackPositionLerpFactor );

		m_camera.transform.rotation = Quaternion.LookRotation( m_objectToTrackPosition - m_camera.transform.position );
	}

	public float GetCurrentAngle()
	{
		return m_currentAngle;
	}
}
