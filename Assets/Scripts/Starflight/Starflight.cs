
using UnityEngine;
using UnityEngine.Playables;

public class Starflight : MonoBehaviour
{
	// the different activities
	[SerializeField] GameObject m_starport;
	[SerializeField] GameObject m_dockingBay;
	[SerializeField] GameObject m_starSystem;

	// the different playable directors
	[SerializeField] PlayableDirector m_dockingBayLaunchPlayableDirector;

	// the different settings
	[SerializeField] float m_encounterRange;

	// static instance to this starflight class
	public static Starflight m_instance;

	void Awake()
	{
		Debug.Log( "Starflight Awake" );

		m_instance = this;
	}

	void Start()
	{
		Debug.Log( "Starflight Start" );

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// switch to the bridge command set
		CommandPanel.m_instance.SwitchToBridge();

		// switch to the current activity
		SwitchActivity( playerData.m_general.m_activity );
	}

	public void SwitchActivity( PD_General.Activity newActivity )
	{
		// switch to the new activity
		Debug.Log( "Switching to activity " + newActivity );

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// are we switching to a new location?
		bool activityIsDifferent = false;

		if ( playerData.m_general.m_activity != newActivity )
		{
			// yes - remember the last activity
			playerData.m_general.m_lastActivity = playerData.m_general.m_activity;

			// update the player activity
			playerData.m_general.m_activity = newActivity;

			// remember that the new location is different
			activityIsDifferent = true;
		}

		// make sure the display is updated (in case we are loading from a save game)
		MessagePanel.m_instance.Refresh();

		// hide all activities
		m_dockingBay.SetActive( false );
		m_starSystem.SetActive( false );

		// switching to starport is a special case
		if ( playerData.m_general.m_activity == PD_General.Activity.Starport )
		{
			gameObject.SetActive( false );
			m_starport.SetActive( true );
		}
		else
		{
			// switch the location
			switch ( playerData.m_general.m_activity )
			{
				case PD_General.Activity.DockingBay:
					m_dockingBay.SetActive( true );
					DisplayPanel.m_instance.SwitchTo( DisplayPanel.Panel.Status );
					break;
			}
		}

		// did we switch to a new location?
		if ( activityIsDifferent )
		{
			// yes - save the player data since the location was changed
			DataController.m_instance.SaveActiveGame();
		}
	}

	public void PlayDockingBayLaunch()
	{
		// play the animation
		m_dockingBayLaunchPlayableDirector.Play();
	}

#if UNITY_EDITOR

	// draw gizmos to help debug the game
	void OnDrawGizmos()
	{
		if ( DataController.m_instance == null )
		{
			return;
		}

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the current player location
		var location = playerData.m_general.m_activity;

		// if we are in hyperspace draw the map grid
		if ( location == PD_General.Activity.Hyperspace )
		{
			Gizmos.color = Color.green;

			for ( var x = 0; x < 250; x += 10 )
			{
				var start = Tools.GameToWorldCoordinates( new Vector3( x, 0.0f, 0.0f ) );
				var end = Tools.GameToWorldCoordinates( new Vector3( x, 0.0f, 250.0f ) );

				Gizmos.DrawLine( start, end );
			}

			for ( var z = 0; z < 250; z += 10 )
			{
				var start = Tools.GameToWorldCoordinates( new Vector3( 0.0f, 0.0f, z ) );
				var end = Tools.GameToWorldCoordinates( new Vector3( 250.0f, 0.0f, z ) );

				Gizmos.DrawLine( start, end );
			}
		}

		// if we are in hyperspace draw the flux paths
		if ( location == PD_General.Activity.Hyperspace )
		{
			Gizmos.color = Color.cyan;

			foreach ( var flux in gameData.m_fluxList )
			{
				Gizmos.DrawLine( flux.GetFrom(), flux.GetTo() );
			}
		}

		// if we are in hyperspace draw the coordinates around the cursor point
		if ( location == PD_General.Activity.Hyperspace )
		{
			var ray = UnityEditor.HandleUtility.GUIPointToWorldRay( Event.current.mousePosition );

			var plane = new Plane( Vector3.up, 0.0f );

			if ( plane.Raycast( ray, out float enter ) )
			{
				var worldCoordinates = ray.origin + ray.direction * enter;

				var gameCoordinates = Tools.WorldToGameCoordinates( worldCoordinates );

				UnityEditor.Handles.color = Color.blue;

				UnityEditor.Handles.Label( worldCoordinates + Vector3.forward * 64.0f + Vector3.right * 64.0f, Mathf.RoundToInt( gameCoordinates.x ) + "," + Mathf.RoundToInt( gameCoordinates.z ) );
			}
		}

		// if we are in either hyperspace or a star system then draw the alien encounters
		if ( ( location == PD_General.Activity.StarSystem ) || ( location == PD_General.Activity.Hyperspace ) )
		{
			// draw the encounter radius
			UnityEditor.Handles.color = Color.red;
			UnityEditor.Handles.DrawWireDisc( playerData.m_general.m_coordinates, Vector3.up, m_encounterRange );

			// draw the radar range
			UnityEditor.Handles.color = Color.magenta;

			if ( location == PD_General.Activity.Hyperspace )
			{
				UnityEditor.Handles.DrawWireDisc( playerData.m_general.m_coordinates, Vector3.up, Radar.m_instance.m_maxPlayerDetectionDistanceInHyperspace );
			}
			else
			{
				UnityEditor.Handles.DrawWireDisc( playerData.m_general.m_coordinates, Vector3.up, Radar.m_instance.m_maxPlayerDetectionDistanceInStarSystem );
			}

			// draw the positions on each encounter
			UnityEditor.Handles.color = Color.red;

			foreach ( var pdEncounter in playerData.m_encounterList )
			{
				var encounterLocation = pdEncounter.GetLocation();

				if ( ( encounterLocation == location ) && ( location == PD_General.Activity.Hyperspace ) || ( pdEncounter.GetStarId() == playerData.m_general.m_currentStarId ) )
				{
					// get access to the encounter
					var gdEncounter = gameData.m_encounterList[ pdEncounter.m_encounterId ];

					// draw alien position
					UnityEditor.Handles.color = Color.red;
					UnityEditor.Handles.DrawWireDisc( pdEncounter.m_currentCoordinates, Vector3.up, 16.0f );

					// print alien race
					UnityEditor.Handles.Label( pdEncounter.m_currentCoordinates + Vector3.up * 16.0f, gdEncounter.m_race.ToString() );

					// draw the alien radar range
					UnityEditor.Handles.color = Color.yellow;

					if ( location == PD_General.Activity.Hyperspace )
					{
						UnityEditor.Handles.DrawWireDisc( pdEncounter.m_currentCoordinates, Vector3.up, Radar.m_instance.m_maxAlienDetectionDistanceInHyperspace );
					}
					else
					{
						UnityEditor.Handles.DrawWireDisc( pdEncounter.m_currentCoordinates, Vector3.up, Radar.m_instance.m_maxAlienDetectionDistanceInStarSystem );
					}
				}
			}
		}
	}

#endif // UNITY_EDITOR
}
