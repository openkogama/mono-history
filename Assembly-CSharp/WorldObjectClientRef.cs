public class WorldObjectClientRef
{
	private readonly int woId = -1;

	public MVWorldObjectClient WorldObjectClient => MVGameControllerBase.WOCM.GetWorldObjectClient(woId);

	protected WorldObjectClientRef(int woId)
	{
		this.woId = woId;
	}
}
