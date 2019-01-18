using System.Collections.Generic;
using UnityEngine;

public class PlayerJoinedNotification : PlayerNotification
{
	private Dictionary<string, string> Country = new Dictionary<string, string>
	{
		{
			"da_DK",
			TM._("Denmark")
		},
		{
			"de_DE",
			TM._("Germany")
		},
		{
			"en_US",
			TM._("The United States")
		},
		{
			"en_GB",
			TM._("The United Kingdom")
		},
		{
			"es_ES",
			TM._("Spain")
		},
		{
			"fi",
			TM._("Finland")
		},
		{
			"fr_FR",
			TM._("France")
		},
		{
			"id_ID",
			TM._("Indonesia")
		},
		{
			"it_IT",
			TM._("Italy")
		},
		{
			"nb_NO",
			TM._("Norway")
		},
		{
			"nl_NL",
			TM._("The Netherlands")
		},
		{
			"pl_PL",
			TM._("Poland")
		},
		{
			"pt_BR",
			TM._("Brazil")
		},
		{
			"ru_RU",
			TM._("Russia")
		},
		{
			"sv_SE",
			TM._("Sweden")
		},
		{
			"tr_TR",
			TM._("Turkey")
		},
		{
			"pt",
			TM._("Portugal")
		}
	};

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		MVPlayer mVPlayer = MVGameControllerBase.Game.MVPlayerContainer[(int)data[(byte)9]];
		string text = (string)data[(byte)12];
		if (text == MVGameControllerBase.GameSessionData.language)
		{
			lifeTime = NotificationLifetime.High;
			string empty = string.Empty;
			if (!Country.ContainsKey(text))
			{
				Debug.LogWarning("Country dictionary doesn't contain regioncode " + text + " returning English.");
				empty = Country["en_US"];
			}
			else
			{
				empty = Country[text];
			}
			NameLabel.text = mVPlayer.UserProfileData.UserName + TM._(" joined from ") + empty;
		}
		else
		{
			NameLabel.text = mVPlayer.UserProfileData.UserName + TM._(" joined!");
		}
	}
}
