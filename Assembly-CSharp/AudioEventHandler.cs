using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class AudioEventHandler
{
	private static AudioBuild audioBuild;

	private static List<TranslateSoundData> translateSoundDatas = new List<TranslateSoundData>();

	public static void PlaySound(AudioActions audioAction, IntVector localPos, GameObject gameObject)
	{
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

	public static void Init()
	{
		audioBuild = Object.FindObjectOfType(typeof(AudioBuild)) as AudioBuild;
	}

	public static void AddTranslateSoundData(float moveValue, bool moveToGridPos, Vector3 worldPos)
	{
		translateSoundDatas.Add(new TranslateSoundData(moveValue, moveToGridPos, worldPos));
	}

	public static void Update()
	{
		HandleTranslateData();
	}

	private static void HandleTranslateData()
	{
		foreach (TranslateSoundData translateSoundData in translateSoundDatas)
		{
			audioBuild.Translate(translateSoundData.moveValue, translateSoundData.moveToGridPos, translateSoundData.worldPos);
		}
		translateSoundDatas.Clear();
	}
}
