
using System;
using System.Collections.Generic;

using TMPro;
using UnityEngine;

public class TradeDepot : DoorPanelController
{
	enum State
	{
		MenuBar, BuyItem, BuyAmount, SellItem, SellAmount, AnalyzeItem, AnalyzeConfirm, AnalyzeShow, Message
	}

	class Item
	{
		public int m_row;
		public int m_type;
		public int m_id;
		public int m_volume;

		public Item( int row, int type, int id, int volume )
		{
			m_row = row;
			m_type = type;
			m_id = id;
			m_volume = volume;
		}
	}

	public const int c_welcomePageIndex = 0;
	public const int c_tradePageIndex = 1;

	public const int c_buyButtonIndex = 0;
	public const int c_sellButtonIndex = 1;
	public const int c_analyzeButtonIndex = 2;
	public const int c_exitButtonIndex = 3;

	public const int c_analyzeYesButtonIndex = 4;
	public const int c_analyzeNoButtonIndex = 5;

	public const int c_upButtonIndex = 6;
	public const int c_downButtonIndex = 7;
	public const int c_actionButtonIndex = 8;

	public const int c_dismissButtonIndex = 9;

	[SerializeField] TextMeshProUGUI m_itemListTMP;
	[SerializeField] TextMeshProUGUI m_volumeListTMP;
	[SerializeField] TextMeshProUGUI m_unitValueListTMP;
	[SerializeField] TextMeshProUGUI m_currentBalanceTMP;
	[SerializeField] TextMeshProUGUI m_amountLabelTMP;
	[SerializeField] TextMeshProUGUI m_messageLabelTMP;
	[SerializeField] TextMeshProUGUI m_amountInputTMP;

	[SerializeField] RectTransform m_itemSelection;

	[SerializeField] GameObject m_itemListMask;
	[SerializeField] GameObject m_overlay;
	[SerializeField] GameObject m_amountModal;
	[SerializeField] GameObject m_messageModal;
	[SerializeField] GameObject m_analyzeModal;

	[SerializeField] float m_renderedHeightFudge;

	State m_currentState;
	State m_stateBeforeMessageState;
	int m_startingBankBalance;
	int m_currentItemIndex;
	List<Item> m_itemList;
	int m_rowCount;
	int m_currentRowOffset;
	string m_amount;

	Vector2 m_firstSelectionAnchoredPosition;

	Vector2 m_initialItemListAnchoredPosition;
	Vector2 m_initialVolumeListAnchoredPosition;
	Vector2 m_initialUnitValueAnchoredPosition;

	protected override void Initialize()
	{
		m_firstSelectionAnchoredPosition = m_itemSelection.anchoredPosition;

		m_initialItemListAnchoredPosition = m_itemListTMP.rectTransform.anchoredPosition;
		m_initialVolumeListAnchoredPosition = m_volumeListTMP.rectTransform.anchoredPosition;
		m_initialUnitValueAnchoredPosition = m_unitValueListTMP.rectTransform.anchoredPosition;
	}

	protected override void Restart()
	{
		// remember the starting bank balance
		m_startingBankBalance = DataController.m_instance.m_playerData.m_bank.m_currentBalance;

		// update the bank balance text
		UpdateBankBalanceText();

		// so the proper button is selected
		m_currentState = State.MenuBar;

		// switch to the menu bar state
		SwitchToMenuBarState( false );
	}

