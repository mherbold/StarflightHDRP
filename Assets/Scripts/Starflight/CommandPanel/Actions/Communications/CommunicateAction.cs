
class CommunicateAction : Action
{
	public override string GetLabel()
	{
		if ( true )
		{
			return "Hail";
		}
		else
		{
			return "Respond";
		}
	}

	public override bool Execute()
	{
		return false;
	}
}
