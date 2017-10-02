using UnityEngine;

public class ESStateBase : IState
{
	protected EditorEvent stateType;

	protected WorldObjectClientRef tintedWo = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();

	private ILogger logger;

	private MVWorldObjectClientManager WOCM => MVGameControllerBase.WOCM;

	private EditorEvent StateType => stateType;

	public ESStateBase()
	{
		logger = LoggerManager.Instance.GetLogger(GetType());
	}

	public void SetStateType(EditorEvent stateTypeEvent)
	{
		stateType = stateTypeEvent;
	}

	public virtual void Enter(EditorStateMachine esm)
	{
		logger.Log("Enter " + stateType);
	}

	public virtual void Execute(EditorStateMachine e)
	{
	}

	public virtual void Exit(EditorStateMachine esm)
	{
	}

	public void Enter(FSMEntity e)
	{
		Debug.Log("Enter " + stateType);
		Enter((EditorStateMachine)e);
	}

	public void Execute(FSMEntity e)
	{
		Execute((EditorStateMachine)e);
	}

	public void Exit(FSMEntity e)
	{
		Exit((EditorStateMachine)e);
	}

	protected void DeTintCurrent()
	{
		if (tintedWo.WorldObjectClient != null)
		{
			tintedWo.WorldObjectClient.DeSelect();
			tintedWo = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();
		}
	}

	protected static bool SelectionIsAllowedByLogicEnabled(int woId)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woId);
		if (worldObjectClient == null || worldObjectClient.GroupId == -1)
		{
			return false;
		}
		MVWorldObjectClient worldObjectClientRoot = MVGameControllerBase.WOCM.GetWorldObjectClientRoot(woId);
		if (!MVGameControllerBase.CameraController.IsLogicRendered && worldObjectClientRoot != null)
		{
			int layerNumber = LayerUtil.GetLayerNumber(LayerFlags.Default);
			if (worldObjectClientRoot.GameObject.layer == layerNumber)
			{
				return true;
			}
			Transform[] componentsInChildren = worldObjectClientRoot.GameObject.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].gameObject.layer == layerNumber && componentsInChildren[i].gameObject.activeInHierarchy)
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	protected void TintObjectsOnMouseOver(EditorStateMachine e)
	{
		VoxelHit hit = default;
		bool pickSuccess = EditModeObjectPicker.Pick(ref hit);
		TintObjectsOnMouseOver(e, pickSuccess, hit);
	}

	protected void TintObjectsOnMouseOver(EditorStateMachine e, bool pickSuccess, VoxelHit hit)
	{
		if (tintedWo.WorldObjectClient != null && e.IsSelected(tintedWo.WorldObjectClient.Id))
		{
			tintedWo = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();
		}
		if (pickSuccess)
		{
			bool flag = (hit.interactionFlags & InteractionFlags.Selectable) != 0;
			bool flag2 = (hit.interactionFlags & InteractionFlags.DirectlySelectable) != 0;
			if (flag)
			{
				WorldObjectClientRef worldObjectClientRefNullRef = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();
				if (flag2)
				{
					worldObjectClientRefNullRef = WOCM.GetWorldObjectClientRef(hit.woId);
				}
				else
				{
					int parentBelow = MVGroup.GetParentBelow(e.ParentGroupID, hit.woId);
					if (parentBelow == -1 || e.IsSelected(parentBelow))
					{
						return;
					}
					worldObjectClientRefNullRef = WOCM.GetWorldObjectClientRef(parentBelow);
				}
				if (worldObjectClientRefNullRef.WorldObjectClient != null)
				{
					if (tintedWo.WorldObjectClient != null && tintedWo.WorldObjectClient.Id != worldObjectClientRefNullRef.WorldObjectClient.Id)
					{
						DeTintCurrent();
					}
					else if (tintedWo.WorldObjectClient == null)
					{
						tintedWo = worldObjectClientRefNullRef;
						Color color = new Color(0f, 0.8f, 0f, 1f);
						tintedWo.WorldObjectClient.Select(color);
					}
				}
			}
			else
			{
				DeTintCurrent();
			}
		}
		else
		{
			WOCM.GetWorldObjectClient(hit.woId)?.DeSelect();
			DeTintCurrent();
		}
	}
}
