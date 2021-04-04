
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipConfiguration : DoorPanelController
{
	enum State
	{
		MenuBar, BuyPart, SellPart, SelectClass, GiveName
	}

	const int c_buyButtonIndex = 0;
	const int c_sellButtonIndex = 1;
	const int c_repairButtonIndex = 2;
	const int c_nameButtonIndex = 3;
	const int c_exitButtonIndex = 4;
	const int c_upButtonIndex = 5;
	const int c_downButtonIndex = 6;
	const int c_actionButtonIndex = 7;
	const int c_dismissButtonIndex = 8;

	[SerializeField] TextMeshProUGUI m_componentNamesTMP;
	[SerializeField] TextMeshProUGUI m_componentValuesTMP;
	[SerializeField] TextMeshProUGUI m_configurationValuesTMP;
	[SerializeField] TextMeshProUGUI m_statusValuesTMP;
	[SerializeField] TextMeshProUGUI m_currentBalanceTMP;
	[SerializeField] TextMeshProUGUI m_nameInputTMP;
	[SerializeField] TextMeshProUGUI m_errorMessageTMP;

	[SerializeField] RectTransform m_componentSelection;
	[SerializeField] RectTransform m_selectedComponent;

	[SerializeField] Image m_shieldImage;

	[SerializeField] GameObject m_overlay;
	[SerializeField] GameObject m_nameModal;
	[SerializeField] GameObject m_errorModal;

	[SerializeField] GameObject m_missileLauncher;
	[SerializeField] GameObject m_laserCannon;
	[SerializeField] GameObject[] m_cargoPods;

	int m_currentPartIndex;
	int m_currentClassIndex;
	int m_startingBankBalance;

	State m_currentState;

	string m_name;

	Vector2 m_firstSelectionAnchoredPosition;

	const int c_numParts = 6;
	const int c_numClasses = 5;
	const int c_numComponentValuesLines = 13;

	protected override void Initialize()
	{
		m_firstSelectionAnchoredPosition = m_componentSelection.anchoredPosition;
	}

	protected override void Restart()
	{
		// remember the starting bank balance
		m_startingBankBalance = DataController.m_instance.m_playerData.m_bank.m_currentBalance;

		// switch to the menu bar state
		SwitchToMenuBarState( false );
	}

	public override void OnFire()
	{
		base.OnFire();

		switch ( m_currentButtonIndex )
		{
			case c_buyButtonIndex:
				SwitchToBuyPartState( true );
				break;

			case c_sellButtonIndex:
				SwitchToSellPartState();
				break;

			case c_nameButtonIndex:
				SwitchToGiveNameState();
				break;

			case c_exitButtonIndex:
				ClosePanel();
				break;

			case c_upButtonIndex:
				SelectPreviousThing();
				break;

			case c_downButtonIndex:
				SelectNextThing();
				break;

			case c_actionButtonIndex:
				PerformAction();
				break;

			case c_dismissButtonIndex:
				DismissErrorModal();
				break;
		}
	}

	public override void OnCancel()
	{
		base.OnCancel();

		switch ( m_currentButtonIndex )
		{
			case c_buyButtonIndex:
			case c_sellButtonIndex:
			case c_repairButtonIndex:
			case c_nameButtonIndex:
			case c_exitButtonIndex:
				ClosePanel();
				break;

			case c_actionButtonIndex:
				SwitchToPreviousState();
				break;

			case c_dismissButtonIndex:
				DismissErrorModal();
				break;
		}
	}

	public override void OnTextInput( char ch )
	{
		base.OnTextInput( ch );

		if ( ch == (char) KeyCode.Return )
		{
			// strip off whitespace at the ends
			m_name = m_name.Trim();

			if ( m_name.Length == 0 )
			{
				// buzz
				Sounds.m_instance.PlayError();
			}
			else
			{
				// stop handling keyboard input
				SetInputFocus( false );

				// update the ship name in the player data
				DataController.m_instance.m_playerData.m_playerShip.m_name = m_name;
				// play a ui sound
				Sounds.m_instance.PlayUpdate();

				// switch to the menu bar state
				SwitchToMenuBarState( false );
			}
		}
		else if ( ch == (char) KeyCode.Backspace )
		{
			if ( m_name.Length > 0 )
			{
				m_name = m_name.Remove( m_name.Length - 1 );
			}
			else
			{
				Sounds.m_instance.PlayError();
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
				Sounds.m_instance.PlayError();
			}
		}
		else
		{
			Sounds.m_instance.PlayError();
		}

		m_nameInputTMP.text = m_name;
	}

	// call this to switch to the menu bar state
	void SwitchToMenuBarState( bool makeNoise = true )
	{
		// update the current state
		m_currentState = State.MenuBar;

		// hide stuff not visible in this state
		m_componentSelection.gameObject.SetActive( false );
		m_selectedComponent.gameObject.SetActive( false );
		m_componentValuesTMP.gameObject.SetActive( false );
		m_overlay.SetActive( false );
		m_nameModal.SetActive( false );
		m_errorModal.SetActive( false );

		// enable the menu bar buttons
		EnableButton( c_buyButtonIndex, true );
		EnableButton( c_sellButtonIndex, true );
		EnableButton( c_repairButtonIndex, true );
		EnableButton( c_nameButtonIndex, true );
		EnableButton( c_exitButtonIndex, true );

		// select the buy button
		SetCurrentButton( c_buyButtonIndex, false );

		// update the information panels
		UpdateInformationPanels();

		// beep
		if ( makeNoise )
		{
			Sounds.m_instance.PlayDeactivate();
		}
	}

	// call this to switch to the buy part state
	void SwitchToBuyPartState( bool resetCurrentPartIndex = true )
	{
		// update the current state
		m_currentState = State.BuyPart;

		// select the first part by default
		if ( resetCurrentPartIndex )
		{
			m_currentPartIndex = 0;
		}

		// hide stuff that should not be visible
		m_selectedComponent.gameObject.SetActive( false );

		// show stuff that should be visible
		m_componentValuesTMP.gameObject.SetActive( true );
		m_componentSelection.gameObject.SetActive( true );

		// disable menu buttons
		EnableButton( c_buyButtonIndex, false );
		EnableButton( c_sellButtonIndex, false );
		EnableButton( c_repairButtonIndex, false );
		EnableButton( c_nameButtonIndex, false );
		EnableButton( c_exitButtonIndex, false );

		// set the current button
		SetCurrentButton( c_actionButtonIndex, false );

		// update the screen
		UpdateScreenForBuyOrSellPartState();

		// beep
		if ( resetCurrentPartIndex )
		{
			Sounds.m_instance.PlayActivate();
		}
	}

	// call this to switch to the sell part state
	void SwitchToSellPartState( bool resetCurrentPartIndex = true )
	{
		// update the current state
		m_currentState = State.SellPart;

		// select the first part by default
		if ( resetCurrentPartIndex )
		{
			m_currentPartIndex = 0;
		}

		// show stuff that should be visible
		m_componentValuesTMP.gameObject.SetActive( true );
		m_componentSelection.gameObject.SetActive( true );

		// disable menu buttons
		EnableButton( c_buyButtonIndex, false );
		EnableButton( c_sellButtonIndex, false );
		EnableButton( c_repairButtonIndex, false );
		EnableButton( c_nameButtonIndex, false );
		EnableButton( c_exitButtonIndex, false );

		// set the current button
		SetCurrentButton( c_actionButtonIndex, false );

		// update the screen
		UpdateScreenForBuyOrSellPartState();

		// beep
		if ( resetCurrentPartIndex )
		{
			Sounds.m_instance.PlayActivate();
		}
	}

	// call this to switch to the give name state
	void SwitchToGiveNameState()
	{
		// update the current state
		m_currentState = State.GiveName;

		// set the current text input to the current name of the ship
		m_nameInputTMP.text = m_name = DataController.m_instance.m_playerData.m_playerShip.m_name;

		// show the name modal
		m_overlay.SetActive( true );
		m_nameModal.SetActive( true );

		// disable menu buttons
		EnableButton( c_buyButtonIndex, false );
		EnableButton( c_sellButtonIndex, false );
		EnableButton( c_repairButtonIndex, false );
		EnableButton( c_nameButtonIndex, false );
		EnableButton( c_exitButtonIndex, false );

		// beep
		Sounds.m_instance.PlayActivate();

		// read input from any key on the keyboard
		SetInputFocus( true );
	}

	// call this to switch to the select class state
	void SwitchToSelectClassState( bool resetCurrentClassIndex = true )
	{
		// update the current state
		m_currentState = State.SelectClass;

		// select the first part by default
		if ( resetCurrentClassIndex )
		{
			m_currentClassIndex = 0;
		}

		// show stuff that should be visible
		m_selectedComponent.gameObject.SetActive( true );

		// update the screen
		UpdateScreenForBuyOrSellPartState();
	}

	// call this to switch to the error message state
	void SwitchToErrorMessageState( string errorMessage )
	{
		// set the error message
		m_errorMessageTMP.text = errorMessage;

		// show the error modal
		m_overlay.SetActive( true );
		m_errorModal.SetActive( true );

		// disable selection buttons
		EnableButton( c_upButtonIndex, false );
		EnableButton( c_actionButtonIndex, false );
		EnableButton( c_downButtonIndex, false );

		// set the current button
		SetCurrentButton( c_dismissButtonIndex, false );

		// play a ui sound
		Sounds.m_instance.PlayError();
	}

	void SwitchToPreviousState()
	{
		switch ( m_currentState )
		{
			case State.SelectClass:
				SwitchToBuyPartState( false );
				break;

			case State.BuyPart:
			case State.SellPart:
				SwitchToMenuBarState();
				break;
		}
	}

	void DismissErrorModal()
	{
		// hide the error modal
		m_overlay.SetActive( false );
		m_errorModal.SetActive( false );

		// restart the current state
		switch ( m_currentState )
		{
			case State.BuyPart:
				SwitchToBuyPartState( false );
				break;

			case State.SellPart:
				SwitchToSellPartState( false );
				break;

			case State.SelectClass:
				SwitchToSelectClassState( false );
				break;
		}

		Sounds.m_instance.PlayDeactivate();
	}

	// update screen for the buy part state
	void UpdateScreenForBuyOrSellPartState()
	{
		// put in the prices for the parts
		UpdatePartPrices();

		if ( m_currentState == State.SelectClass )
		{
			// show the up arrow only if the first class is not selected
			ShowButton( c_upButtonIndex, m_currentClassIndex > 0 );

			// show the down arrow only if the last class is not selected
			ShowButton( c_downButtonIndex, m_currentClassIndex < ( c_numClasses - 1 ) );

			// put the selected part box in the right place
			float offset = ( m_currentPartIndex + 1 ) * m_componentNamesTMP.renderedHeight / c_numComponentValuesLines;

			m_selectedComponent.anchoredPosition = m_firstSelectionAnchoredPosition + new Vector2( 0.0f, -offset );

			// put the class selection box in the right place
			offset = ( m_currentClassIndex + 8 ) * m_componentNamesTMP.renderedHeight / c_numComponentValuesLines;

			m_componentSelection.anchoredPosition = m_firstSelectionAnchoredPosition + new Vector2( 0.0f, -offset );
		}
		else
		{
			// show the up arrow only if the first part is not selected
			ShowButton( c_upButtonIndex, m_currentPartIndex > 0 );

			// show the down arrow only if the last part is not selected
			ShowButton( c_downButtonIndex, m_currentPartIndex < ( c_numParts - 1 ) );

			// put the part selection box in the right place
			var offset = ( ( m_currentPartIndex == 0 ) ? 0 : ( m_currentPartIndex + 1 ) ) * m_componentNamesTMP.renderedHeight / c_numComponentValuesLines;

			m_componentSelection.anchoredPosition = m_firstSelectionAnchoredPosition + new Vector2( 0.0f, -offset );
		}
	}

	void UpdateInformationPanels()
	{
		// get the game data
		var gameData = DataController.m_instance.m_gameData;

		// get the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the ship player data
		var ship = playerData.m_playerShip;

		// update configuration values
		m_configurationValuesTMP.text = ship.m_numCargoPods.ToString() + Environment.NewLine;
		m_configurationValuesTMP.text += Environment.NewLine;
		m_configurationValuesTMP.text += ship.GetEngines().m_name + Environment.NewLine;
		m_configurationValuesTMP.text += ship.GetSheilding().m_name + Environment.NewLine;
		m_configurationValuesTMP.text += ship.GetArmor().m_name + Environment.NewLine;
		m_configurationValuesTMP.text += ship.GetMissileLauncher().m_name + Environment.NewLine;
		m_configurationValuesTMP.text += ship.GetLaserCannon().m_name + Environment.NewLine;

		// show only as many cargo pods as we have purchased
		for ( var cargoPodId = 0; cargoPodId < m_cargoPods.Length; cargoPodId++ )
		{
			m_cargoPods[ cargoPodId ].SetActive( cargoPodId < ship.m_numCargoPods );
		}

		// hide or show the shield image depending on if we have them
		m_shieldImage.gameObject.SetActive( ship.m_shieldingClass > 0 );

		// hide or show the missile launchers depending on if we have them
		m_missileLauncher.SetActive( ship.m_missileLauncherClass > 0 );

		// hide or show the missile launchers depending on if we have them
		m_laserCannon.SetActive( ship.m_laserCannonClass > 0 );

		// update status values
		m_statusValuesTMP.text = ship.m_mass + " Tons" + Environment.NewLine;
		m_statusValuesTMP.text += ship.m_acceleration + " G" + Environment.NewLine;

		// report the amount of endurium on the ship
		var elementReference = playerData.m_playerShip.m_elementStorage.Find( gameData.m_misc.m_enduriumElementId );

		if ( elementReference == null )
		{
			m_statusValuesTMP.text += "0.0M<sup>3</sup>";
		}
		else
		{
			m_statusValuesTMP.text += ( elementReference.m_volume / 10 ) + "." + ( elementReference.m_volume % 10 ) + "M<sup>3</sup>";
		}

		// update bank balance amount
		m_currentBalanceTMP.text = "Your account balance is: " + playerData.m_bank.m_currentBalance + " M.U.";
	}

	void UpdatePartPrices( bool includeCargoPods = true )
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// check if we are selling
		if ( m_currentState == State.SellPart )
		{
			// hide the sell prices for cargo pods if we don't have any
			if ( playerData.m_playerShip.m_numCargoPods == 0 )
			{
				includeCargoPods = false;
			}
		}

		if ( includeCargoPods )
		{
			// show the cargo pod price
			m_componentValuesTMP.text = gameData.m_misc.m_cargoPodBuyPrice.ToString() + Environment.NewLine;
		}
		else
		{
			m_componentValuesTMP.text = Environment.NewLine;
		}

		// skip to the class prices
		for ( int i = 0; i < 7; i++ )
		{
			m_componentValuesTMP.text += Environment.NewLine;
		}

		// get what is currently installed on the ship
		var currentClass = 0;

		switch ( m_currentPartIndex )
		{
			case 1: currentClass = playerData.m_playerShip.m_enginesClass; break;
			case 2: currentClass = playerData.m_playerShip.m_shieldingClass; break;
			case 3: currentClass = playerData.m_playerShip.m_armorClass; break;
			case 4: currentClass = playerData.m_playerShip.m_missileLauncherClass; break;
			case 5: currentClass = playerData.m_playerShip.m_laserCannonClass; break;
		}

		// update part class prices (if we have anything but cargo pods selected)
		if ( m_currentPartIndex > 0 )
		{
			GD_ShipPart[] shipPartList = null;

			switch ( m_currentPartIndex )
			{
				case 1: shipPartList = gameData.m_enginesList; break;
				case 2: shipPartList = gameData.m_shieldingList; break;
				case 3: shipPartList = gameData.m_armorList; break;
				case 4: shipPartList = gameData.m_missileLauncherList; break;
				case 5: shipPartList = gameData.m_laserCannonList; break;
			}

			for ( var classIndex = 1; classIndex < shipPartList.Length; classIndex++ )
			{
				// check if we are selling
				if ( m_currentState == State.SellPart )
				{
					// yep - show the sell price only for the class we have installed
					if ( classIndex == currentClass )
					{
						m_componentValuesTMP.text += shipPartList[ classIndex ].m_sellPrice.ToString() + Environment.NewLine;
					}
					else
					{
						m_componentValuesTMP.text += Environment.NewLine;
					}
				}
				else
				{
					// no - we are buying - show all prices
					m_componentValuesTMP.text += shipPartList[ classIndex ].m_buyPrice.ToString() + Environment.NewLine;
				}
			}
		}
	}

	void SelectPreviousThing()
	{
		switch ( m_currentState )
		{
			case State.BuyPart:
			case State.SellPart:
				SelectPreviousPart();
				break;

			case State.SelectClass:
				SelectPreviousClass();
				break;
		}
	}

	void SelectNextThing()
	{
		switch ( m_currentState )
		{
			case State.BuyPart:
			case State.SellPart:
				SelectNextPart();
				break;

			case State.SelectClass:
				SelectNextClass();
				break;
		}
	}

	void SelectPreviousPart()
	{
		if ( m_currentPartIndex > 0 )
		{
			m_currentPartIndex--;

			UpdateScreenForBuyOrSellPartState();
		}

		SetCurrentButton( c_actionButtonIndex, false );
	}

	void SelectNextPart()
	{
		if ( m_currentPartIndex < ( c_numParts - 1 ) )
		{
			m_currentPartIndex++;

			UpdateScreenForBuyOrSellPartState();
		}

		SetCurrentButton( c_actionButtonIndex, false );
	}

	void SelectPreviousClass()
	{
		if ( m_currentClassIndex > 0 )
		{
			m_currentClassIndex--;

			UpdateScreenForBuyOrSellPartState();
		}

		SetCurrentButton( c_actionButtonIndex, false );
	}

	void SelectNextClass()
	{
		if ( m_currentClassIndex < ( c_numClasses - 1 ) )
		{
			m_currentClassIndex++;

			UpdateScreenForBuyOrSellPartState();
		}

		SetCurrentButton( c_actionButtonIndex, false );
	}

	void PerformAction()
	{
		switch ( m_currentState )
		{
			case State.BuyPart:
				BuySelectedPart();
				break;

			case State.SelectClass:
				BuySelectedClass();
				break;

			case State.SellPart:
				SellSelectedPart();
				break;
		}
	}

	// buy the currently selected part
	void BuySelectedPart()
	{
		// check if we are buying a cargo pod and handle that differently
		if ( m_currentPartIndex == 0 )
		{
			BuyCargoPod();
		}
		else // we are buying a part
		{
			// get to the player data
			var playerData = DataController.m_instance.m_playerData;

			// get what is currently installed on the ship
			var currentClass = 0;

			switch ( m_currentPartIndex )
			{
				case 1: currentClass = playerData.m_playerShip.m_enginesClass; break;
				case 2: currentClass = playerData.m_playerShip.m_shieldingClass; break;
				case 3: currentClass = playerData.m_playerShip.m_armorClass; break;
				case 4: currentClass = playerData.m_playerShip.m_missileLauncherClass; break;
				case 5: currentClass = playerData.m_playerShip.m_laserCannonClass; break;
			}

			// check if the ship has this part installed already
			if ( currentClass != 0 )
			{
				// yes - tell the player to sell it first
				SwitchToErrorMessageState( "De-equip first" );
			}
			else
			{
				// no - let the player select the class to buy
				SwitchToSelectClassState();
			}
		}
	}

	// buy the currently selected class
	void BuySelectedClass()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get the ship part list
		GD_ShipPart[] shipPartList = null;

		switch ( m_currentPartIndex )
		{
			case 1: shipPartList = gameData.m_enginesList; break;
			case 2: shipPartList = gameData.m_shieldingList; break;
			case 3: shipPartList = gameData.m_armorList; break;
			case 4: shipPartList = gameData.m_missileLauncherList; break;
			case 5: shipPartList = gameData.m_laserCannonList; break;
		}

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the class to buy
		var classIndex = m_currentClassIndex + 1;

		// check if the player can afford it
		if ( playerData.m_bank.m_currentBalance < shipPartList[ classIndex ].m_buyPrice )
		{
			// nope!
			SwitchToErrorMessageState( "Insufficient funds" );
		}
		else
		{
			// yes so make the transaction and install the part
			playerData.m_bank.m_currentBalance -= shipPartList[ classIndex ].m_buyPrice;

			// upgrade the ship part
			switch ( m_currentPartIndex )
			{
				case 1: playerData.m_playerShip.m_enginesClass = classIndex; break;
				case 2: playerData.m_playerShip.m_shieldingClass = classIndex; break;
				case 3: playerData.m_playerShip.m_armorClass = classIndex; break;
				case 4: playerData.m_playerShip.m_missileLauncherClass = classIndex; break;
				case 5: playerData.m_playerShip.m_laserCannonClass = classIndex; break;
			}

			// recalculate the ship mass and acceleration
			playerData.m_playerShip.RecalculateMass();
			playerData.m_playerShip.RecalculateAcceleration();

			// if we just bought armor then update the armor points on the ship
			if ( m_currentPartIndex == 3 )
			{
				var armor = playerData.m_playerShip.GetArmor();

				playerData.m_playerShip.m_armorPoints = armor.m_points;
			}

			// update the information panels
			UpdateInformationPanels();

			// switch back to the buy part state
			SwitchToBuyPartState( false );

			// play a ui sound
			Sounds.m_instance.PlayUpdate();
		}
	}

	// sell the currently selected part
	void SellSelectedPart()
	{
		// check if we are selling a cargo pod and handle that differently
		if ( m_currentPartIndex == 0 )
		{
			SellCargoPod();
		}
		else // we are selling a part
		{
			// get to the player data
			var playerData = DataController.m_instance.m_playerData;

			// get what is currently installed on the ship
			var currentClass = 0;

			switch ( m_currentPartIndex )
			{
				case 1: currentClass = playerData.m_playerShip.m_enginesClass; break;
				case 2: currentClass = playerData.m_playerShip.m_shieldingClass; break;
				case 3: currentClass = playerData.m_playerShip.m_armorClass; break;
				case 4: currentClass = playerData.m_playerShip.m_missileLauncherClass; break;
				case 5: currentClass = playerData.m_playerShip.m_laserCannonClass; break;
			}

			// check if the ship has this part installed already
			if ( currentClass != 0 )
			{
				// get to the game data
				var gameData = DataController.m_instance.m_gameData;

				// get the part list
				GD_ShipPart[] shipPartList = null;

				switch ( m_currentPartIndex )
				{
					case 1: shipPartList = gameData.m_enginesList; break;
					case 2: shipPartList = gameData.m_shieldingList; break;
					case 3: shipPartList = gameData.m_armorList; break;
					case 4: shipPartList = gameData.m_missileLauncherList; break;
					case 5: shipPartList = gameData.m_laserCannonList; break;
				}

				// add the funds to the players bank balance
				playerData.m_bank.m_currentBalance += shipPartList[ currentClass ].m_sellPrice;

				// remove the part from the ship
				switch ( m_currentPartIndex )
				{
					case 1: playerData.m_playerShip.m_enginesClass = 0; break;
					case 2: playerData.m_playerShip.m_shieldingClass = 0; break;
					case 3: playerData.m_playerShip.m_armorClass = 0; break;
					case 4: playerData.m_playerShip.m_missileLauncherClass = 0; break;
					case 5: playerData.m_playerShip.m_laserCannonClass = 0; break;
				}

				// recalculate the ship mass and acceleration
				playerData.m_playerShip.RecalculateMass();
				playerData.m_playerShip.RecalculateAcceleration();

				// update the information panels
				UpdateInformationPanels();

				// update the screen
				UpdateScreenForBuyOrSellPartState();

				// play a ui sound
				Sounds.m_instance.PlayUpdate();
			}
			else
			{
				// buzz
				Sounds.m_instance.PlayError();
			}
		}
	}

	// buy a cargo pod
	void BuyCargoPod()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// check if we have room for another cargo pod
		if ( playerData.m_playerShip.m_numCargoPods == 16 )
		{
			// nope - show an error message
			SwitchToErrorMessageState( "No cargo pod bays available" );
		}
		else
		{
			// can we afford it?
			if ( playerData.m_bank.m_currentBalance < gameData.m_misc.m_cargoPodBuyPrice )
			{
				// nope - show an error message
				SwitchToErrorMessageState( "Insufficient funds" );
			}
			else
			{
				// deduct the price of the cargo pod from the player's bank balance
				playerData.m_bank.m_currentBalance -= gameData.m_misc.m_cargoPodBuyPrice;

				// add one cargo pod to the ship
				playerData.m_playerShip.AddCargoPod();

				// update the screen
				UpdateInformationPanels();

				// play a ui sound
				Sounds.m_instance.PlayUpdate();
			}
		}
	}

	// sell a cargo pod
	void SellCargoPod()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// check if we have another cargo pod to sell
		if ( playerData.m_playerShip.m_numCargoPods > 0 )
		{
			// calculate how much cargo we can hold with one less cargo pod
			var m_volume = gameData.m_misc.m_baseShipVolume + gameData.m_misc.m_cargoPodVolume * ( playerData.m_playerShip.m_numCargoPods - 1 );

			// does the player need to sell stuff first?
			if ( m_volume < playerData.m_playerShip.m_volumeUsed )
			{
				// yep - show an error message
				SwitchToErrorMessageState( "You need to sell some cargo first" );
			}
			else
			{
				// pay for the cargo pod into the player's bank balance
				playerData.m_bank.m_currentBalance += gameData.m_misc.m_cargoPodSellPrice;

				// remove one cargo pod from the ship
				playerData.m_playerShip.RemoveCargoPod();

				// update the screen
				UpdateInformationPanels();

				// play a ui sound
				Sounds.m_instance.PlayUpdate();
			}
		}
		else
		{
			// buzz
			Sounds.m_instance.PlayError();
		}
	}

	void ClosePanel()
	{
		// if the bank balance has changed then record it in the bank transaction log
		var deltaBalance = m_startingBankBalance - DataController.m_instance.m_playerData.m_bank.m_currentBalance;

		if ( deltaBalance != 0 )
		{
			var sign = ( deltaBalance > 0 ) ? "-" : "+";

			var transaction = new PD_Bank.Transaction( DataController.m_instance.m_playerData.m_general.m_currentStardateYMD, "Ship Configuration", deltaBalance.ToString() + sign );

			DataController.m_instance.m_playerData.m_bank.m_transactionList.Add( transaction );
		}

		Exit();
	}
}
