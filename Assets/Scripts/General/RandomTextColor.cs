
using TMPro;
using UnityEngine;

[RequireComponent( typeof( TextMeshPro ) )]
public class RandomTextColor : MonoBehaviour
{
	[SerializeField] float m_colorChangeInterval;

	TextMeshPro m_tmp;

	float m_colorChangeTimer;

	void Awake()
	{
		Debug.Log( "RandomTextColor Awake" );

		m_tmp = GetComponent<TextMeshPro>();
	}

	void OnEnable()
	{
		Debug.Log( "RandomTextColor OnEnable" );

		m_colorChangeTimer = 0.0f;
	}

	void Update()
	{
		m_colorChangeTimer -= Time.deltaTime;

		if ( m_colorChangeTimer <= 0.0f )
		{
			m_colorChangeTimer += m_colorChangeInterval;

			var hue = Random.Range( 0.0f, 1.0f );

			var newColor = Color.HSVToRGB( hue, 1.0f, 1.0f );

			newColor.a = m_tmp.color.a;

			m_tmp.color = newColor;
		}
	}
}
