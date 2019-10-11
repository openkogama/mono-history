using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGamePointChest : MVGamePointRewardLogicObject
{
	public enum GamePointChestClientState
	{
		Closed,
		Opening,
		Open
	}

	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private bool canRespawn;

	private int respawnTime;

	private float pickUpTime;

	private const string respawnString = "respawnTime";

	private const string gamePointAwardedString = "gamePointAmount";

	private GamePointChestClientState state;

	private UseInteractor useInteractor;

	private MVGamePointChestObject chestObject;

	private const int gamePointAmountSettingDefaultValue = 30;

	private int gamePointsRewarded = 30;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.GamePointChest;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public override bool Visible
	{
		get
		{
			return chestObject.ModelSelector.IsVisible();
		}
		set
		{
			if (state == GamePointChestClientState.Closed)
			{
				chestObject.ModelSelector.Close();
			}
			else
			{
				chestObject.ModelSelector.Open();
			}
		}
	}

	public MVGamePointChest(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.GamePointChestPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags |= InteractionFlags.CanRespawn;
		chestObject = (MVGamePointChestObject)component;
		if (chestObject.TriggerBoxEvents != null)
		{
			chestObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		UpdateCanRespawn(Data);
	}

	public override void Initialize()
	{
		UpdateGamePointsRewardedAmount();
		UpdateChestSize();
		SetupUseInteractor();
		useInteractor.UpdateData(Data);
		base.Initialize();
		SetupCulling(chestObject.VisualObject, 5f);
		HandleStandaloneDisabling();
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
		UpdateCanRespawn(Data);
	}

	private void SetupUseInteractor()
	{
		useInteractor = new UseInteractor(this, chestObject.useInteractionRotator, reset: false, chestObject.TriggerBoxEvents.Collider, OpenChest, IsUsable);
		LevelBasedUseRequirement useRequirement = new LevelBasedUseRequirement(chestObject.useInteractionRotator, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		chestObject.TriggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		chestObject.TriggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
	}

	private void UpdateGamePointsRewardedAmount()
	{
		if (Data.ContainsKey("gamePointAmount"))
		{
			gamePointsRewarded = (int)Data["gamePointAmount"];
		}
		else
		{
			gamePointsRewarded = 30;
		}
	}

	private void UpdateChestSize()
	{
		float num = 0.25f;
		for (float num2 = 0f; num2 <= 1f; num2 += num)
		{
			bool flag = false;
			if (HandleDifferentChestSizeStages(num2))
			{
				break;
			}
		}
	}

	private void UpdateCanRespawn(Dictionary<object, object> newData)
	{
		canRespawn = newData.ContainsKey("respawnTime");
		if (canRespawn)
		{
			respawnTime = (int)newData["respawnTime"];
		}
		chestObject.ModelSelector.ShouldGreyOut = canRespawn;
	}

	private bool HandleDifferentChestSizeStages(float percentage)
	{
		int num = 100;
		if ((float)gamePointsRewarded <= (float)num * percentage)
		{
			transform.localScale = Vector3.one * percentage;
			return true;
		}
		return false;
	}

	public override void OnDataUpdate()
	{
		UpdateGamePointsRewardedAmount();
		UpdateChestSize();
		useInteractor.UpdateData(Data);
		base.OnDataUpdate();
		UpdateCanRespawn(Data);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		chestObject.ModelSelector.Close();
	}

	protected override void OnUpdate()
	{
		if (state == GamePointChestClientState.Opening)
		{
			OpenChest(MVGameControllerBase.LocalPlayer.WoId);
		}
		if (canRespawn && state == GamePointChestClientState.Open && Time.time > pickUpTime + (float)respawnTime)
		{
			Reset();
		}
	}

	public bool IsUsable(int id, MVInteractableBase avatarInteractable)
	{
		if (state != GamePointChestClientState.Closed)
		{
			return false;
		}
		return true;
	}

	private bool OpenChest(int instigatorID)
	{
		chestObject.ModelSelector.Open();
		if ((bool)chestObject.AudioSource)
		{
			chestObject.AudioSource.Play();
		}
		chestObject.Particles.Play();
		state = GamePointChestClientState.Open;
		return true;
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (state == GamePointChestClientState.Closed && (useInteractor.EvaluateRequirementsUsability() & purchaseOptions) == 0)
		{
			state = GamePointChestClientState.Opening;
			MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, e.instigatorWOID);
			FakeGamePointGainEffectManager.FakeGainEffect(gamePointsRewarded);
			pickUpTime = Time.time;
		}
	}

	public override void Reset()
	{
		base.Reset();
		SetToClosed();
		HandleStandaloneDisabling();
	}

	private void OnWinningConditionFulfilled(IWinningCondition winningCondition)
	{
		Reset();
	}

	private void SetToClosed()
	{
		if (state == GamePointChestClientState.Open || state == GamePointChestClientState.Opening)
		{
			chestObject.ModelSelector.Close();
			state = GamePointChestClientState.Closed;
		}
	}

	private void HandleStandaloneDisabling()
	{
		bool flag = false;
		flag = true;
		bool flag2 = MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit;
		if (flag && !flag2)
		{
			Disable();
		}
	}

	private void Disable()
	{
		state = GamePointChestClientState.Open;
		chestObject.ModelSelector.Disable();
		canRespawn = false;
	}

	public override MVWorldObjectClient Clone(int ownerActorNumber, int cloneGroupId, CloneBookkeeping cloneBookkeeping, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
	{
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		return base.Clone(ownerActorNumber, cloneGroupId, cloneBookkeeping, worldObjects, prototypes);
	}

	public override MVWorldObject DeepCopy()
	{
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		return base.DeepCopy();
	}

	public override void Destroy()
	{
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		if (useInteractor != null)
		{
			chestObject.TriggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
			chestObject.TriggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
			useInteractor.OnDestroy(Data);
			useInteractor = null;
		}
		if (MVGameControllerBase.Game != null)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
		}
		base.Destroy();
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2.6f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(chestObject.TriggerBoxEvents.transform.localPosition, chestObject.TriggerBoxEvents.transform.localScale);
	}
}
