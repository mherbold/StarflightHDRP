
class ToggleOrbitAction : Action
{
	public override string GetLabel()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		if ( ( playerData.m_general.m_activity == PD_General.Activity.DockingBay ) || ( playerData.m_general.m_activity == PD_General.Activity.Planetside ) )
		{
			return "Launch";
		}
		else
		{
			return "Land";
		}
	}

	public override bool Execute()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		switch ( playerData.m_general.m_activity )
		{
			case PD_General.Activity.JustLaunched:

				Sounds.m_instance.PlayError();

				MessagePanel.m_instance.Clear();

				MessagePanel.m_instance.AddText( "<color=white>We can't land on Arth.</color>" );

				break;

			case PD_General.Activity.InOrbit:

				// show the terrian map display
				//SpaceflightController.m_instance.m_displayController.ChangeDisplay( SpaceflightController.m_instance.m_displayController.m_terrainMapDisplay );

				// change the buttons
				CommandPanel.m_instance.ChangeCommandSet( CommandPanel.CommandSet.Land );

				break;

			case PD_General.Activity.DockingBay:
			case PD_General.Activity.Planetside:

				MessagePanel.m_instance.Clear();

				MessagePanel.m_instance.AddText( "<color=yellow>Confirm launch?</color>" );

				CommandPanel.m_instance.ChangeCommandSet( CommandPanel.CommandSet.Launch );

				break;

			default:

				Sounds.m_instance.PlayError();

				MessagePanel.m_instance.Clear();

				MessagePanel.m_instance.AddText( "<color=white>We're not in orbit.</color>" );

				break;
		}

		return false;
	}
}
