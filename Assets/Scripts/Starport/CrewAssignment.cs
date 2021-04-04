
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrewAssignment : DoorPanelController
{
	const int c_assignButtonIndex = 0;
	const int c_exitButtonIndex = 1;

	const int c_upButtonIndex = 2;
	const int c_downButtonIndex = 3;

	const int c_leftButtonIndex = 4;
	const int c_rightButtonIndex = 5;

	const int c_assignNowButtonIndex = 6;

	const int c_numSkills = 5;

	[SerializeField] TextMeshProUGUI m_positionValuesTMP;
	[SerializeField] TextMeshProUGUI m_nameTMP;
	[SerializeField] TextMeshProUGUI m_skillNamesTMP;
	[SerializeField] TextMeshProUGUI m_skillValuesTMP;
	[SerializeField] TextMeshProUGUI m_everybodyIsDeadTMP;

	[SerializeField] Image m_upArrowImage;
	[SerializeField] Image m_downArrowImage;
	[SerializeField] Image m_leftArrowImage;
	[SerializeField] Image m_rightArrowImage;

	[SerializeField] RectTransform m_positionSelection;

	[SerializeField] float m_positionSelectionSpacing = 10.0f;

	PD_CrewAssignment.Role m_currentRole;
	int m_currentPersonnelId;

	Vector2 m_firstSelectionAnchoredPosition;

	protected override void Initialize()
	{
		m_firstSelectionAnchoredPosition = m_positionSelection.anchoredPosition;
	}

	protected override void Restart()
	{
		// reset this
		m_currentPersonnelId = 0;

		// switch to menu bar state
		SwitchToMenuBarState( false );
	}

	public override void OnFire()
	{
		base.OnFire();

		switch ( m_currentButtonIndex )
		{
			case c_assignButtonIndex:
				SwitchToAssignPersonnelState();
				break;

			case c_exitButtonIndex:
				Exit();
				break;

			case c_upButtonIndex:
				SelectPreviousPosition();
				break;

			case c_downButtonIndex:
				SelectNextPosition();
				break;

			case c_leftButtonIndex:
				SelectPreviousPersonnel();
				break;

			case c_rightButtonIndex:
				SelectNextPersonnel();
				break;
		}
	}

	public override void OnCancel()
	{
		base.OnCancel();

		switch ( m_currentButtonIndex )
		{
			case c_assignButtonIndex:
			case c_exitButtonIndex:
				Exit();
				break;

			case c_assignNowButtonIndex:
				SwitchToMenuBarState( true, true );
				break;
		}
	}

	// call this to switch to the menu bar state
	void SwitchToMenuBarState( bool makeNoise = true, bool selectExitButtonInstead = false )
	{
		// hide the selection bar
		m_positionSelection.gameObject.SetActive( false );

		// hide (and disable) the assign now button and arrows
		ShowButton( c_assignNowButtonIndex, false );
		ShowButton( c_upButtonIndex, false );
		ShowButton( c_downButtonIndex, false );
		ShowButton( c_leftButtonIndex, false );
		ShowButton( c_rightButtonIndex, false );

		// hide stuff in the skills panel
		m_nameTMP.gameObject.SetActive( false );
		m_skillNamesTMP.gameObject.SetActive( false );
		m_skillValuesTMP.gameObject.SetActive( false );

		// enable the exit button
		EnableButton( c_exitButtonIndex, true );

		// update the assigned crewmember list
		UpdateAssignedCrewmemberList();

		// get access to the personnel player data
		var personnel = DataController.m_instance.m_playerData.m_personnel;

		// check if we have at least one living crewmember in personnel
		if ( personnel.AnyLiving() )
		{
			// hide the everyone is dead message
			m_everybodyIsDeadTMP.gameObject.SetActive( false );

			if ( selectExitButtonInstead )
			{
				// enable the assign button
				EnableButton( c_assignButtonIndex, true );

				// make the exit button the current button
				SetCurrentButton( c_exitButtonIndex, false );
			}
			else
			{
				// make the assign button the current button
				SetCurrentButton( c_assignButtonIndex, false );
			}

			// update the screen
			UpdateScreen();
		}
		else
		{
			// show the everyone is dead message
			m_everybodyIsDeadTMP.gameObject.SetActive( true );

			// disable the assign button
			EnableButton( c_assignButtonIndex, false );

			// select the exit button
			SetCurrentButton( c_exitButtonIndex, false );
		}

		if ( makeNoise )
		{
			Sounds.m_instance.PlayDeactivate();
		}
	}

	// call this to switch to the select race state
	void SwitchToAssignPersonnelState()
	{
		// start with the captain
		ChangeCurrentRole( PD_CrewAssignment.Role.Captain );

		// show stuff in the skills panel
		m_nameTMP.gameObject.SetActive( true );
		m_skillNamesTMP.gameObject.SetActive( true );
		m_skillValuesTMP.gameObject.SetActive( true );

		// show the selection bar
		m_positionSelection.gameObject.SetActive( true );

		// show (and enable) the assign now button
		ShowButton( c_assignNowButtonIndex, true );

		// get access to the personnel player data
		var personnel = DataController.m_instance.m_playerData.m_personnel;

		// show the left and right arrows only if we have more than one personnel on file
		if ( personnel.m_personnelList.Count > 1 )
		{
			ShowButton( c_leftButtonIndex, true );
			ShowButton( c_rightButtonIndex, true );
		}

		// disable the assign and exit buttons
		EnableButton( c_assignButtonIndex, false );
		EnableButton( c_exitButtonIndex, false );

		// set the current button
		SetCurrentButton( c_assignNowButtonIndex, false );

		// update the display
		UpdateScreen();

		// beep
		Sounds.m_instance.PlayActivate();
	}

	// update the assigned crewmember list
	void UpdateAssignedCrewmemberList()
	{
		// get access to the crew assignment player data
		var crewAssignment = DataController.m_instance.m_playerData.m_crewAssignment;

		// start with an empty text string
		m_positionValuesTMP.text = "";

		// go through each position
		for ( var role = PD_CrewAssignment.Role.First; role < PD_CrewAssignment.Role.Count; role++ )
		{
			// get the file id for the assigned crewmember
			if ( crewAssignment.IsAssigned( role ) )
			{
				// get the personnel file for that role
				var personnelFile = crewAssignment.GetPersonnelFile( role );

				// add the crewmember's name
				m_positionValuesTMP.text += personnelFile.m_name;
			}
			else
			{
				// add the not assigned text
				m_positionValuesTMP.text += "[Not Assigned]";
			}

			if ( role < ( PD_CrewAssignment.Role.Count - 1 ) )
			{
				m_positionValuesTMP.text += Environment.NewLine;
			}
		}
	}

	void ChangeCurrentRole( PD_CrewAssignment.Role role )
	{
		// update the current position index
		m_currentRole = role;

		// get access to the crew assignment player data
		var crewAssignment = DataController.m_instance.m_playerData.m_crewAssignment;

		// check if we have don't have someone assigned to this position
		if ( !crewAssignment.IsAssigned( m_currentRole ) )
		{
			// automatically select the first personnel file
			ChangeCurrentPersonnelId( 0, true );
		}
		else
		{
			// get the current file id for this position
			var fileId = crewAssignment.GetFileId( m_currentRole );

			// get access to the personnel player data
			var personnel = DataController.m_instance.m_playerData.m_personnel;

			// update the current personnel id
			m_currentPersonnelId = personnel.GetPersonnelId( fileId );
		}
	}

	void ChangeCurrentPersonnelId( int personnelId, bool forceUpdate = false )
	{
		// don't do anything if we aren't changing the file index to a different one
		if ( ( personnelId != m_currentPersonnelId ) || forceUpdate )
		{
			// update the current personnel id
			m_currentPersonnelId = personnelId;

			// get access to the crew assignment player data
			var crewAssignment = DataController.m_instance.m_playerData.m_crewAssignment;

			// get access to the personnel player data
			var personnel = DataController.m_instance.m_playerData.m_personnel;

			// get the personnel file
			var personnelFile = personnel.m_personnelList[ m_currentPersonnelId ];

			// assign this personnel to this position
			crewAssignment.Assign( m_currentRole, personnelFile.m_fileId );

			// update the assigned crewmember list
			UpdateAssignedCrewmemberList();

			// play a sound
			Sounds.m_instance.PlayUpdate();
		}
	}

	void UpdateScreen()
	{
		// show the up arrow only if we are not at the first position index
		ShowButton( c_upButtonIndex, m_currentRole != PD_CrewAssignment.Role.First );

		// show the down arrow only if we are not at the last position index
		ShowButton( c_downButtonIndex, m_currentRole != ( PD_CrewAssignment.Role.Count - 1 ) );

		// put the position selection box in the right place
		var offset = (int) m_currentRole * m_positionSelectionSpacing;

		m_positionSelection.anchoredPosition = m_firstSelectionAnchoredPosition + new Vector2( 0.0f, offset );

		// get access to the personnel player data
		var personnel = DataController.m_instance.m_playerData.m_personnel;

		// get the personnel file
		var personnelFile = personnel.m_personnelList[ m_currentPersonnelId ];

		// update the crewmember name
		if ( personnelFile.m_vitality > 0 )
		{
			m_nameTMP.text = personnelFile.m_name + " - " + personnelFile.m_vitality + "% vitality";
		}
		else
		{
			m_nameTMP.text = personnelFile.m_name + " - DEAD";
		}

		// update the skill values
		m_skillValuesTMP.text = "";

		for ( var skillIndex = 0; skillIndex < c_numSkills; skillIndex++ )
		{
			m_skillValuesTMP.text += personnelFile.GetSkill( skillIndex ).ToString();

			if ( skillIndex < ( c_numSkills - 1 ) )
			{
				m_skillValuesTMP.text += Environment.NewLine;
			}
		}
	}

	void SelectPreviousPosition()
	{
		ChangeCurrentRole( m_currentRole - 1 );

		SetCurrentButton( c_assignNowButtonIndex, false );

		UpdateScreen();
	}

	void SelectNextPosition()
	{
		ChangeCurrentRole( m_currentRole + 1 );

		SetCurrentButton( c_assignNowButtonIndex, false );

		UpdateScreen();
	}

	void SelectPreviousPersonnel()
	{
		// get access to the personnel player data
		var personnel = DataController.m_instance.m_playerData.m_personnel;

		ChangeCurrentPersonnelId( ( m_currentPersonnelId + personnel.m_personnelList.Count - 1 ) % personnel.m_personnelList.Count );

		SetCurrentButton( c_assignNowButtonIndex, false );

		UpdateScreen();
	}

	void SelectNextPersonnel()
	{
		// get access to the personnel player data
		var personnel = DataController.m_instance.m_playerData.m_personnel;

		ChangeCurrentPersonnelId( ( m_currentPersonnelId + 1 ) % personnel.m_personnelList.Count );

		SetCurrentButton( c_assignNowButtonIndex, false );

		UpdateScreen();
	}
}
