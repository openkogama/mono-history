using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGameCoinChest : MVLogicObject
{
	private enum GameCoinChestClientState
	{
		Closed,
		Opening,
		Open
	}

	private const UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private GameCoinChestClientState state;

	private UseInteractor useInteractor;

	private MVGameCoinChestObject chestObject;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.CoinChest;

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
			if (state == GameCoinChestClientState.Closed)
			{
				chestObject.ModelSelector.Close();
			}
			else
			{
				chestObject.ModelSelector.Open();
			}
		}
	}

	public MVGameCoinChest(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVGameCoinChestPrefab, worldObjects)
	{
		chestObject = (MVGameCoinChestObject)component;
		if (chestObject.TriggerBoxEvents != null)
		{
			chestObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags |= InteractionFlags.CanUseLevel;
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
	}

	public override void Initialize()
	{
		SetupUseInteractor();
		useInteractor.UpdateData(Data);
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		base.Initialize();
		SetupCulling(chestObject.VisualObject);
	}

	private void SetupUseInteractor()
	{
		useInteractor = new UseInteractor(this, chestObject.useInteractionRotator, reset: false, chestObject.TriggerBoxEvents.Collider, OpenChest, IsUsable);
		LevelBasedUseRequirement useRequirement = new LevelBasedUseRequirement(chestObject.useInteractionRotator, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		chestObject.TriggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		chestObject.TriggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
	}

	public override void OnDataUpdate()
	{
		useInteractor.UpdateData(Data);
		base.OnDataUpdate();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		chestObject.ModelSelector.Close();
	}

	protected override void OnUpdate()
	{
		if (state == GameCoinChestClientState.Opening)
		{
			OpenChest(MVGameControllerBase.WOCM.AvatarLocal.Id);
		}
	}

	public bool IsUsable(MVInteractableBase avatarInteractable)
	{
		if (state != GameCoinChestClientState.Closed)
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
		MVGameControllerBase.Game.GameCoinManager.GameCoinChestCollect((int)Data["gameCoinAmount"]);
		if (MVClientSettings.IsFlagSet(ClientSettingFlags.GamePassSilentReleaseEnabled))
		{
			MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, instigatorID);
		}
		state = GameCoinChestClientState.Open;
		return true;
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (state == GameCoinChestClientState.Closed && (useInteractor.EvaluateRequirementsUsability() & (UseGUIResult.CanAfford | UseGUIResult.CannotAfford)) == 0)
		{
			state = GameCoinChestClientState.Opening;
		}
	}

	public override void Reset()
	{
		base.Reset();
		DoOpen();
	}

	private void DoOpen()
	{
		if (state == GameCoinChestClientState.Open || state == GameCoinChestClientState.Opening)
		{
			chestObject.ModelSelector.Close();
			state = GameCoinChestClientState.Closed;
		}
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
		base.Destroy();
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2.6f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}
}
