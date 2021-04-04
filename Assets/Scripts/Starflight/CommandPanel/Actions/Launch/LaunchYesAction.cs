
class LaunchYesAction : Action
{
	public override string GetLabel()
	{
		return "Yes";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_general.m_activity == PD_General.Activity.DockingBay )
		{
			// update the messages log
			MessagePanel.m_instance.Clear();

			MessagePanel.m_instance.AddText( "<color=white>Opening docking bay doors...</color>" );

			// start the launch animation from the docking bay
			Starflight.m_instance.PlayDockingBayLaunch();
		}
		else
		{
			// update the messages log
			MessagePanel.m_instance.Clear();

			MessagePanel.m_instance.AddText( "<color=white>Commencing launch sequence...</color>" );

			// start the launch animation from planetside
			// SpaceflightController.m_instance.m_planetside.StartLaunchAnimation();
		}

		return true;
	}
}
