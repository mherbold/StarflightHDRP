
class MedicalAction : Action
{
	public override string GetLabel()
	{
		return "Medical";
	}

	public override bool Execute()
	{
		// change the command set to the command command set
		CommandPanel.m_instance.ChangeCommandSet( CommandPanel.CommandSet.Medical );

		// change the name shown in the crew member panel
		CrewMemberPanel.m_instance.SelectRole( PD_CrewAssignment.Role.Doctor );

		// there is no update
		return false;
	}
}
