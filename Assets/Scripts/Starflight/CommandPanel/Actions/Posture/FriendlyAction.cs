
class FriendlyAction : Action
{
	public override string GetLabel()
	{
		return "Friendly";
	}

	public override bool Execute()
	{
		return false;
	}
}
