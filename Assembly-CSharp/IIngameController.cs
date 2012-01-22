public abstract class IIngameController
{
	public virtual void Initialize()
	{
	}

	public virtual void Deinitialize()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void LateUpdate()
	{
		MVGameController.Instance.WOCM.WorldInventory.LateUpdate();
	}

	public virtual void HandleInput()
	{
	}

	public virtual void FixedUpdate()
	{
	}
}
