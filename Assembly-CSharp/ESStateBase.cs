using UnityEngine;

public class ESStateBase : IState
{
	private bool debug;

	protected EditorEvent stateType;

	protected MVWorldObjectClient tintedWo;

	private ILogger logger;

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

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
		logger.Log("Enter");
	}

	public virtual void Execute(EditorStateMachine e)
	{
	}

	public virtual void Exit(EditorStateMachine esm)
	{
	}

	public void Enter(FSMEntity e)
	{
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
		bool pickSuccess = MVGameController.Instance.WOCM.Pick(ref hit);
		TintObjectsOnMouseOver(e, pickSuccess, hit);
	}

	protected void TintObjectsOnMouseOver(EditorStateMachine e, bool pickSuccess, VoxelHit hit)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		if (tintedWo != null && debug)
		{
			MeshFilter componentInChildren = tintedWo.GameObject.GetComponentInChildren<MeshFilter>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				Transform transform = tintedWo.Transform;
				Bounds bounds = componentInChildren.sharedMesh.bounds;
				Vector3 position = transform.TransformPoint(bounds.center);
				Transform transform2 = CollisionDetectionTests.sphere.transform;
				Bounds bounds2 = componentInChildren.sharedMesh.bounds;
				transform2.localScale = MathFunctions.MultiplyVector(bounds2.size / 2f, tintedWo.Scale);
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
