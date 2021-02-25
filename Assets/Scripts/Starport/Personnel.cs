
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Personnel : DoorPanelController
{
	const int c_createButtonIndex = 0;
	const int c_previousButtonIndex = 1;
	const int c_nextButtonIndex = 2;
	const int c_exitButtonIndex = 3;
	const int c_trainButtonIndex = 4;
	const int c_deleteButtonIndex = 5;
	const int c_selectButtonIndex = 6;
	const int c_cancelButtonIndex = 7;
	const int c_yesButtonIndex = 8;
	const int c_noButtonIndex = 9;
	const int c_upButtonIndex = 10;
	const int c_downButtonIndex = 11;
	const int c_leftButtonIndex = 12;
	const int c_rightButtonIndex = 13;
	const int c_trainNowButtonIndex = 14;

	const int c_numRaces = 5;
	const int c_numSkills = 5;

	[SerializeField] TextMeshProUGUI m_fileNumberTMP;
	[SerializeField] TextMeshProUGUI m_skillValuesTMP;
	[SerializeField] TextMeshProUGUI m_raceNameTMP;
	[SerializeField] TextMeshProUGUI m_bioValuesTMP;
	[SerializeField] TextMeshProUGUI m_vitalityTMP;
	[SerializeField] TextMeshProUGUI m_bankBalanceTMP;
	[SerializeField] TextMeshProUGUI m_trainingCostTMP;
	[SerializeField] TextMeshProUGUI m_nameInputTMP;

	[SerializeField] Image m_humanImage;
	[SerializeField] Image m_veloxImage;
	[SerializeField] Image m_thrynnImage;
	[SerializeField] Image m_elowanImage;
	[SerializeField] Image m_androidImage;

	[SerializeField] Image m_leftArrowImage;
	[SerializeField] Image m_rightArrowImage;

	[SerializeField] RectTransform m_skillSelection;

	[SerializeField] GameObject m_overlay;
	[SerializeField] GameObject m_nameModal;
	[SerializeField] GameObject m_deleteModal;

	[SerializeField] float m_skillSelectionSpacing = 10.0f;

	int m_currentFileIndex;
	int m_currentRaceIndex;
	int m_currentSkillIndex;

	int m_startingBankBalance;

	string m_name;

	Vector2 m_firstSelectionAnchoredPosition;

	protected override void Initialize()
	{
		m_firstSelectionAnchoredPosition = m_skillSelection.anchoredPosition;
	}

	protected override void Restart()
	{
		// view the first file
		m_currentFileIndex = 0;

		// remember the starting bank balance
		m_startingBankBalance = DataController.m_instance.m_playerData.m_bank.m_currentBalance;

		// switch to the default view
		SwitchToViewFileState( false );
	}

	public override void OnFire()
	{
		base.OnFire();

		switch ( m_currentButtonIndex )
		{
			case c_createButtonIndex:
				SwitchToSelectRaceState();
				break;

			case c_previousButtonIndex:
				SelectPreviousFile();
				break;

			case c_nextButtonIndex:
				SelectNextFile();
				break;

			case c_exitButtonIndex:
				ClosePanel();
				break;

			case c_trainButtonIndex:
				StartTraining();
				break;

			case c_deleteButtonIndex:
				SwitchToDeleteCrewmemberState();
				break;

			case c_selectButtonIndex:
				SwitchToGiveNameState();
				break;

			case c_cancelButtonIndex:
				SwitchToViewFileState( true, true );
				break;

			case c_yesButtonIndex:
				DeleteCrewmember();
				break;

			case c_noButtonIndex:
				SwitchToViewFileState( true, true );
				break;

			case c_upButtonIndex:
				SelectPreviousSkill();
				break;

			case c_downButtonIndex:
				SelectNextSkill();
				break;

			case c_leftButtonIndex:
				SelectPreviousRace();
				break;

			case c_rightButtonIndex:
				SelectNextRace();
				break;

			case c_trainNowButtonIndex:
				TrainSelectedSkill();
				break;
		}
	}
	public override void OnCancel()
	{
		base.OnCancel();

		switch ( m_currentButtonIndex )
		{
			case c_createButtonIndex:
			case c_previousButtonIndex:
			case c_nextButtonIndex:
			case c_exitButtonIndex:
			case c_trainButtonIndex:
			case c_deleteButtonIndex:
				ClosePanel();
				break;

			case c_selectButtonIndex:
			case c_cancelButtonIndex:
				SwitchToViewFileState( true, true );
				break;

			case c_yesButtonIndex:
			case c_noButtonIndex:
				SwitchToViewFileState( true, true );
				break;

			case c_trainNowButtonIndex:
				SwitchToViewFileState( true, true );
				break;
		}
	}

	public override void OnTextInput( char ch )
	{
		base.OnTextInput( ch );

		if ( ch == (char) KeyCode.Return )
		{
			// stop handling keyboard input
			SetInputFocus( false );

			// strip off whitespace at the ends
			m_name = m_name.Trim();

			if ( m_name.Length == 0 )
			{
				// play a ui sound
				PlayDeactivateSound();
			}
			else
			{
				// get the current race game data
				var race = DataController.m_instance.m_gameData.m_crewRaceList[ m_currentRaceIndex ];

				// create a new personnel file
				var personnelFile = DataController.m_instance.m_playerData.m_personnel.CreateNewPersonnel();

				// set up the personnel file
				personnelFile.m_name = m_name;
				personnelFile.m_vitality = 100.0f;
				personnelFile.m_crewRaceId = m_currentRaceIndex;
				personnelFile.m_science = race.m_scienceInitial;
				personnelFile.m_navigation = race.m_navigationInitial;
				personnelFile.m_engineering = race.m_engineeringInitial;
				personnelFile.m_communications = race.m_communicationsInitial;
				personnelFile.m_medicine = race.m_medicineInitial;

				// add the new personnel file to the list
				DataController.m_instance.m_playerData.m_personnel.m_personnelList.Add( personnelFile );

				// make the new file our current one
				m_currentFileIndex = DataController.m_instance.m_playerData.m_personnel.m_personnelList.Count - 1;

				// play a ui sound
				PlayUpdateSound();
			}

			// switch to the view file state
			SwitchToViewFileState( false );
		}
		else if ( ch == (char) KeyCode.Backspace )
		{
			if ( m_name.Length > 0 )
			{
				m_name = m_name.Remove( m_name.Length - 1 );
			}
			else
			{
				PlayErrorSound();
			}
		}
		else if ( ( ch >= (char) KeyCode.Space ) && ( ch <= (char) KeyCode.Tilde ) )
		{
			if ( m_name.Length < 32 )
			{
				m_name += ch;
			}
			else
			{
				PlayErrorSound();
			}
		}
		else
		{
			PlayErrorSound();
		}

		m_nameInputTMP.text = m_name;
	}

	// call this to switch to the view file state
	void SwitchToViewFileState( bool makeNoise = true, bool playDeactivateSoundInstead = false )
	{
		// hide all the stuff that isn't visible in this state
		m_skillSelection.gameObject.SetActive( false );
		m_vitalityTMP.gameObject.SetActive( false );
		m_bankBalanceTMP.gameObject.SetActive( false );
		m_trainingCostTMP.gameObject.SetActive( false );
		m_leftArrowImage.gameObject.SetActive( false );
		m_rightArrowImage.gameObject.SetActive( false );
		m_overlay.SetActive( false );
		m_nameModal.SetActive( false );
		m_deleteModal.SetActive( false );

		ShowButton( c_trainButtonIndex, false );
		ShowButton( c_deleteButtonIndex, false );
		ShowButton( c_selectButtonIndex, false );
		ShowButton( c_cancelButtonIndex, false );

		// update the screen
		UpdateScreenForViewFileState();

		// show the delete button (but not necessarily enable it)
		ShowDeleteButton();

		// select the create button or the exit button if the create button is not enabled
		SetCurrentButton( ButtonIsEnbled( c_createButtonIndex ) ? c_createButtonIndex : c_exitButtonIndex );

		// beep
		if ( makeNoise )
		{
			if ( playDeactivateSoundInstead )
			{
				PlayDeactivateSound();
			}
			else
			{
				PlayActivateSound();
			}
		}
	}

	// call this to switch to the select race state
	void SwitchToSelectRaceState()
	{
		// disable main buttons
		EnableButton( c_createButtonIndex, false );
		EnableButton( c_previousButtonIndex, false );
		EnableButton( c_nextButtonIndex, false );
		EnableButton( c_exitButtonIndex, false );

		// start with the human
		m_currentRaceIndex = 0;

		// update the screen
		UpdateScreenForSelectRaceState();

		// select the select button by default
		SetCurrentButton( c_selectButtonIndex );

		// beep
		PlayActivateSound();
	}

	// call this to switch to the delete crewmember state
	private void SwitchToDeleteCrewmemberState()
	{
		// disable various buttons
		EnableButton( c_trainButtonIndex, false );
		EnableButton( c_deleteButtonIndex, false );
		EnableButton( c_createButtonIndex, false );
		EnableButton( c_previousButtonIndex, false );
		EnableButton( c_nextButtonIndex, false );
		EnableButton( c_exitButtonIndex, false );

		//  update the screen
		UpdateScreenForDeleteCrewmemberState();

		// select the no button by default
		SetCurrentButton( c_noButtonIndex );

		// beep
		PlayActivateSound();
	}

	// call this to switch to the give name state
	private void SwitchToGiveNameState()
	{
		// disable select race buttons
		EnableButton( c_selectButtonIndex, false );
		EnableButton( c_cancelButtonIndex, false );

		// erase the current text input
		m_nameInputTMP.text = m_name = "";

		// update the screen
		UpdateScreenForGiveNameState();

		// beep
		PlayActivateSound();

		// read input from any key on the keyboard
		SetInputFocus( true );
	}

	// call this to switch to the train crewmember state
	private void SwitchToTrainCrewmemberState()
	{
		// disable various buttons
		EnableButton( c_trainButtonIndex, false );
		EnableButton( c_deleteButtonIndex, false );
		EnableButton( c_createButtonIndex, false );
		EnableButton( c_previousButtonIndex, false );
		EnableButton( c_nextButtonIndex, false );
		EnableButton( c_exitButtonIndex, false );

		// hide the vitality text
		m_vitalityTMP.gameObject.SetActive( false );

		// select the first skill by default
		m_currentSkillIndex = 0;

		// update the screen
		UpdateScreenForTrainCrewmemberState();

		// beep
		PlayActivateSound();
	}

	// update screen for the view file state
	void UpdateScreenForViewFileState()
	{
		// get access to the player data
		var playerData = DataController.m_instance.m_playerData;

		// enable the create button only if we have less than 20 personnel files
		EnableButton( c_createButtonIndex, playerData.m_personnel.m_personnelList.Count < 20 );

		// check if we have any personnel files
		if ( playerData.m_personnel.m_personnelList.Count == 0 )
		{
			// we dont have any personnel files
			m_fileNumberTMP.text = "No Personnel Files Found";

			m_vitalityTMP.gameObject.SetActive( false );

			// disable the previous and next buttons
			EnableButton( c_previousButtonIndex, false );
			EnableButton( c_nextButtonIndex, false );

			// hide the delete and train buttons (it also automatically disables them)
			ShowButton( c_deleteButtonIndex, false );
			ShowButton( c_trainButtonIndex, false );

			// hide the personnel file
			ShowPersonnelFile( false );
		}
		else
		{
			// get access to the current personnel file we are looking at
			var personnelFile = playerData.m_personnel.m_personnelList[ m_currentFileIndex ];

			// update the current race index
			m_currentRaceIndex = personnelFile.m_crewRaceId;

			// update the personnel file number
			m_fileNumberTMP.text = "File # " + ( m_currentFileIndex + 1 ) + ": " + personnelFile.m_name;

			// enable the previous button if we are not looking at the first personnel file
			EnableButton( c_previousButtonIndex, m_currentFileIndex > 0 );

			// enable the next button if we are not looking at the last personnel file
			EnableButton( c_nextButtonIndex, m_currentFileIndex < ( playerData.m_personnel.m_personnelList.Count - 1 ) );

			// show the vitaliaty text
			m_vitalityTMP.gameObject.SetActive( true );

			// show the train button (it also automatically enables it)
			ShowButton( c_trainButtonIndex, true );

			// update the skill values
			UpdateSkillValues();

			// show the personnel file
			ShowPersonnelFile( true );
		}

		// always enable the exit button
		EnableButton( c_exitButtonIndex, true );
	}

	// update screen for the select race state
	void UpdateScreenForSelectRaceState()
	{
		// update the file number text
		m_fileNumberTMP.text = "New Personnel File";

		// show the left and right arrows
		ShowButton( c_leftButtonIndex, true );
		ShowButton( c_rightButtonIndex, true );

		// show the select and cancel buttons
		ShowButton( c_selectButtonIndex, true );
		ShowButton( c_cancelButtonIndex, true );

		// get the current race game data
		var race = DataController.m_instance.m_gameData.m_crewRaceList[ m_currentRaceIndex ];

		// update the skill values text to show the race's initial values
		m_skillValuesTMP.text = "";

		for ( var skillIndex = 0; skillIndex < c_numSkills; skillIndex++ )
		{
			m_skillValuesTMP.text += race.GetInitialSkill( skillIndex ).ToString();

			if ( skillIndex < ( c_numSkills - 1 ) )
			{
				m_skillValuesTMP.text += Environment.NewLine;
			}
		}

		// we do want to show the personnel file
		ShowPersonnelFile( true );
	}

	// update screen for the delete crewmember state
	void UpdateScreenForDeleteCrewmemberState()
	{
		// show the overlay
		m_overlay.SetActive( true );

		// show the delete modal
		m_deleteModal.SetActive( true );
	}

	// update screen for the give name state
	void UpdateScreenForGiveNameState()
	{
		// show the overlay
		m_overlay.SetActive( true );

		// show the name modal
		m_nameModal.SetActive( true );
	}

	// update screen for the train crewmember state
	bool UpdateScreenForTrainCrewmemberState()
	{
		// show the selection xform object
		m_skillSelection.gameObject.SetActive( true );

		// show the up arrow only if the first skill is not selected
		ShowButton( c_upButtonIndex, m_currentSkillIndex > 0 );

		// show the down arrow only if the last skill is not selected
		ShowButton( c_downButtonIndex, m_currentSkillIndex < ( c_numSkills - 1 ) );

		// make the dummy button the current button
		SetCurrentButton( c_trainNowButtonIndex, false );

		// put the skill selection box in the right place
		var offset = m_currentSkillIndex * m_skillSelectionSpacing;

		m_skillSelection.anchoredPosition = m_firstSelectionAnchoredPosition + new Vector2( 0.0f, offset );

		// update the bank balance text
		UpdateBankBalanceText();

		// update the training text
		UpdateTrainingCostText( 0 );

		// we do want to show the personnel file
		return true;
	}

	// update the skill values
	void UpdateSkillValues()
	{
		// get access to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get access to the current personnel file we are looking at
		var personnelFile = playerData.m_personnel.m_personnelList[ m_currentFileIndex ];

		// update the skill values with the ones in this personnel file
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

	void ShowPersonnelFile( bool showPersonnelFile )
	{
		ShowRace( showPersonnelFile );

		m_skillValuesTMP.gameObject.SetActive( showPersonnelFile );
		m_bioValuesTMP.gameObject.SetActive( showPersonnelFile );
	}

	void ShowRace( bool showRace )
	{
		// turn off all the race images
		m_humanImage.gameObject.SetActive( false );
		m_veloxImage.gameObject.SetActive( false );
		m_thrynnImage.gameObject.SetActive( false );
		m_elowanImage.gameObject.SetActive( false );
		m_androidImage.gameObject.SetActive( false );

		// do we want to show the race?
		if ( showRace )
		{
			// get the current race game data
			var race = DataController.m_instance.m_gameData.m_crewRaceList[ m_currentRaceIndex ];

			// update the race name
			m_raceNameTMP.text = race.m_name;

			// update the bio values text to show the race's bio
			m_bioValuesTMP.text = race.m_type + Environment.NewLine + race.m_averageHeight + Environment.NewLine + race.m_averageWeight + Environment.NewLine + race.m_durability + Environment.NewLine + race.m_learningRate;

			// show the correct image for this race
			switch ( m_currentRaceIndex )
			{
				case 0: m_humanImage.gameObject.SetActive( true ); break;
				case 1: m_veloxImage.gameObject.SetActive( true ); break;
				case 2: m_thrynnImage.gameObject.SetActive( true ); break;
				case 3: m_elowanImage.gameObject.SetActive( true ); break;
				case 4: m_androidImage.gameObject.SetActive( true ); break;
			}
		}
		else
		{
			m_raceNameTMP.text = "Race";
		}
	}

	void SelectPreviousFile()
	{
		// go back one personnel file
		m_currentFileIndex--;

		// update the screen
		UpdateScreenForViewFileState();

		// switch to the next button if the prev button is no longer enabled
		if ( !ButtonIsEnbled( c_previousButtonIndex ) )
		{
			SetCurrentButton( c_nextButtonIndex );
		}

		// show the delete button (but not necessarily enable it)
		ShowDeleteButton();

		// play a ui sound
		PlayActivateSound();
	}

	void SelectNextFile()
	{
		// go to the next personnel file
		m_currentFileIndex++;

		// update the screen
		UpdateScreenForViewFileState();

		// switch to the prev button if the next button is no longer enabled
		if ( !ButtonIsEnbled( c_nextButtonIndex ) )
		{
			SetCurrentButton( c_previousButtonIndex );
		}

		// show the delete button (but not necessarily enable it)
		ShowDeleteButton();

		// play a ui sound
		PlayActivateSound();
	}

	void ShowDeleteButton()
	{
		// show the button
		ShowButton( c_deleteButtonIndex, true );

		// get access to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get access to the crew assignment player data
		var crewAssignment = playerData.m_crewAssignment;

		// get access to the personnel file for the selected personnel
		var personnelFile = playerData.m_personnel.m_personnelList[ m_currentFileIndex ];

		// is this guy on the ship crew? if so then don't let the player delete him
		if ( crewAssignment.IsAssignedToAnyRole( personnelFile.m_fileId ) )
		{
			EnableButton( c_deleteButtonIndex, false );
		}
	}

	// this is called if we clicked on the train button
	void StartTraining()
	{
		// check if the current race is an android
		if ( m_currentRaceIndex == 4 )
		{
			UpdateTrainingCostText( 1 );

			PlayErrorSound();
		}
		else
		{
			// get access to the player data
			var playerData = DataController.m_instance.m_playerData;

			// get access to the current personnel file we are looking at
			var personnelFile = playerData.m_personnel.m_personnelList[ m_currentFileIndex ];

			// get access to the race data for this personnel file
			var race = DataController.m_instance.m_gameData.m_crewRaceList[ m_currentRaceIndex ];

			// enable the train button only if the current personnel is not maxxed out
			var maxTotalPoints = 0;
			var currentTotalPoints = 0;

			for ( var skillIndex = 0; skillIndex < c_numSkills; skillIndex++ )
			{
				maxTotalPoints = race.GetMaximumSkill( skillIndex );
				currentTotalPoints = personnelFile.GetSkill( skillIndex );
			}

			// check if we are maxxed out
			if ( currentTotalPoints < maxTotalPoints )
			{
				// switch to the train select skill state
				SwitchToTrainCrewmemberState();
			}
			else
			{
				UpdateTrainingCostText( 2 );

				PlayErrorSound();
			}
		}
	}

	// update the bank balance text and show it
	void UpdateBankBalanceText()
	{
		// get access to the bank player data
		var bank = DataController.m_instance.m_playerData.m_bank;

		// update the bank balance
		m_bankBalanceTMP.text = "Bank balance: " + string.Format( "{0:n0}", bank.m_currentBalance ) + " M.U.";

		// show the bank balance text
		m_bankBalanceTMP.gameObject.SetActive( true );
	}

	// update the training text and show it
	void UpdateTrainingCostText( int messageIndex )
	{
		switch ( messageIndex )
		{
			case 0:
				m_trainingCostTMP.text = "Cost: 300 M.U. per session";
				break;

			case 1:
				m_trainingCostTMP.text = "Android skills are hard-wired";
				break;

			case 2:
				m_trainingCostTMP.text = "This crewmember is fully trained";
				break;

			case 3:
				m_trainingCostTMP.text = "This skill is fully trained";
				break;

			case 4:
				m_trainingCostTMP.text = "This skill cannot be trained";
				break;

			case 5:
				m_trainingCostTMP.text = "Your bank balance is too low";
				break;
		}

		// show the training text
		m_trainingCostTMP.gameObject.SetActive( true );
	}

	// this is called when the yes button in the delete panel is clicked
	void DeleteCrewmember()
	{
		// get to the personnel player data
		var personnel = DataController.m_instance.m_playerData.m_personnel;

		// delete the crewmember
		personnel.m_personnelList.RemoveAt( m_currentFileIndex );

		// change the current file index if necessary
		if ( m_currentFileIndex >= personnel.m_personnelList.Count )
		{
			m_currentFileIndex = Math.Max( 0, personnel.m_personnelList.Count - 1 );
		}

		// switch to the doing nothing state
		SwitchToViewFileState();
	}

	void SelectPreviousSkill()
	{
		m_currentSkillIndex--;

		UpdateTrainingCostText( 0 );

		UpdateScreenForTrainCrewmemberState();

		SetCurrentButton( c_trainNowButtonIndex, false );
	}

	void SelectNextSkill()
	{
		m_currentSkillIndex++;

		UpdateTrainingCostText( 0 );

		UpdateScreenForTrainCrewmemberState();

		SetCurrentButton( c_trainNowButtonIndex, false );
	}

	void SelectPreviousRace()
	{
		m_currentRaceIndex = ( m_currentRaceIndex + c_numRaces - 1 ) % c_numRaces;

		UpdateScreenForSelectRaceState();

		SetCurrentButton( c_selectButtonIndex, false );
	}

	void SelectNextRace()
	{
		m_currentRaceIndex = ( m_currentRaceIndex + c_numRaces + 1 ) % c_numRaces;

		UpdateScreenForSelectRaceState();

		SetCurrentButton( c_selectButtonIndex, false );
	}

	// train the currently selected skill
	void TrainSelectedSkill()
	{
		// get access to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get access to the bank player data
		var bank = playerData.m_bank;

		if ( bank.m_currentBalance < 300 )
		{
			UpdateTrainingCostText( 5 );

			PlayErrorSound();
		}
		else
		{
			// get access to the current personnel file we are looking at
			var personnelFile = playerData.m_personnel.m_personnelList[ m_currentFileIndex ];

			// get access to the race data for this personnel file
			var race = DataController.m_instance.m_gameData.m_crewRaceList[ m_currentRaceIndex ];

			// calculate the current skill and maximum skill points for the selected skill
			var currentSkill = personnelFile.GetSkill( m_currentSkillIndex );
			var maximumSkill = race.GetMaximumSkill( m_currentSkillIndex );

			// check if the maximum skill is zero
			if ( maximumSkill == 0 )
			{
				UpdateTrainingCostText( 4 );

				PlayErrorSound();
			}
			else if ( currentSkill < maximumSkill ) // check if we are still below the maximum skill points
			{
				// increase the skill by the learn amount
				personnelFile.SetSkill( m_currentSkillIndex, Math.Min( maximumSkill, currentSkill + race.m_learningRate ) );

				// take off 300 credits from the bank balance
				bank.m_currentBalance -= 300;

				// update the bank balance text
				UpdateBankBalanceText();

				// update the skill values text
				UpdateSkillValues();

				// play a ui sound
				PlayUpdateSound();
			}
			else // the selected skill is already maxxed out
			{
				UpdateTrainingCostText( 3 );

				PlayErrorSound();
			}
		}
	}

	void ClosePanel()
	{
		// if the bank balance has changed then record it in the bank transaction log
		var deltaBalance = m_startingBankBalance - DataController.m_instance.m_playerData.m_bank.m_currentBalance;

		if ( deltaBalance > 0 )
		{
			var transaction = new PD_Bank.Transaction( DataController.m_instance.m_playerData.m_general.m_currentStardateYMD, "Personnel", deltaBalance.ToString() + "-" );

			DataController.m_instance.m_playerData.m_bank.m_transactionList.Add( transaction );
		}

		Exit();
	}
}
