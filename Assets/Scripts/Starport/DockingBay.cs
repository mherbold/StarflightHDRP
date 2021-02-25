
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class DockingBay : DoorPanelController
{
	const int c_exitButtonIndex = 0;

	[SerializeField] TextMeshProUGUI m_messageTMP;
	[SerializeField] Astronaut m_astronaut;
	[SerializeField] PlayableDirector m_playableDirector;

	protected override void Restart()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// automatically select the "exit" button for the player
		SetCurrentButton( c_exitButtonIndex, false );

		// let's start the pre-flight check
		var preflightCheckPassed = true;
		m_messageTMP.text = "";

		// do we have all of the crew positions assigned?
		for ( var role = PD_CrewAssignment.Role.First; role < PD_CrewAssignment.Role.Count; role++ )
		{
			if ( !playerData.m_crewAssignment.IsAssigned( role ) )
			{
				m_messageTMP.text += "\u2022 Report to Crew Assignment.\n";
				preflightCheckPassed = false;
				break;
			}

			var personnelFile = playerData.m_crewAssignment.GetPersonnelFile( role );

			if ( personnelFile.m_vitality == 0 )
			{
				m_messageTMP.text += "\u2022 At least one of your crew members is dead.\n";
				preflightCheckPassed = false;
				break;
			}
		}

		// did we name the ship?
		if ( playerData.m_playerShip.m_name.Length == 0 )
		{
			m_messageTMP.text += "\u2022 Report to Ship Configuration to christen ship.\n";
			preflightCheckPassed = false;
		}

		// does the ship have engines?
		if ( playerData.m_playerShip.m_enginesClass == 0 )
		{
			m_messageTMP.text += "\u2022 Report to Ship Configuration to purchase engines.\n";
			preflightCheckPassed = false;
		}

		// do we have fuel?
		if ( playerData.m_playerShip.m_elementStorage.Find( 5 ) == null )
		{
			m_messageTMP.text += "\u2022 Report to Trade Depot to purchase fuel.\n";
			preflightCheckPassed = false;
		}

		// did we pass the pre-flight checks?
		if ( preflightCheckPassed )
		{
			m_playableDirector.Play();

			gameObject.SetActive( false );
		}
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
