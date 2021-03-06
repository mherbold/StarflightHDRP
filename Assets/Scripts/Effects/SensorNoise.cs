
using UnityEngine;
using UnityEngine.UI;

public class SensorNoise : MonoBehaviour
{
	[SerializeField] float m_scanTime = 3.0f;
	[SerializeField] float m_minimumAlpha = 1.0f;
	[SerializeField] float m_maximumAlpha = 20.0f;

	Image m_image;
	Material m_material;
	float m_scanTimer;
	bool m_scanning;
	float m_imageAlpha;
	Vector3 m_imageOffset;

	void OnEnable()
	{
		var noise = transform.Find( "Noise" );

		m_image = noise.GetComponent<Image>();
		m_material = m_image.material;
		m_scanTimer = 0.0f;
		m_scanning = true;
		m_imageAlpha = 1.0f;
		m_imageOffset = Vector3.zero;
	}

	void Update()
	{
		if ( m_scanning )
		{
			m_scanTimer += Time.deltaTime;

			if ( m_scanTimer >= m_scanTime )
			{
				m_scanning = false;

				m_imageAlpha = 1.0f;
				m_imageOffset = Vector3.zero;
			}
			else
			{
				m_imageAlpha = m_minimumAlpha + ( m_maximumAlpha - m_minimumAlpha ) * Mathf.Sin( Mathf.Lerp( 0.0f, Mathf.PI, m_scanTimer / m_scanTime ) );

				var offset = Quaternion.AngleAxis( Random.Range( 0.0f, 360.0f ), Vector3.forward ) * Vector3.up * 0.05f;

				m_imageOffset += offset;
			}

			m_material.SetFloat( "ImageAlpha", m_imageAlpha );
			m_material.SetVector( "ImageOffset", m_imageOffset );
		}
	}
}
