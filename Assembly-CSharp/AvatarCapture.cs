using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvatarCapture : MonoBehaviour
{
	[SerializeField]
	private Camera renderCam;

	[SerializeField]
	private Vector3 cameraOffset;

	[SerializeField]
	[Tooltip("Space between players on winningscreen")]
	private Vector3 formationSpacing = new Vector3(2f, 0.6f, 1.2f);

	[SerializeField]
	private float formationRandomness = 1f;

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

	public void CaptureAllPlayersInGame()
	{
		InitializeCamera();
		List<List<MVPlayer>> list = new List<List<MVPlayer>>();
		List<MVPlayer> item = MVGameControllerBase.Game.MVPlayerContainer.Values.ToList();
		list.Add(item);
		CapturePlayerGroup(list);
	}

	public void CapturePlayersInTeam(List<ScoreTeamEntry> scoreTeamEntries, GameStatCounterType counterType)
	{
		InitializeCamera();
		List<List<MVPlayer>> list = new List<List<MVPlayer>>();
		for (int i = 0; i < scoreTeamEntries.Count; i++)
		{
			List<MVPlayer> item = (from o in MVGameControllerBase.Game.TeamManager.GetPlayersInTeam(scoreTeamEntries[i].team)
				orderby o.GetGameStat(counterType)
				select o).ToList();
			list.Add(item);
		}
		CapturePlayerGroup(list);
	}

	private void CapturePlayerGroup(List<List<MVPlayer>> sortedList)
	{
		for (int i = 0; i < sortedList.Count; i++)
		{
			int count = sortedList[i].Count;
			List<Vector3> positions = new List<Vector3>();
			CreateTriangleFormation(ref positions, formationSpacing, count);
			positions.Reverse();
			int currentWinner = i + 1;
			int count2 = sortedList.Count;
			for (int j = 0; j < positions.Count; j++)
			{
				List<Vector3> list2;
				List<Vector3> list = (list2 = positions);
				int index2;
				int index = (index2 = j);
				Vector3 vector = list2[index2];
				list[index] = vector + CalculateTieOffset(currentWinner, count2);
			}
			for (int k = 0; k < count; k++)
			{
				Transform transform = sortedList[i][k].Avatar.Body.Transform;
				Transform transform2 = renderCam.transform;
				transform2.position = transform.position;
				transform2.position += transform.right * cameraOffset.x;
				transform2.position += transform.right * (0f - positions[k].x);
				transform2.position += transform.forward * (0f - cameraOffset.z);
				transform2.position += transform.forward * (0f - positions[k].z);
				transform2.position += transform.up * cameraOffset.y;
				transform2.position += transform.up * positions[k].y;
				DrawObject(transform2, transform);
				renderCam.Render();
			}
		}
	}

	public void CapturePlayer(List<MVPlayer> players)
	{
		InitializeCamera();
		int count = players.Count;
		List<Vector3> positions = new List<Vector3>();
		CreateTriangleFormation(ref positions, formationSpacing, count);
		positions.Reverse();
		for (int i = 0; i < players.Count; i++)
		{
			Transform transform = players[i].Avatar.Body.Transform;
			Transform transform2 = renderCam.transform;
			transform2.position = transform.position;
			transform2.position += transform.right * cameraOffset.x;
			transform2.position += transform.right * (0f - positions[i].x);
			transform2.position += transform.forward * (0f - cameraOffset.z);
			transform2.position += transform.forward * (0f - positions[i].z);
			transform2.position += transform.up * cameraOffset.y;
			transform2.position += transform.up * positions[i].y;
			DrawObject(transform2, transform);
			renderCam.Render();
		}
	}

	private void InitializeCamera()
	{
		MVGameControllerBase.WOCM.AvatarLocal.Avatar.AvatarFader.SetTransparency(1f);
		renderCam.clearFlags = CameraClearFlags.Depth;
		RenderTexture temporary = RenderTexture.GetTemporary(1024, 512, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		RenderTexture.active = active;
		temporary.wrapMode = TextureWrapMode.Clamp;
		temporary.filterMode = FilterMode.Bilinear;
		renderCam.targetTexture = temporary;
	}

	private void DrawObject(Transform cameraTransform, Transform objectTransform)
	{
		float num = Random.Range(-35, 35);
		cameraTransform.position = RotatePointAroundPivot(cameraTransform.position, objectTransform.position, new Vector3(0f, num, 0f));
		cameraTransform.rotation = Quaternion.AngleAxis(objectTransform.rotation.eulerAngles.y + 180f + num, Vector3.up);
		MeshFilter[] componentsInChildren = objectTransform.GetComponentsInChildren<MeshFilter>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			MeshFilter meshFilter = componentsInChildren[i];
			Mesh mesh = componentsInChildren[i].mesh;
			MeshRenderer component = meshFilter.gameObject.GetComponent<MeshRenderer>();
			for (int j = 0; j < mesh.subMeshCount; j++)
			{
				Material material = component.sharedMaterials[j];
				Matrix4x4 localToWorldMatrix = meshFilter.transform.localToWorldMatrix;
				Graphics.DrawMesh(mesh, localToWorldMatrix, material, LayerMask.NameToLayer("UXElementSecondary"), renderCam, j);
			}
		}
	}

	private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		Vector3 vector = point - pivot;
		vector = Quaternion.Euler(angles) * vector;
		point = vector + pivot;
		return point;
	}

	private int CreateTriangleFormation(ref List<Vector3> positions, Vector3 formationSpacing, int numberOfPositions)
	{
		Vector3 item = new Vector3(0f, 0f, 0f);
		positions.Add(item);
		if (numberOfPositions > 1)
		{
			return CreateTriangleFormation(ref positions, formationSpacing, numberOfPositions - 1, 2, 0f - formationSpacing.y, 0f - formationSpacing.z);
		}
		return numberOfPositions;
	}

	private int CreateTriangleFormation(ref List<Vector3> positions, Vector3 formationSpacing, int positionsRemaining, int unitsThisRow, float targetY, float targetZ)
	{
		for (int i = 0; i < unitsThisRow; i++)
		{
			Vector3 item = new Vector3(0f, targetY, targetZ);
			item.x = ((float)unitsThisRow / 2f - 0.5f - (float)i + Random.Range(0f - formationRandomness, formationRandomness)) * formationSpacing.x;
			positions.Add(item);
			if (positionsRemaining - i - 1 <= 0)
			{
				return unitsThisRow;
			}
		}
		return CreateTriangleFormation(ref positions, formationSpacing, positionsRemaining - unitsThisRow, unitsThisRow + 1, targetY - formationSpacing.y, targetZ - formationSpacing.z);
	}

	private Vector3 CalculateTieOffset(int currentWinner, int amountOfWinners)
	{
		Vector3 zero = Vector3.zero;
		zero.z = -0.5f * (float)amountOfWinners;
		if (amountOfWinners % 2 != 0 && currentWinner == 1)
		{
			return zero;
		}
		zero.x = 4f * (1f / (float)Mathf.FloorToInt((float)(amountOfWinners + 2) / 2f)) * ((float)Mathf.FloorToInt(((float)currentWinner - 1f) / 2f) + 1f);
		zero.x += 1f * ((float)Mathf.FloorToInt(((float)currentWinner - 1f) / 2f) + 1f);
		if (currentWinner % 2 == 0)
		{
			zero.x *= -1f;
		}
		return zero;
	}

	private void OnDestroy()
	{
		renderCam.targetTexture.DiscardContents();
		RenderTexture.ReleaseTemporary(renderCam.targetTexture);
		renderCam.targetTexture = null;
	}
}
