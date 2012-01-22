public class MVCubeModelFineGrainedTerrain : MVCubeModelBase
{
	protected override void CreateMVWOC(bool local)
	{
		base.CreateMVWOC(local);
		interactionFlags = InteractionFlags.None;
		MVGameController.Instance.WOCM.FineGrainedTerrain = this;
	}

	public override void Initialize()
	{
	}

	public override void Destroy()
	{
		prototypeCubeModel.RemoveInstance(id);
	}
}
