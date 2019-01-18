using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdminToolController : MonoBehaviour
{
	private struct DefaultBan(string reason, string duration, string format)
	{
		public string BanReason = reason;

		public string BanDuration = duration;

		public string BanDurationFormat = format;
	}

	[SerializeField]
	private Dropdown presetBansDropdown;

	[SerializeField]
	private Dropdown banDurationMultiplier;

	[SerializeField]
	private Text playerName;

	[SerializeField]
	private InputField duration;

	[SerializeField]
	private InputField reason;

	[SerializeField]
	private Button ownerKickButton;

	private static readonly Dictionary<string, int> durationMultiplier = new Dictionary<string, int>
	{
		{ "Hours", 1 },
		{ "Days", 24 },
		{ "Weeks", 168 }
	};

	private static readonly Dictionary<string, DefaultBan> defaultBanLookup = new Dictionary<string, DefaultBan>
	{
		{
			"Cheating",
			new DefaultBan("You are banned for cheating.", "7", "Days")
		},
		{
			"Abusive chat",
			new DefaultBan("You are banned for inappropriate language.", "24", "Hours")
		},
		{
			"Sexual behavior",
			new DefaultBan("You are banned for sexual behaviour.", "2", "Weeks")
		},
		{
			"Admin impersonation",
			new DefaultBan("You are banned for pretending to be an admin.", "2", "Weeks")
		}
	};

	public void Initialize(string playerNameString)
	{
		playerName.text = playerNameString;
		presetBansDropdown.onValueChanged.AddListener(OnDefaultBanDropdownChanged);
		ownerKickButton.gameObject.SetActive(MVGameControllerBase.GameMode == MVGameMode.Edit);
		OnDefaultBanDropdownChanged(0);
	}

	public void OnBanClicked()
	{
		MVPlayer player = GetPlayer(playerName.text);
		if (player == null)
		{
			Debug.LogWarning("Player non-existant in game");
		}
		else if (banDurationMultiplier.options[banDurationMultiplier.value].text == "Expel")
		{
			MVGameControllerBase.Game.OperationRequestSender.Expel(player, reason.text);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.GameObjectUI);
			});
		}
		else if (IsBanFieldsValid())
		{
			int num = int.Parse(duration.text);
			int hours = num * durationMultiplier[banDurationMultiplier.options[banDurationMultiplier.value].text];
			Debug.Log("Banning " + playerName.text + ": " + num + " " + banDurationMultiplier.options[banDurationMultiplier.value].text + " for: " + reason.text);
			MVGameControllerBase.Game.OperationRequestSender.Ban(hours, player, reason.text);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.GameObjectUI);
			});
		}
		else
		{
			Debug.Log(duration.text + " " + reason.text);
			Debug.LogError("Invalid admin fields. Specify reason and duration correctly.");
		}
	}

	public void OnKickClicked()
	{
		MVPlayer player = GetPlayer(playerName.text);
		if (player == null)
		{
			Debug.LogWarning("Player non-existant in game");
			return;
		}
		Debug.Log(playerName.text + " kicked by admin.");
		MVGameControllerBase.Game.OperationRequestSender.Kick(player, reason.text);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.GameObjectUI);
		});
	}

	public void OnRevokeEditRightsClicked()
	{
		MVPlayer player = GetPlayer(playerName.text);
		if (player != null)
		{
			OwnerOps.RevokeEditRightsAndKick(this, player);
		}
		else
		{
			Debug.LogWarning("Player is not present in session.");
		}
	}

	private void OnDefaultBanDropdownChanged(int option)
	{
		string text = presetBansDropdown.options[option].text;
		reason.text = defaultBanLookup[text].BanReason;
		duration.text = defaultBanLookup[text].BanDuration;
		string banDurationFormat = defaultBanLookup[text].BanDurationFormat;
		for (int i = 0; i < banDurationMultiplier.options.Count; i++)
		{
			if (banDurationMultiplier.options[i].text == banDurationFormat)
			{
				banDurationMultiplier.value = i;
				break;
			}
		}
	}

	private bool IsBanFieldsValid()
	{
		if (reason.text == string.Empty)
		{
			return false;
		}
		if (!int.TryParse(duration.text, out var _))
		{
			return false;
		}
		return true;
	}

	private static MVPlayer GetPlayer(string userName)
	{
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			if (value.UserProfileData.UserName == userName)
			{
				return value;
			}
		}
		return null;
	}
}
