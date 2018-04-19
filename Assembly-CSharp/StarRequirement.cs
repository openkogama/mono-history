using System.Collections.Generic;
using UnityEngine;

public class StarRequirement : UseRequirement
{
	private int starAmount;

	private UseRequirementType requirementType = UseRequirementType.Star;

	private Vector3 displayObjectOffset = new Vector3(0f, 0f, 0f);

	private GameObject displayGO;

	private StarDisplayObject displayObject;

	private GameObject displayObjectRoot;

	private bool hasUseWhenFree = true;

	public int StarAmount => starAmount;

	public override GameObject GameObject => displayGO.gameObject;

	public StarRequirement(GameObject root, bool hasUseButtonWhenFree = true)
	{
		hasUseWhenFree = hasUseButtonWhenFree;
		displayObjectRoot = root;
	}

	public StarRequirement(GameObject root, Vector3 displayOffset, bool hasUseButtonWhenFree = true)
	{
		hasUseWhenFree = hasUseButtonWhenFree;
		displayObjectOffset += displayOffset;
		displayObjectRoot = root;
	}

	public override UseGUIResult GetCanUseGUIResult()
	{
		if (starAmount == 0 || MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.Collectible) >= starAmount)
		{
			if (hasUseWhenFree)
			{
				return UseGUIResult.NoCost;
			}
			return UseGUIResult.NoUseButton;
		}
		if (MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.Collectible) >= starAmount)
		{
			return UseGUIResult.CanAfford;
		}
		return UseGUIResult.CannotAfford;
	}

	public override UseRequirementType GetRequirementType()
	{
		return requirementType;
	}

	public override int GetRequirementValue()
	{
		return starAmount;
	}

	public override ShowUseOption GetShowOption()
	{
		if (starAmount == 0)
		{
			return ShowUseOption.Normal;
		}
		ShowUseOption showUseOption = ShowUseOption.UsingStars;
		if (MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.Collectible) >= starAmount)
		{
			return showUseOption | ShowUseOption.StarsEnough;
		}
		return showUseOption | ShowUseOption.StarsInsufficient;
	}

	public override void OnDataUpdate(Dictionary<object, object> data, int ownerID)
	{
		if (data.ContainsKey("starAmount"))
		{
			starAmount = (int)data["starAmount"];
			if (displayObject == null)
			{
				CreateDisplayObject();
			}
			if (starAmount > 0)
			{
				displayObject.SetAmount(starAmount);
			}
			if (starAmount == 0)
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add("starAmount", 0);
				MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(ownerID, dictionary);
				Object.Destroy(displayObject.gameObject);
			}
		}
	}

	public override void SetScale(Vector3 scale)
	{
		displayObject.SetScale(scale / 2f);
	}

	private void CreateDisplayObject()
	{
		displayGO = Object.Instantiate(PrefabPool.Instance.StarDisplayPrefab.gameObject);
		displayGO.transform.parent = displayObjectRoot.transform;
		displayGO.transform.localPosition = displayObjectOffset;
		displayObject = displayGO.GetComponent<StarDisplayObject>();
	}

	public override void DestroyRequirement(Dictionary<object, object> data)
	{
		if (displayObject != null)
		{
			displayObject.Destroy();
			Object.Destroy(displayGO);
		}
	}

	public override void PayUseCost()
	{
	}

	public override bool IsActive()
	{
		return displayGO != null;
	}

	public override void CalculatePosAroundPivot(Vector3 pivot, float spacingAngle, float distanceFromPivot)
	{
		Vector3 vector = new Vector3(0f, 0f, distanceFromPivot) + pivot;
		Vector3 vector2 = pivot - vector;
		vector2 = Quaternion.Euler(0f, spacingAngle, 0f) * vector2;
		vector = vector2 + pivot;
		displayGO.transform.localPosition = vector;
		displayGO.transform.LookAt(pivot + displayObjectRoot.transform.position);
		displayObject.transform.position += displayObjectOffset;
	}
}
