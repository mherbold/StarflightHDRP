
class AbortAction : Action
{
	public override string GetLabel()
	{
		return "Abort";
	}

	public override bool Execute()
	{
		return false;
	}
}
