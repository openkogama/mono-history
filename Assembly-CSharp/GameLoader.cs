using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
	[SerializeField]
	private int gameBaseSceneIndex = 1;

	private static bool applicationStartUp = true;

	public static void UnloadGame()
	{
		SceneManager.LoadScene(0, LoadSceneMode.Single);
		applicationStartUp = false;
	}

	protected void Awake()
	{
		if (!applicationStartUp)
		{
			Cleanup();
		}
	}

	protected void Start()
	{
		if (!applicationStartUp)
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
			CloseGame();
		}
	}

	private void Cleanup()
	{
		CullingApiWrapper.PostDestroyCleanup();
		MVGameControllerBase.PostDestroyCleanup();
		AsyncWWWManager.PostResetCleanup();
		TimedPlayReward.RewardTracker.PostResetCleanup();
		BackButtonManager.PostDestroyCleanup();
		FlagDebriefingControl.PostResetCleanup();
		GamePointGainEffectManager.PostResetCleanup();
	}

	private void CloseGame()
	{
	}
}
