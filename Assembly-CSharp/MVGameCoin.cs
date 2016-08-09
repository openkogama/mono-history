using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVGameCoin : MVLogicObject
{
	public enum GameCoinClientState
	{
		Visible,
		PickedUp,
		ReShowing,
		Invisible
	}

	private MVGameCoinObject pickupObject;

	private UseInteractor useInteractor;

	private GameCoinClientState state;

	private bool isVisible = true;

	private float pickedUpStateDuration = 0.8f;

	private float reshowingStateDuration = 0.5f;

	private float pickedUpTime;

	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVGameCoin(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVGameCoinPrefab, worldObjects)
	{
		pickupObject = (MVGameCoinObject)component;
		interactionFlags |= InteractionFlags.CanUseLevel;
		if (pickupObject.TriggerBoxEvents != null)
		{
			pickupObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		SetVisible();
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
	}

	public override void Initialize()
	{
		SetupUserInteractor();
		useInteractor.UpdateData(Data);
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		base.Initialize();
		SetupCulling(pickupObject.VisualObject);
	}

	private void SetupUserInteractor()
	{
		useInteractor = new UseInteractor(Id, pickupObject.useInteractionRotator, reset: false, pickupObject.TriggerBoxEvents.Collider, OnPickup, IsCoinTakeable);
		LevelBasedUseRequirement useRequirement = new LevelBasedUseRequirement(pickupObject.useInteractionRotator, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		pickupObject.TriggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		pickupObject.TriggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
	}

	public override void OnDataUpdate()
	{
		useInteractor.UpdateData(Data);
		base.OnDataUpdate();
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
			pickupObject.TriggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
			pickupObject.TriggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
			useInteractor.OnDestroy(Data);
			useInteractor = null;
		}
		base.Destroy();
	}

	private void SetVisible()
	{
		if (!isVisible)
		{
			pickupObject.PickupItem.GreyIn();
			isVisible = true;
		}
		state = GameCoinClientState.Visible;
	}

	public virtual bool OnPickup(int instigatorID)
	{
		if (instigatorID == MVGameControllerBase.WOCM.AvatarLocal.Id && state == GameCoinClientState.Visible)
		{
			isVisible = false;
			pickupObject.PickupItem.GreyOut();
			state = GameCoinClientState.PickedUp;
			pickedUpTime = Time.realtimeSinceStartup;
			if ((bool)pickupObject.AudioSource)
			{
				pickupObject.AudioSource.Play();
			}
			pickupObject.Particles.Play();
			MVGameControllerBase.Game.GameCoinManager.GameCoinCollect();
			return true;
		}
		return false;
	}

	public bool IsCoinTakeable(MVInteractableBase avatarInteractable)
	{
		if (state != GameCoinClientState.Visible)
		{
			return false;
		}
		return true;
	}

	public override void Reset()
	{
		SetVisible();
	}

	protected override void OnUpdate()
	{
		if (state == GameCoinClientState.Visible || state == GameCoinClientState.Invisible)
		{
			if (!pickupObject.RotateLocal.enabled)
			{
				pickupObject.RotateLocal.enabled = true;
			}
		}
		else if (state == GameCoinClientState.PickedUp)
		{
			if (pickupObject.RotateLocal.enabled)
			{
				pickupObject.RotateLocal.enabled = false;
			}
			if (Time.realtimeSinceStartup - pickedUpTime > pickedUpStateDuration)
			{
				state = GameCoinClientState.ReShowing;
			}
		}
		else if (state == GameCoinClientState.ReShowing)
		{
			if (pickupObject.RotateLocal.enabled)
			{
				pickupObject.RotateLocal.enabled = false;
			}
			float num = pickedUpTime + pickedUpStateDuration;
			if (Time.realtimeSinceStartup - num > reshowingStateDuration)
			{
				state = GameCoinClientState.Invisible;
			}
		}
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if ((useInteractor.EvaluateRequirementsUsability() & purchaseOptions) == 0)
		{
			OnPickup(MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID).Id);
		}
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}
}
