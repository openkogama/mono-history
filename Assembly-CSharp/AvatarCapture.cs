using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvatarCapture : MonoBehaviour
{
	private class RenderTextureTargetDef
	{
		public Transform transform;

		public Dictionary<GameObject, int> storedGameObjectLayers = new Dictionary<GameObject, int>();

		public RenderTextureTargetDef(Transform t)
		{
			transform = t;
		}

		public void ChangeChildLayers(Transform parent)
		{
			if (parent.gameObject.layer == LayerUtil.GetLayerNumber(LayerFlags.Player) || parent.gameObject.layer == LayerUtil.GetLayerNumber(LayerFlags.Default))
			{
				storedGameObjectLayers.Add(parent.gameObject, parent.gameObject.layer);
				parent.gameObject.layer = LayerUtil.GetLayerNumber(LayerFlags.UXElementSecondary);
			}
			foreach (Transform item in parent)
			{
				ChangeChildLayers(item);
			}
		}

		public void RestoreChildLayers(Transform parent)
		{
			if (parent.gameObject.layer == LayerUtil.GetLayerNumber(LayerFlags.UXElementSecondary))
			{
				parent.gameObject.layer = storedGameObjectLayers[parent.gameObject];
			}
			foreach (Transform item in parent)
			{
				RestoreChildLayers(item);
			}
		}
	}

	[SerializeField]
	private Camera renderCam;

	[SerializeField]
	private Vector3 offset;

	private RenderTextureTargetDef currentTargetWinner;

	public Camera RenderCam
	{
		get
		{
			return renderCam;
		}
		private set
		{
			renderCam = value;
		}
	}

	public void CaptureAllPlayersInGame(CameraClearFlags flags)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(512, 512, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
		temporary.wrapMode = TextureWrapMode.Clamp;
		temporary.filterMode = FilterMode.Bilinear;
		renderCam.targetTexture = temporary;
		renderCam.clearFlags = flags;
		List<MVPlayer> list = new List<MVPlayer>();
		foreach (MVPlayer value in MVGameControllerBase.Game.Players.Values)
		{
			list.Add(value);
		}
		CapturePlayerGroup(list);
	}

	public void CapturePlayersInTeam(List<ScoreTeamEntry> scoreTeamEntries, CameraClearFlags flags, GameStatCounterType counterType)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(512, 512, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
		temporary.wrapMode = TextureWrapMode.Clamp;
		temporary.filterMode = FilterMode.Bilinear;
		renderCam.targetTexture = temporary;
		renderCam.clearFlags = flags;
		List<MVPlayer> sortedList = (from o in MVGameControllerBase.Game.TeamManager.GetPlayersInTeam(scoreTeamEntries[0].team)
			orderby o.GetGameStat(counterType)
			select o).ToList();
		CapturePlayerGroup(sortedList);
	}

	private void CapturePlayerGroup(List<MVPlayer> sortedList)
	{
		for (int i = 0; i < sortedList.Count; i++)
		{
			sortedList[i].Avatar.ChangeLOD(0f);
		}
		int count = sortedList.Count;
		List<Vector3> positions = new List<Vector3>();
		float num = CreateTriangleFormation(ref positions, count, 1, 0f, 0f);
		positions.Reverse();
		List<RenderTextureTargetDef> list = new List<RenderTextureTargetDef>();
		for (int j = 0; j < count; j++)
		{
			list.Add(new RenderTextureTargetDef(sortedList[j].Avatar.GameObject.transform));
		}
		for (int k = 0; k < count; k++)
		{
			currentTargetWinner = list[k];
			GameObject gameObject = sortedList[k].Avatar.GameObject;
			renderCam.transform.position = gameObject.transform.position;
			renderCam.transform.position += gameObject.transform.right * offset.x + gameObject.transform.right * (0f - positions[k].x);
			renderCam.transform.position += gameObject.transform.forward * (offset.z + num / 2f) + gameObject.transform.forward * (0f - positions[k].z);
			renderCam.transform.position += gameObject.transform.up * (offset.y + num / 3f) + gameObject.transform.up * positions[k].y;
			float num2 = Random.Range(-35f, 35f);
			renderCam.transform.position = RotatePointAroundPivot(renderCam.transform.position, gameObject.transform.position, new Vector3(0f, num2, 0f));
			renderCam.transform.rotation = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.y + 180f + num2, Vector3.up);
			renderCam.Render();
		}
	}

	public void CaptureGO(GameObject avatarObject, CameraClearFlags flags)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(512, 512, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
		temporary.wrapMode = TextureWrapMode.Clamp;
		temporary.filterMode = FilterMode.Bilinear;
		renderCam.targetTexture = temporary;
		renderCam.clearFlags = flags;
		currentTargetWinner = new RenderTextureTargetDef(avatarObject.transform);
		renderCam.transform.position = currentTargetWinner.transform.position + currentTargetWinner.transform.forward * offset.z + currentTargetWinner.transform.up * offset.y;
		float num = Random.Range(-35, 35);
		renderCam.transform.position = RotatePointAroundPivot(renderCam.transform.position, currentTargetWinner.transform.position, new Vector3(0f, num, 0f));
		renderCam.transform.rotation = Quaternion.AngleAxis(currentTargetWinner.transform.rotation.eulerAngles.y + 180f + num, Vector3.up);
		renderCam.Render();
	}

	private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		Vector3 vector = point - pivot;
		vector = Quaternion.Euler(angles) * vector;
		point = vector + pivot;
		return point;
	}

	private int CreateTriangleFormation(ref List<Vector3> positions, int positionsRemaining, int unitsPerRow, float targetY, float targetZ)
	{
		for (int i = 0; i < unitsPerRow; i++)
		{
			Vector3 item = new Vector3(0f, targetY, targetZ);
			item.x = ((float)unitsPerRow / 2f - 0.5f) * 2f;
			if (i != 0)
			{
				item.x = ((float)unitsPerRow / 2f - (float)i + Random.Range(-0.5f, 0.5f) - 0.5f) * 2f;
			}
			positions.Add(item);
			if (positionsRemaining - i - 1 <= 0)
			{
				return unitsPerRow;
			}
		}
		return CreateTriangleFormation(ref positions, positionsRemaining - unitsPerRow, unitsPerRow + 1, targetY - 0.6f, targetZ - 1.2f);
	}

	private void OnDestroy()
	{
		RenderTexture.ReleaseTemporary(renderCam.targetTexture);
		renderCam.targetTexture = null;
	}

	private void OnPreCull()
	{
		currentTargetWinner.storedGameObjectLayers.Clear();
		currentTargetWinner.ChangeChildLayers(currentTargetWinner.transform);
	}

	private void OnPostRender()
	{
		currentTargetWinner.RestoreChildLayers(currentTargetWinner.transform);
	}
}
