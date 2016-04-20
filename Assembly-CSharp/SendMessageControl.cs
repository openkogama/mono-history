using System.Collections.Generic;
using System.Text.RegularExpressions;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SendMessageControl : MonoBehaviour
{
	private string helpString = "/h";

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
		if (!(chatMsg == string.Empty))
		{
			if (chatMsg.Length > 256)
			{
				chatMsg = chatMsg.Substring(0, 256);
			}
			if (chatMsg == helpString)
			{
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, CreateHelpTxt());
			}
			else
			{
				MVGameControllerBase.OperationRequests.PostGameMsg(MVGameMsgType.Chat, new Dictionary<object, object>
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
	}

	public string CreateHelpTxt()
	{
		string text = TM._("<M> Menu\n<H> Toggle HD Mode");
		switch (MVGameControllerBase.GameMode)
		{
		case MVGameMode.CharacterEditor:
			return string.Empty;
		case MVGameMode.Edit:
			text += TM._("\n\n<PgDown> Move Workplane Down\n<PgUp> Move Workplane Up\n<TAB> Show Players\n<P> Play Mode\n<P> Edit Mode\n<1> Edit Cube\n<2> Delete Cube\n<3> Paint Cube\n<G> Toggle Grid Snap Size\n");
			text += TM._("<F> Toggle Workplane\n<H> Toggle Vanity Item\n<L> Toggle Show Logic Cubes\n<R> Change Cube Material\n<I> Open Inventory\n<N> Create New Model\n<V> Focus on selected object");
			break;
		case MVGameMode.Play:
			text += TM._("\n\n<WASD> Move\n<Space> Jump\n<K> Respawn\n<Left Mouse> Fire Weapon\n<Q> Drop currently equipped weapon");
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
