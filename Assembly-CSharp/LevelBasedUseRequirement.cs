using System.Collections.Generic;
using UnityEngine;

public class LevelBasedUseRequirement : UseRequirement
{
	private int levelAmount;

	private static readonly UseRequirementType requirementType;

	private bool hasUseWhenFree = true;

	private LevelDisplayCube displayObject;

	private Vector3 displayObjectOffset = new Vector3(0f, 0f, 0f);

	private GameObject displayObjectRoot;

	private GameObject go;

	public override GameObject GameObject => displayObject.gameObject;

	public LevelBasedUseRequirement(GameObject root, bool hasUseButtonWhenFree = true)
	{
		hasUseWhenFree = hasUseButtonWhenFree;
		displayObjectRoot = root;
	}

	public LevelBasedUseRequirement(GameObject root, Vector3 displayOffset, bool hasUseButtonWhenFree = true)
	{
		hasUseWhenFree = hasUseButtonWhenFree;
		displayObjectOffset += displayOffset;
		displayObjectRoot = root;
	}

	public override UseGUIResult GetCanUseGUIResult()
	{
		if (levelAmount == 0 || MVGameControllerBase.Game.LocalPlayer.Level >= levelAmount)
		{
			if (hasUseWhenFree)
			{
				return UseGUIResult.NoCost;
			}
			return UseGUIResult.NoUseButton;
		}
		if (MVGameControllerBase.Game.LocalPlayer.Level >= levelAmount)
		{
			return UseGUIResult.CanAfford;
		}
		return UseGUIResult.CannotAfford;
	}

	public override void OnDataUpdate(Dictionary<object, object> data, int ownerID)
	{
		if (data.ContainsKey("levelAmount"))
		{
			levelAmount = (int)data["levelAmount"];
			if (displayObject == null)
			{
				CreateDisplayObject();
			}
			if (levelAmount > 0)
			{
				displayObject.SetAmount(levelAmount);
			}
			if (levelAmount == 0)
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add("levelAmount", 0);
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
		go = Object.Instantiate(PrefabPool.Instance.LevelDisplayPrefab.gameObject);
		go.transform.parent = displayObjectRoot.transform;
		go.transform.localPosition = displayObjectOffset;
		displayObject = go.GetComponent<LevelDisplayCube>();
		displayObject.Initialize();
	}

	public override void DestroyRequirement(Dictionary<object, object> data)
	{
		if (displayObject != null)
		{
			displayObject.Destroy();
			Object.Destroy(go);
		}
	}

	public override void PayUseCost()
	{
	}

	public override ShowUseOption GetShowOption()
	{
		if (levelAmount == 0)
		{
			return ShowUseOption.Normal;
		}
		ShowUseOption showUseOption = ShowUseOption.UsingLevels;
		if (MVGameControllerBase.Game.LocalPlayer.Level >= levelAmount)
		{
			return showUseOption | ShowUseOption.LevelEnough;
		}
		return showUseOption | ShowUseOption.LevelInsufficient;
	}

	public override int GetRequirementValue()
	{
		return levelAmount;
	}

	public override UseRequirementType GetRequirementType()
	{
		return requirementType;
	}

	public override bool IsActive()
	{
		return go != null;
	}

	public override void CalculatePosAroundPivot(Vector3 pivot, float spacingAngle, float distanceFromPivot)
	{
		Vector3 vector = new Vector3(0f, 0f, distanceFromPivot) + pivot;
		Vector3 vector2 = pivot - vector;
		vector2 = Quaternion.Euler(0f, spacingAngle, 0f) * vector2;
		vector = vector2 + pivot;
		go.transform.localPosition = vector;
		go.transform.LookAt(pivot + displayObjectRoot.transform.position);
		displayObject.transform.position += displayObjectOffset;
	}
}
