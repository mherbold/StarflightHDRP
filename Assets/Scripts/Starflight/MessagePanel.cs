
using TMPro;
using UnityEngine;

public class MessagePanel : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI m_text;

	public static MessagePanel m_instance;

	// the current state of the dock (out or in)
	bool m_isOut;

	// keep track of whether or not we need to update the messages ui
	bool m_textChanged;

	// unity awake
	void Awake()
	{
		Debug.Log( "MessagePanel Awake" );

		// global access to the message panel
		m_instance = this;

		// dock is in
		m_isOut = false;
	}

	// unity late update
	void LateUpdate()
	{
		// update the display if the text has changed
		if ( m_textChanged )
		{
			m_textChanged = false;

			var playerData = DataController.m_instance.m_playerData;

			m_text.text = "";

			for ( var i = 0; i < playerData.m_general.m_messageList.Count; i++ )
			{
				if ( i != 0 )
				{
					m_text.text += "\n<line-height=25%>\n</line-height>";
				}

				m_text.text += playerData.m_general.m_messageList[ i ];
			}

			// switch from top left alignment to bottom left alignment when text height is taller than message window
			m_text.ForceMeshUpdate();

			RectTransform rectTransform = GetComponent<RectTransform>();

			var rectHeight = rectTransform.rect.height; // TODO fix this as this is is wrong for the re-remake

			m_text.alignment = ( m_text.renderedHeight > rectHeight ) ? TextAlignmentOptions.BottomLeft : TextAlignmentOptions.TopLeft;
		}
	}

	// call this to clear out all the messages
	public void Clear()
	{
		var playerData = DataController.m_instance.m_playerData;

		playerData.m_general.m_messageList.Clear();

		m_textChanged = true;
	}

	// call this to add some text
	public void AddText( string newMessage )
	{
		var playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_general.m_messageList.Count == 10 )
		{
			playerData.m_general.m_messageList.RemoveAt( 0 );
		}

		playerData.m_general.m_messageList.Add( newMessage );

		m_textChanged = true;
	}

	// call this to force a refresh
	public void Refresh()
	{
		m_textChanged = true;
	}

	// call this to slide out the messages "dock"
	public void SlideOut()
	{
		if ( !m_isOut )
		{
			m_isOut = true;

			// TODO stop slide in timeline and start slide out timeline
		}
	}

	// call this to slide back in the messages "dock"
	public void SlideIn()
	{
		if ( m_isOut )
		{
			m_isOut = false;

			// TODO stop slide out timeline and start slide in timeline
		}
	}
}
