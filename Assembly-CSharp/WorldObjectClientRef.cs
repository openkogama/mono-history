public class WorldObjectClientRef<T> where T : MVWorldObjectClient
{
	private readonly int woId = -1;

	public T WorldObjectClient => MVGameControllerBase.WOCM.GetWorldObjectClient<T>(woId);

	protected WorldObjectClientRef(int woId)
	{
		this.woId = woId;
	}
}
public class WorldObjectClientRef : WorldObjectClientRef<MVWorldObjectClient>
{
	protected WorldObjectClientRef(int woId)
		: base(woId)
	{
	}
}
