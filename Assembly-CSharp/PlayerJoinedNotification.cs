using System.Collections.Generic;
using UnityEngine;

public class PlayerJoinedNotification : PlayerNotification
{
	private Dictionary<string, string> Country = new Dictionary<string, string>
	{
		{ "da_DK", "Denmark" },
		{ "de_DE", "Germany" },
		{ "en_US", "The United States" },
		{ "en_GB", "The United Kingdom" },
		{ "es_ES", "Spain" },
		{ "fi", "Finland" },
		{ "fr_FR", "France" },
		{ "id_ID", "Indonesia" },
		{ "it_IT", "Italy" },
		{ "nb_NO", "Norway" },
		{ "nl_NL", "The Netherlands" },
		{ "pl_PL", "Poland" },
		{ "pt_BR", "Brazil" },
		{ "ru_RU", "Russia" },
		{ "sv_SE", "Sweden" },
		{ "tr_TR", "Turkey" },
		{ "pt", "Portuguese" }
	};

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[(int)data[(byte)9]];
		string text = (string)data[(byte)12];
		if (text == MVGameControllerBase.GameSessionData.language)
		{
			Lifetime = NotificationLifetime.High;
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
			NameLabel.text = mVPlayer.Username + TM._(" joined from ") + empty;
		}
		else
		{
			NameLabel.text = mVPlayer.Username + TM._(" joined!");
		}
	}
}