	public override void OnFire()
	{
		base.OnFire();

		switch ( m_currentButtonIndex )
		{
			case c_buyButtonIndex:
				SwitchToBuyItemState();
				break;

			case c_sellButtonIndex:
				SwitchToSellItemState();
				break;

			case c_analyzeButtonIndex:
				SwitchToAnalyzeItemState();
				break;

			case c_exitButtonIndex:
				ClosePanel();
				break;

			case c_upButtonIndex:
				SelectPreviousItem();
				break;

			case c_downButtonIndex:
				SelectNextItem();
				break;

			case c_actionButtonIndex:
				DoAction();
				break;

			case c_dismissButtonIndex:
				SwitchToStateBeforeMessageState();
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
			case c_analyzeButtonIndex:
			case c_exitButtonIndex:
				ClosePanel();
				break;

			case c_actionButtonIndex:
				SwitchToMenuBarState( true );
				break;

			case c_dismissButtonIndex:
				SwitchToStateBeforeMessageState();
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
			m_amount = m_amount.Trim();

			// get access to the player data
			var playerData = DataController.m_instance.m_playerData;

			// get access to the game data
			var gameData = DataController.m_instance.m_gameData;

			// get the amount to transfer
			var desiredAmount = 0;

			if ( m_amountInputTMP.text.IndexOf( '.' ) == -1 )
			{
				// player did not enter a decimal point
				if ( m_amountInputTMP.text.Length > 0 )
				{
					desiredAmount = Convert.ToInt32( m_amountInputTMP.text ) * 10;
				}
			}
			else
			{
				// player did enter a decimal point
				string[] amountParts = m_amountInputTMP.text.Split( '.' );

				if ( amountParts[ 0 ].Length > 0 )
				{
					desiredAmount = Convert.ToInt32( amountParts[ 0 ] ) * 10;
				}

				if ( amountParts[ 1 ].Length > 0 )
				{
					desiredAmount += Convert.ToInt32( amountParts[ 1 ] );
				}
			}

			// if the desired amount was zero then that's the same as canceling
			if ( desiredAmount <= 0 )
			{
				if ( m_currentState == State.BuyAmount )
				{
					// switch back to the buy item state
					SwitchToBuyItemState( false );
				}
				else
				{
					// switch back to the sell item state
					SwitchToSellItemState( false );
				}

				// play a ui sound
				Sounds.m_instance.PlayDeactivate();
			}
			else
			{
				// get the currently selected element id
				var elementId = m_itemList[ m_currentItemIndex ].m_id;

				// check if we are buying or selling
				if ( m_currentState == State.BuyAmount )
				{
					// check if the player has enough money to buy that amount
					var maximumAmount = GetMaximumBuyAmountDueToCurrentBalance( elementId );

					if ( desiredAmount > maximumAmount )
					{
						SwitchToMessageState( "Insufficent funds" );
					}
					else
					{
						// check if the ship has room in the cargo hold
						if ( desiredAmount > playerData.m_playerShip.GetRemainingVolme() )
						{
							SwitchToMessageState( "Insufficient cargo space" );
						}
						else
						{
							// get the starport price of this element
							var starportPrice = gameData.m_elementList[ elementId ].m_starportPrice;

							// deduct the price of the artifact from the player's bank balance
							playerData.m_bank.m_currentBalance -= starportPrice * desiredAmount / 10;

							// transfer the element to the ship
							playerData.m_playerShip.AddElement( elementId, desiredAmount );

							// switch back to the buy item state
							SwitchToBuyItemState( false );

							// play a ui sound
							Sounds.m_instance.PlayUpdate();
						}
					}
				}
				else
				{
					// get the sell price of this element
					var sellPrice = gameData.m_elementList[ elementId ].m_actualValue;

					// add the sell price of the artifact to the player's bank balance
					playerData.m_bank.m_currentBalance += sellPrice * desiredAmount / 10;

					// transfer the element to starport
					playerData.m_playerShip.RemoveElement( elementId, desiredAmount );

					// switch back to the sell item state
					SwitchToSellItemState( false );

					// play a ui sound
					Sounds.m_instance.PlayUpdate();
				}
			}
		}
		else if ( ch == (char) KeyCode.Backspace )
		{
			if ( m_amount.Length > 0 )
			{
				m_amount = m_amount.Remove( m_amount.Length - 1 );
			}
			else
			{
				Sounds.m_instance.PlayError();
			}
		}
		else if ( ( ( ch >= '0' ) && ( ch <= '9' ) ) || ( ch == '.' ) )
		{
			if ( m_amount.Length < 32 )
			{
				m_amount += ch;
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

		m_amountInputTMP.text = m_amount;
	}

	// call this to switch to the menu bar state
	void SwitchToWelcomeScreen( bool makeNoise )
	{
		// show the welcome page
		SetCurrentPage( c_welcomePageIndex, makeNoise, true );

		// hide stuff not visible when switching screens
		m_overlay.SetActive( false );
		m_amountModal.SetActive( false );
		m_messageModal.SetActive( false );
		m_analyzeModal.SetActive( false );

		// enable the menu bar buttons
		EnableButton( c_buyButtonIndex, true );
		EnableButton( c_sellButtonIndex, true );
		EnableButton( c_analyzeButtonIndex, true );
		EnableButton( c_exitButtonIndex, true );
	}

	void SwitchToTradeScreen( bool makeNoise )
	{
		// show the trade page
		SetCurrentPage( c_tradePageIndex, makeNoise );

		// hide stuff not visible when switching screens
		m_overlay.SetActive( false );
		m_amountModal.SetActive( false );
		m_messageModal.SetActive( false );
		m_analyzeModal.SetActive( false );

		// disable the menu bar buttons
		EnableButton( c_buyButtonIndex, false );
		EnableButton( c_sellButtonIndex, false );
		EnableButton( c_analyzeButtonIndex, false );
		EnableButton( c_exitButtonIndex, false );

		// set the current button
		SetCurrentButton( c_actionButtonIndex, false );

		// update the trade panel
		UpdateTradePanel();

		// update the selection
		UpdateItemSelection();
	}

	void SwitchToMenuBarState( bool makeNoise )
	{
		// switch to the welcome screen
		SwitchToWelcomeScreen( makeNoise );

		// remember the state we were in
		var oldState = m_currentState;

		// temporarily change to the sell item state
		m_currentState = State.SellItem;

		// update the trade panel
		UpdateTradePanel();

		// see if we have anything to sell
		bool hasThingsToSell = m_itemList.Count > 0;

		// enable the sell button if we do
		EnableButton( c_sellButtonIndex, hasThingsToSell );

		// select the button based on where we were
		if ( oldState == State.AnalyzeItem )
		{
			SetCurrentButton( c_analyzeButtonIndex, false );
		}
		else if ( ( oldState == State.SellItem ) && hasThingsToSell )
		{
			SetCurrentButton( c_sellButtonIndex, false );
		}
		else
		{
			SetCurrentButton( c_buyButtonIndex, false );
		}

		// change the current state
		m_currentState = State.MenuBar;
	}

	void SwitchToBuyItemState( bool resetCurrentItemIndex = true )
	{
		// change the current state
		m_currentState = State.BuyItem;

		// select the first part by default
		if ( resetCurrentItemIndex )
		{
			m_currentItemIndex = 0;
			m_currentRowOffset = 0;
		}

		// switch screens
		SwitchToTradeScreen( resetCurrentItemIndex );
	}

	void SwitchToBuyAmountState()
	{
		// change the current state
		m_currentState = State.BuyAmount;

		// erase the current text input
		m_amountInputTMP.text = m_amount = "";

		// get access to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the currently selected element id
		var elementId = m_itemList[ m_currentItemIndex ].m_id;

		// figure out the maximum amount the player can buy
		var maximumAmount = GetMaximumBuyAmountDueToCurrentBalance( elementId );

		if ( maximumAmount == 0 )
		{
			// the player is broke and cannot buy anything - so immediately block the player
			SwitchToMessageState( "Insufficient funds" );
		}
		else
		{
			var remainingVolume = playerData.m_playerShip.GetRemainingVolme();

			if ( remainingVolume == 0 )
			{
				// the cargo hold is full and the player cannot buy anything - so immediately block the player
				SwitchToMessageState( "Insufficient cargo space" );
			}
			else
			{

				// the maximum amount the player can buy is the lesser of the funds remaining or the cargo hold space remaining
				if ( remainingVolume < maximumAmount )
				{
					maximumAmount = remainingVolume;
				}

				// update the amount label
				m_amountLabelTMP.text = "Transfer how many cubic meters? (0.0 to " + ( maximumAmount / 10 ) + "." + ( maximumAmount % 10 ) + ")";

				// show the modal
				m_overlay.SetActive( true );
				m_amountModal.SetActive( true );
			}
		}

		// read input from any key on the keyboard
		SetInputFocus( true );

		// beep
		Sounds.m_instance.PlayActivate();
	}

	void SwitchToSellItemState( bool resetCurrentItemIndex = true )
	{
		// change the current state
		m_currentState = State.SellItem;

		// select the first part by default
		if ( resetCurrentItemIndex )
		{
			m_currentItemIndex = 0;
			m_currentRowOffset = 0;
		}

		// switch screens
		SwitchToTradeScreen( resetCurrentItemIndex );
	}

	void SwitchToSellAmountState()
	{
		// change the current state
		m_currentState = State.SellAmount;

		// erase the current text input
		m_amountInputTMP.text = m_amount = "";

		// get access to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the currently selected element id
		var elementId = m_itemList[ m_currentItemIndex ].m_id;

		// the maximum amount is however much the player has in the ships cargo hold
		var elementReference = playerData.m_playerShip.m_elementStorage.Find( elementId );

		var maximumAmount = elementReference.m_volume;

		// update the amount label
		m_amountLabelTMP.text = "Transfer how many cubic meters? (0.0 to " + ( maximumAmount / 10 ) + "." + ( maximumAmount % 10 ) + ")";

		// show the modal
		m_overlay.SetActive( true );
		m_amountModal.SetActive( true );

		// read input from any key on the keyboard
		SetInputFocus( true );

		// beep
		Sounds.m_instance.PlayActivate();
	}

	void SwitchToAnalyzeItemState( bool resetCurrentItemIndex = true )
	{
		// change the current state
		m_currentState = State.AnalyzeItem;

		// select the first part by default
		if ( resetCurrentItemIndex )
		{
			m_currentItemIndex = 0;
			m_currentRowOffset = 0;
		}

		// switch screens
		SwitchToTradeScreen( resetCurrentItemIndex );
	}

	void SwitchToAnalyzeConfirmState()
	{
		// change the current state
		m_currentState = State.AnalyzeConfirm;

		// select the no button by default
		SetCurrentButton( c_analyzeNoButtonIndex, false );
	}

	void SwitchToAnalyzeShowState()
	{
		// get the currently selected item
		var item = m_itemList[ m_currentItemIndex ];

		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// change the current state
		m_currentState = State.AnalyzeShow;

		// show the message
		SwitchToMessageState( gameData.m_artifactList[ item.m_id ].m_analysisText, true );
	}

	void SwitchToMessageState( string message, bool playUpdateSoundInstead = false )
	{
		// remember the current state
		m_stateBeforeMessageState = m_currentState;

		// change the current state
		m_currentState = State.Message;

		// set the error message
		m_messageLabelTMP.text = message;

		// show the modal
		m_overlay.SetActive( true );
		m_messageModal.SetActive( true );

		// set the current button
		SetCurrentButton( c_dismissButtonIndex, false );

		// beep
		if ( playUpdateSoundInstead )
		{
			Sounds.m_instance.PlayUpdate();
		}
		else
		{
			Sounds.m_instance.PlayError();
		}
	}

	void SwitchToStateBeforeMessageState()
	{
		switch ( m_stateBeforeMessageState )
		{
			case State.BuyItem:
			case State.BuyAmount:
				SwitchToBuyItemState( false );
				break;

			case State.SellItem:
			case State.SellAmount:
				SwitchToSellItemState( false );
				break;

			case State.AnalyzeItem:
			case State.AnalyzeShow:
				SwitchToAnalyzeItemState( false );
				break;
		}

		Sounds.m_instance.PlayDeactivate();
	}

	// call this whenever we change state or do something that would result in something changing the data in the trade panel
	void UpdateTradePanel()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get access to the player data
		var playerData = DataController.m_instance.m_playerData;

		// reset the trade item list
		m_itemList = new List<Item>();

		// clear out the text
		m_itemListTMP.text = "";
		m_volumeListTMP.text = "";
		m_unitValueListTMP.text = "";

		m_rowCount = 0;

		if ( ( m_currentState == State.BuyItem ) || ( m_currentState == State.SellItem ) )
		{
			// add elements heading
			m_itemListTMP.text += "<color=#FFFFFF>Elements</color>" + Environment.NewLine;
			m_volumeListTMP.text += Environment.NewLine;
			m_unitValueListTMP.text += Environment.NewLine;

			m_rowCount++;

			// get access to the ship cargo data for elements
			var elementStorage = playerData.m_playerShip.m_elementStorage;

			if ( m_currentState == State.BuyItem )
			{
				// add all elements available to buy in starport
				for ( var elementId = 0; elementId < gameData.m_elementList.Length; elementId++ )
				{
					var elementGameData = gameData.m_elementList[ elementId ];

					if ( elementGameData.m_availableInStarport )
					{
						m_itemListTMP.text += elementGameData.m_name + Environment.NewLine;

						var elementReference = elementStorage.Find( elementId );

						if ( elementReference == null )
						{
							m_volumeListTMP.text += "0.0" + Environment.NewLine;
						}
						else
						{
							m_volumeListTMP.text += ( elementReference.m_volume / 10 ) + "." + ( elementReference.m_volume % 10 ) + Environment.NewLine;
						}

						m_unitValueListTMP.text += elementGameData.m_starportPrice + Environment.NewLine;

						m_itemList.Add( new Item( m_rowCount++, 0, elementId, 0 ) );
					}
				}
			}
			else
			{
				// add all elements in the ship cargo hold
				foreach ( var elementReference in elementStorage.m_elementList )
				{
					var elementGameData = elementReference.GetElementGameData();

					m_itemListTMP.text += elementGameData.m_name + Environment.NewLine;
					m_volumeListTMP.text += ( elementReference.m_volume / 10 ) + "." + ( elementReference.m_volume % 10 ) + Environment.NewLine;
					m_unitValueListTMP.text += elementGameData.m_starportPrice + Environment.NewLine;

					m_itemList.Add( new Item( m_rowCount++, 0, elementReference.m_elementId, 0 ) );
				}
			}
		}

		if ( ( m_currentState == State.BuyItem ) || ( m_currentState == State.AnalyzeItem ) )
		{
			// add artifacts heading
			m_itemListTMP.text += "<color=#FFFFFF>Artifacts</color>" + Environment.NewLine;
			m_volumeListTMP.text += Environment.NewLine;
			m_unitValueListTMP.text += Environment.NewLine;

			m_rowCount++;

			// get access to the starport data for artifacts
			var artifactStorage = playerData.m_starport.m_artifactStorage;

			// add all artifacts available to buy in starport
			foreach ( PD_ArtifactReference artifactReference in artifactStorage.m_artifactList )
			{
				var artifactGameData = artifactReference.GetArtifactGameData();

				m_itemListTMP.text += artifactGameData.m_name + Environment.NewLine;
				m_volumeListTMP.text += ( artifactGameData.m_volume / 10 ) + "." + ( artifactGameData.m_volume % 10 ) + Environment.NewLine;
				m_unitValueListTMP.text += artifactGameData.m_starportPrice + Environment.NewLine;

				m_itemList.Add( new Item( m_rowCount++, 1, artifactReference.m_artifactId, 0 ) );
			}
		}

		if ( ( m_currentState == State.SellItem ) || ( m_currentState == State.AnalyzeItem ) )
		{
			// add artifacts heading
			m_itemListTMP.text += "<color=#FFFFFF>Artifacts</color>" + Environment.NewLine;
			m_volumeListTMP.text += Environment.NewLine;
			m_unitValueListTMP.text += Environment.NewLine;

			m_rowCount++;

			// get access to the ship storage for artifacts
			var artifactStorage = playerData.m_playerShip.m_artifactStorage;

			// add all artifacts in the ship cargo hold
			foreach ( var artifactReference in artifactStorage.m_artifactList )
			{
				var artifactGameData = artifactReference.GetArtifactGameData();

				m_itemListTMP.text += artifactGameData.m_name + Environment.NewLine;
				m_volumeListTMP.text += ( artifactGameData.m_volume / 10 ) + "." + ( artifactGameData.m_volume % 10 ) + Environment.NewLine;
				m_unitValueListTMP.text += artifactGameData.m_actualValue + Environment.NewLine;

				m_itemList.Add( new Item( m_rowCount++, 1, artifactReference.m_artifactId, 0 ) );
			}
		}

		// add end of list heading
		m_itemListTMP.text += "<color=#FFFFFF>End of List</color>" + Environment.NewLine;
		m_volumeListTMP.text += Environment.NewLine;
		m_unitValueListTMP.text += Environment.NewLine;

		m_rowCount++;
	}

	void UpdateItemSelection()
	{
		// if we have no items then switch to the menu bar state
		if ( m_itemList.Count == 0 )
		{
			SwitchToMenuBarState( false );
		}
		else
		{
			// keep the current selection index within bounds
			if ( m_currentItemIndex >= m_itemList.Count )
			{
				m_currentItemIndex = m_itemList.Count - 1;
			}

			// force the text object to update (so we can get the correct height)
			m_itemListTMP.ForceMeshUpdate();

			// force the canvas to update
			Canvas.ForceUpdateCanvases();

			// show the up arrow only if the first item is not selected
			ShowButton( c_upButtonIndex, m_currentItemIndex > 0 );

			// show the down arrow only if the last part is not selected
			ShowButton( c_downButtonIndex, m_currentItemIndex < ( m_itemList.Count - 1 ) );

			// get the row number of the currently selected item
			var row = m_itemList[ m_currentItemIndex ].m_row;

			// get the height of the item list viewport
			var viewportHeight = m_itemListMask.GetComponent<RectTransform>().rect.height;

			// calculate height of each text row
			var rowHeight = ( m_itemListTMP.renderedHeight + m_renderedHeightFudge ) / m_rowCount;

			// figure out the offset for the selection box
			float selectionBoxOffset;

			while ( true )
			{
				selectionBoxOffset = ( row + m_currentRowOffset ) * rowHeight;

				if ( ( selectionBoxOffset + rowHeight * 2 ) >= viewportHeight )
				{
					m_currentRowOffset--;
				}
				else if ( selectionBoxOffset < rowHeight )
				{
					m_currentRowOffset++;
				}
				else
				{
					break;
				}
			}

			// put the item selection box in the right place
			m_itemSelection.anchoredPosition = m_firstSelectionAnchoredPosition + new Vector2( 0.0f, -selectionBoxOffset );

			// calculate the offset for the text
			var textOffset = m_currentRowOffset * rowHeight;

			// move the text in all 3 columns
			m_itemListTMP.rectTransform.anchoredPosition = m_initialItemListAnchoredPosition + new Vector2( 0.0f, -textOffset );
			m_volumeListTMP.rectTransform.anchoredPosition = m_initialVolumeListAnchoredPosition + new Vector2( 0.0f, -textOffset );
			m_unitValueListTMP.rectTransform.anchoredPosition = m_initialUnitValueAnchoredPosition + new Vector2( 0.0f, -textOffset );
		}
	}

	// calculate the maximum amount the player can buy of an element with remaining funds
	int GetMaximumBuyAmountDueToCurrentBalance( int elementId )
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get access to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the starport price of this element
		var starportPrice = gameData.m_elementList[ elementId ].m_starportPrice;

		// get the player's current bank balance
		var currentBalance = playerData.m_bank.m_currentBalance;

		// calculate and return the maximum amount the player can buy
		return currentBalance * 10 / starportPrice;
	}

