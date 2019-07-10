using MV.WorldObject.AntiCheat;
using Newtonsoft.Json.Utilities;

public static class GenericStrippingPreventionManager
{
	public static void Initialize()
	{
		RangeValidator<int> rangeValidator = new RangeValidator<int>();
		rangeValidator.GetType();
		rangeValidator = null;
		CollectionWrapper<int> collectionWrapper = new CollectionWrapper<int>(CollectionUtils.CreateGenericList(2.GetType()));
		collectionWrapper.GetType();
		collectionWrapper = null;
	}
}
