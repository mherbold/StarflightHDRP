
class AnswerNoAction : Action
{
	public override string GetLabel()
	{
		return "No";
	}

	public override bool Execute()
	{
		return false;
	}
}
