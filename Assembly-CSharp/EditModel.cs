using MV.WorldObject;
using UnityEngine;

internal class EditModel
{
	private BuildState currentState = BuildState.MainState;

	private float delta;

	private float deltaAccum;

	private CubePickingInfo selectedCube;

	private CubePickingInfo prevSelectedCube;

	private CubePickingInfo cubeNotToBeDeleted;

	private CubePickingInfo movingEdgeCube;

	private float detailEditModeMaxDistance = 30f;

	private ModelCursor modelCursor;

	private float prevMouseUpTime;

	private float mouseUpTimeBeforeMoveEdge = 0.3f;

	private byte prevMaterial;

	private bool edgeHasMoved;

	private float mouseSensitivity;

	private float mouseSensitivityExtrude = 20.2f;

	private MVCubeModelBase targetCubeModel;

	public BuildState CurrentState => currentState;

	public MVCubeModelBase TargetCubeModel
	{
		get
		{
			return targetCubeModel;
		}
		set
		{
			targetCubeModel = value;
		}
	}

	public EditModel()
	{
		delta = 0f;
		deltaAccum = 0f;
		modelCursor = new ModelCursor();
	}

	public void ExecuteCubeEditing(EditorStateMachine e)
	{
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			prevMouseUpTime = Time.time;
			cubeNotToBeDeleted = null;
		}
		if (targetCubeModel == null)
		{
			Debug.Log((object)"No set");
			return;
		}
		prevSelectedCube = selectedCube;
		selectedCube = DoPicking();
		SetEditDetail(e);
		switch (currentState)
		{
		case BuildState.Idle:
			if (currentState == BuildState.Idle && MVInputWrapper.GetKeyUp((KeyCode)323))
			{
				Screen.showCursor = true;
				currentState = BuildState.MainState;
			}
			break;
		case BuildState.MainState:
			if (MVInputWrapper.GetKey((KeyCode)304) && MVInputWrapper.GetKey((KeyCode)323))
			{
				currentState = BuildState.SetMaterial;
				break;
			}
			if (selectedCube != null)
			{
				if (MVInputWrapper.GetKey((KeyCode)323) && MVInputWrapper.GetKey((KeyCode)306))
				{
					if (cubeNotToBeDeleted == null || selectedCube.iLocalPos != cubeNotToBeDeleted.iLocalPos)
					{
						HandleAudio(e, selectedCube.iLocalPos, AudioActions.CubeRemoved);
						targetCubeModel.RemoveCube(selectedCube.iLocalPos);
						selectedCube = null;
						cubeNotToBeDeleted = DoPicking();
					}
					break;
				}
				if (MVInputWrapper.GetKeyUp((KeyCode)323) && !MVInputWrapper.GetKey((KeyCode)306))
				{
					AddCube(e);
					break;
				}
			}
			else
			{
				cubeNotToBeDeleted = null;
				if (MVInputWrapper.GetKeyDown((KeyCode)323) && !MVInputWrapper.GetKey((KeyCode)306))
				{
					currentState = BuildState.PaintCubes;
					break;
				}
			}
			if (MVInputWrapper.GetKey((KeyCode)323) && Time.time - prevMouseUpTime > mouseUpTimeBeforeMoveEdge && GotoMultiChangeCubes() && !MVInputWrapper.GetKey((KeyCode)306))
			{
				prevMaterial = e.CurrentMaterialId;
				byte material = CubeBase.GetMaterial(prevSelectedCube.cube, prevSelectedCube.pickedFace);
				e.CurrentMaterialId = material;
				movingEdgeCube = prevSelectedCube;
				currentState = BuildState.MultiChangeCubes;
				deltaAccum = 0f;
				Screen.showCursor = false;
			}
			break;
		case BuildState.PaintCubes:
			if (!MVInputWrapper.GetKeyUp((KeyCode)323))
			{
				IntVector cubePosOnDrawplane = MVGameController.Instance.EditorController.WorldEditorDrawPlane.GetCubePosOnDrawplane(targetCubeModel.GameObject);
				IsWithinDynamicConstraint(e, cubePosOnDrawplane);
				if (IsWithinDynamicConstraint(e, cubePosOnDrawplane))
				{
					HandleAudio(e, cubePosOnDrawplane, AudioActions.CubeAdded);
					targetCubeModel.AddCube(cubePosOnDrawplane, new Cube(CubeDataPacker.CornersToByteArray(CubeBase.IdentityCorners), Cube.CreateMaterialArray(0)));
				}
			}
			else
			{
				currentState = BuildState.MainState;
			}
			break;
		case BuildState.SetMaterial:
			if (selectedCube != null)
			{
				HandleAudio(e, selectedCube.iLocalPos, AudioActions.CubeAdded);
				targetCubeModel.ReplaceCube(selectedCube.iLocalPos, e.CurrentMaterialId);
			}
			if (!MVInputWrapper.GetKey((KeyCode)304) || !MVInputWrapper.GetKey((KeyCode)323))
			{
				currentState = BuildState.MainState;
			}
			break;
		case BuildState.MultiChangeCubes:
		{
			if (movingEdgeCube == null)
			{
				currentState = BuildState.Idle;
				e.CurrentMaterialId = prevMaterial;
				edgeHasMoved = false;
				break;
			}
			if (MVInputWrapper.GetKeyUp((KeyCode)323))
			{
				currentState = BuildState.MainState;
				e.CurrentMaterialId = prevMaterial;
				Screen.showCursor = true;
				if (edgeHasMoved)
				{
					if (Cube.IsCollapsed(movingEdgeCube.cube.Corners))
					{
						HandleAudio(e, movingEdgeCube.iLocalPos, AudioActions.CubeRemoved);
						targetCubeModel.RemoveCube(movingEdgeCube.iLocalPos);
					}
					else
					{
						HandleAudio(e, movingEdgeCube.iLocalPos, AudioActions.EdgeMoved);
						targetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					}
				}
				else
				{
					selectedCube = movingEdgeCube;
					HandleAudio(e, selectedCube.iLocalPos, AudioActions.CubeAdded);
					AddCube(e);
				}
				movingEdgeCube = null;
				edgeHasMoved = false;
				break;
			}
			Vector3 mousePositionDelta = mouseSensitivityExtrude * new Vector3(MVInputWrapper.GetAxisRaw("Mouse X"), MVInputWrapper.GetAxisRaw("Mouse Y"), 0f);
			bool edgeMoved = false;
			CubeOutOfBoundState cubeOutOfBoundState = SharedCubeFunctions.MoveEdge(targetCubeModel, movingEdgeCube, mousePositionDelta, ref delta, ref deltaAccum, mouseSensitivity, ref edgeMoved, movingEdgeCube.pickedEdgeIndex0, movingEdgeCube.pickedEdgeIndex1);
			if (!edgeHasMoved && edgeMoved)
			{
				edgeHasMoved = true;
			}
			switch (cubeOutOfBoundState)
			{
			case CubeOutOfBoundState.OutOfBoundsAdd:
			{
				IntVector cubePos2 = GetCubePos(movingEdgeCube.iLocalPos, movingEdgeCube.pickedFace);
				if (IsWithinDynamicConstraint(e, cubePos2) && targetCubeModel.GetCube(cubePos2) == null)
				{
					HandleAudio(e, cubePos2, AudioActions.FaceMoved);
					targetCubeModel.AddCube(cubePos2, new Cube(CubeDataPacker.CornersToByteArray(Cube.GetCorners(movingEdgeCube.cube, movingEdgeCube.pickedFace)), Cube.CreateMaterialArray(e.CurrentMaterialId)));
					CubePickingInfo cubePickingInfo3 = new CubePickingInfo(movingEdgeCube);
					cubePickingInfo3.cube = Cube.Clone(targetCubeModel.GetCube(cubePos2));
					cubePickingInfo3.iLocalPos = cubePos2;
					targetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = cubePickingInfo3;
					CubeOutOfBoundState outOfBoundState2 = CubeOutOfBoundState.WithinBounds;
					Cube.MoveFace(movingEdgeCube, -0.75f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), ref outOfBoundState2);
					targetCubeModel.CornersChanged(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
				}
				else
				{
					modelCursor.SetErrorCursor(cubePos2, targetCubeModel.GameObject);
					targetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = null;
				}
				break;
			}
			case CubeOutOfBoundState.OutOfBoundsAddEdge:
			{
				IntVector cubePos = GetCubePos(movingEdgeCube.iLocalPos, movingEdgeCube.pickedFace);
				if (IsWithinDynamicConstraint(e, cubePos) && targetCubeModel.GetCube(cubePos) == null)
				{
					HandleAudio(e, movingEdgeCube.iLocalPos, AudioActions.CubeAdded);
					targetCubeModel.AddCube(cubePos, new Cube(CubeDataPacker.CornersToByteArray(Cube.GetCorners(movingEdgeCube.cube, movingEdgeCube.pickedFace)), Cube.CreateMaterialArray(e.CurrentMaterialId)));
					CubePickingInfo cubePickingInfo2 = new CubePickingInfo(movingEdgeCube);
					cubePickingInfo2.cube = Cube.Clone(targetCubeModel.GetCube(cubePos));
					cubePickingInfo2.iLocalPos = cubePos;
					targetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = cubePickingInfo2;
					CubeOutOfBoundState outOfBoundState = CubeOutOfBoundState.WithinBounds;
					Cube.MoveFace(movingEdgeCube, -1f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), ref outOfBoundState);
					Cube.MoveEdge(movingEdgeCube, -0.75f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), ref outOfBoundState);
					targetCubeModel.CornersChanged(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
				}
				else
				{
					modelCursor.SetErrorCursor(cubePos, targetCubeModel.GameObject);
					Debug.Log((object)"Failed to add cube");
					targetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = null;
				}
				break;
			}
			case CubeOutOfBoundState.OutOfBoundsRemove:
			{
				IntVector cubePosNeighborOppositeFace = GetCubePosNeighborOppositeFace(movingEdgeCube.iLocalPos, movingEdgeCube.pickedFace);
				HandleAudio(e, movingEdgeCube.iLocalPos, AudioActions.CubeRemoved);
				targetCubeModel.RemoveCube(movingEdgeCube.iLocalPos);
				Cube cube = targetCubeModel.GetCube(cubePosNeighborOppositeFace);
				if (cube != null)
				{
					CubePickingInfo cubePickingInfo = new CubePickingInfo(movingEdgeCube);
					cubePickingInfo.cube = Cube.Clone(cube);
					cubePickingInfo.iLocalPos = cubePosNeighborOppositeFace;
					movingEdgeCube = cubePickingInfo;
				}
				else
				{
					movingEdgeCube = null;
				}
				break;
			}
			case CubeOutOfBoundState.WithinBounds:
				HandleAudio(e, movingEdgeCube.iLocalPos, AudioActions.FaceMoved);
				targetCubeModel.CornersChanged(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
				break;
			}
			break;
		}
		}
		HandleCursor();
		targetCubeModel.HandleDelta();
	}

	private CubePickingInfo DoPicking()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		CubePickingInfo info = new CubePickingInfo();
		bool updateIndentArea = true;
		if (movingEdgeCube != null)
		{
			updateIndentArea = false;
		}
		if (SharedCubeFunctions.GetPickingInfo(targetCubeModel, ref info, modelCursor.IndentArea, updateIndentArea))
		{
			Vector3 hit = Vector3.zero;
			if (MVGameController.Instance.EditorController.WorldEditorDrawPlane.Pick(ref hit))
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

	public void Destroy()
	{
		Screen.showCursor = true;
		modelCursor.Destroy();
	}

	private void HandleAudio(EditorStateMachine e, IntVector pos, AudioActions action)
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

	private bool IsWithinDynamicConstraint(EditorStateMachine e, IntVector requestedCubePos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		Bounds meshBounds = targetCubeModel.GetMeshBounds();
		if ((targetCubeModel.InteractionFlags & InteractionFlags.IsTerrain) != 0)
		{
			return true;
		}
		Vector3 val = MathFunctions.AbsVector(new Vector3((float)requestedCubePos.x, (float)requestedCubePos.y, (float)requestedCubePos.z));
		if (val.x > (float)(SharedCubeFunctions.CubeConstraint.x - 1) || val.y > (float)(SharedCubeFunctions.CubeConstraint.y - 1) || val.z > (float)(SharedCubeFunctions.CubeConstraint.z - 1))
		{
			return false;
		}
		Vector3 val2 = new Vector3((float)requestedCubePos.x - 0.5f, (float)requestedCubePos.y - 0.5f, (float)requestedCubePos.z - 0.5f);
		Vector3 val3 = new Vector3((float)requestedCubePos.x + 0.5f, (float)requestedCubePos.y + 0.5f, (float)requestedCubePos.z + 0.5f);
		for (int i = 0; i < 3; i++)
		{
			float num = val2[i];
			Vector3 min = meshBounds.min;
			if (num < min[i])
			{
				Vector3 min2 = meshBounds.min;
				min2[i] = val2[i];
				meshBounds.SetMinMax(min2, meshBounds.max);
			}
			float num2 = val3[i];
			Vector3 max = meshBounds.max;
			if (num2 > max[i])
			{
				Vector3 max2 = meshBounds.max;
				max2[i] = val3[i];
				meshBounds.SetMinMax(meshBounds.min, max2);
			}
		}
		IntVector min3 = default;
		IntVector max3 = default;
		SharedCollisionFunctions.GetVoxelBounds(ref min3, ref max3, meshBounds);
		IntVector intVector = max3 - min3 + new IntVector(1, 1, 1);
		if (intVector.x <= SharedCubeFunctions.CubeConstraint.x && intVector.y <= SharedCubeFunctions.CubeConstraint.y && intVector.z <= SharedCubeFunctions.CubeConstraint.z)
		{
			return true;
		}
		return false;
	}

	private void HandleCursor()
	{
		modelCursor.UpdateCursor(movingEdgeCube, selectedCube, targetCubeModel.GameObject);
	}

	private bool GotoMultiChangeCubes()
	{
		if (prevSelectedCube != null)
		{
			float num = Mathf.Abs(MVInputWrapper.GetAxis("Mouse X"));
			float num2 = Mathf.Abs(MVInputWrapper.GetAxis("Mouse Y"));
			if (num != 0f || num2 != 0f)
			{
				return true;
			}
		}
		return false;
	}

	private void AddCube(EditorStateMachine e)
	{
		if (Cube.IsFaceBoxSideAligened(selectedCube.cube, selectedCube.pickedFace))
		{
			IntVector cubePos = GetCubePos(selectedCube.iLocalPos, selectedCube.pickedFace);
			if (IsWithinDynamicConstraint(e, cubePos) && targetCubeModel.GetCube(cubePos) == null)
			{
				HandleAudio(e, cubePos, AudioActions.CubeAdded);
				targetCubeModel.AddCube(cubePos, new Cube(CubeDataPacker.CornersToByteArray(Cube.GetCorners(selectedCube.cube, selectedCube.pickedFace)), Cube.CreateMaterialArray(e.CurrentMaterialId)));
			}
			else
			{
				modelCursor.SetErrorCursor(cubePos, targetCubeModel.GameObject);
			}
		}
		else
		{
			HandleAudio(e, selectedCube.iLocalPos, AudioActions.FaceMoved);
			targetCubeModel.UnIndentCubeFace(selectedCube.iLocalPos, selectedCube.pickedFace, selectedCube.cube);
		}
	}

	private void SetEditDetail(EditorStateMachine e)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (selectedCube != null && movingEdgeCube == null)
		{
			Vector3 val = SharedCubeFunctions.LocalToWorld(targetCubeModel.GameObject, selectedCube.iLocalPos);
			Vector3 val2 = ((Component)e.WeCamera).transform.position - val;
			if (val2.magnitude > detailEditModeMaxDistance * targetCubeModel.Scale.y)
			{
				mouseSensitivity = 1.325f;
				modelCursor.SetIndentAreaSize(1f);
			}
			else
			{
				mouseSensitivity = 0.225f;
				modelCursor.SetIndentAreaSize(0.5f);
			}
		}
	}

	private IntVector GetCubePos(IntVector localPos, Face face)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		IntVector result = new IntVector(localPos.x, localPos.y, localPos.z);
		Vector3 faceAxis = Cube.GetFaceAxis(face);
		result.x += (short)faceAxis.x;
		result.y += (short)faceAxis.y;
		result.z += (short)faceAxis.z;
		return result;
	}

	private IntVector GetCubePosNeighborOppositeFace(IntVector localPos, Face face)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		IntVector result = new IntVector(localPos.x, localPos.y, localPos.z);
		Vector3 faceAxis = Cube.GetFaceAxis(face);
		result.x -= (short)faceAxis.x;
		result.y -= (short)faceAxis.y;
		result.z -= (short)faceAxis.z;
		return result;
	}
}
