using MV.WorldObject;
using UnityEngine;

internal class SprayCubes : CubeModelTool
{
	private SprayCursor sprayCursor;

	private CubePickingInfo cubeNotToBeSprayed;

	private WorldEditorDrawPlane DrawPlane => MVGameController.Instance.EditController.WorldEditorDrawPlane;

	public override void Enter(CubeModelingStateMachine e)
	{
		sprayCursor = new SprayCursor();
		MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.SprayCubes);
		waitForMouseUp = MVInputWrapper.GetKey((KeyCode)323);
	}

	public override void Execute(CubeModelingStateMachine e)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if (waitForMouseUp)
		{
			waitForMouseUp = MVInputWrapper.GetKey((KeyCode)323);
			return;
		}
		bool addCube = false;
		if (MVInputWrapper.GetKey((KeyCode)323))
		{
			if (e.SelectedCube != null)
			{
				if (cubeNotToBeSprayed == null || e.SelectedCube.iLocalPos != cubeNotToBeSprayed.iLocalPos)
				{
					e.HandleAudio(e.SelectedCube.iLocalPos, AudioActions.CubeAdded);
					addCube = e.AddCube();
					cubeNotToBeSprayed = e.DoPicking();
				}
			}
			else
			{
				Vector3 hit = default;
				if (DrawPlane.Active && DrawPlane.Pick(ref hit) && DrawPlane.GetCubePosOnDrawplane(e.TargetCubeModel.GameObject, out var intVectorHitPos) && e.CanAddCubeAt(intVectorHitPos))
				{
					e.HandleAudio(intVectorHitPos, AudioActions.CubeAdded);
					e.TargetCubeModel.AddCube(intVectorHitPos, new Cube(CubeDataPacker.CornersToByteArray(CubeBase.IdentityCorners), Cube.CreateMaterialArray(e.CurrentMaterialId)));
					addCube = true;
				}
				cubeNotToBeSprayed = null;
			}
		}
		else
		{
			cubeNotToBeSprayed = null;
		}
		sprayCursor.UpdateCursor(e.SelectedCube, e.TargetCubeModel, addCube);
	}

	public override void Exit(CubeModelingStateMachine e)
	{
		HideCursor();
		MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
	}

	public override void HideCursor()
	{
		sprayCursor.Remove();
	}
}
