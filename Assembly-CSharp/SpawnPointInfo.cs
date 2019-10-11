using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPointInfo : MonoBehaviour, IGamePassShopContent
{
	[SerializeField]
	private Image TeamRequirementImage;

	public void Initialize(MVTeam teamRequirement)
	{
		bool darkTeam = false;
		if (teamRequirement == MVTeam.None || MVGameControllerBase.Game.TeamManager.GetTeamList().Count <= 1)
		{
			darkTeam = true;
		}
		TeamRequirementImage.color = Styles.GetTeamColor(teamRequirement, darkTeam);
	}

	public void Activate()
	{
	}

	public void Deactivate()
	{
	}
}
