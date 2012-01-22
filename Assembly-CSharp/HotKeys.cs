using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

internal class HotKeys
{
	public enum DamageFalloutType
	{
		Linear
	}

	public void HandleInput()
	{
		if (!MVGameController.Instance.Game.EditorMode)
		{
			return;
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)102))
		{
			MVGameController.Instance.EditorController.ToggleWorkPlane();
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)116))
		{
			MVGameController.Instance.EditorController.ShowChat();
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)107))
		{
			MVGameController.Instance.WOCM.WoAvatar.AvatarController.Die();
		}
		if (!Application.isEditor)
		{
			return;
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)98))
		{
			TestHandleRemoveCubes();
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)111))
		{
			MVWorldObjectClient contextMenuSelectionWO = MVGameController.Instance.EditorController.GetContextMenuSelectionWO();
			if (contextMenuSelectionWO != null && contextMenuSelectionWO is MVCubeModelBase)
			{
				MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)contextMenuSelectionWO;
				GameObject mesh = MVGameController.Instance.WOCM.WorldInventory.RuntimePrototypes[mVCubeModelBase.Pid].GetMesh();
			}
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)110))
		{
			FireBazooka();
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)108) && !MVInputWrapper.GetKey((KeyCode)304))
		{
			MVWorldObjectClient contextMenuSelectionWO2 = MVGameController.Instance.EditorController.GetContextMenuSelectionWO();
			if (contextMenuSelectionWO2 != null && contextMenuSelectionWO2 is MVCubeModelBase)
			{
				MVCubeModelBase cm = (MVCubeModelBase)contextMenuSelectionWO2;
				ObjExporterScript.CubeModelToFile(cm);
			}
		}
		else if (MVInputWrapper.GetKeyDown((KeyCode)108) && MVInputWrapper.GetKey((KeyCode)304))
		{
			ObjExporterScript.CubeModelToFile(MVGameController.Instance.WOCM.Terrain);
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)109))
		{
			MVWorldObjectClient contextMenuSelectionWO3 = MVGameController.Instance.EditorController.GetContextMenuSelectionWO();
			if (contextMenuSelectionWO3 != null && contextMenuSelectionWO3 is MVCubeModelBase)
			{
				MVCubeModelBase mVCubeModelBase2 = (MVCubeModelBase)contextMenuSelectionWO3;
				RuntimePrototypeCubeModel prototypeCubeModel = mVCubeModelBase2.PrototypeCubeModel;
				prototypeCubeModel.CreateMipMapMeshes();
			}
		}
	}

	private void TransferCubesFromModelToOtherModelTest()
	{
		Dictionary<int, MVWorldObjectClient> worldObjects = MVGameController.Instance.WOCM.WorldObjects;
		MVCubeModelBase mVCubeModelBase = null;
		MVCubeModelBase mVCubeModelBase2 = null;
		foreach (KeyValuePair<int, MVWorldObjectClient> item in worldObjects)
		{
			if (item.Value.WorldObjectType == WorldObjectType.CubeModel)
			{
				MVCubeModelBase mVCubeModelBase3 = (MVCubeModelBase)item.Value;
				if (mVCubeModelBase3.PrototypeCubeModel.Name == "InitCell")
				{
					mVCubeModelBase = mVCubeModelBase3;
				}
				if (mVCubeModelBase3.PrototypeCubeModel.Name == "TargetCell")
				{
					mVCubeModelBase2 = mVCubeModelBase3;
				}
			}
		}
		if (mVCubeModelBase != null && mVCubeModelBase2 != null)
		{
			IntVector intVector = new IntVector(-1, -1, -1);
			IntVector intVector2 = new IntVector(0, 0, 0);
			IntVector intVector3 = new IntVector(1, 1, 1);
		}
	}

	private void TestHandleRemoveCubes()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		MVSpawnPoint mVSpawnPoint = (MVSpawnPoint)MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPoint)[0];
		float radius = 5.5f;
		List<CommonOverlapArg> wOIdsWithinRadius = MVGameController.Instance.Game.GetWOIdsWithinRadius(radius, mVSpawnPoint.Position);
		int[] array = new int[wOIdsWithinRadius.Count];
		for (int i = 0; i < wOIdsWithinRadius.Count; i++)
		{
			array[i] = wOIdsWithinRadius[i].wo.Id;
		}
	}

	private void CommonOverlapTestTest()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		MVSpawnPoint mVSpawnPoint = (MVSpawnPoint)MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPoint)[0];
		float num = 1.5f;
		Vector3 position = mVSpawnPoint.Position;
		DebugClass.DrawPoint(mVSpawnPoint.Position, 100f);
		Collider[] array = Physics.OverlapSphere(mVSpawnPoint.Position, num);
		List<CommonOverlapResult> list = new List<CommonOverlapResult>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < array.Length; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(((Component)array[i]).transform);
			if (mVObject is MVCubeModelBase)
			{
				CommonOverlapArg overlapArg = new CommonOverlapArg(mVObject);
				if (SphereOverlapTest.OverlapWo(overlapArg, num, position, out var overlapResult))
				{
					list.Add(overlapResult);
					Debug.Log((object)overlapResult.cubes.Count);
					list2.Add(overlapResult.woId);
				}
			}
		}
		MVGameController.Instance.Game.RequestRemoveCubesWithinRadius(list2.ToArray(), num, position, 200f, DamageFallOffType.None);
	}

	private void FireBazooka()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		VoxelHit voxelHit = default;
		if (CollisionDetection.MVHit(ray, out voxelHit))
		{
			Missile missile = Missile.CreateMissile();
			((Component)missile).transform.position = MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position;
			missile.Fire(voxelHit.point);
		}
	}
}