	void SelectPreviousItem()
	{
		if ( m_currentItemIndex > 0 )
		{
			m_currentItemIndex--;

			UpdateItemSelection();
		}

		SetCurrentButton( c_actionButtonIndex, false );
	}

	void SelectNextItem()
	{
		if ( m_currentItemIndex < ( m_itemList.Count - 1 ) )
		{
			m_currentItemIndex++;

			UpdateItemSelection();
		}

		SetCurrentButton( c_actionButtonIndex, false );
	}

	void DoAction()
	{
		switch ( m_currentState )
		{
			case State.BuyItem:
				BuySelectedItem();
				break;

			case State.SellItem:
				SellSelectedItem();
				break;

			case State.AnalyzeItem:
				AnalyzeSelectedItem();
				break;
		}
	}

	// buy the currently selected item
	void BuySelectedItem()
	{
		// check if the currently selected item is an element (0) or an artifact (1)
		var item = m_itemList[ m_currentItemIndex ];

		if ( item.m_type == 0 )
		{
			// it's an element - we need to know how much
			SwitchToBuyAmountState();
		}
		else
		{
			// get access to the game data
			var gameData = DataController.m_instance.m_gameData;

			// get access to the player data
			var playerData = DataController.m_instance.m_playerData;

			// get access to the artifact data
			var artifactGameData = gameData.m_artifactList[ item.m_id ];

			// it's an artifact - check if the player can afford it
			if ( playerData.m_bank.m_currentBalance < artifactGameData.m_starportPrice )
			{
				// nope - show a message
				SwitchToMessageState( "Insufficient funds" );
			}
			else if ( artifactGameData.m_volume > playerData.m_playerShip.GetRemainingVolme() )
			{
				// player's ship has no room for it - show a message
				SwitchToMessageState( "Insufficient cargo space" );
			}
			else
			{
				// deduct the price of the artifact from the player's bank balance
				playerData.m_bank.m_currentBalance -= gameData.m_artifactList[ item.m_id ].m_starportPrice;

				// transfer the artifact from the starport to the ship
				playerData.m_starport.m_artifactStorage.Remove( item.m_id );
				playerData.m_playerShip.AddArtifact( item.m_id );

				// update the screen
				UpdateTradePanel();

				// update the selection
				UpdateItemSelection();

				// play a ui sound
				Sounds.m_instance.PlayUpdate();
			}
		}
	}

