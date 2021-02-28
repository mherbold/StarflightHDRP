
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof( Camera ) )]
public class RenderTextureResizer : MonoBehaviour
{
	[SerializeField] Canvas m_canvas;
	[SerializeField] RawImage m_rawImage;

	[SerializeField] float m_left;
	[SerializeField] float m_right;
	[SerializeField] float m_top;
	[SerializeField] float m_bottom;

	Camera m_camera;

	void OnEnable()
	{
		m_camera = GetComponent<Camera>();
	}

	void Update()
	{
		// get the current canvas width and height
		var canvasWidth = m_canvas.GetComponent<RectTransform>().rect.width;
		var canvasHeight = m_canvas.GetComponent<RectTransform>().rect.height;

		// calculate the new rect for a viewport that fits exactly in the hole
		var x = m_left / canvasWidth;
		var y = m_top / canvasHeight;

		var width = Mathf.RoundToInt( Screen.width * ( 1.0f - ( m_right / canvasWidth ) - x ) );
		var height = Mathf.RoundToInt( Screen.height * ( 1.0f - ( m_bottom / canvasHeight ) - y ) );

		// get the descriptor of the current render texture
		var renderTextureDescriptor = m_camera.targetTexture.descriptor;

		// check if the current render texture is still the size we need
		if ( ( renderTextureDescriptor.width == width ) && ( renderTextureDescriptor.height == height ) )
		{
			// it is - don't update it
			return;
		}

		// it is not - remove the old render texture
		m_camera.targetTexture.Release();

		renderTextureDescriptor.width = width;
		renderTextureDescriptor.height = height;

		// create a new render texture
		var renderTexture = new RenderTexture( renderTextureDescriptor );

		// give the new render texture to the camera
		m_camera.targetTexture = renderTexture;

		// update the texture on the raw image
		m_rawImage.texture = renderTexture;

		// debug log
		Debug.Log( "Render texture resized to " + width + " x " + height + "." );
	}
}
