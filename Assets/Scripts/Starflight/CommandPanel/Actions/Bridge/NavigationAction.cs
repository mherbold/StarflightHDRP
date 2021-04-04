
class NavigationAction : Action
{
	public override string GetLabel()
	{
		return "Navigation";
	}

	public override bool Execute()
	{
		// change the command set to the command command set
		CommandPanel.m_instance.ChangeCommandSet( CommandPanel.CommandSet.Navigation );

		// change the name shown in the crew member panel
		CrewMemberPanel.m_instance.SelectRole( PD_CrewAssignment.Role.Navigator );

		// there is no update
		return false;
	}
}
