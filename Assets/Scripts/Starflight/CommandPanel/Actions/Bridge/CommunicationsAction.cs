
class CommunicationsAction : Action
{
	public override string GetLabel()
	{
		return "Communications";
	}

	public override bool Execute()
	{
		// change the command set to the command command set
		CommandPanel.m_instance.ChangeCommandSet( CommandPanel.CommandSet.Communications );

		// change the name shown in the crew member panel
		CrewMemberPanel.m_instance.SelectRole( PD_CrewAssignment.Role.CommunicationsOfficer );

		// there is no update
		return false;
	}
}
