using System.Collections.Generic;
using UnityEngine;

public class GameCoinLogic : UseRequirement
{
	private int purchaseAmount;

	private UseRequirementType requirementType = UseRequirementType.GameCoin;

	private Vector3 displayObjectOffset = new Vector3(0f, 1f, 0f);

	private GameObject displayGO;

	private GameCoinDisplayObject displayObject;

	private GameObject displayObjectRoot;

	private bool hasUseWhenFree = true;

	public int PurchaseAmount => purchaseAmount;

	public GameCoinLogic(GameObject root, bool hasUseButtonWhenFree = true)
	{
		hasUseWhenFree = hasUseButtonWhenFree;
		displayObjectRoot = root;
	}

	public GameCoinLogic(GameObject root, Vector3 displayObjectOffset, bool hasUseButtonWhenFree = true)
	{
		hasUseWhenFree = hasUseButtonWhenFree;
		this.displayObjectOffset = displayObjectOffset;
		displayObjectRoot = root;
	}

	public override UseGUIResult GetCanUseGUIResult()
	{
		if (purchaseAmount == 0)
		{
			if (hasUseWhenFree)
			{
				return UseGUIResult.NoCost;
			}
			return UseGUIResult.NoUseButton;
		}
		if (MVGameControllerBase.Game.GameCoinManager.GameCoinAmount >= purchaseAmount)
		{
			return UseGUIResult.CanAfford;
		}
		return UseGUIResult.CannotAfford;
	}

	public override bool IsActive()
	{
		return displayGO != null;
	}

	public override void PayUseCost()
	{
		MVGameControllerBase.Game.GameCoinManager.Consume(this);
	}

	public override void OnDataUpdate(Dictionary<object, object> data, int ownerID)
	{
		SetAmount(data, ownerID);
	}

	private void CreateDisplayObject()
	{
		displayGO = Object.Instantiate(PrefabPool.Instance.GameCoinDisplayPrefab.gameObject);
		displayGO.transform.parent = displayObjectRoot.transform;
		displayGO.transform.localPosition = displayObjectOffset;
		displayObject = displayGO.GetComponent<GameCoinDisplayObject>();
	}

	private void DestroyDisplayObject()
	{
		if (displayObject != null)
		{
			displayObject.Destroy();
			Object.Destroy(displayGO);
		}
	}

	private void SetAmount(Dictionary<object, object> data, int ownerID)
	{
		if (data.ContainsKey("gameCoinAmount"))
		{
			int num = purchaseAmount;
			purchaseAmount = (int)data["gameCoinAmount"];
			MVGameControllerBase.Game.GameCoinManager.ReportPurchaseAmountInEditor(purchaseAmount - num);
			if (purchaseAmount > 0 && displayObject == null)
			{
				CreateDisplayObject();
			}
			if (purchaseAmount == 0)
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add("gameCoinAmount", 0);
				MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(ownerID, dictionary);
				if (displayObject != null)
				{
					DestroyDisplayObject();
				}
			}
		}
		if (displayObject != null)
		{
			displayObject.SetAmount(purchaseAmount);
		}
	}

	public override void DestroyRequirement(Dictionary<object, object> data)
	{
		if (displayObject != null)
		{
			Object.Destroy(displayObject.gameObject);
		}
		if (data.ContainsKey("gameCoinAmount"))
		{
			MVGameControllerBase.Game.GameCoinManager.ReportPurchaseAmountInEditor(-purchaseAmount);
		}
	}

	public override ShowUseOption GetShowOption()
	{
		if (purchaseAmount == 0)
		{
			return ShowUseOption.Normal;
		}
		ShowUseOption showUseOption = ShowUseOption.UsingGameCoins;
		if (MVGameControllerBase.Game.GameCoinManager.GameCoinAmount >= purchaseAmount)
		{
			return showUseOption | ShowUseOption.GameCoinsEnough;
		}
		return showUseOption | ShowUseOption.GameCoinsInsufficient;
	}

	public override int GetRequirementValue()
	{
		return purchaseAmount;
	}

	public override UseRequirementType GetRequirementType()
	{
		return requirementType;
	}

	public override void CalculatePosAroundPivot(Vector3 pivot, float spacingAngle, float distanceFromPivot)
	{
		Vector3 vector = new Vector3(0f, 0f, distanceFromPivot) + pivot;
		Vector3 vector2 = pivot - vector;
		vector2 = Quaternion.Euler(0f, spacingAngle, 0f) * vector2;
		vector = vector2 + pivot;
		displayObject.transform.localPosition = vector;
		displayObject.transform.LookAt(pivot + displayObjectRoot.transform.position);
	}
}
