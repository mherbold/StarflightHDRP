
using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
	[SerializeField] Transform m_objectToFollow;
	[SerializeField] Vector3 m_rotationScale;

	void Update()
	{
		if ( m_objectToFollow != null )
		{
			var rotation = m_objectToFollow.rotation;

			var eulerAngles = rotation.eulerAngles;

			rotation = Quaternion.Euler( eulerAngles.x * m_rotationScale.x, eulerAngles.y * m_rotationScale.y, eulerAngles.z * m_rotationScale.z );

			transform.rotation = rotation;
		}
	}
}
