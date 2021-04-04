
class ToggleWeaponAction : Action
{
	public override string GetLabel()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_playerShip.m_weaponsAreArmed )
		{
			return "Disarm Weapon";
		}
		else
		{
			return "Arm Weapon";
		}
	}

	public override bool Execute()
	{
		return false;
	}
}
