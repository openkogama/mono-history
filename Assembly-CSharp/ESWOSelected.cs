using UnityEngine;

internal class ESWOSelected : ESStateBase
{
	private bool canLeave;

	private bool canMove;

	private Vector3 mousePosInitPossibleMove = Vector3.zero;

	public ESWOSelected()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Enter(EditorStateMachine e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		canLeave = false;
		mousePosInitPossibleMove = Input.mousePosition;
		canMove = true;
	}

	public bool HandleSelect(EditorStateMachine e)
	{
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			VoxelHit hit = default;
			if (!MVGameController.Instance.WOCM.Pick(ref hit) || hit.woId == -1)
			{
				e.DeSelect();
				if (e.ParentGroup == MVGameController.Instance.WOCM.RootGroup.Id)
				{
					e.Event = EditorEvent.EditCubes;
				}
				return true;
			}
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)323))
		{
			VoxelHit hit2 = default;
			if (MVGameController.Instance.WOCM.Pick(ref hit2) && hit2.woId != -1)
			{
				bool flag = false;
				flag = ((!MVInputWrapper.GetKey((KeyCode)306) && !MVInputWrapper.GetKey((KeyCode)305)) ? e.Select(addToSelection: false) : e.Select(addToSelection: true));
				if (!flag && canLeave)
				{
					Debug.Log((object)"Here");
					e.DeSelect();
					if (e.ParentGroup == MVGameController.Instance.WOCM.RootGroup.Id)
					{
						e.Event = EditorEvent.EditCubes;
					}
					return true;
				}
				e.Event = EditorEvent.ObjectSelected;
				return true;
			}
		}
		return false;
	}

	public bool ContinueToSelect(EditorStateMachine e)
	{
		VoxelHit hit = default;
		if (MVInputWrapper.GetKey((KeyCode)323) && MVGameController.Instance.WOCM.Pick(ref hit) && !e.IsSelected(hit.woId))
		{
			Debug.Log((object)"returning true");
			return true;
		}
		return false;
	}

	public bool HandleMove(EditorStateMachine e)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			canMove = false;
			return false;
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)323))
		{
			VoxelHit hit = default;
			if (!MVGameController.Instance.WOCM.Pick(ref hit))
			{
				canMove = false;
				return false;
			}
			canMove = true;
			mousePosInitPossibleMove = Input.mousePosition;
		}
		if (MVInputWrapper.GetKey((KeyCode)323) && canMove)
		{
			Vector3 val = mousePosInitPossibleMove - Input.mousePosition;
			if (val.magnitude > 0.5f || MVInputWrapper.GetAxisRaw("Mouse ScrollWheel") != 0f)
			{
				e.PushState(EditorEvent.ESTranslate);
				return true;
			}
		}
		return false;
	}

	private bool HandleEnterObject(EditorStateMachine e)
	{
		if (MVInputWrapper.GetDoubleClick((KeyCode)323) && e.SingleSelectedWO != null)
		{
			if ((object)e.SingleSelectedWO.GetType() == typeof(MVCubeModelInstance))
			{
				ZoomExtents(e);
				e.Event = EditorEvent.EditCubes;
				return true;
			}
			if ((object)e.SingleSelectedWO.GetType() == typeof(MVGroup))
			{
				if (!e.ParentGroupIsRoot)
				{
					SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroup).GameObject.transform, select: false);
				}
				e.PushParent(e.SingleSelectedWO.Id);
				e.Select(addToSelection: false);
				SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroup).GameObject.transform, select: true);
				((Behaviour)((Component)e.WeCamera).GetComponent<GrayscaleEffect>()).enabled = true;
				e.Event = EditorEvent.ObjectSelected;
				return true;
			}
		}
		return false;
	}

	public bool HandleEscape(EditorStateMachine e)
	{
		if (MVInputWrapper.GetKeyDown((KeyCode)27))
		{
			e.DeSelect();
			int parentGroup = e.ParentGroup;
			if (e.ParentGroupIsRoot)
			{
				e.Event = EditorEvent.EditCubes;
				return true;
			}
			e.PopParent();
			if (e.ParentGroupIsRoot)
			{
				((Behaviour)((Component)e.WeCamera).GetComponent<GrayscaleEffect>()).enabled = false;
				SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(parentGroup).GameObject.transform, select: false);
			}
			else
			{
				((Behaviour)((Component)e.WeCamera).GetComponent<GrayscaleEffect>()).enabled = true;
				SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroup).GameObject.transform, select: true);
			}
			e.SelectWo(parentGroup, addToSelection: false);
			return true;
		}
		return false;
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (e.ParentGroup == MVGameController.Instance.WOCM.RootGroup.Id)
		{
			e.WeCamera.SecondaryCameraActive = false;
		}
		else
		{
			e.WeCamera.SecondaryCameraActive = true;
		}
		if (HandleEscape(e))
		{
			return;
		}
		TintObjectsOnMouseOver(e);
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			canLeave = true;
		}
		if (MVInputWrapper.GetKey((KeyCode)323) && (MVInputWrapper.GetKey((KeyCode)304) || MVInputWrapper.GetKey((KeyCode)303)))
		{
			e.PushState(EditorEvent.Rotating);
		}
		else if (!HandleMove(e) && !HandleSelect(e))
		{
			if (MVInputWrapper.GetKeyDown((KeyCode)127))
			{
				MVGameController.Instance.EditorController.Delete();
			}
			else if (!HandleEnterObject(e) && MVInputWrapper.GetKeyDown((KeyCode)104))
			{
				ZoomExtents(e);
			}
		}
	}

	private void ZoomExtents(EditorStateMachine e)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		if (e.SingleSelectedWO != null)
		{
			Vector3 val = e.SingleSelectedWO.GameObject.transform.position;
			if ((object)e.SelectedWOs.GetType() == typeof(MVCubeModelInstance))
			{
				val = SharedCubeFunctions.GetWorldCenter(e.SingleSelectedWO.GameObject);
			}
			Vector3 position = ((Component)e.WeCamera).transform.position;
			Vector3 val2 = val - position;
			Vector3 normalized = val2.normalized;
			Vector3 val3 = val - position;
			float magnitude = val3.magnitude;
			float num = 10f;
			if (magnitude >= num)
			{
				val -= normalized * 10f * e.SingleSelectedWO.GameObject.transform.localScale.y;
				e.WeCamera.ForceCamera(val, e.SingleSelectedWO.GameObject.transform.position);
			}
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		if (e.ParentGroupIsRoot)
		{
			((Behaviour)((Component)e.WeCamera).GetComponent<GrayscaleEffect>()).enabled = false;
		}
	}
}
