
using System;
using TMPro;
using UnityEngine;

[Serializable]
class Notices
{
	[SerializeField] TextMeshProUGUI m_stardateTMP;
	[SerializeField] TextMeshProUGUI m_messageTMP;

	[SerializeField] float m_messageSmoothScrollLerpFactor;

	int m_latestNoticeId;
	int m_currentNoticeId;
	int m_currentLine;
	float m_currentMessageOffset;
	bool m_endOfMessageReached;

	DoorPanelController m_doorPanelController;

	public void Initialize( DoorPanelController doorPanelController )
	{
		m_doorPanelController = doorPanelController;

		// reset some variables
		m_latestNoticeId = 0;
		m_currentNoticeId = 0;

		// get access to the player progress
		var playerData = DataController.m_instance.m_playerData;

		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// update the stardate text
		var dateTime = DateTime.ParseExact( playerData.m_general.m_currentStardateYMD, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture );

		m_stardateTMP.text = dateTime.ToShortDateString();

		// figure out which notice we should be showing (current notice)
		var earliestNewNoticeStardate = "9999-99-99";

		for ( int noticeId = 0; noticeId < gameData.m_noticeList.Length; noticeId++ )
		{
			var notice = gameData.m_noticeList[ noticeId ];

			if ( string.Compare( playerData.m_general.m_currentStardateYMD, notice.m_stardate ) >= 0 )
			{
				m_latestNoticeId = noticeId;

				if ( string.Compare( notice.m_stardate, playerData.m_starport.m_lastReadNoticeStardate ) >= 0 )
				{
					if ( string.Compare( earliestNewNoticeStardate, notice.m_stardate ) >= 0 )
					{
						if ( notice.m_stardate != playerData.m_starport.m_lastReadNoticeStardate )
						{
							earliestNewNoticeStardate = notice.m_stardate;
						}

						m_currentNoticeId = noticeId;
					}
				}
			}
		}

		// show the current message
		ShowMessage();
	}

	public void Update()
	{
		// smooth scroll the position (y)
		m_currentMessageOffset = Mathf.Lerp( m_currentMessageOffset, m_messageTMP.renderedHeight, Time.deltaTime * m_messageSmoothScrollLerpFactor );

		// move up the message text game object
		m_messageTMP.rectTransform.anchoredPosition = new Vector3( 0.0f, m_currentMessageOffset, 0.0f);
	}

	// call this to start displaying the current message
	void ShowMessage()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// reset some variables
		m_currentLine = 0;
		m_currentMessageOffset = 0.0f;
		m_endOfMessageReached = false;

		// show the first line of the current message
		ShowNextLine();

		// update the buttons
		UpdateButtons();

		// add the current notice to the ships log
		playerData.m_shipsLog.AddStarportNotice( m_currentNoticeId );
	}

	// this is called if we clicked on the prev button
	public void ShowPreviousNotice()
	{
		// show the previous notice
		if ( m_currentNoticeId > 0 )
		{
			m_currentNoticeId--;

			ShowMessage();
		}
	}

	// this is called if we clicked on the next button
	public void ShowNextNotice()
	{
		// show the next notice
		if ( m_currentNoticeId < m_latestNoticeId )
		{
			m_currentNoticeId++;

			ShowMessage();
		}
	}

	// call this to show the next line of the current message
	public void ShowNextLine()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get the current notice
		var currentNotice = gameData.m_noticeList[ m_currentNoticeId ];

		// check if we are displaying the first line
		if ( m_currentLine == 0 )
		{
			// clear the text and add the date
			var messageDate = DateTime.ParseExact( currentNotice.m_stardate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture );

			m_messageTMP.text = messageDate.ToShortDateString();

			// remember the newest notice read
			if ( string.Compare( currentNotice.m_stardate, DataController.m_instance.m_playerData.m_starport.m_lastReadNoticeStardate ) > 0 )
			{
				DataController.m_instance.m_playerData.m_starport.m_lastReadNoticeStardate = currentNotice.m_stardate;
			}
		}

		// get the current line of the message
		var subStrings = currentNotice.m_message.Split( new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );

		if ( m_currentLine < subStrings.Length )
		{
			m_messageTMP.text += "\r\n\r\n" + subStrings[ m_currentLine ];
			m_currentLine++;

			// check if we have reached the end of this message
			if ( m_currentLine == subStrings.Length )
			{
				m_endOfMessageReached = true;

				UpdateButtons();
			}
		}

		// force the mesh to update so the rendered height calculation is correct
		m_messageTMP.ForceMeshUpdate();

		// play a ui sound
		Sounds.m_instance.PlayUpdate();
	}

	// update which notices buttons are enabled and selected
	void UpdateButtons()
	{
		m_doorPanelController.SetCurrentButton( Operations.c_noticesQuitButtonIndex );

		// enable / disable the previous and next buttons based on what our current notice index is
		m_doorPanelController.EnableButton( Operations.c_noticesPreviousButtonIndex, m_currentNoticeId > 0 );
		m_doorPanelController.EnableButton( Operations.c_noticesNextButtonIndex, m_currentNoticeId < m_latestNoticeId );

		// check if we have reached the end of the current notice
		if ( m_endOfMessageReached )
		{
			m_doorPanelController.EnableButton( Operations.c_noticesMoreButtonIndex, false );

			if ( m_currentNoticeId < m_latestNoticeId )
			{
				m_doorPanelController.SetCurrentButton( Operations.c_noticesNextButtonIndex );
			}
		}
		else
		{
			m_doorPanelController.EnableButton( Operations.c_noticesMoreButtonIndex, true );

			m_doorPanelController.SetCurrentButton( Operations.c_noticesMoreButtonIndex );
		}
	}
}
