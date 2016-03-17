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
			ScenesForMode.PlayMode2D,
			new string[1] { "DesktopPlayModeGUI" }
		},
		{
			ScenesForMode.PlayModeTourist,
			new string[1] { "DesktopPlayModeGUI" }
		},
		{
			ScenesForMode.PlayModeTourist2D,
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

	public void LoadScenes(MVGameMode gameMode, MVGameType gameType, bool tourist, Action callback)
	{
		switch (gameMode)
		{
		case MVGameMode.Play:
			switch (gameType)
			{
			case MVGameType.Classic:
				if (tourist)
				{
					LoadScenes(ScenesForMode.PlayModeTourist, callback);
				}
				else
				{
					LoadScenes(ScenesForMode.PlayMode, callback);
				}
				break;
			case MVGameType.Platformer:
				if (tourist)
				{
					LoadScenes(ScenesForMode.PlayModeTourist2D, callback);
				}
				else
				{
					LoadScenes(ScenesForMode.PlayMode2D, callback);
				}
				break;
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
		string[] array = scenesForModeMap[mode];
		for (int i = 0; i < array.Length; i++)
		{
			SceneManager.LoadScene(array[i], LoadSceneMode.Additive);
		}
		callback();
	}
}
