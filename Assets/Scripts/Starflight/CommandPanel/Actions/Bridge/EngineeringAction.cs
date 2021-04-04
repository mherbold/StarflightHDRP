
class EngineeringAction : Action
{
	public override string GetLabel()
	{
		return "Engineering";
	}

	public override bool Execute()
	{
		// change the command set to the command command set
		CommandPanel.m_instance.ChangeCommandSet( CommandPanel.CommandSet.Engineering );

		// change the name shown in the crew member panel
		CrewMemberPanel.m_instance.SelectRole( PD_CrewAssignment.Role.Engineer );

		// there is no update
		return false;
	}
}
