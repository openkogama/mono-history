internal class PaintCubes : CubeModelTool
{
	private PaintCursor paintCursor;

	public override void Enter(CubeModelingStateMachine e)
	{
		paintCursor = new PaintCursor(e.CubeCorners);
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.PaintCubes);
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
		bool isPainting = false;
		if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect) && e.SelectedCube != null)
		{
			if (e.SelectedCube.cube.FaceMaterials[0] != e.CurrentMaterialId)
			{
				e.HandleAudio(e.SelectedCube.iLocalPos, AudioActions.CubeAdded);
				e.TargetCubeModel.ReplaceCube(e.SelectedCube.iLocalPos, e.CurrentMaterialId);
				CubeModelTool.SendCubeEvent(e.TargetCubeModel.CubeCount, EditCubeChange.CubePainted);
			}
			isPainting = true;
		}
		paintCursor.UpdateCursor(e.SelectedCube, e.TargetCubeModel, isPainting);
	}

	public override void Exit(CubeModelingStateMachine e)
	{
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		HideCursor();
	}

	public override void HideCursor()
	{
		paintCursor.Remove();
	}
}
