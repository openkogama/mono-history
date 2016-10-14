internal class DeleteCubes : CubeModelTool
{
	private CubePickingInfo cubeNotToBeDeleted;

	private DeleteCursor deleteCursor;

	public override void Enter(CubeModelingStateMachine e)
	{
		deleteCursor = new DeleteCursor(e.CubeCorners);
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.DeletingCubes);
		waitForMouseUp = MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect);
	}

	public override void Execute(CubeModelingStateMachine e)
	{
		base.Execute(e);
		if (waitForMouseUp)
		{
			waitForMouseUp = MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect);
			return;
		}
		bool deletedCube = false;
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			cubeNotToBeDeleted = null;
		}
		if (e.SelectedCube != null)
		{
			if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect) && e.CanRemoveCubeAt(e.SelectedCube.iLocalPos) && (cubeNotToBeDeleted == null || e.SelectedCube.iLocalPos != cubeNotToBeDeleted.iLocalPos))
			{
				e.HandleAudio(e.SelectedCube.iLocalPos, AudioActions.CubeRemoved);
				e.TargetCubeModel.RemoveCube(e.SelectedCube.iLocalPos);
				deletedCube = true;
				cubeNotToBeDeleted = e.DoPicking();
			}
		}
		else
		{
			cubeNotToBeDeleted = null;
		}
		deleteCursor.UpdateCursor(e.SelectedCube, e.TargetCubeModel, deletedCube);
	}

	public override void Exit(CubeModelingStateMachine e)
	{
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		HideCursor();
	}

	public override void HideCursor()
	{
		deleteCursor.Remove();
	}
}
