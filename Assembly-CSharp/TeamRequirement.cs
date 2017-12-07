using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class TeamRequirement : UseRequirement
{
	public const string teamStr = "team";

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
		MVTeam mVTeam = MVGameControllerBase.Game.LocalPlayer.Team;
		if (team == MVTeam.None || (mVTeam == team && MVGameControllerBase.Game.TeamManager.TeamCount() != 1))
		{
			if (hasUseButtonWhenFree)
			{
				return UseGUIResult.NoCost;
			}
			return UseGUIResult.NoUseButton;
		}
		if (mVTeam == team && MVGameControllerBase.Game.TeamManager.TeamCount() != 1)
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
		if (data.ContainsKey("team"))
		{
			team = (MVTeam)(int)data["team"];
			if (team == MVTeam.None)
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add("team", 0);
				MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(ownerID, dictionary);
			}
			if (tintObject != null)
			{
				tintObject.TeamTint(team);
			}
			else
			{
				Debug.LogError("Unable to tint null.");
			}
		}
	}

	public override ShowUseOption GetShowOption()
	{
		if (team == MVTeam.None)
		{
			return ShowUseOption.Normal;
		}
		ShowUseOption showUseOption = ShowUseOption.UsingTeam;
		MVTeam mVTeam = MVGameControllerBase.Game.LocalPlayer.Team;
		if (mVTeam == team && MVGameControllerBase.Game.TeamManager.TeamCount() != 1)
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
