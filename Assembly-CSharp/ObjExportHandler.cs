using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjExportHandler : MonoBehaviour
{
	private static bool picking = false;

	private const int defaultMask = -262149;

	private static HashSet<int> ignoreIds = new HashSet<int>();

	public static void InitializePicking()
	{
		picking = true;
		MVGameControllerBase.PlayModeUI.GetCrossHair().Visible = true;
		ignoreIds.Add(MVGameControllerBase.Game.LocalPlayer.SpawnRoleDataMediator.WoId);
	}

	public static void ExportSelfAvatar()
	{
		try
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(MVGameControllerBase.Game.LocalPlayer.SpawnRoleDataMediator.WoId);
			MeshFilter[] componentsInChildren = worldObjectClient.GameObject.GetComponentsInChildren<MeshFilter>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ObjExporterScript.MeshToFile(componentsInChildren[i], Application.dataPath + "/../" + componentsInChildren[i].gameObject.name + i + ".obj", append: false);
				Debug.Log("Adding obj file: " + componentsInChildren[i].gameObject.name);
			}
			Debug.Log("Exported: " + componentsInChildren.Length + " files successfully to " + Application.dataPath + " (kogama_data folder).");
		}
		catch
		{
			Debug.LogWarning("An error occurred while exporting model.");
		}
	}

	private void Update()
	{
		if (!picking || !Input.GetKeyDown(KeyCode.Mouse0))
		{
			return;
		}
		try
		{
			picking = false;
			VoxelHit hit = default;
			bool flag = Pick(ref hit, ignoreIds);
			if (flag && hit.woId == -1)
			{
				flag = false;
			}
			Debug.Log("WoPickSuccess: " + flag);
			if (flag)
			{
				MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(hit.woId);
				MeshFilter[] componentsInChildren = worldObjectClient.GameObject.GetComponentsInChildren<MeshFilter>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					ObjExporterScript.MeshToFile(componentsInChildren[i], Application.dataPath + "/../" + componentsInChildren[i].gameObject.name + i + ".obj", append: false);
					Debug.Log("Adding obj file: " + componentsInChildren[i].gameObject.name);
				}
				Debug.Log("Exported: " + componentsInChildren.Length + " files successfully to " + Application.dataPath + " (kogama_data folder).");
			}
		}
		catch
		{
			Debug.LogWarning("An error occurred while exporting model.");
		}
	}

	private static bool Pick(ref VoxelHit hit, HashSet<int> ignoreWoIds = null, int layerMask = -262149)
	{
		if (MVInputWrapper.IsAllInputSuppressed)
		{
			return false;
		}
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return false;
		}
		if (MVGameControllerBase.JoinState != MVJoinState.Playing)
		{
			return false;
		}
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
		float num = 0f;
		bool flag = false;
		List<VoxelHit> list = CollisionDetection.MVHitAll(ray, float.PositiveInfinity, ignoreWoIds, layerMask);
		if (list.Count == 0)
		{
			return false;
		}
		float num2 = float.PositiveInfinity;
		bool result = false;
		foreach (VoxelHit item in list)
		{
			if ((item.distance < num || !flag) && item.transform.gameObject.activeInHierarchy)
			{
				float num3 = Vector3.Distance(ray.origin, item.point);
				if (num3 < num2)
				{
					num2 = num3;
					hit = item;
					result = true;
				}
			}
		}
		return result;
	}

	private static bool MVObjectIsType(Transform t, Type type, out int woId)
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
