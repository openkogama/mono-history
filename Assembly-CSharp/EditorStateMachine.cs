using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class EditorStateMachine : FSMEntity
{
	public const float sqrEpsilon = 0.64f;

	private SelectionController selectionController;

	private MVCameraController weCamera;

	private CubeModelingStateMachine cubeModelingStateMachine;

	private MVNetworkSelector networkSelector;

	private GameObject gameObject;

	public MVNetworkSelector NetworkSelector => networkSelector;

	public MVCameraController CameraController => weCamera;

	public bool GridMode { get; set; }

	public CubeModelingStateMachine CubeModelingStateMachine => cubeModelingStateMachine;

	public EditorEvent CurEvent => (EditorEvent)(int)curEvent;

	public EditorEvent PrevEvent => (EditorEvent)(int)prevEvent;

	public EditorEvent NextEvent => (EditorEvent)(int)nextEvent;

	public ISelectionController SelectionController => selectionController;

	public HashSet<int> SelectedIDs => selectionController.SelectedIDs;

	public HashSet<MVWorldObjectClient> SelectedWOs => selectionController.SelectedWOs;

	public MVWorldObjectClient SingleSelectedWO => selectionController.SingleSelectedWO;

	public int ParentGroupID => selectionController.ParentGroupID;

	public MVGroup ParentGroup => selectionController.ParentGroup;

	public bool ParentGroupIsRoot => selectionController.ParentGroupID == MVGameControllerBase.WOCM.RootGroup.Id;

	public GameObject GameObject => gameObject;

	private EditorStateMachine(GameObject gameObject)
	{
		this.gameObject = gameObject;
		cubeModelingStateMachine = new CubeModelingStateMachine(gameObject);
		networkSelector = new MVNetworkSelector(this);
		selectionController = new SelectionController();
		weCamera = MVGameControllerBase.CameraController;
		GridMode = true;
	}

	public EditorStateMachine(GameObject gameObject, Vector3 avatarEditModeCenterPos)
		: this(gameObject)
	{
		transitionTable = new CEEditorStateTransitionTableUUI(avatarEditModeCenterPos);
	}

	public EditorStateMachine(GameObject gameObject, ContextMenuController contextMenuController, GizmoController gizmoController)
		: this(gameObject)
	{
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			transitionTable = new EditorStateTransitionTable3D(contextMenuController, gizmoController);
		}
		else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			transitionTable = new EditorStateTransitionTable2D(contextMenuController, gizmoController);
		}
	}

	public void EnterGroup(MVGroup group)
	{
		selectionController.EnterGroup(group);
	}

	public int ExitGroup()
	{
		return selectionController.ExitGroup();
	}

	public int ExitGroupToRoot()
	{
		return selectionController.ExitGroupToRoot();
	}

	public MVWorldObjectClient Select(bool addToSelection, int layerMask = -5)
	{
		return selectionController.Select(addToSelection, showVisuals: true, layerMask);
	}

	public MVWorldObjectClient Select(VoxelHit hit, bool addToSelection)
	{
		return selectionController.Select(hit, addToSelection);
	}

	public void DeSelectWorldObject(MVWorldObjectClient wo)
	{
		selectionController.DeSelectWorldObject(wo);
	}

	public MVWorldObjectClient SelectWO(int id, bool addToSelection, bool showVisuals = true)
	{
		return selectionController.SelectWO(id, addToSelection, showVisuals);
	}

	public void DeSelectAll()
	{
		selectionController.DeSelectAll();
	}

	public void DeSelectAllExcept(int id)
	{
		selectionController.DeSelectAllExcept(id);
	}

	public bool IsSelected(int id)
	{
		return selectionController.IsSelected(id);
	}

	public override void Update()
	{
		base.Update();
	}
}
