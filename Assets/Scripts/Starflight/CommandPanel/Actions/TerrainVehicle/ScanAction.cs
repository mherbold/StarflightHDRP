
class ScanAction : Action
{
	public override string GetLabel()
	{
		return "Scan";
	}

	public override bool Execute()
	{
		// set the name of the officer
		CrewMemberPanel.m_instance.SelectRole( PD_CrewAssignment.Role.Navigator );

		return false;
	}
}
