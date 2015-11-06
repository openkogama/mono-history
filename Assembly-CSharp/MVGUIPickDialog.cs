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
		base.OnShowDialog();
		screen = UXUtils.UXScreen;
		Vector3 vector = DialogWindow.transform.InverseTransformPoint(screen.GetPosition(UXHorizontal.Left, UXVertical.Top, 0f));
		vector.z = 0f;
		DialogWindow.transform.localPosition = vector + new Vector3(9f, -3f, 0f);
	}

	private void Update()
	{
		if (!MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			return;
		}
		VoxelHit hit = default;
		bool flag = MVGameControllerLegacyUI.Pick(ref hit);
		if (flag && (MVGameControllerBase.WOCM.IsType(hit.woId, WorldObjectType.CubeModelPrototypeTerrain) || hit.woId == -1))
		{
			flag = false;
		}
		if (flag)
		{
			int woId = hit.woId;
			if (MVObjectIsType(MVGameControllerBase.WOCM.GetWorldObjectClient(hit.woId).Transform, pickType, out woId))
			{
				pickedWOId = woId;
				OnPositiveClose();
				DialogFactory.CloseDialog();
			}
			else
			{
				Debug.Log("picked something else: " + MVGameControllerBase.WOCM.GetWorldObjectClient(hit.woId).GetType());
			}
		}
	}

	private bool MVObjectIsType(Transform t, Type type, out int woId)
	{
		MVWorldObjectClient worldObjectByGoId = MVGameControllerBase.WOCM.GetWorldObjectByGoId(t.gameObject.GetInstanceID());
		if (worldObjectByGoId != null)
		{
			woId = worldObjectByGoId.Id;
			if (type == null)
			{
				return true;
			}
			if (MVGameControllerBase.WOCM.IsType(woId, WorldObjectType.CubeModelPrototypeTerrain))
			{
				return false;
			}
			if (type.IsAssignableFrom(worldObjectByGoId.GetType()))
			{
				return true;
			}
		}
		if (t.parent != null)
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
