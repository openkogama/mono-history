using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class TeamAnnouncement : Notification
{
	[SerializeField]
	private Text teamColorText;

	[SerializeField]
	private NotificationFade fader;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		fader.Activate();
		base.Initialize(data);
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			MVTeam teamFromActorNr = MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(MVGameControllerBase.Game.LocalPlayer.ActorNr);
			Color teamColor = Styles.GetTeamColor(teamFromActorNr);
			teamColorText.color = teamColor;
			teamColorText.text = string.Format(TM._("{0} Team"), teamFromActorNr.ToString());
		}
		else
		{
			teamColorText.text = string.Empty;
		}
	}
}
