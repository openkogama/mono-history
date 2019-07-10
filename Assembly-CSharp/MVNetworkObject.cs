public abstract class MVNetworkObject
{
	protected MVWorldObjectClient worldObject;

	public abstract bool RemoveFromUpdate { get; }

	public MVWorldObjectClient WorldObject => worldObject;

	public MVNetworkObject(MVWorldObjectClient owner)
	{
		worldObject = owner;
	}

	public abstract void Update(MVNetworkGame game);
}
