
using TMPro;
using UnityEngine;

public class CrewMemberPanel : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI m_text;

	public static CrewMemberPanel m_instance;

	void Awake()
	{
		Debug.Log( "CrewMemberPanel Awake" );

		m_instance = this;
	}

	public void Clear()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// change the current officer label to the name of the ship
		m_text.text = "ISS " + playerData.m_playerShip.m_name;
	}

	public void SelectRole( PD_CrewAssignment.Role role )
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the personnel file for this role
		var personnelFile = playerData.m_crewAssignment.GetPersonnelFile( role );

		// this person is either captain or officer
		string title;

		switch ( role )
		{
			case PD_CrewAssignment.Role.Captain:
				title = "Captain";
				break;

			case PD_CrewAssignment.Role.Doctor:
				title = "Doctor";
				break;

			default:
				title = "Officer";
				break;
		}

		// set the name of the officer
		m_text.text = title + " " + personnelFile.m_name;
	}
}
