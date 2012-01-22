internal class ESOpenModelEdit : ESStateBase
{
	private EditModel editModel;

	public override void Enter(EditorStateMachine e)
	{
		editModel = new EditModel();
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (editModel.CurrentState == BuildState.Idle || editModel.CurrentState == BuildState.MainState || editModel.TargetCubeModel == null)
		{
			VoxelHit hit = default;
			if (MVGameController.Instance.WOCM.Pick(ref hit))
			{
				if (hit.isCubeHit)
				{
					editModel.TargetCubeModel = (MVCubeModelBase)MVGameController.Instance.WOCM.GetWorldObjectClient(hit.woId);
				}
			}
			else
			{
				editModel.TargetCubeModel = MVGameController.Instance.WOCM.Terrain;
			}
		}
		editModel.ExecuteCubeEditing(e);
	}

	public override void Exit(EditorStateMachine e)
	{
		editModel.Destroy();
	}
}
