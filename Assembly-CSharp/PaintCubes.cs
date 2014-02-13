using UnityEngine;

internal class PaintCubes : CubeModelTool
{
	private PaintCursor paintCursor;

	public override void Enter(CubeModelingStateMachine e)
	{
		Debug.Log((object)GetType().ToString());
		paintCursor = new PaintCursor();
		MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.PaintCubes);
		waitForMouseUp = MVInputWrapper.GetKey((KeyCode)323);
	}

	public override void Execute(CubeModelingStateMachine e)
	{
		base.Execute(e);
		if (waitForMouseUp)
		{
			waitForMouseUp = MVInputWrapper.GetKey((KeyCode)323);
			return;
		}
		bool isPainting = false;
		if (MVInputWrapper.GetKey((KeyCode)323) && e.SelectedCube != null)
		{
			if (e.SelectedCube.cube.FaceMaterials[0] != e.CurrentMaterialId)
			{
				e.HandleAudio(e.SelectedCube.iLocalPos, AudioActions.CubeAdded);
				e.TargetCubeModel.ReplaceCube(e.SelectedCube.iLocalPos, e.CurrentMaterialId);
			}
			isPainting = true;
		}
		paintCursor.UpdateCursor(e.SelectedCube, e.TargetCubeModel, isPainting);
	}

	public override void Exit(CubeModelingStateMachine e)
	{
		MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		HideCursor();
	}

	public override void HideCursor()
	{
		paintCursor.Remove();
	}
}
