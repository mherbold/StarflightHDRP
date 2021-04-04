
class ScienceAction : Action
{
	public override string GetLabel()
	{
		return "Science";
	}

	public override bool Execute()
	{
		// change the command set to the command command set
		CommandPanel.m_instance.ChangeCommandSet( CommandPanel.CommandSet.Science );

		// change the name shown in the crew member panel
		CrewMemberPanel.m_instance.SelectRole( PD_CrewAssignment.Role.ScienceOfficer );

		// there is no update
		return false;
	}
}
