using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class EditModeObjectPicker
{
	private const int defaultMask = -262149;

	public static bool Pick(ref VoxelHit hit, HashSet<int> ignoreWoIds = null, int layerMask = -262149)
	{
		if (MVInputWrapper.IsInputSuppressed)
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
		if (DrawPlane.IsDrawPlaneActive)
		{
			Vector3 hit2 = Vector3.zero;
			if (DrawPlane.Pick(ref hit2))
			{
				num = (hit2 - ray.origin).magnitude;
				flag = true;
			}
		}
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

	public static bool GetPickingInfo(MVCubeModelBase cr, ref CubePickingInfo info)
	{
		if (MVInputWrapper.IsInputSuppressed)
		{
			return false;
		}
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return false;
		}
		Ray ray = Camera.main.ScreenPointToRay(MVInputWrapper.GetPointerPosition());
		VoxelHit voxelHit = default;
		if (CollisionDetection.MVHit(ray, cr, out voxelHit))
		{
			info.cube = Cube.Clone(voxelHit.cube);
			info.iLocalPos = voxelHit.cubePos;
			info.pickedFace = voxelHit.face;
			info.point = voxelHit.point;
			info.normal = voxelHit.normal;
			info.pickedEdge = Cube.GetEdge(cr.GameObject, info.cube, info.pickedFace, voxelHit.point, info.iLocalPos);
			SharedCubeFunctions.GetVertices(info, cr.GameObject);
			return true;
		}
		return false;
	}

	private static bool IsHitPickup(VoxelHit hit)
	{
		Transform parent = hit.transform.parent;
		return parent.GetComponent<GreyOutObjectScript>() != null;
	}
}
