using MV.WorldObject.AntiCheat;

public static class GenericStrippingPreventionManager
{
	public static void Initialize()
	{
		RangeValidator<int> rangeValidator = new RangeValidator<int>();
		rangeValidator.GetType();
		rangeValidator = null;
	}
}
