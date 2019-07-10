using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	private static Dictionary<ScenesForMode, string[]> scenesForModeMap = new Dictionary<ScenesForMode, string[]>
	{
		{
			ScenesForMode.PlayMode,
			new string[1] { "DesktopPlayModeGUI" }
		},
		{
			ScenesForMode.PlayModeTourist,
			new string[1] { "DesktopPlayModeGUI" }
		},
		{
			ScenesForMode.EditMode,
			new string[2] { "DesktopEditModeGUI", "DesktopPlayModeGUI" }
		},
		{
			ScenesForMode.AvatarEditMode,
			new string[1] { "DesktopAvatarEditModeGUI" }
		}
	};

	private List<AsyncOperation> pendingScenes = new List<AsyncOperation>();

	private Action callback;

	public void LoadScenes(MVGameMode gameMode, bool tourist, Action callback)
	{
		switch (gameMode)
		{
		case MVGameMode.Play:
			if (tourist)
			{
				LoadScenes(ScenesForMode.PlayModeTourist, callback);
			}
			else
			{
				LoadScenes(ScenesForMode.PlayMode, callback);
			}
			break;
		case MVGameMode.Edit:
			LoadScenes(ScenesForMode.EditMode, callback);
			break;
		case MVGameMode.CharacterEditor:
			LoadScenes(ScenesForMode.AvatarEditMode, callback);
			break;
		}
	}

	private void LoadScenes(ScenesForMode mode, Action callback)
	{
		this.callback = callback;
		string[] array = scenesForModeMap[mode];
		for (int i = 0; i < array.Length; i++)
		{
			pendingScenes.Add(SceneManager.LoadSceneAsync(array[i], LoadSceneMode.Additive));
		}
	}

	private void Update()
	{
		for (int num = pendingScenes.Count - 1; num >= 0; num--)
		{
			if (pendingScenes[num].isDone)
			{
				pendingScenes.RemoveAt(num);
			}
		}
		if (pendingScenes.Count <= 0 && callback != null)
		{
			callback();
			callback = null;
			StartCoroutine(WaitForFrames.Frames(3, () =>
			{
				BrowserComm.ToJavaScript.ExternalCall("readyForAd");
			}));
		}
	}
}
