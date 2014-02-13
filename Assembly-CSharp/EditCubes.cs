using MV.WorldObject;
using UnityEngine;

internal class EditCubes : CubeModelTool
{
	private float delta;

	private float deltaAccum;

	private byte prevMaterial;

	private bool edgeHasMoved;

	private float mouseSensitivity;

	private float mouseSensitivityExtrude = 20.2f;

	private float mouseUpTimeBeforeMoveEdge = 0.3f;

	private float detailEditModeMaxDistance = 30f;

	private float prevMouseUpTime;

	private BuildState currentInternalState = BuildState.MainState;

	private CubePickingInfo prevSelectedCube;

	private CubePickingInfo movingEdgeCube;

	private Cube prevCubeState;

	private ModelCursor modelCursor;

	private WorldEditorDrawPlane DrawPlane => MVGameController.Instance.EditController.WorldEditorDrawPlane;

	public override bool CursorVisible
	{
		get
		{
			return modelCursor.CursorVisible;
		}
		set
		{
			modelCursor.CursorVisible = value;
		}
	}

	public override void Enter(CubeModelingStateMachine e)
	{
		delta = 0f;
		deltaAccum = 0f;
		modelCursor = new ModelCursor();
		MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.EditingCube);
		waitForMouseUp = MVInputWrapper.GetKey((KeyCode)323);
	}

	public override void Execute(CubeModelingStateMachine e)
	{
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0953: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if (waitForMouseUp)
		{
			waitForMouseUp = MVInputWrapper.GetKey((KeyCode)323);
			return;
		}
		bool flag = true;
		if (movingEdgeCube != null)
		{
			flag = false;
		}
		if (modelCursor.IndentArea != null && flag && e.SelectedCube != null)
		{
			modelCursor.IndentArea.UpdateIndentArea(e.SelectedCube, e.TargetCubeModel.GameObject);
			if (modelCursor.IndentArea.IsColliding())
			{
				e.SelectedCube.pickedEdge = Edge.None;
				modelCursor.IndentArea.GameObject.active = true;
			}
			else
			{
				modelCursor.IndentArea.GameObject.active = false;
			}
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			prevMouseUpTime = Time.time;
		}
		SetEditDetail(e);
		bool addCube = false;
		switch (currentInternalState)
		{
		case BuildState.Idle:
			if (currentInternalState == BuildState.Idle && MVInputWrapper.GetKeyUp((KeyCode)323))
			{
				Screen.showCursor = true;
				currentInternalState = BuildState.MainState;
			}
			break;
		case BuildState.MainState:
			if (e.SelectedCube != null)
			{
				if (MVInputWrapper.GetKeyUp((KeyCode)323))
				{
					if (!e.AddCube())
					{
						IntVector cubePosAboveFace5 = Cube.GetCubePosAboveFace(e.SelectedCube.iLocalPos, e.SelectedCube.pickedFace);
						modelCursor.SetErrorCursor(cubePosAboveFace5, e.TargetCubeModel.GameObject);
					}
					else
					{
						addCube = true;
					}
					break;
				}
			}
			else if (MVInputWrapper.GetKeyDown((KeyCode)323) && DrawPlane.Active)
			{
				currentInternalState = BuildState.PaintCubes;
				break;
			}
			if (MVInputWrapper.GetKey((KeyCode)323) && Time.time - prevMouseUpTime > mouseUpTimeBeforeMoveEdge && GotoMultiChangeCubes(e))
			{
				prevMaterial = e.CurrentMaterialId;
				byte material = CubeBase.GetMaterial(prevSelectedCube.cube, prevSelectedCube.pickedFace);
				e.CurrentMaterialId = material;
				movingEdgeCube = prevSelectedCube;
				currentInternalState = BuildState.MultiChangeCubes;
				deltaAccum = 0f;
				Screen.showCursor = false;
			}
			break;
		case BuildState.PaintCubes:
			if (!MVInputWrapper.GetKeyUp((KeyCode)323))
			{
				if (DrawPlane.GetCubePosOnDrawplane(e.TargetCubeModel.GameObject, out var intVectorHitPos) && e.CanAddCubeAt(intVectorHitPos))
				{
					e.HandleAudio(intVectorHitPos, AudioActions.CubeAdded);
					e.TargetCubeModel.AddCube(intVectorHitPos, new Cube(CubeDataPacker.CornersToByteArray(CubeBase.IdentityCorners), Cube.CreateMaterialArray(e.CurrentMaterialId)));
				}
			}
			else
			{
				currentInternalState = BuildState.MainState;
			}
			break;
		case BuildState.MultiChangeCubes:
		{
			if (movingEdgeCube == null)
			{
				currentInternalState = BuildState.Idle;
				e.CurrentMaterialId = prevMaterial;
				edgeHasMoved = false;
				break;
			}
			if (MVInputWrapper.GetKeyUp((KeyCode)323))
			{
				currentInternalState = BuildState.MainState;
				e.CurrentMaterialId = prevMaterial;
				Screen.showCursor = true;
				if (edgeHasMoved)
				{
					if (Cube.IsCollapsed(movingEdgeCube.cube.Corners))
					{
						if (e.CanRemoveCubeAt(movingEdgeCube.iLocalPos))
						{
							e.HandleAudio(movingEdgeCube.iLocalPos, AudioActions.CubeRemoved);
							e.TargetCubeModel.RemoveCube(movingEdgeCube.iLocalPos);
						}
						else
						{
							modelCursor.SetErrorCursor(movingEdgeCube.iLocalPos, e.TargetCubeModel.GameObject);
							e.HandleAudio(movingEdgeCube.iLocalPos, AudioActions.FaceMoved);
							e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, prevCubeState);
						}
					}
					else
					{
						e.HandleAudio(movingEdgeCube.iLocalPos, AudioActions.EdgeMoved);
						e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					}
				}
				else
				{
					e.SelectedCube = movingEdgeCube;
					e.HandleAudio(e.SelectedCube.iLocalPos, AudioActions.CubeAdded);
					if (!e.AddCube())
					{
						IntVector cubePosAboveFace = Cube.GetCubePosAboveFace(e.SelectedCube.iLocalPos, e.SelectedCube.pickedFace);
						modelCursor.SetErrorCursor(cubePosAboveFace, e.TargetCubeModel.GameObject);
					}
				}
				movingEdgeCube = null;
				edgeHasMoved = false;
				break;
			}
			Vector3 mousePositionDelta = mouseSensitivityExtrude * new Vector3(MVInputWrapper.GetAxisRaw("Mouse X"), MVInputWrapper.GetAxisRaw("Mouse Y"), 0f);
			bool edgeMoved = false;
			CubeOutOfBoundState cubeOutOfBoundState = SharedCubeFunctions.MoveEdge(e.TargetCubeModel, movingEdgeCube, mousePositionDelta, ref delta, ref deltaAccum, mouseSensitivity, ref edgeMoved, movingEdgeCube.pickedEdgeIndex0, movingEdgeCube.pickedEdgeIndex1);
			if (!edgeHasMoved && edgeMoved)
			{
				edgeHasMoved = true;
			}
			switch (cubeOutOfBoundState)
			{
			case CubeOutOfBoundState.OutOfBoundsAdd:
			{
				IntVector cubePosAboveFace3 = Cube.GetCubePosAboveFace(movingEdgeCube.iLocalPos, movingEdgeCube.pickedFace);
				if (e.CanAddCubeAt(cubePosAboveFace3) && e.TargetCubeModel.GetCube(cubePosAboveFace3) == null)
				{
					e.HandleAudio(cubePosAboveFace3, AudioActions.FaceMoved);
					e.TargetCubeModel.AddCube(cubePosAboveFace3, new Cube(CubeDataPacker.CornersToByteArray(Cube.GetCorners(movingEdgeCube.cube, movingEdgeCube.pickedFace)), Cube.CreateMaterialArray(e.CurrentMaterialId)));
					CubePickingInfo cubePickingInfo2 = new CubePickingInfo(movingEdgeCube);
					cubePickingInfo2.cube = Cube.Clone(e.TargetCubeModel.GetCube(cubePosAboveFace3));
					cubePickingInfo2.iLocalPos = cubePosAboveFace3;
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = cubePickingInfo2;
					CubeOutOfBoundState outOfBoundState2 = CubeOutOfBoundState.WithinBounds;
					Cube.MoveFace(movingEdgeCube, -0.75f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), ref outOfBoundState2);
					e.TargetCubeModel.CornersChanged(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
				}
				else
				{
					modelCursor.SetErrorCursor(cubePosAboveFace3, e.TargetCubeModel.GameObject);
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = null;
				}
				break;
			}
			case CubeOutOfBoundState.OutOfBoundsAddEdge:
			{
				IntVector cubePosAboveFace2 = Cube.GetCubePosAboveFace(movingEdgeCube.iLocalPos, movingEdgeCube.pickedFace);
				if (e.CanAddCubeAt(cubePosAboveFace2) && e.TargetCubeModel.GetCube(cubePosAboveFace2) == null)
				{
					e.HandleAudio(movingEdgeCube.iLocalPos, AudioActions.CubeAdded);
					e.TargetCubeModel.AddCube(cubePosAboveFace2, new Cube(CubeDataPacker.CornersToByteArray(Cube.GetCorners(movingEdgeCube.cube, movingEdgeCube.pickedFace)), Cube.CreateMaterialArray(e.CurrentMaterialId)));
					CubePickingInfo cubePickingInfo = new CubePickingInfo(movingEdgeCube);
					cubePickingInfo.cube = Cube.Clone(e.TargetCubeModel.GetCube(cubePosAboveFace2));
					cubePickingInfo.iLocalPos = cubePosAboveFace2;
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = cubePickingInfo;
					CubeOutOfBoundState outOfBoundState = CubeOutOfBoundState.WithinBounds;
					Cube.MoveFace(movingEdgeCube, -1f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), ref outOfBoundState);
					Cube.MoveEdge(movingEdgeCube, -0.75f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), ref outOfBoundState);
					e.TargetCubeModel.CornersChanged(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
				}
				else
				{
					modelCursor.SetErrorCursor(cubePosAboveFace2, e.TargetCubeModel.GameObject);
					Debug.Log((object)"Failed to add cube");
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = null;
				}
				break;
			}
			case CubeOutOfBoundState.OutOfBoundsAddVertex:
			{
				IntVector cubePosAboveFace4 = Cube.GetCubePosAboveFace(movingEdgeCube.iLocalPos, movingEdgeCube.pickedFace);
				if (e.CanAddCubeAt(cubePosAboveFace4) && e.TargetCubeModel.GetCube(cubePosAboveFace4) == null)
				{
					e.HandleAudio(movingEdgeCube.iLocalPos, AudioActions.CubeAdded);
					e.TargetCubeModel.AddCube(cubePosAboveFace4, new Cube(CubeDataPacker.CornersToByteArray(Cube.GetCorners(movingEdgeCube.cube, movingEdgeCube.pickedFace)), Cube.CreateMaterialArray(e.CurrentMaterialId)));
					CubePickingInfo cubePickingInfo3 = new CubePickingInfo(movingEdgeCube);
					cubePickingInfo3.cube = Cube.Clone(e.TargetCubeModel.GetCube(cubePosAboveFace4));
					cubePickingInfo3.iLocalPos = cubePosAboveFace4;
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = cubePickingInfo3;
					CubeOutOfBoundState outOfBoundState3 = CubeOutOfBoundState.WithinBounds;
					Cube.MoveFace(movingEdgeCube, -1f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), ref outOfBoundState3);
					Cube.MoveVertex(movingEdgeCube, -0.75f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), movingEdgeCube.pickedEdgeIndex0, movingEdgeCube.pickedEdgeIndex1, ref outOfBoundState3);
					e.TargetCubeModel.CornersChanged(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
				}
				else
				{
					modelCursor.SetErrorCursor(cubePosAboveFace4, e.TargetCubeModel.GameObject);
					Debug.Log((object)"Failed to add cube");
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = null;
				}
				break;
			}
			case CubeOutOfBoundState.OutOfBoundsRemove:
				if (e.CanRemoveCubeAt(movingEdgeCube.iLocalPos))
				{
					IntVector cubePosNeighborOppositeFace = GetCubePosNeighborOppositeFace(movingEdgeCube.iLocalPos, movingEdgeCube.pickedFace);
					e.HandleAudio(movingEdgeCube.iLocalPos, AudioActions.CubeRemoved);
					e.TargetCubeModel.RemoveCube(movingEdgeCube.iLocalPos);
					Cube cube = e.TargetCubeModel.GetCube(cubePosNeighborOppositeFace);
					if (cube != null)
					{
						CubePickingInfo cubePickingInfo4 = new CubePickingInfo(movingEdgeCube);
						cubePickingInfo4.cube = Cube.Clone(cube);
						cubePickingInfo4.iLocalPos = cubePosNeighborOppositeFace;
						movingEdgeCube = cubePickingInfo4;
					}
					else
					{
						movingEdgeCube = null;
					}
				}
				else
				{
					modelCursor.SetErrorCursor(movingEdgeCube.iLocalPos, e.TargetCubeModel.GameObject);
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, prevCubeState);
					movingEdgeCube = null;
				}
				break;
			case CubeOutOfBoundState.WithinBounds:
				if (Cube.IsCollapsed(movingEdgeCube.cube.Corners) && !e.CanRemoveCubeAt(movingEdgeCube.iLocalPos))
				{
					modelCursor.SetErrorCursor(movingEdgeCube.iLocalPos, e.TargetCubeModel.GameObject);
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, prevCubeState);
					movingEdgeCube = null;
				}
				else
				{
					e.HandleAudio(movingEdgeCube.iLocalPos, AudioActions.FaceMoved);
					e.TargetCubeModel.CornersChanged(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
				}
				break;
			}
			break;
		}
		}
		if (movingEdgeCube != null)
		{
			prevCubeState = movingEdgeCube.cube.Clone();
		}
		else
		{
			prevCubeState = null;
		}
		prevSelectedCube = e.SelectedCube;
		if (modelCursor != null)
		{
			modelCursor.UpdateCursor(movingEdgeCube, e.SelectedCube, e.TargetCubeModel.GameObject, currentInternalState, addCube);
		}
	}

	public override void Exit(CubeModelingStateMachine e)
	{
		if (currentInternalState == BuildState.MultiChangeCubes)
		{
			currentInternalState = BuildState.MainState;
			e.CurrentMaterialId = prevMaterial;
			if (movingEdgeCube != null)
			{
				movingEdgeCube.cube.Corners = CubeBase.IdentityCorners;
				e.TargetCubeModel.CornersChanged(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
			}
			movingEdgeCube = null;
			edgeHasMoved = false;
		}
		HideCursor();
		Screen.showCursor = true;
		MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
	}

	public override void HideCursor()
	{
		modelCursor.Remove();
	}

	private bool GotoMultiChangeCubes(CubeModelingStateMachine e)
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

	private void SetEditDetail(CubeModelingStateMachine e)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (e.SelectedCube != null && movingEdgeCube == null)
		{
			Vector3 val = SharedCubeFunctions.LocalToWorld(e.TargetCubeModel.GameObject, e.SelectedCube.iLocalPos);
			Vector3 val2 = ((Component)MVGameController.Instance.Game.CameraController).transform.position - val;
			if (val2.magnitude > detailEditModeMaxDistance * e.TargetCubeModel.Scale.y)
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
}
