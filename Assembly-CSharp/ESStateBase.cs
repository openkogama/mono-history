using UnityEngine;

internal class ESStateBase : IState
{
	private bool debug;

	protected EditorEvent stateType;

	protected MVWorldObjectClient tintedWo;

	private ILogger logger;

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

	private void DeTintCurrent()
	{
		if (tintedWo != null)
		{
			tintedWo.DeSelect();
			tintedWo = null;
		}
	}

	protected void TintObjectsOnMouseOver(EditorStateMachine e)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		if (tintedWo != null && debug)
		{
			MeshFilter componentInChildren = tintedWo.GameObject.GetComponentInChildren<MeshFilter>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				Transform transform = tintedWo.GameObject.transform;
				Bounds bounds = componentInChildren.sharedMesh.bounds;
				Vector3 position = transform.TransformPoint(bounds.center);
				Transform transform2 = CollisionDetectionTests.sphere.transform;
				Bounds bounds2 = componentInChildren.sharedMesh.bounds;
				transform2.localScale = MathFunctions.MultiplyVector(bounds2.size / 2f, tintedWo.GameObject.transform.localScale);
				CollisionDetectionTests.sphere.transform.position = position;
				CollisionDetectionTests.sphere.transform.rotation = tintedWo.GameObject.transform.rotation;
			}
		}
		if (tintedWo != null && e.IsSelected(tintedWo.Id))
		{
			tintedWo = null;
		}
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit))
		{
			if ((hit.interactionFlags & InteractionFlags.Selectable) != 0)
			{
				int parentBelow = e.GetParentBelow(e.ParentGroup, hit.woId);
				if (parentBelow != -1 && !e.IsSelected(parentBelow))
				{
					MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(parentBelow);
					if (tintedWo != null && tintedWo.Id != worldObjectClient.Id)
					{
						DeTintCurrent();
					}
					else if (tintedWo == null)
					{
						tintedWo = worldObjectClient;
						Color color = new Color(1f, 0.8f, 0.8f, 0.2f);
						tintedWo.Select(color);
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
			DeTintCurrent();
		}
	}
}
