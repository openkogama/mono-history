using MV.WorldObject;
using UnityEngine;

public class CubeModelingStateMachine : FSMEntity
{
	public delegate void OnCurrentMaterialChangeDelegate(byte currentMaterialId, Material currentMaterial);

	private byte currentMaterialId;

	private Material currentMaterial;

	public OnCurrentMaterialChangeDelegate OnCurrentMaterialChange;

	public bool useLasers = true;

	private MVCubeModelBase targetCubeModel;

	private IModelingConstraint constraint;

	public byte CurrentMaterialId
	{
		get
		{
			return currentMaterialId;
		}
		set
		{
			currentMaterialId = value;
			if (OnCurrentMaterialChange != null)
			{
				OnCurrentMaterialChange(currentMaterialId, CurrentMaterial);
			}
		}
	}

	public Material CurrentMaterial => MVGameController.Instance.Game.MaterialRepository.GetMaterial(currentMaterialId).material;

	public CubePickingInfo SelectedCube { get; set; }

	public MVCubeModelBase TargetCubeModel => targetCubeModel;

	public bool CursorVisible
	{
		get
		{
			return ((CubeModelTool)currentState).CursorVisible;
		}
		set
		{
			((CubeModelTool)currentState).CursorVisible = value;
		}
	}

	public CubeModelingStateMachine()
	{
		transitionTable = new CubeModelingTransitionTable();
		Event = CubeModelingEvent.EditCubes;
	}

	public void StartEdit(MVCubeModelBase targetCubeModel, IModelingConstraint constraint = null)
	{
		if (this.targetCubeModel != null)
		{
			this.targetCubeModel.BeingEdited = false;
		}
		this.targetCubeModel = targetCubeModel;
		this.constraint = constraint;
		this.targetCubeModel.BeingEdited = true;
		Event = curEvent;
	}

	public void EndEdit()
	{
		targetCubeModel.BeingEdited = false;
		targetCubeModel = null;
	}

	public override void Update()
	{
		if (targetCubeModel == null)
		{
			Debug.Log((object)"Not set");
			return;
		}
		SelectedCube = DoPicking();
		base.Update();
		targetCubeModel.HandleDelta();
	}

	public CubePickingInfo DoPicking()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		CubePickingInfo info = new CubePickingInfo();
		if (SharedCubeFunctions.GetPickingInfo(targetCubeModel, ref info))
		{
			Vector3 hit = Vector3.zero;
			if (MVGameController.Instance.EditController.WorldEditorDrawPlane.Pick(ref hit))
			{
				Vector3 val = hit - ((Component)Camera.main).transform.position;
				float magnitude = val.magnitude;
				Vector3 val2 = info.point - ((Component)Camera.main).transform.position;
				float magnitude2 = val2.magnitude;
				if (magnitude - 0.01f < magnitude2)
				{
					return null;
				}
			}
			return info;
		}
		return null;
	}

	public void RemoveCursors()
	{
		Screen.showCursor = true;
		((CubeModelTool)currentState).HideCursor();
	}

	public void HandleAudio(IntVector pos, AudioActions action)
	{
		switch (action)
		{
		case AudioActions.CubeAdded:
			if (targetCubeModel.GetCube(pos) == null)
			{
				AudioEventHandler.PlaySound(action, pos, targetCubeModel.GameObject);
			}
			break;
		case AudioActions.FaceMoved:
			AudioEventHandler.PlaySound(action, pos, targetCubeModel.GameObject);
			break;
		case AudioActions.CubeRemoved:
			if (targetCubeModel.GetCube(pos) != null)
			{
				AudioEventHandler.PlaySound(action, pos, targetCubeModel.GameObject);
			}
			break;
		}
	}

	public bool AddCube()
	{
		if (Cube.IsFaceBoxSideAligened(SelectedCube.cube, SelectedCube.pickedFace))
		{
			IntVector cubePosAboveFace = Cube.GetCubePosAboveFace(SelectedCube.iLocalPos, SelectedCube.pickedFace);
			if (CanAddCubeAt(cubePosAboveFace) && TargetCubeModel.GetCube(cubePosAboveFace) == null)
			{
				HandleAudio(cubePosAboveFace, AudioActions.CubeAdded);
				TargetCubeModel.AddCube(cubePosAboveFace, new Cube(CubeDataPacker.CornersToByteArray(Cube.GetCorners(SelectedCube.cube, SelectedCube.pickedFace)), Cube.CreateMaterialArray(CurrentMaterialId)));
				return true;
			}
			return false;
		}
		HandleAudio(SelectedCube.iLocalPos, AudioActions.FaceMoved);
		TargetCubeModel.UnIndentCubeFace(SelectedCube.iLocalPos, SelectedCube.pickedFace, SelectedCube.cube);
		return true;
	}

	public bool CanAddCubeAt(IntVector requestedCubePos)
	{
		if ((targetCubeModel.InteractionFlags & InteractionFlags.IsTerrain) != 0)
		{
			return true;
		}
		if (constraint == null)
		{
			return true;
		}
		bool flag = constraint.CanAddCubeAt(requestedCubePos);
		if (!flag)
		{
			Debug.Log((object)string.Concat("Pos ", requestedCubePos, " not within constraint "));
		}
		return flag;
	}

	public bool CanRemoveCubeAt(IntVector requestedCubePos)
	{
		if ((targetCubeModel.InteractionFlags & InteractionFlags.IsTerrain) != 0)
		{
			return true;
		}
		if (constraint == null)
		{
			return true;
		}
		return constraint.CanRemoveCubeAt(requestedCubePos);
	}

	public bool CanEditCubeAt(IntVector requestedCubePos)
	{
		if ((targetCubeModel.InteractionFlags & InteractionFlags.IsTerrain) != 0)
		{
			return true;
		}
		if (constraint == null)
		{
			return true;
		}
		return constraint.CanEditCubeAt(requestedCubePos);
	}
}
