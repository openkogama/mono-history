using MV.WorldObject;
using UnityEngine;

internal class EditCubes2D : CubeModelTool
{
	private float delta;

	private float deltaAccum;

	private byte prevMaterial;

	private bool edgeHasMoved;

	private float mouseSensitivity;

	private float mouseSensitivityExtrude = 2.02f;

	private float mouseUpTimeBeforeMoveEdge = 0.3f;

	private float detailEditModeMaxDistance = 30f;

	private float prevMouseUpTime;

	private BuildState currentInternalState = BuildState.MainState;

	private CubePickingInfo prevSelectedCube;

	private CubePickingInfo movingEdgeCube;

	private Cube prevCubeState;

	private ModelCursor2d modelCursor;

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
		Debug.Log("Enter edit cubes 2d");
		delta = 0f;
		deltaAccum = 0f;
		modelCursor = new ModelCursor2d(e.CubeCorners);
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.EditingCube);
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
		e.SelectedCube = Convert3DCubePickingInfoTo2DModeling(e.SelectedCube, e.TargetCubeModel);
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			prevMouseUpTime = Time.time;
		}
		SetEditDetail(e);
		bool addCube = false;
		switch (currentInternalState)
		{
		case BuildState.Idle:
			if (currentInternalState == BuildState.Idle && MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
			{
				Cursor.visible = true;
				currentInternalState = BuildState.MainState;
			}
			break;
		case BuildState.MainState:
			if (e.SelectedCube != null)
			{
				if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
				{
					e.HandleAudio(e.SelectedCube.iLocalPos, AudioActions.FaceMoved);
					e.TargetCubeModel.UnIndentCubeFace(e.SelectedCube.iLocalPos, e.SelectedCube.pickedFace, e.SelectedCube.cube);
					break;
				}
			}
			else if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect) && DrawPlane.IsDrawPlaneActive)
			{
				currentInternalState = BuildState.PaintCubes;
				break;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect) && Time.time - prevMouseUpTime > mouseUpTimeBeforeMoveEdge && GotoMultiChangeCubes())
			{
				prevMaterial = e.CurrentMaterialId;
				byte material = CubeBase.GetMaterial(prevSelectedCube.cube, prevSelectedCube.pickedFace);
				e.CurrentMaterialId = material;
				movingEdgeCube = prevSelectedCube;
				currentInternalState = BuildState.MultiChangeCubes;
				deltaAccum = 0f;
				Cursor.visible = false;
			}
			break;
		case BuildState.PaintCubes:
			if (!MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
			{
				if (!DrawPlane.GetCubePosOnDrawplane(e.TargetCubeModel.GameObject, out var intVectorHitPosition) || !e.CanAddCubeAt(intVectorHitPosition))
				{
					break;
				}
				if (e.TargetCubeModel.ContainsCube(intVectorHitPosition))
				{
					Cube cube2 = e.TargetCubeModel.GetCube(intVectorHitPosition);
					for (int i = 0; i < cube2.ByteCorners.Length; i++)
					{
						if (e.ByteCubeCorners[i] != cube2.ByteCorners[i])
						{
							e.TargetCubeModel.RemoveCube(intVectorHitPosition);
							break;
						}
					}
				}
				e.HandleAudio(intVectorHitPosition, AudioActions.CubeAdded);
				e.TargetCubeModel.AddCube(intVectorHitPosition, new Cube(e.ByteCubeCorners, Cube.CreateMaterialArray(e.CurrentMaterialId)));
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
			if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
			{
				currentInternalState = BuildState.MainState;
				e.CurrentMaterialId = prevMaterial;
				Cursor.visible = true;
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
				}
				movingEdgeCube = null;
				edgeHasMoved = false;
				break;
			}
			Vector3 mousePositionDelta = mouseSensitivityExtrude * new Vector3(MVInputWrapper.GetAxisRaw("Mouse X"), MVInputWrapper.GetAxisRaw("Mouse Y"), 0f);
			bool edgeMoved = false;
			CubeOutOfBoundState cubeOutOfBoundState = MoveEdge(e.TargetCubeModel, movingEdgeCube, mousePositionDelta, ref delta, ref deltaAccum, mouseSensitivity, ref edgeMoved, movingEdgeCube.pickedEdgeIndex0, movingEdgeCube.pickedEdgeIndex1);
			if (!edgeHasMoved && edgeMoved)
			{
				edgeHasMoved = true;
			}
			switch (cubeOutOfBoundState)
			{
			case CubeOutOfBoundState.OutOfBoundsAdd:
			{
				IntVector cubePosAboveFace2 = Cube.GetCubePosAboveFace(movingEdgeCube.iLocalPos, movingEdgeCube.pickedFace);
				if (e.CanAddCubeAt(cubePosAboveFace2) && e.TargetCubeModel.GetCube(cubePosAboveFace2) == null)
				{
					e.HandleAudio(cubePosAboveFace2, AudioActions.FaceMoved);
					e.TargetCubeModel.AddCube(cubePosAboveFace2, new Cube(CubeDataPacker.CornersToByteArray(Cube.GetCorners(movingEdgeCube.cube, movingEdgeCube.pickedFace)), Cube.CreateMaterialArray(e.CurrentMaterialId)));
					CubePickingInfo cubePickingInfo2 = new CubePickingInfo(movingEdgeCube);
					cubePickingInfo2.cube = Cube.Clone(e.TargetCubeModel.GetCube(cubePosAboveFace2));
					cubePickingInfo2.iLocalPos = cubePosAboveFace2;
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = cubePickingInfo2;
					CubeOutOfBoundState outOfBoundState2 = CubeOutOfBoundState.WithinBounds;
					Cube.MoveFace(movingEdgeCube, -0.75f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), ref outOfBoundState2);
					e.TargetCubeModel.CornersChanged(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
				}
				else
				{
					modelCursor.SetErrorCursor(cubePosAboveFace2, e.TargetCubeModel.GameObject);
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = null;
				}
				break;
			}
			case CubeOutOfBoundState.OutOfBoundsAddEdge:
			{
				IntVector cubePosAboveFace = Cube.GetCubePosAboveFace(movingEdgeCube.iLocalPos, movingEdgeCube.pickedFace);
				if (e.CanAddCubeAt(cubePosAboveFace) && e.TargetCubeModel.GetCube(cubePosAboveFace) == null)
				{
					e.HandleAudio(movingEdgeCube.iLocalPos, AudioActions.CubeAdded);
					Debug.Log("Add");
					e.TargetCubeModel.AddCube(cubePosAboveFace, new Cube(CubeDataPacker.CornersToByteArray(Cube.GetCorners(movingEdgeCube.cube, movingEdgeCube.pickedFace)), Cube.CreateMaterialArray(e.CurrentMaterialId)));
					CubePickingInfo cubePickingInfo = new CubePickingInfo(movingEdgeCube);
					cubePickingInfo.cube = Cube.Clone(e.TargetCubeModel.GetCube(cubePosAboveFace));
					cubePickingInfo.iLocalPos = cubePosAboveFace;
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = cubePickingInfo;
					CubeOutOfBoundState outOfBoundState = CubeOutOfBoundState.WithinBounds;
					Cube.MoveFace(movingEdgeCube, -1f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), ref outOfBoundState);
					Cube.MoveEdge(movingEdgeCube, -0.75f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), ref outOfBoundState);
					e.TargetCubeModel.CornersChanged(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
				}
				else
				{
					modelCursor.SetErrorCursor(cubePosAboveFace, e.TargetCubeModel.GameObject);
					Debug.Log("Failed to add cube");
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = null;
				}
				break;
			}
			case CubeOutOfBoundState.OutOfBoundsAddVertex:
			{
				IntVector cubePosAboveFace3 = Cube.GetCubePosAboveFace(movingEdgeCube.iLocalPos, movingEdgeCube.pickedFace);
				if (e.CanAddCubeAt(cubePosAboveFace3) && e.TargetCubeModel.GetCube(cubePosAboveFace3) == null)
				{
					e.HandleAudio(movingEdgeCube.iLocalPos, AudioActions.CubeAdded);
					e.TargetCubeModel.AddCube(cubePosAboveFace3, new Cube(CubeDataPacker.CornersToByteArray(Cube.GetCorners(movingEdgeCube.cube, movingEdgeCube.pickedFace)), Cube.CreateMaterialArray(e.CurrentMaterialId)));
					CubePickingInfo cubePickingInfo3 = new CubePickingInfo(movingEdgeCube);
					cubePickingInfo3.cube = Cube.Clone(e.TargetCubeModel.GetCube(cubePosAboveFace3));
					cubePickingInfo3.iLocalPos = cubePosAboveFace3;
					e.TargetCubeModel.CornersChangedDone(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
					movingEdgeCube = cubePickingInfo3;
					CubeOutOfBoundState outOfBoundState3 = CubeOutOfBoundState.WithinBounds;
					Cube.MoveFace(movingEdgeCube, -1f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), ref outOfBoundState3);
					Cube.MoveVertex(movingEdgeCube, -0.75f, Cube.GetFaceAxis(movingEdgeCube.pickedFace), movingEdgeCube.pickedEdgeIndex0, movingEdgeCube.pickedEdgeIndex1, ref outOfBoundState3);
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
			modelCursor.UpdateCursor(Convert2DCubePickingInfoInto3D(movingEdgeCube, e.TargetCubeModel), Convert2DCubePickingInfoInto3D(e.SelectedCube, e.TargetCubeModel), e.TargetCubeModel.GameObject, currentInternalState, addCube);
		}
	}

	private static CubeOutOfBoundState MoveEdge(MVCubeModelBase cmb, CubePickingInfo info, Vector3 mousePositionDelta, ref float delta, ref float deltaAccum, float mouseSensitivity, ref bool edgeMoved, bool edgeIndex0, bool edgeIndex1)
	{
		if (cmb.GetCube(info.iLocalPos) == null)
		{
			return CubeOutOfBoundState.WithinBounds;
		}
		CubeOutOfBoundState outOfBoundState = CubeOutOfBoundState.NoChange;
		float num = SharedCubeFunctions.ScaleFactor(cmb.GameObject, info.pickedFace) * 0.25f;
		float num2 = (cmb.Scale.x + cmb.Scale.y + cmb.Scale.z) / 3f;
		float num3 = num / num2;
		Vector3 vector = cmb.Transform.localToWorldMatrix * Cube.GetFaceAxis(info.pickedFace);
		Vector3 vector2 = info.point + vector * deltaAccum;
		Debug.DrawLine(vector2, vector2 + vector, Color.magenta);
		Debug.DrawLine(vector2, vector2 + Vector3.up, Color.magenta);
		Vector3 vector3 = Camera.main.WorldToScreenPoint(vector2 + vector) - Camera.main.WorldToScreenPoint(vector2);
		if (vector3.magnitude > 0f)
		{
			float num4 = Vector3.Dot(vector3.normalized, mousePositionDelta) / vector3.magnitude;
			delta += num4 * 1.3f;
		}
		if (Mathf.Abs(delta) >= num3)
		{
			float num5 = delta % num3;
			delta -= num5;
			deltaAccum += delta;
			if (info.pickedEdge != Edge.None)
			{
				Cube.MoveEdge(info, delta, (cmb.Transform.worldToLocalMatrix * vector.normalized).normalized, ref outOfBoundState);
			}
			else
			{
				Cube.MoveFace(info, delta, (cmb.Transform.worldToLocalMatrix * vector.normalized).normalized, ref outOfBoundState);
			}
			delta = 0f;
			edgeMoved = true;
		}
		else
		{
			edgeMoved = false;
		}
		return outOfBoundState;
	}

	public override void Exit(CubeModelingStateMachine e)
	{
		if (currentInternalState == BuildState.MultiChangeCubes)
		{
			currentInternalState = BuildState.MainState;
			e.CurrentMaterialId = prevMaterial;
			if (movingEdgeCube != null)
			{
				movingEdgeCube.cube.Corners = e.CubeCorners;
				e.TargetCubeModel.CornersChanged(movingEdgeCube.iLocalPos, movingEdgeCube.cube);
			}
			movingEdgeCube = null;
			edgeHasMoved = false;
		}
		HideCursor();
		Cursor.visible = true;
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
	}

	public override void HideCursor()
	{
		modelCursor.Remove();
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

	private static CubePickingInfo Convert3DCubePickingInfoTo2DModeling(CubePickingInfo cpi, MVCubeModelBase cubeModelBase)
	{
		if (cpi == null)
		{
			return null;
		}
		if (cpi.pickedFace != Face.Front)
		{
			return null;
		}
		CubePickingInfo cubePickingInfo = new CubePickingInfo(cpi);
		switch (cubePickingInfo.pickedEdge)
		{
		case Edge.Back:
			cubePickingInfo.pickedFace = Face.Top;
			break;
		case Edge.Right:
			cubePickingInfo.pickedFace = Face.Right;
			break;
		case Edge.Left:
			cubePickingInfo.pickedFace = Face.Left;
			break;
		case Edge.Front:
			cubePickingInfo.pickedFace = Face.Bottom;
			break;
		}
		if (!cubePickingInfo.pickedEdgeIndex0 && !cubePickingInfo.pickedEdgeIndex1)
		{
			cubePickingInfo.pickedEdge = Edge.None;
		}
		else
		{
			Vector3 zero = Vector3.zero;
			Vector3[] edgeVerticesWorld = Cube.GetEdgeVerticesWorld(cubeModelBase.GameObject, cpi.cube, cpi.pickedFace, cpi.pickedEdge, cpi.iLocalPos);
			cubePickingInfo.pickedEdge = Cube.GetEdge(pos: ((!cpi.pickedEdgeIndex0) ? edgeVerticesWorld[1] : edgeVerticesWorld[0]) - cubePickingInfo.normal * 0.5f, gameObject: cubeModelBase.GameObject, cube: cubeModelBase.GetCube(cubePickingInfo.iLocalPos), face: cubePickingInfo.pickedFace, iVector: cubePickingInfo.iLocalPos);
		}
		return cubePickingInfo;
	}

	private static CubePickingInfo Convert2DCubePickingInfoInto3D(CubePickingInfo cpi, MVCubeModelBase cubeModelBase)
	{
		if (cpi == null)
		{
			return null;
		}
		CubePickingInfo cubePickingInfo = new CubePickingInfo(cpi);
		switch (cpi.pickedFace)
		{
		case Face.Top:
			cubePickingInfo.pickedEdge = Edge.Back;
			break;
		case Face.Right:
			cubePickingInfo.pickedEdge = Edge.Right;
			break;
		case Face.Left:
			cubePickingInfo.pickedEdge = Edge.Left;
			break;
		case Face.Bottom:
			cubePickingInfo.pickedEdge = Edge.Front;
			break;
		}
		cubePickingInfo.pickedFace = Face.Front;
		return cubePickingInfo;
	}

	private static IntVector GetCubePosNeighborOppositeFace(IntVector localPos, Face face)
	{
		IntVector result = new IntVector(localPos.x, localPos.y, localPos.z);
		Vector3 faceAxis = Cube.GetFaceAxis(face);
		result.x -= (short)faceAxis.x;
		result.y -= (short)faceAxis.y;
		result.z -= (short)faceAxis.z;
		return result;
	}

	private void SetEditDetail(CubeModelingStateMachine e)
	{
		if (e.SelectedCube != null && movingEdgeCube == null)
		{
			Vector3 vector = SharedCubeFunctions.LocalToWorld(e.TargetCubeModel.GameObject, e.SelectedCube.iLocalPos);
			if ((MVGameControllerBase.CameraController.transform.position - vector).magnitude > detailEditModeMaxDistance * e.TargetCubeModel.Scale.y)
			{
				mouseSensitivity = 0.1325f;
			}
			else
			{
				mouseSensitivity = 0.0225f;
			}
		}
	}
}
