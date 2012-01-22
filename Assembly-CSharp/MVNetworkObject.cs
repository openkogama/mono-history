public abstract class MVNetworkObject
{
	protected const int broadcastInterval = 100;

	protected const int clientDelay = 100;

	private MVWorldObjectClient worldObject;

	public MVWorldObjectClient WorldObject => worldObject;

	public MVNetworkObject(MVWorldObjectClient owner)
	{
		worldObject = owner;
	}

	public abstract void Update(MVNetworkGame game);
}
