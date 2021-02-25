
using UnityEngine;

[RequireComponent( typeof( Renderer ) )]
public class RandomUVOffset : MonoBehaviour
{
	[SerializeField] int m_materialIndex = 0;
	[SerializeField] int m_columns = 1;
	[SerializeField] int m_rows = 1;
	[SerializeField] float m_frameRate = 3.0f;
	[SerializeField] float m_jitterPercentage = 0.1f;

	Material m_material;
	float m_delayTime;
	float m_nextUpdateTime;

	void OnEnable()
	{
		var renderer = GetComponent<Renderer>();

		Debug.Assert( m_materialIndex <= renderer.materials.Length );

		m_material = renderer.materials[ m_materialIndex ];

		m_delayTime = 1.0f / m_frameRate;

		m_nextUpdateTime = Random.Range( 0.0f, m_delayTime );
	}

	void Update()
	{
		m_nextUpdateTime -= Time.deltaTime;

		if ( m_nextUpdateTime <= 0.0f )
		{
			var x = 1.0f / m_columns * Random.Range( 0, m_columns );
			var y = 1.0f / m_rows * Random.Range( 0, m_rows );

			m_material.SetTextureOffset( "_EmissiveColorMap", new Vector2( x, y ) );

			m_nextUpdateTime += m_delayTime + m_delayTime * Random.Range( -m_jitterPercentage * 0.5f, m_jitterPercentage * 0.5f );
		}
	}
}
