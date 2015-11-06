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

	private GameCoinClientState state;

	private bool isVisible = true;

	private float pickedUpStateDuration = 0.8f;

	private float reshowingStateDuration = 0.5f;

	private float pickedUpTime;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVGameCoin(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/GameCoinObject", worldObjects)
	{
		pickupItem = gameObject.GetComponent<GreyOutObjectScript>();
		pickupMesh = pickupItem.pickupObject;
		particles = gameObject.GetComponent<ObjectParticleEmitterScript>();
		TriggerBoxEvents componentInChildren = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if (componentInChildren != null)
		{
			componentInChildren.TriggerEnter += triggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		SetVisible();
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
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
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		base.Initialize();
	}

	public override void Destroy()
	{
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
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

	public virtual void OnPickup(int actorNr)
	{
		if (actorNr == MVGameControllerBase.Game.LocalPlayer.ActorNr && state == GameCoinClientState.Visible)
		{
			Debug.Log("GameCoin OnPickup");
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
		}
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
		if (state == GameCoinClientState.Visible)
		{
			OnPickup(MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID).OwnerActorNr);
		}
	}
}
