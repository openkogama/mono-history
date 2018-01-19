using System;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PickHelper : MonoBehaviour
{
	public UnityAction<MVWorldObjectClient, MVWorldObjectClient> pickCallback;

	[SerializeField]
	private Text message;

	public void Initialize(UnityAction<MVWorldObjectClient, MVWorldObjectClient> onPickCallback, string msg)
	{
		message.text = msg;
		pickCallback = onPickCallback;
	}

	private void Update()
	{
		MVInputWrapper.SuppressInGameInput();
		if (pickCallback == null || !Input.GetKeyUp(KeyCode.Mouse0))
		{
			return;
		}
		VoxelHit hit = default;
		bool flag = EditModeObjectPicker.Pick(ref hit);
		if (flag && (MVGameControllerBase.WOCM.IsType(hit.woId, WorldObjectType.CubeModelPrototypeTerrain) || hit.woId == -1))
		{
			flag = false;
		}
		if (flag)
		{
			int woId = hit.woId;
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(hit.woId);
			if (MVObjectIsType(worldObjectClient.Transform, null, out woId))
			{
				Transform parent = worldObjectClient.Transform.parent;
				pickCallback(worldObjectClient, MVGameControllerBase.WOCM.GetWorldObjectByGoId(parent.GetInstanceID()));
			}
			else
			{
				Debug.Log("tried to pick something else: " + worldObjectClient.GetType());
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
}
