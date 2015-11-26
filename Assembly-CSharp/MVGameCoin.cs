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

	private const string prefabPath = "Prefabs/GameCoinObject";

	private float rotationSpeed = 0.9f;

	private GameObject pickupMesh;

	private GreyOutObjectScript pickupItem;

	private ObjectParticleEmitterScript particles;

	private UseInteractor useInteractor;

	private GameCoinClientState state;

	private bool isVisible = true;

	private float pickedUpStateDuration = 0.8f;

	private float reshowingStateDuration = 0.5f;

	private float pickedUpTime;

	private TriggerBoxEvents triggerBoxEvents;

	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVGameCoin(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/GameCoinObject", worldObjects)
	{
		pickupItem = gameObject.GetComponent<GreyOutObjectScript>();
		pickupMesh = pickupItem.pickupObject;
		particles = gameObject.GetComponent<ObjectParticleEmitterScript>();
		interactionFlags |= InteractionFlags.CanUseLevel;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if (triggerBoxEvents != null)
		{
			triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		useInteractor = new UseInteractor(Id, gameObject, reset: false, triggerBoxEvents.GetComponent<Collider>(), OnPickup, IsCoinTakeable);
		LevelBasedUseRequirement useRequirement = new LevelBasedUseRequirement(gameObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		SetVisible();
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
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

	public override void Initialize()
	{
		useInteractor.UpdateData(Data);
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		base.Initialize();
	}

	public override void Destroy()
	{
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		useInteractor.OnDestroy(Data);
		base.Destroy();
	}

	private void SetVisible()
	{
		if (!isVisible)
		{
			pickupItem.GreyIn();
			isVisible = true;
		}
		state = GameCoinClientState.Visible;
	}

	public virtual bool OnPickup(int instigatorID)
	{
		if (instigatorID == MVGameControllerBase.WOCM.AvatarLocal.Id && state == GameCoinClientState.Visible)
		{
			isVisible = false;
			pickupItem.GreyOut();
			state = GameCoinClientState.PickedUp;
			pickedUpTime = Time.realtimeSinceStartup;
			if ((bool)gameObject.GetComponent<AudioSource>())
			{
				gameObject.GetComponent<AudioSource>().Play();
			}
			particles.Play();
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
			pickupMesh.transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed * 57.29578f);
		}
		else if (state == GameCoinClientState.PickedUp)
		{
			if (Time.realtimeSinceStartup - pickedUpTime > pickedUpStateDuration)
			{
				state = GameCoinClientState.ReShowing;
			}
		}
		else if (state == GameCoinClientState.ReShowing)
		{
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
}
