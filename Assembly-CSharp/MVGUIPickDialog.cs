using System;
using MV.WorldObject;
using UnityEngine;

public class MVGUIPickDialog : UXCustomDialogBox
{
	private UXScreen screen;

	private Type pickType;

	private int pickedWOId;

	public void SetPickType(Type type)
	{
		pickType = type;
	}

	public override void OnShowDialog()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		base.OnShowDialog();
		screen = UXUtils.FindGUIObjectOfType<UXScreen>();
		Vector3 val = ((Component)DialogWindow).transform.InverseTransformPoint(screen.GetPosition(UXHorizontal.Left, UXVertical.Top, 0f));
		val.z = 0f;
		((Component)DialogWindow).transform.localPosition = val + new Vector3(9f, -3f, 0f);
	}

	private void Update()
	{
		if (!Input.GetMouseButtonUp(0))
		{
			return;
		}
		VoxelHit hit = default;
		bool flag = MVGameController.Instance.WOCM.Pick(ref hit);
		if (flag && (MVGameController.Instance.WOCM.IsType(hit.woId, WorldObjectType.CubeModelPrototypeTerrain) || hit.woId == -1))
		{
			flag = false;
		}
		if (flag)
		{
			int woId = hit.woId;
			if (MVObjectIsType(MVGameController.Instance.WOCM.GetWorldObjectClient(hit.woId).Transform, pickType, out woId))
			{
				pickedWOId = woId;
				OnPositiveClose();
				DialogFactory.CloseDialog();
			}
			else
			{
				Debug.Log((object)("picked something else: " + MVGameController.Instance.WOCM.GetWorldObjectClient(hit.woId).GetType()));
			}
		}
	}

	private bool MVObjectIsType(Transform t, Type type, out int woId)
	{
		MVWorldObjectClient worldObjectByGoId = MVGameController.Instance.WOCM.GetWorldObjectByGoId(((Object)((Component)t).gameObject).GetInstanceID());
		if (worldObjectByGoId != null)
		{
			woId = worldObjectByGoId.Id;
			if ((object)type == null)
			{
				return true;
			}
			if (MVGameController.Instance.WOCM.IsType(woId, WorldObjectType.CubeModelPrototypeTerrain))
			{
				return false;
			}
			if (type.IsAssignableFrom(worldObjectByGoId.GetType()))
			{
				return true;
			}
		}
		if ((Object)(object)t.parent != (Object)null)
		{
			return MVObjectIsType(t.parent, type, out woId);
		}
		woId = -1;
		return false;
	}

	public override object GetResult()
	{
		return pickedWOId;
	}
}
