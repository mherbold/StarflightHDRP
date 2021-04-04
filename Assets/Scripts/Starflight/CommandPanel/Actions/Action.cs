
using UnityEngine;

public abstract class Action
{
	public abstract string GetLabel();
	public abstract bool Execute(); // return true if this action has an update (is ongoing) or false if this is a one-shot action with no update

	public void Update()
	{
	}

	public void OnMove( Vector2 moveVector )
	{
	}

	public void OnLeft()
	{
	}

	public void OnRight()
	{
	}

	public void OnUp()
	{
	}

	public void OnDown()
	{
	}

	public void OnFire()
	{
	}

	public void OnCancel()
	{
	}
}
