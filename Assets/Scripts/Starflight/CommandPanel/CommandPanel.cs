
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommandPanel : MonoBehaviour
{
	const int c_numCommands = 6;

	public enum CommandSet
	{
		AnswerQuestion,
		AskQuestion,
		Bridge,
		Command,
		CommLog,
		Communications,
		Dialogue,
		Engineering,
		Land,
		Launch,
		Medical,
		Navigation,
		Posture,
		Science,
		ShipsLog,
		TerrainVehicle,
		Count
	};

	[SerializeField] Command[] m_commandList;
	[SerializeField] TextMeshProUGUI m_currentOfficer;
	[SerializeField] float m_activateDelayTime = 0.35f;
	[SerializeField] float m_debounceTime = 0.25f;

	Action m_currentAction;
	Action[][] m_commandSetActionList;

	int m_selectedCommandIndex;
	bool m_activatingCommand;
	float m_activatingCommandTimer;

	Vector2 m_moveVector;
	float m_nextMoveTime;

	public static CommandPanel m_instance;

	void Awake()
	{
		Debug.Log( "CommandPanel Awake" );

		m_instance = this;

		m_commandSetActionList = new Action[ (int) CommandSet.Count ][];

		m_commandSetActionList[ (int) CommandSet.AnswerQuestion ] = new Action[] { new AnswerYesAction(), new AnswerNoAction(), new TerminateAction() };
		m_commandSetActionList[ (int) CommandSet.AskQuestion ] = new Action[] { new ThemselvesAskAction(), new OtherRacesAskAction(), new OldEmpireAskAction(), new TheAncientsAskAction(), new GeneralInfoAskAction() };
		m_commandSetActionList[ (int) CommandSet.Bridge ] = new Action[] { new CommandAction(), new ScienceAction(), new NavigationAction(), new EngineeringAction(), new CommunicationsAction(), new MedicalAction() };
		m_commandSetActionList[ (int) CommandSet.Command ] = new Action[] { new ToggleOrbitAction(), new DisembarkAction(), new ShipCargoAction(), new LogPlanetAction(), new ShipsLogAction(), new BridgeAction() };
		m_commandSetActionList[ (int) CommandSet.CommLog ] = new Action[] { new ThemselvesLogAction(), new OtherRacesLogAction(), new OldEmpireLogAction(), new TheAncientsLogAction(), new GeneralInfoLogAction(), new CancelLogAction() };
		m_commandSetActionList[ (int) CommandSet.Communications ] = new Action[] { new CommunicateAction(), new DistressAction(), new BridgeAction() };
		m_commandSetActionList[ (int) CommandSet.Dialogue ] = new Action[] { new StatementAction(), new QuestionAction(), new PostureAction(), new TerminateAction() };
		m_commandSetActionList[ (int) CommandSet.Engineering ] = new Action[] { new DamageAction(), new RepairAction(), new BridgeAction() };
		m_commandSetActionList[ (int) CommandSet.Land ] = new Action[] { new SelectSiteAction(), new DescendAction(), new AbortAction() };
		m_commandSetActionList[ (int) CommandSet.Launch ] = new Action[] { new LaunchYesAction(), new LaunchNoAction() };
		m_commandSetActionList[ (int) CommandSet.Medical ] = new Action[] { new ExamineAction(), new TreatAction(), new BridgeAction() };
		m_commandSetActionList[ (int) CommandSet.Navigation ] = new Action[] { new ManeuverAction(), new StarmapAction(), new ToggleShieldsAction(), new ToggleWeaponAction(), new CombatAction(), new BridgeAction() };
		m_commandSetActionList[ (int) CommandSet.Posture ] = new Action[] { new FriendlyAction(), new HostileAction(), new ObsequiousAction() };
		m_commandSetActionList[ (int) CommandSet.Science ] = new Action[] { new SensorsAction(), new AnalysisAction(), new StatusAction(), new BridgeAction() };
		m_commandSetActionList[ (int) CommandSet.ShipsLog ] = new Action[] { new StarportNoticesAction(), new AlienCommsAction(), new MessagesAction(), new BridgeAction() };
		m_commandSetActionList[ (int) CommandSet.TerrainVehicle ] = new Action[] { new MapAction(), new MoveAction(), new TVCargoAction(), new LookAction(), new ScanAction(), new WeaponAction() };
	}

	// unity on enable
	void OnEnable()
	{
		Debug.Log( "CommandPanel OnEnable" );

		// reset everything
		m_activatingCommand = false;
		m_activatingCommandTimer = 0.0f;
	}

	// unity update
	void Update()
	{
		// allow actions to do non-ui moving (i.e. player ship)
		if ( m_currentAction != null )
		{
			m_currentAction.OnMove( m_moveVector );
		}

		// ui moving (left, right, up, down) with debounce
		if ( m_nextMoveTime > 0.0f )
		{
			m_nextMoveTime -= Time.deltaTime;
		}

		var x = Mathf.RoundToInt( m_moveVector.x );
		var y = Mathf.RoundToInt( m_moveVector.y );

		if ( ( x == 0 ) && ( y == 0 ) )
		{
			m_nextMoveTime = 0.0f;
		}
		else if ( m_nextMoveTime <= 0.0f )
		{
			m_nextMoveTime = m_debounceTime;

			if ( x < 0 )
			{
				if ( m_currentAction != null )
				{
					m_currentAction.OnLeft();
				}
			}
			else if ( x > 0 )
			{
				if ( m_currentAction != null )
				{
					m_currentAction.OnRight();
				}
			}
			else if ( y > 0 )
			{
				if ( m_currentAction != null )
				{
					m_currentAction.OnUp();
				}
				else if ( !m_activatingCommand )
				{
					if ( m_selectedCommandIndex == 0 )
					{
						Sounds.m_instance.PlayError();
					}
					else
					{
						SelectCommand( m_selectedCommandIndex - 1 );

						Sounds.m_instance.PlayClick();
					}
				}
			}
			else if ( y < 0 )
			{
				if ( m_currentAction != null )
				{
					m_currentAction.OnDown();
				}
				else if ( !m_activatingCommand )
				{
					var nextSelectedCommandIndex = m_selectedCommandIndex + 1;

					if ( ( nextSelectedCommandIndex == c_numCommands ) || !m_commandList[ nextSelectedCommandIndex ].HasAction() )
					{
						Sounds.m_instance.PlayError();
					}
					else
					{
						SelectCommand( nextSelectedCommandIndex );

						Sounds.m_instance.PlayClick();
					}
				}
			}
		}

		// check if we are activating the currently selected command
		if ( m_activatingCommand )
		{
			// yes - so update the timer
			m_activatingCommandTimer += Time.deltaTime;

			// after a certain amount of time, execute the command
			if ( m_activatingCommandTimer >= m_activateDelayTime )
			{
				// reset the activate command flag and timer
				m_activatingCommand = false;
				m_activatingCommandTimer = 0.0f;

				// get the action
				var action = m_commandList[ m_selectedCommandIndex ].GetAction();

				// execute the current button and check if it returned true
				if ( action.Execute() )
				{
					// update the current button
					m_currentAction = action;
				}
				else
				{
					m_commandList[ m_selectedCommandIndex ].SetSelected();
				}
			}
		}

		// check if we have a current action
		if ( m_currentAction != null )
		{
			// yes - call update on it
			m_currentAction.Update();
		}
	}

	// set the bridge commands
	public void SwitchToBridge()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// restore the bridge commands
		ChangeCommandSet( CommandSet.Bridge );

		// show the ships name
		CrewMemberPanel.m_instance.Clear();

		// update the message
		switch ( playerData.m_general.m_activity )
		{
			case PD_General.Activity.DockingBay:
				MessagePanel.m_instance.Clear();
				MessagePanel.m_instance.AddText( "<color=white>Ship computer activated.\nPre-launch procedures complete.\nStanding by to initiate launch.</color>" );
				break;

			case PD_General.Activity.JustLaunched:
				MessagePanel.m_instance.Clear();
				MessagePanel.m_instance.AddText( "<color=white>Starport clear.\nStanding by to maneuver.</color>" );
				break;
		}
	}

	// update the commands and change the current command index
	public void ChangeCommandSet( CommandSet newCommandSet )
	{
		Debug.Log( "Changing command set to " + newCommandSet );

		// clear the current action
		m_currentAction = null;

		// get the desired command set
		var commandSet = m_commandSetActionList[ (int) newCommandSet ];

		// go through all 6 commands
		for ( var i = 0; i < c_numCommands; i++ )
		{
			if ( i < commandSet.Length )
			{
				m_commandList[ i ].SetAction( commandSet[ i ] );
			}
			else
			{
				m_commandList[ i ].ClearAction();
			}
		}

		// reset the current command index to the first one
		SelectCommand( 0 );
	}

	void SelectCommand( int selectedCommandIndex )
	{
		// go through all of the command and set the icon state to "off"
		for ( var i = 0; i < c_numCommands; i++ )
		{
			m_commandList[ i ].SetOff();
		}

		// remember the selected command index
		m_selectedCommandIndex = selectedCommandIndex;

		// set the icon state to "selected" for the selected command
		m_commandList[ m_selectedCommandIndex ].SetSelected();
	}

	void ActivateSelectedCommand()
	{
		// set the activate button flag and reset the timer
		m_activatingCommand = true;
		m_activatingCommandTimer = 0.0f;

		// update the button sprite for the currently selected button
		m_commandList[ m_selectedCommandIndex ].SetActive();

		// play the activate sound
		Sounds.m_instance.PlayActivate();
	}

	public void OnMove( InputAction.CallbackContext context )
	{
		m_moveVector = context.ReadValue<Vector2>();
	}

	public void OnFire( InputAction.CallbackContext context )
	{
		if ( !context.canceled && context.action.triggered )
		{
			if ( m_currentAction != null )
			{
				m_currentAction.OnFire();
			}
			else if ( !m_activatingCommand )
			{
				ActivateSelectedCommand();
			}
		}
	}

	public void OnCancel( InputAction.CallbackContext context )
	{
		if ( !context.canceled && context.action.triggered )
		{
			if ( m_currentAction != null )
			{
				m_currentAction.OnCancel();
			}
		}
	}
}