	// sell the currently selected item
	void SellSelectedItem()
	{
		// check if the currently selected item is an element (0) or an artifact (1)
		var item = m_itemList[ m_currentItemIndex ];

		if ( item.m_type == 0 )
		{
			// it's an element - we need to know how much
			SwitchToSellAmountState();
		}
		else
		{
			// get access to the game data
			var gameData = DataController.m_instance.m_gameData;

			// get access to the player data
			var playerData = DataController.m_instance.m_playerData;

			// add the sale price of the artifact to the player's bank balance
			playerData.m_bank.m_currentBalance += gameData.m_artifactList[ item.m_id ].m_actualValue;

			// transfer the artifact from the ship to the starport
			playerData.m_starport.m_artifactStorage.Add( item.m_id );
			playerData.m_playerShip.RemoveArtifact( item.m_id );

			// update the screen
			UpdateTradePanel();

			// update the selection
			UpdateItemSelection();

			// play a ui sound
			Sounds.m_instance.PlayUpdate();
		}
	}

	// analyze the currently selected item
	void AnalyzeSelectedItem()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get access to the player data
		var playerData = DataController.m_instance.m_playerData;

		// check if the selected artifact is a known artifact
		var item = m_itemList[ m_currentItemIndex ];

		if ( playerData.m_knownArtifacts.IsKnown( item.m_id ) )
		{
			// yes we know it - go straight to show analysis
			SwitchToAnalyzeShowState();
		}
		else
		{
			// we do not know it - check if the player can afford it
			if ( playerData.m_bank.m_currentBalance < gameData.m_misc.m_artifactAnalysisPrice )
			{
				// the player cannot afford it
				SwitchToMessageState( "Insufficient funds" );
			}
			else
			{
				// switch to confirm analysis
				SwitchToAnalyzeConfirmState();
			}
		}
	}

	void UpdateBankBalanceText()
	{
		// get access to the player data
		var playerData = DataController.m_instance.m_playerData;

		// update bank balance amount
		m_currentBalanceTMP.text = "Your account balance is: " + playerData.m_bank.m_currentBalance + " M.U.";
	}

	void ClosePanel()
	{
		// if the bank balance has changed then record it in the bank transaction log
		var deltaBalance = m_startingBankBalance - DataController.m_instance.m_playerData.m_bank.m_currentBalance;

		if ( deltaBalance != 0 )
		{
			var sign = ( deltaBalance > 0 ) ? "-" : "+";

			var transaction = new PD_Bank.Transaction( DataController.m_instance.m_playerData.m_general.m_currentStardateYMD, "Trade Depot", deltaBalance.ToString() + sign );

			DataController.m_instance.m_playerData.m_bank.m_transactionList.Add( transaction );
		}

		Exit();
	}
}
