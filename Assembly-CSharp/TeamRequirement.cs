using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class TeamRequirement : UseRequirement
{
	private const float tintStrength = 0.3f;

	private TintObject tintObject;

	private MVTeam team = MVTeam.None;

	private bool hasUseButtonWhenFree;

	public override GameObject GameObject => null;

	public TintObject ToTint
	{
		set
		{
			tintObject = value;
		}
	}

	public TeamRequirement(TintObject tintObject, bool hasUseButtonWhenFree = true)
	{
		this.tintObject = tintObject;
		this.hasUseButtonWhenFree = hasUseButtonWhenFree;
	}

	public override UseGUIResult GetCanUseGUIResult()
	{
		MVTeam teamFromActorNr = MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(MVGameControllerBase.WOCM.AvatarLocal.OwnerActorNr);
		if (team == MVTeam.None)
		{
			if (hasUseButtonWhenFree)
			{
				return UseGUIResult.NoCost;
			}
			return UseGUIResult.NoUseButton;
		}
		if (teamFromActorNr == team)
		{
			return UseGUIResult.CanAfford;
		}
		return UseGUIResult.CannotAfford;
	}

	public override void PayUseCost()
	{
	}

	public override void DestroyRequirement(Dictionary<object, object> data)
	{
	}

	public override void OnDataUpdate(Dictionary<object, object> data, int ownerID)
	{
		if (!data.ContainsKey("team"))
		{
			return;
		}
		team = (MVTeam)(int)data["team"];
		if (team == MVTeam.None)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("team", 0);
			MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(ownerID, dictionary);
			tintObject.Tint(1f, 1f, 1f);
		}
		else if (tintObject != null)
		{
			switch (team)
			{
			case MVTeam.Blue:
				tintObject.Tint(0.3f, 0.3f, 1f, 0f);
				break;
			case MVTeam.Red:
				tintObject.Tint(1f, 0.3f, 0.3f, 0f);
				break;
			case MVTeam.Green:
				tintObject.Tint(0.3f, 1f, 0.3f, 0f);
				break;
			case MVTeam.Yellow:
				tintObject.Tint(1f, 1f, 0.3f, 0f);
				break;
			default:
				tintObject.Tint(1f, 1f, 1f, 0f);
				break;
			}
		}
		else
		{
			Debug.LogError("Unable to tint null.");
		}
	}

	public override ShowUseOption GetShowOption()
	{
		if (team == MVTeam.None)
		{
			return ShowUseOption.Normal;
		}
		ShowUseOption showUseOption = ShowUseOption.UsingTeam;
		MVTeam teamFromActorNr = MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(MVGameControllerBase.WOCM.AvatarLocal.OwnerActorNr);
		if (teamFromActorNr == team)
		{
			return showUseOption | ShowUseOption.TeamAllowed;
		}
		return showUseOption | ShowUseOption.TeamRestricted;
	}

	public override int GetRequirementValue()
	{
		return (int)team;
	}

	public override UseRequirementType GetRequirementType()
	{
		return UseRequirementType.Team;
	}

	public override bool IsActive()
	{
		return false;
	}

	public override void CalculatePosAroundPivot(Vector3 pivot, float spacingAngle, float distanceFromPivot)
	{
	}

	public override void SetScale(Vector3 scale)
	{
	}
}
