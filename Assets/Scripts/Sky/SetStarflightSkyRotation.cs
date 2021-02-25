
using UnityEngine;

[ExecuteInEditMode]
public class SetStarflightSkyRotation : MonoBehaviour
{
	void Update()
	{
		StarflightSkyRenderer.SetRotation( transform.rotation );
	}
}
