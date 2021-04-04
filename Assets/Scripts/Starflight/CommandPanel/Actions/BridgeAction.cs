
class BridgeAction : Action
{
	public override string GetLabel()
	{
		return "Bridge";
	}

	public override bool Execute()
	{
		// change the command set to the command command set
		CommandPanel.m_instance.SwitchToBridge();

		// there is no update
		return false;
	}
}
