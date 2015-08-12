public class InteractionInput
{
	private bool use;

	private bool fire;

	private bool drop;

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
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.Use))
		{
			use = true;
		}
		if (MVInputWrapper.GetBooleanControl(KogamaControls.Fire))
		{
			fire = true;
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.DropCurrentItem))
		{
			drop = true;
		}
	}
}
