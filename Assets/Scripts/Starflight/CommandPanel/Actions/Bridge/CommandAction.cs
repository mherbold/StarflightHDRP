
class CommandAction : Action
{
	public override string GetLabel()
	{
		return "Command";
	}

	public override bool Execute()
	{
		// change the command set to the command command set
		CommandPanel.m_instance.ChangeCommandSet( CommandPanel.CommandSet.Command );

		// change the name shown in the crew member panel
		CrewMemberPanel.m_instance.SelectRole( PD_CrewAssignment.Role.Captain );

		// there is no update
		return false;
	}
}
