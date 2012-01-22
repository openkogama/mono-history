using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class AudioEventHandler
{
	private static AudioBuild audioBuild;

	private static List<TranslateSoundData> translateSoundDatas = new List<TranslateSoundData>();

	public static void PlaySound(AudioActions audioAction, IntVector localPos, GameObject gameObject)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Vector3 worldPos = SharedCubeFunctions.LocalToWorld(gameObject, localPos);
		switch (audioAction)
		{
		case AudioActions.CubeAdded:
			audioBuild.CubeAdded(worldPos);
			break;
		case AudioActions.CubeRemoved:
			audioBuild.CubeRemoved(worldPos);
			break;
		case AudioActions.FaceMoved:
			audioBuild.FaceMoved(worldPos);
			break;
		case AudioActions.EdgeMoved:
			audioBuild.EdgeMoved(worldPos);
			break;
		case AudioActions.VertexMoved:
			audioBuild.VertexMoved(worldPos);
			break;
		case AudioActions.CubePainted:
			audioBuild.CubePainted(worldPos);
			break;
		}
	}

	public static void Awake()
	{
		audioBuild = Object.FindObjectOfType(typeof(AudioBuild)) as AudioBuild;
	}

	public static void AddTranslateSoundData(float moveValue, bool moveToGridPos, Vector3 worldPos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		translateSoundDatas.Add(new TranslateSoundData(moveValue, moveToGridPos, worldPos));
	}

	public static void Update()
	{
		HandleTranslateData();
	}

	private static void HandleTranslateData()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		foreach (TranslateSoundData translateSoundData in translateSoundDatas)
		{
			audioBuild.Translate(translateSoundData.moveValue, translateSoundData.moveToGridPos, translateSoundData.worldPos);
		}
		translateSoundDatas.Clear();
	}
}
