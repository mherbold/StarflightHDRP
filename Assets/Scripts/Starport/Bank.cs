
using System;
using TMPro;
using UnityEngine;

public class Bank : DoorPanelController
{
	const int c_exitButtonIndex = 0;

	[SerializeField] TextMeshProUGUI m_dateValuesTMP;
	[SerializeField] TextMeshProUGUI m_transactionsValuesTMP;
	[SerializeField] TextMeshProUGUI m_amountValuesTMP;
	[SerializeField] TextMeshProUGUI m_currentBalanceTMP;

	[SerializeField] float m_maskHeight;

	Vector2 m_initialDateValuesAnchoredPosition;
	Vector2 m_initialTransactionsValueAnchoredPosition;
	Vector2 m_initialAmountValuesAnchoredPosition;

	protected override void Initialize()
	{
		m_initialDateValuesAnchoredPosition = m_dateValuesTMP.rectTransform.anchoredPosition;
		m_initialTransactionsValueAnchoredPosition = m_transactionsValuesTMP.rectTransform.anchoredPosition;
		m_initialAmountValuesAnchoredPosition = m_amountValuesTMP.rectTransform.anchoredPosition;
	}

	protected override void Restart()
	{
		// get access to the bank player data
		var bank = DataController.m_instance.m_playerData.m_bank;

		// update the current balance text
		m_currentBalanceTMP.text = "Your current balance is " + string.Format( "{0:n0}", bank.m_currentBalance ) + " M.U.";

		// update the date, transactions, and amount list text
		m_dateValuesTMP.text = "";
		m_transactionsValuesTMP.text = "";
		m_amountValuesTMP.text = "";

		for ( var transactionId = 0; transactionId < bank.m_transactionList.Count; transactionId++ )
		{
			var transaction = bank.m_transactionList[ transactionId ];

			var dateTime = DateTime.ParseExact( transaction.m_stardate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture );

			m_dateValuesTMP.text += dateTime.ToShortDateString();
			m_transactionsValuesTMP.text += transaction.m_description;
			m_amountValuesTMP.text += transaction.m_amount;

			if ( transactionId < ( bank.m_transactionList.Count - 1 ) )
			{
				m_dateValuesTMP.text += Environment.NewLine;
				m_transactionsValuesTMP.text += Environment.NewLine;
				m_amountValuesTMP.text += Environment.NewLine;
			}
		}

		// force the text object to update (so we can get the correct height)
		m_transactionsValuesTMP.ForceMeshUpdate();

		// calculate the offset we need to show the bottom of the list
		float offset = Mathf.Max( 0.0f, m_transactionsValuesTMP.renderedHeight - m_maskHeight );

		// move up the text in all 3 columns
		m_dateValuesTMP.rectTransform.anchoredPosition = m_initialDateValuesAnchoredPosition + new Vector2( 0.0f, offset );
		m_transactionsValuesTMP.rectTransform.anchoredPosition = m_initialTransactionsValueAnchoredPosition + new Vector2( 0.0f, offset );
		m_amountValuesTMP.rectTransform.anchoredPosition = m_initialAmountValuesAnchoredPosition + new Vector2( 0.0f, offset );

		// automatically select the "exit" button for the player
		SetCurrentButton( c_exitButtonIndex, false );
	}

	public override void OnFire()
	{
		base.OnFire();

		switch ( m_currentButtonIndex )
		{
			case c_exitButtonIndex:
				Exit();
				break;
		}
	}

	public override void OnCancel()
	{
		base.OnCancel();

		switch ( m_currentButtonIndex )
		{
			case c_exitButtonIndex:
				Exit();
				break;
		}
	}
}
