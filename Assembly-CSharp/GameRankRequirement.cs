using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class GameRankRequirement : UseRequirement
{
	private GamePassTier requiredRank;

	private UseRequirementType requirementType = UseRequirementType.Star;

	private MVWorldObjectClient worldObject;

	private Vector3 displayObjectOffset = new Vector3(0f, 0f, 0f);

	private GameObject displayGO;

	private GameRankDisplayObject displayObject;

	private GameObject displayObjectRoot;

	private bool hasUseWhenFree = true;

	private WorldObjectType worldObjectType;

	public GamePassTier RequiredRank => requiredRank;

	public override GameObject GameObject => displayGO.gameObject;

	public GameRankRequirement(GameObject root, MVWorldObjectClient worldObject, bool hasUseButtonWhenFree = true)
	{
		hasUseWhenFree = hasUseButtonWhenFree;
		this.worldObject = worldObject;
		displayObjectRoot = root;
	}

	public GameRankRequirement(GameObject root, Vector3 displayOffset, MVWorldObjectClient worldObject, bool hasUseButtonWhenFree = true)
	{
		hasUseWhenFree = hasUseButtonWhenFree;
		this.worldObject = worldObject;
		displayObjectOffset += displayOffset;
		displayObjectRoot = root;
	}

	public override UseGUIResult GetCanUseGUIResult()
	{
		if (requiredRank == GamePassTier.Tier0 || (int)GetLocalPLayerRank() >= (int)requiredRank)
		{
			if (hasUseWhenFree)
			{
				return UseGUIResult.NoCost;
			}
			return UseGUIResult.NoUseButton;
		}
		if ((int)GetLocalPLayerRank() >= (int)requiredRank)
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
		return (int)requiredRank;
	}

	public override ShowUseOption GetShowOption()
	{
		if (requiredRank == GamePassTier.Tier0)
		{
			return ShowUseOption.Normal;
		}
		ShowUseOption showUseOption = ShowUseOption.UsingGameRank;
		if ((int)GetLocalPLayerRank() >= (int)requiredRank)
		{
			return showUseOption | ShowUseOption.GameRankEnough;
		}
		return showUseOption | ShowUseOption.GameRankInsufficient;
	}

	public override void OnDataUpdate(Dictionary<object, object> data, int ownerID)
	{
		if (!data.ContainsKey("RequiredRank"))
		{
			return;
		}
		GamePassTier gamePassTier = requiredRank;
		requiredRank = (GamePassTier)(int)data["RequiredRank"];
		if (displayObject == null)
		{
			CreateDisplayObject();
			if (ShouldBeDisplayedInTierShop(worldObject))
			{
				MVGameControllerBase.Game.GameTierShopRepository.AddItemToTierShop(requiredRank, worldObject.DocumentationType, worldObject);
			}
		}
		if (gamePassTier != GamePassTier.Tier0 && gamePassTier != requiredRank && ShouldBeDisplayedInTierShop(worldObject))
		{
			MVGameControllerBase.Game.GameTierShopRepository.RemoveItemToTierShop(gamePassTier, worldObject.DocumentationType, worldObject.Id);
			MVGameControllerBase.Game.GameTierShopRepository.AddItemToTierShop(requiredRank, worldObject.DocumentationType, worldObject);
		}
		if ((int)requiredRank > 0)
		{
			displayObject.SetAmount(requiredRank);
		}
		if (requiredRank == GamePassTier.Tier0)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("RequiredRank", 0);
			MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(ownerID, dictionary);
			Object.Destroy(displayObject.gameObject);
		}
	}

	public void OnDelete()
	{
		if (MVGameControllerBase.IsAlive && ShouldBeDisplayedInTierShop(worldObject))
		{
			MVGameControllerBase.Game.GameTierShopRepository.RemoveItemToTierShop(requiredRank, worldObject.DocumentationType, worldObject.Id);
		}
	}

	public override void SetScale(Vector3 scale)
	{
		displayObject.SetScale(scale);
	}

	private bool ShouldBeDisplayedInTierShop(MVWorldObjectClient worldObject)
	{
		return worldObject is MVPickupItemBase || worldObject is MVWorldObjectSpawnerVehicle;
	}

	private void CreateDisplayObject()
	{
		displayGO = Object.Instantiate(PrefabPool.Instance.GameRankDisplayPrefab.gameObject);
		displayGO.transform.parent = displayObjectRoot.transform;
		displayGO.transform.localPosition = displayObjectOffset;
		displayObject = displayGO.GetComponent<GameRankDisplayObject>();
	}

	public override void DestroyRequirement(Dictionary<object, object> data)
	{
		if (displayObject != null)
		{
			displayObject.Destroy();
			Object.Destroy(displayGO);
			OnDelete();
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

	private GamePassTier GetLocalPLayerRank()
	{
		return GamePassesManager.PlayerPlanetData.gamePassTier;
	}
}
