public class InputToInGameAction
{
	private bool use;

	private bool fire;

	private bool drop;

	private bool ignorePickupOwner;

	public bool IgnorePickupOwner
	{
		get
		{
			return ignorePickupOwner;
		}
		set
		{
			ignorePickupOwner = true;
		}
	}

	public bool Fire
	{
		get
		{
			return fire;
		}
		set
		{
			fire = value;
		}
	}

	public bool Drop => drop;

	public bool Use => use;

	public void HandleInputState()
	{
		use = false;
		fire = false;
		drop = false;
		ignorePickupOwner = false;
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.Use))
		{
			use = true;
		}
		if (MVInputWrapper.GetBooleanControl(KogamaControls.Fire) || MVInputWrapper.GetBooleanControlDown(KogamaControls.Fire))
		{
			fire = true;
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.DropCurrentItem))
		{
			drop = true;
		}
	}
}
