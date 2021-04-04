
using TMPro;
using UnityEngine;

[RequireComponent( typeof(TextMeshProUGUI))]
public class Coordinates : MonoBehaviour
{
	public static Coordinates m_instance;

	TextMeshProUGUI m_text;

	void Awake()
	{
		m_instance = this;

		m_text = GetComponent<TextMeshProUGUI>();
	}


	// call this to update the coordinates display
	public void UpdateDisplay()
	{
		var playerData = DataController.m_instance.m_playerData;

		string text;

		if ( playerData.m_general.m_activity == PD_General.Activity.Disembarked )
		{
			Tools.WorldToLatLongCoordinates( playerData.m_general.m_lastDisembarkedCoordinates, out var x, out var z );

			var latitude = Mathf.RoundToInt( x );
			var longitude = Mathf.RoundToInt( z );

			if ( latitude < 0 )
			{
				text = ( -latitude ).ToString() + " W";
			}
			else
			{
				text = latitude.ToString() + " E";
			}

			text += "   ";

			if ( longitude < 0 )
			{
				text += ( -longitude ).ToString() + " S";
			}
			else
			{
				text += longitude.ToString() + " N";
			}
		}
		else
		{
			var gameCoordinates = Tools.WorldToGameCoordinates( playerData.m_general.m_lastHyperspaceCoordinates );

			var x = Mathf.RoundToInt( gameCoordinates.x );
			var y = Mathf.RoundToInt( gameCoordinates.z );

			text = x.ToString() + "   " + y.ToString();
		}

		m_text.text = text;
	}
}
