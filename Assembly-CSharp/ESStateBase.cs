using UnityEngine;

public class ESStateBase : IState
{
	private bool debug;

	protected EditorEvent stateType;

	protected MVWorldObjectClient tintedWo;

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
		if (tintedWo != null)
		{
			tintedWo.DeSelect();
			tintedWo = null;
		}
	}

	protected void TintObjectsOnMouseOver(EditorStateMachine e)
	{
		VoxelHit hit = default;
		bool pickSuccess = MVGameControllerLegacyUI.Pick(ref hit);
		TintObjectsOnMouseOver(e, pickSuccess, hit);
	}

	protected void TintObjectsOnMouseOver(EditorStateMachine e, bool pickSuccess, VoxelHit hit)
	{
		if (tintedWo != null && debug)
		{
			MeshFilter componentInChildren = tintedWo.GameObject.GetComponentInChildren<MeshFilter>();
			if (componentInChildren != null)
			{
				Vector3 position = tintedWo.Transform.TransformPoint(componentInChildren.sharedMesh.bounds.center);
				CollisionDetectionTests.sphere.transform.localScale = MathFunctions.MultiplyVector(componentInChildren.sharedMesh.bounds.size / 2f, tintedWo.Scale);
				CollisionDetectionTests.sphere.transform.position = position;
				CollisionDetectionTests.sphere.transform.rotation = tintedWo.WorldRotation;
			}
		}
		if (tintedWo != null && e.IsSelected(tintedWo.Id))
		{
			tintedWo = null;
		}
		if (pickSuccess)
		{
			bool flag = (hit.interactionFlags & InteractionFlags.Selectable) != 0;
			bool flag2 = (hit.interactionFlags & InteractionFlags.DirectlySelectable) != 0;
			if (flag)
			{
				MVWorldObjectClient worldObjectClient;
				if (flag2)
				{
					worldObjectClient = WOCM.GetWorldObjectClient(hit.woId);
				}
				else
				{
					int parentBelow = MVGroup.GetParentBelow(e.ParentGroupID, hit.woId);
					if (parentBelow == -1 || e.IsSelected(parentBelow))
					{
						return;
					}
					worldObjectClient = WOCM.GetWorldObjectClient(parentBelow);
				}
				if (tintedWo != null && tintedWo.Id != worldObjectClient.Id)
				{
					DeTintCurrent();
				}
				else if (tintedWo == null)
				{
					tintedWo = worldObjectClient;
					Color color = new Color(0f, 0.8f, 0f, 1f);
					tintedWo.Select(color);
				}
			}
			else
			{
				DeTintCurrent();
			}
		}
		else
		{
			DeTintCurrent();
		}
	}
}
