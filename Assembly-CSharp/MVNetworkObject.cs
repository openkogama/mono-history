public abstract class MVNetworkObject
{
	protected const int broadcastInterval = 200;

	protected const int clientDelay = 200;

	private MVWorldObjectClient worldObject;

	public MVWorldObjectClient WorldObject => worldObject;

	public MVNetworkObject(MVWorldObjectClient owner)
	{
		worldObject = owner;
	}

	public abstract void Update(MVNetworkGame game);
}
