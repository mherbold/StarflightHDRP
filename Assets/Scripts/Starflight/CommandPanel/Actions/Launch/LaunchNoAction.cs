
class LaunchNoAction : Action
{
	public override string GetLabel()
	{
		return "No";
	}

	public override bool Execute()
	{
		// change the command set to the command command set
		CommandPanel.m_instance.SwitchToBridge();

		// clear the messages
		MessagePanel.m_instance.Clear();

		// there is no update
		return false;
	}
}
