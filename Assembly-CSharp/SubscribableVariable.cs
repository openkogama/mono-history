public class SubscribableVariable<T> : SubscribableVariableBase<T>
{
	public T ValueSet
	{
		set
		{
			base.value = value;
			Notify();
		}
	}

	public SubscribableVariable(T value)
		: base(value)
	{
	}
}
