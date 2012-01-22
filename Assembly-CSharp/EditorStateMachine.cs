using System.Collections.Generic;
using UnityEngine;

public class EditorStateMachine : FSMEntity
{
	public delegate void OnCurrentMaterialChangeDelegate(byte currentMaterialId, Material currentMaterial);

	public const float sqrEpsilon = 0.64f;

	private MVNetworkSelector networkSelector;

	private MVCameraController weCamera;

	private SelectionController selectionController;

	private byte currentMaterialId;

	private Material currentMaterial;

	public OnCurrentMaterialChangeDelegate OnCurrentMaterialChange;

	public MVCameraController WeCamera => weCamera;

	public bool GridMode { get; set; }

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

	public Material CurrentMaterial => MVGameController.Instance.WOCM.MaterialRepository.GetMaterial(currentMaterialId).material;

	public MVNetworkSelector NetworkSelector => networkSelector;

	public EditorEvent CurEvent => (EditorEvent)(int)curEvent;

	public EditorEvent PrevEvent => (EditorEvent)(int)prevEvent;

	public EditorEvent NextEvent => (EditorEvent)(int)nextEvent;

	public int ParentGroup => selectionController.ParentGroup;

	public bool ParentGroupIsRoot => selectionController.ParentGroup == MVGameController.Instance.WOCM.RootGroup.Id;

	public HashSet<MVWorldObjectClient> SelectedWOs => selectionController.SelectedWOs;

	public MVWorldObjectClient SingleSelectedWO => selectionController.SingleSelectedWO;

	public HashSet<int> Selected => selectionController.Selected;

	public EditorStateMachine()
	{
		transitionTable = new EditorStateTransitionTable();
		networkSelector = new MVNetworkSelector(this);
		selectionController = new SelectionController();
		GameObject gameObject = ((Component)(MVCameraController)(object)Object.FindObjectOfType(typeof(MVCameraController))).gameObject;
		weCamera = gameObject.GetComponent<MVCameraController>();
		GridMode = true;
	}

	public void PushParent(int id)
	{
		selectionController.PushParent(id);
	}

	public int PopParent()
	{
		return selectionController.PopParent();
	}

	public void DeSelect()
	{
		selectionController.DeSelect();
	}

	public bool IsSelected(int id)
	{
		return selectionController.IsSelected(id);
	}

	public bool Select(bool addToSelection)
	{
		return selectionController.Select(addToSelection);
	}

	public bool SelectWo(int id, bool addToSelection)
	{
		return selectionController.SelectWo(id, addToSelection);
	}

	public void SelectNewRegisteredObject(MVWorldObjectClient wo)
	{
		selectionController.SelectNewRegisteredObject(wo);
	}

	public int GetParentBelow(int parent, int child)
	{
		return selectionController.GetParentBelow(parent, child);
	}

	public override void Update()
	{
		base.Update();
		CollisionDetectionTests.Update();
		selectionController.UpdateSelectionGizmos();
	}
}
