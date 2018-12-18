using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public class CubeModelingStateMachine : FSMEntity
{
	public delegate void OnCurrentMaterialChangeDelegate(byte currentMaterialId, Material currentMaterial);

	public enum HoverType
	{
		Corner,
		Edge,
		Face,
		None
	}

	private static readonly Vector3[] zDepth1Cube = new Vector3[8]
	{
		new Vector3(-0.5f, 0.5f, 0.25f),
		new Vector3(0.5f, 0.5f, 0.25f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, 0.25f),
		new Vector3(-0.5f, -0.5f, 0.25f)
	};

	private static readonly byte[] zDepth1CubeByteCorners = CubeDataPacker.CornersToByteArray(zDepth1Cube);

	private ObscuredByte currentMaterialId = (byte)0;

	private Material currentMaterial;

	private IModelingConstraint constraint;

	public OnCurrentMaterialChangeDelegate OnCurrentMaterialChange;

	public bool useLasers = true;

	private GameObject gameObject;

	private bool editMode2d;

	private Camera mainCamera;

	public CubePickingInfo SelectedCube { get; set; }

	public byte CurrentMaterialId
	{
		get
		{
			return currentMaterialId;
		}
		set
		{
			MaterialsControllerEditMode.targetMaterial = value;
			currentMaterialId = value;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleMaterial x, BaseEventData y) =>
			{
				x.OnMaterialChanged(currentMaterialId);
			});
		}
	}

	public MVCubeModelBase TargetCubeModel { get; private set; }

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

	public Vector3[] CubeCorners
	{
		get
		{
			if (editMode2d)
			{
				return zDepth1Cube;
			}
			return CubeBase.IdentityCorners;
		}
	}

	public byte[] ByteCubeCorners
	{
		get
		{
			if (editMode2d)
			{
				return zDepth1CubeByteCorners;
			}
			return CubeBase.IdentityByteCorners;
		}
	}

	public Material CurrentMaterial => MVGameControllerBase.MaterialLoader.CubeModelMaterial;

	public CubeModelingStateMachine(GameObject gameObject)
	{
		this.gameObject = gameObject;
		transitionTable = new CubeModelingTransitionTable();
		Event = CubeModelingEvent.EditCubes;
		mainCamera = Camera.main;
	}

	public void StartEdit(MVCubeModelBase targetCubeModel, IModelingConstraint constraint = null)
	{
		if (TargetCubeModel != null)
		{
			TargetCubeModel.BeingEdited = false;
		}
		TargetCubeModel = targetCubeModel;
		SetConstraint(constraint);
		TargetCubeModel.BeingEdited = true;
		editMode2d = MVGameControllerBase.Game.GameType == MVGameType.Platformer && targetCubeModel is MVCubeModelPrototypeTerrain;
		if (editMode2d && (CubeModelingEvent)curEvent == CubeModelingEvent.EditCubes)
		{
			curEvent = CubeModelingEvent.EditCubes2D;
		}
		if (!editMode2d && (CubeModelingEvent)curEvent == CubeModelingEvent.EditCubes2D)
		{
			curEvent = CubeModelingEvent.EditCubes;
		}
		Event = curEvent;
	}

	public void SetConstraint(IModelingConstraint constraint)
	{
		this.constraint = constraint;
	}

	public void EndEdit()
	{
		TargetCubeModel.BeingEdited = false;
		TargetCubeModel = null;
	}

	public override void Update()
	{
		if (TargetCubeModel == null)
		{
			Debug.Log("Not set");
			return;
		}
		SelectedCube = DoPicking();
		base.Update();
		TargetCubeModel.HandleDelta();
	}

	public HoverType CurrentlyHovered()
	{
		if (SelectedCube != null)
		{
			if (SelectedCube.pickedEdgeIndex0 || SelectedCube.pickedEdgeIndex1)
			{
				return HoverType.Corner;
			}
			if (SelectedCube.pickedEdge != Edge.None)
			{
				return HoverType.Edge;
			}
			return HoverType.Face;
		}
		return HoverType.None;
	}

	public CubePickingInfo DoPicking()
	{
		CubePickingInfo info = new CubePickingInfo();
		if (EditModeObjectPicker.GetPickingInfo(TargetCubeModel, ref info))
		{
			Vector3 hit = Vector3.zero;
			if (DrawPlane.Pick(ref hit))
			{
				float magnitude = (hit - mainCamera.transform.position).magnitude;
				float magnitude2 = (info.point - mainCamera.transform.position).magnitude;
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
		Cursor.visible = true;
		((CubeModelTool)currentState).HideCursor();
	}

	public void HandleAudio(IntVector pos, AudioActions action)
	{
		switch (action)
		{
		case AudioActions.CubeAdded:
			if (TargetCubeModel.GetCube(pos) == null)
			{
				AudioEventHandler.PlaySound(action, pos, TargetCubeModel.GameObject);
			}
			break;
		case AudioActions.FaceMoved:
			AudioEventHandler.PlaySound(action, pos, TargetCubeModel.GameObject);
			break;
		case AudioActions.CubeRemoved:
			if (TargetCubeModel.GetCube(pos) != null)
			{
				AudioEventHandler.PlaySound(action, pos, TargetCubeModel.GameObject);
			}
			break;
		}
	}

	public EditCubeChange AddCube()
	{
		if (Cube.IsFaceBoxSideAligened(SelectedCube.cube, SelectedCube.pickedFace))
		{
			IntVector cubePosAboveFace = Cube.GetCubePosAboveFace(SelectedCube.iLocalPos, SelectedCube.pickedFace);
			if (CanAddCubeAt(cubePosAboveFace) && TargetCubeModel.GetCube(cubePosAboveFace) == null)
			{
				HandleAudio(cubePosAboveFace, AudioActions.CubeAdded);
				TargetCubeModel.AddCube(cubePosAboveFace, new Cube(CubeDataPacker.CornersToByteArray(Cube.GetCorners(SelectedCube.cube, SelectedCube.pickedFace)), Cube.CreateMaterialArray(CurrentMaterialId)));
				return EditCubeChange.CubeAdded;
			}
			return EditCubeChange.None;
		}
		HandleAudio(SelectedCube.iLocalPos, AudioActions.FaceMoved);
		TargetCubeModel.UnIndentCubeFace(SelectedCube.iLocalPos, SelectedCube.pickedFace, SelectedCube.cube);
		return EditCubeChange.CubeUnindented;
	}

	public bool CanAddCubeAt(IntVector requestedCubePos)
	{
		if ((TargetCubeModel.InteractionFlags & InteractionFlags.IsTerrain) != 0)
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
			Debug.Log(string.Concat("Pos ", requestedCubePos, " not within constraint "));
		}
		return flag;
	}

	public bool CanRemoveCubeAt(IntVector requestedCubePos)
	{
		if ((TargetCubeModel.InteractionFlags & InteractionFlags.IsTerrain) != 0)
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
		if ((TargetCubeModel.InteractionFlags & InteractionFlags.IsTerrain) != 0)
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
