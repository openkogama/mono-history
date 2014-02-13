using UnityEngine;

internal class DeleteCubes : CubeModelTool
{
	private CubePickingInfo cubeNotToBeDeleted;

	private DeleteCursor deleteCursor;

	public override void Enter(CubeModelingStateMachine e)
	{
		Debug.Log((object)GetType().ToString());
		deleteCursor = new DeleteCursor();
		MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.DeletingCubes);
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
		bool deletedCube = false;
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			cubeNotToBeDeleted = null;
		}
		if (e.SelectedCube != null)
		{
			if (MVInputWrapper.GetKey((KeyCode)323) && e.CanRemoveCubeAt(e.SelectedCube.iLocalPos) && (cubeNotToBeDeleted == null || e.SelectedCube.iLocalPos != cubeNotToBeDeleted.iLocalPos))
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
		MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		Debug.Log((object)"Exit");
		HideCursor();
	}

	public override void HideCursor()
	{
		deleteCursor.Remove();
	}
}
