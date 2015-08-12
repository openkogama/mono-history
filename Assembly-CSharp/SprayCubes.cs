using MV.WorldObject;
using UnityEngine;

internal class SprayCubes : CubeModelTool
{
	private SprayCursor sprayCursor;

	private CubePickingInfo cubeNotToBeSprayed;

	private WorldEditorDrawPlane DrawPlane => MVGameController.EditController.WorldEditorDrawPlane;

	public override void Enter(CubeModelingStateMachine e)
	{
		sprayCursor = new SprayCursor();
		MVGameController.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.SprayCubes);
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
		bool addCube = false;
		if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect))
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
		MVGameController.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
	}

	public override void HideCursor()
	{
		sprayCursor.Remove();
	}
}
