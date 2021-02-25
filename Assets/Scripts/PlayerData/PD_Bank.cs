﻿
using System;
using System.Collections.Generic;

[Serializable]
public class PD_Bank
{
	[Serializable]
	public class Transaction
	{
		public string m_stardate;
		public string m_description;
		public string m_amount;

		public Transaction( string stardate, string description, string amount )
		{
			m_stardate = stardate;
			m_description = description;
			m_amount = amount;
		}
	}

	public int m_currentBalance;
	public List<Transaction> m_transactionList;

	public void Reset()
	{
		// reset the bank balance
		//m_currentBalance = 12000;

		// hack - make the player rich for now
		m_currentBalance = 1000000;

		// create a new transactions list
		m_transactionList = new List<Transaction>
		{
			// add the first transaction (game purchase)
			new Transaction( "4620-01-01", "Game purchase", "200-" )
		};
	}
}
