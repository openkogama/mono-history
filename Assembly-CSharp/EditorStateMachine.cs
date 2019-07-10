using System.Collections.Generic;
using UnityEngine;

public class EditorStateMachine : FSMEntity
{
	private SelectionController selectionController;

	private MainCameraManager weCamera;

	private CubeModelingStateMachine cubeModelingStateMachine;

	private MVNetworkSelector networkSelector;

	private GameObject gameObject;

	public const float sqrEpsilon = 0.64f;

	public MVNetworkSelector NetworkSelector => networkSelector;

	public MainCameraManager MainCameraManager => weCamera;

	public bool GridMode { get; set; }

	public CubeModelingStateMachine CubeModelingStateMachine => cubeModelingStateMachine;

	public EditorEvent CurEvent => (EditorEvent)curEvent;

	public EditorEvent PrevEvent => (EditorEvent)prevEvent;

	public EditorEvent NextEvent => (EditorEvent)nextEvent;

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
		weCamera = MVGameControllerBase.MainCameraManager;
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
		transitionTable = new EditorStateTransitionTable3D(contextMenuController, gizmoController);
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

	public WorldObjectClientRef Select(bool addToSelection, int layerMask = -5)
	{
		return selectionController.Select(addToSelection, showVisuals: true, layerMask);
	}

	public WorldObjectClientRef Select(VoxelHit hit, bool addToSelection)
	{
		return selectionController.Select(hit, addToSelection);
	}

	public void DeSelectWorldObject(MVWorldObjectClient wo)
	{
		selectionController.DeSelectWorldObject(wo);
	}

	public WorldObjectClientRef SelectWO(int id, bool addToSelection, bool showVisuals = true)
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
