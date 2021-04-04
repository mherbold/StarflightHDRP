
class ToggleShieldsAction : Action
{
	public override string GetLabel()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_playerShip.m_shieldsAreUp )
		{
			return "Drop Shields";
		}
		else
		{
			return "Raise Shields";
		}
	}

	public override bool Execute()
	{
		return false;
	}
}
