using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class TeamAnnouncement : MonoBehaviour
{
	[SerializeField]
	private Text teamColorText;

	[SerializeField]
	private GameObject announcementDisabler;

	public bool AvatarRespawned { get; set; }

	public void InitializeAnnouncement()
	{
		if (AvatarRespawned)
		{
			bool flag = MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
			announcementDisabler.SetActive(flag);
			if (flag)
			{
				MVTeam teamFromActorNr = MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(MVGameControllerBase.Game.LocalPlayerActorNumber);
				Color teamColor = Styles.GetTeamColor(teamFromActorNr);
				teamColorText.color = teamColor;
				teamColorText.text = string.Format(TM._("{0} Team"), teamFromActorNr.ToString());
			}
			AvatarRespawned = false;
		}
	}
}
