using System.Collections.Generic;
using System.Text.RegularExpressions;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SendMessageControl : MonoBehaviour
{
	private static string helpString = "/h";

	private static string fps = "/f";

	private static string resolution = "/r";

	private static string mathTest = "/m";

	private static string removeUI = "/ru";

	private static string enableHD = "/hd";

	private string buildInformation = "/build";

	private static string startHeadShake = "/no";

	private static string startNod = "/yes";

	private static string startWave = "/wave";

	[SerializeField]
	private InputField inputField;

	private Regex whiteSpaceCheck;

	public UnityAction<bool> DoSend;

	public UnityAction SpamWarning;

	[SerializeField]
	private float intervalForMessages = 5f;

	[SerializeField]
	private int maxMessagesPerInterval = 5;

	private int frameCountSent;

	private List<float> spamList = new List<float>();

	private void Awake()
	{
		whiteSpaceCheck = new Regex("\\S");
	}

	public void Send()
	{
		string text = inputField.text;
		text = Regex.Replace(text, "\\r\\n?|\\n", string.Empty);
		SanitizeMessage(ref text, "size");
		inputField.text = string.Empty;
		if (whiteSpaceCheck.Match(text).Length <= 0)
		{
			if (DoSend != null)
			{
				DoSend(arg0: false);
			}
			return;
		}
		for (int i = 0; i < spamList.Count; i++)
		{
			if (Time.timeSinceLevelLoad - spamList[i] > intervalForMessages)
			{
				spamList.Remove(spamList[i]);
			}
		}
		if (spamList.Count > maxMessagesPerInterval)
		{
			if (SpamWarning != null)
			{
				SpamWarning();
			}
		}
		else
		{
			spamList.Add(Time.timeSinceLevelLoad);
			SendChatMessage(text);
		}
	}

	public void OnInputFieldChange()
	{
		string text = inputField.text;
		if (text.IndexOf("\n") >= 0)
		{
			frameCountSent = Time.frameCount;
			Send();
		}
		else if (frameCountSent == Time.frameCount)
		{
			inputField.text = string.Empty;
		}
	}

	private void SendChatMessage(string chatMsg)
	{
		if (chatMsg == string.Empty)
		{
			return;
		}
		if (chatMsg.Length > 256)
		{
			chatMsg = chatMsg.Substring(0, 256);
		}
		if (chatMsg == helpString)
		{
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, CreateHelpTxt());
		}
		else if (chatMsg == fps)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IToggleFps x, BaseEventData y) =>
			{
				x.ToggleFps();
			});
		}
		else if (chatMsg == resolution)
		{
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, $"{Screen.width} x {Screen.height}");
		}
		else if (chatMsg == mathTest)
		{
			Vector3 one = Vector3.one;
			Quaternion identity = Quaternion.identity;
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Math validation test");
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Vector3.one " + MathFunctions.IsVectorFloatsValid(one));
			one.x = float.PositiveInfinity;
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "vector3.x = float.PositiveInfinity " + MathFunctions.IsVectorFloatsValid(one));
			one.x = float.NaN;
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "vector3.x = float.NaN " + MathFunctions.IsVectorFloatsValid(one));
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Quaternion.identity " + MathFunctions.IsQuaternionFloatsValid(identity));
			identity.x = float.PositiveInfinity;
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "q.x = float.PositiveInfinity " + MathFunctions.IsQuaternionFloatsValid(identity));
			identity.x = float.NaN;
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "q.x = float.NaN " + MathFunctions.IsQuaternionFloatsValid(identity));
		}
		else if (chatMsg == enableHD)
		{
			ToggleHD();
		}
		else if (chatMsg == removeUI)
		{
			GetComponentInParent<Canvas>().gameObject.SetActive(value: false);
		}
		else if (chatMsg == buildInformation)
		{
			ShowBuildInformation();
		}
		else if (chatMsg == startHeadShake)
		{
			MVGameControllerBase.WOCM.AvatarLocal.LimbManager.StartEmote(EmoteTypes.Shake);
		}
		else if (chatMsg == startNod)
		{
			MVGameControllerBase.WOCM.AvatarLocal.LimbManager.StartEmote(EmoteTypes.Nod);
		}
		else if (chatMsg == startWave)
		{
			MVGameControllerBase.WOCM.AvatarLocal.LimbManager.StartEmote(EmoteTypes.wave);
		}
		else
		{
			MVGameControllerBase.OperationRequests.PostChatMsg(new Dictionary<object, object>
			{
				{
					(byte)0,
					MVGameControllerBase.Game.LocalPlayer.ActorNr
				},
				{
					(byte)5,
					chatMsg
				}
			});
		}
		if (DoSend != null)
		{
			DoSend(arg0: false);
		}
	}

	private void ShowBuildInformation()
	{
		string message = string.Format(TM._("Version: {0}\nBranch: {1}\nCommitMessage: {2}"), MVGameControllerBase.KoGaMaSettings.VersionString, MVGameControllerBase.KoGaMaSettings.BranchName, MVGameControllerBase.KoGaMaSettings.LatestCommitMessage);
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, message);
	}

	private void ToggleHD()
	{
		int currentLevel = ((MVQualitySettings.CurrentLevel == 0) ? 1 : 0);
		MVQualitySettings.CurrentLevel = currentLevel;
	}

	public static string CreateHelpTxt()
	{
		string text = "\n";
		text += TM._("\nType: " + enableHD + " to enable HD mode.\n\n");
		MVGameMode mVGameMode = MVGameControllerBase.GameMode;
		if (MVGameControllerBase.IsPlaying)
		{
			mVGameMode = MVGameMode.Play;
		}
		switch (mVGameMode)
		{
		case MVGameMode.CharacterEditor:
			return string.Empty;
		case MVGameMode.Edit:
			text += TM._("\n<Right mouse> Hold to look\n<WASD> Move\n<Scroll wheel> Move up and down\n<Shift> Hold to move fast\n");
			break;
		case MVGameMode.Play:
			text += TM._("<M> Menu");
			text += TM._("\n<H> Toggle HD Mode\n<WASD> Move\n<Space> Jump\n<K> Respawn\n<Left Mouse> Fire Weapon\n<Q> Holster equipped weapon\n<V> Drop equipped weapon\n");
			break;
		}
		return text;
	}

	private void SanitizeMessage(ref string message, string tagToSanitize)
	{
		bool flag = false;
		char[] array = message.ToLower().ToCharArray();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == '<' && !flag)
			{
				num = i;
				flag = true;
			}
			if (array[i] == '>' && flag)
			{
				num2 = i + 1;
				flag = false;
				string text = new string(array, num, num2 - num);
				if (array[num + 1] != ' ' && text.Contains(tagToSanitize))
				{
					message = message.Remove(num, num2 - num);
					array = message.ToLower().ToCharArray();
					i -= num2 - num;
				}
			}
		}
	}
}
