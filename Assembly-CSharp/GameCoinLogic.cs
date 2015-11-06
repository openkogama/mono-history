using System.Collections.Generic;
using UnityEngine;

public class GameCoinLogic
{
	private int purchaseAmount;

	private static string displayObjectPath = "Prefabs/GameCoinDisplayObject";

	private Vector3 displayObjectOffset = new Vector3(0f, 1f, 0f);

	private GameCoinDisplayObject displayObject;

	private GameObject displayObjectRoot;

	public int PurchaseAmount => purchaseAmount;

	public GameCoinLogic(GameObject root, Dictionary<object, object> data)
	{
		Construct(root, data);
	}

	public GameCoinLogic(GameObject root, Dictionary<object, object> data, Vector3 displayObjectOffset)
	{
		this.displayObjectOffset = displayObjectOffset;
		Construct(root, data);
	}

	private void Construct(GameObject root, Dictionary<object, object> data)
	{
		displayObjectRoot = root;
		SetAmount(data);
	}

	public void OnDataUpdate(Dictionary<object, object> data)
	{
		SetAmount(data);
	}

	private void CreateDisplayObject()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load(displayObjectPath));
		gameObject.transform.parent = displayObjectRoot.transform;
		gameObject.transform.localPosition = displayObjectOffset;
		displayObject = gameObject.GetComponent<GameCoinDisplayObject>();
		displayObject.ObjectRoot = displayObjectRoot;
	}

	private void DestroyDisplayObject()
	{
		displayObject.Destroy();
	}

	private void SetAmount(Dictionary<object, object> data)
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
			if (purchaseAmount == 0 && displayObject != null)
			{
				DestroyDisplayObject();
			}
		}
		if (displayObject != null)
		{
			displayObject.SetAmount(purchaseAmount);
		}
	}

	public bool CanUse()
	{
		if (MVGameControllerBase.Game.GameCoinManager.GameCoinAmount >= purchaseAmount)
		{
			return true;
		}
		return false;
	}

	public bool ShowUseGUI()
	{
		if (CanUse())
		{
			MVGameControllerBase.IPlayModeUI.ShowEUseIcon(ShowUseOption.GameCoinsEnough);
			if (MVInputWrapper.GetBooleanControlDown(KogamaControls.Use) && MVGameControllerBase.Game.GameCoinManager.Consume(this))
			{
				return true;
			}
		}
		else
		{
			MVGameControllerBase.IPlayModeUI.ShowEUseIcon(ShowUseOption.GameCoinsInsufficient);
		}
		return false;
	}

	public void OnDestroy(Dictionary<object, object> data)
	{
		if (displayObject != null)
		{
			Object.Destroy(displayObject.gameObject);
		}
		if (data.ContainsKey("gameCoinAmount"))
		{
			Debug.Log("Removing purchase amount: " + purchaseAmount);
			MVGameControllerBase.Game.GameCoinManager.ReportPurchaseAmountInEditor(-purchaseAmount);
		}
	}
}
