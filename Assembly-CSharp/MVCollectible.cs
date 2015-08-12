using System;
using System.Collections.Generic;
using UnityEngine;

public class MVCollectible : MVLogicObject
{
	public enum CollectibleClientState
	{
		Visible,
		PickedUp,
		ReShowing,
		Invisible
	}

	private const string prefabPath = "Prefabs/CollectibleObject";

	private float rotationSpeed = 0.6f;

	private GameObject pickupMesh;

	private GreyOutObjectScript pickupItem;

	private ObjectParticleEmitterScript particles;

	private WorldObjectEnableController worldObjectEnableController;

	private CollectibleClientState state;

	private bool isVisible = true;

	private float pickedUpStateDuration = 0.8f;

	private float reshowingStateDuration = 0.5f;

	private float pickedUpTime;

	private bool initializedInWorld;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVCollectible(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/CollectibleObject", worldObjects)
	{
		Create();
	}

	public MVCollectible(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects, string overridePrefabPath)
		: base(data, overridePrefabPath, worldObjects)
	{
		Create();
	}

	private void Create()
	{
		pickupItem = gameObject.GetComponent<GreyOutObjectScript>();
		pickupMesh = pickupItem.pickupObject;
		particles = gameObject.GetComponent<ObjectParticleEmitterScript>();
		worldObjectEnableController = gameObject.GetComponentInChildren<WorldObjectEnableController>();
		TriggerBoxEvents componentInChildren = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if (componentInChildren != null)
		{
			componentInChildren.TriggerEnter += triggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		AllWorldObjectTriggerBoxEvents componentInChildren2 = gameObject.GetComponentInChildren<AllWorldObjectTriggerBoxEvents>();
		if (componentInChildren2 != null)
		{
			componentInChildren2.TriggerEnter += allWorldObjectTriggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A AllWorldObjectTriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		SetVisible();
	}

	public override void Initialize()
	{
		base.Initialize();
		AllCollectiblesCollectedClient allCollectiblesCollectedClient = MVGameController.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		if (allCollectiblesCollectedClient == null)
		{
			allCollectiblesCollectedClient = MVGameController.Game.WinningConditionManager.CreateWinnerCondition<AllCollectiblesCollectedClient>(new object[0]);
		}
		allCollectiblesCollectedClient.SetLimit(allCollectiblesCollectedClient.Limit + 1);
		initializedInWorld = true;
	}

	public override void Destroy()
	{
		base.Destroy();
		if (initializedInWorld)
		{
			AllCollectiblesCollectedClient singletonWinnerConditionByType = MVGameController.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
			if (singletonWinnerConditionByType == null)
			{
				throw new Exception("AllCollectiblesCollected not found.");
			}
			if (singletonWinnerConditionByType.Limit == 0)
			{
				throw new Exception("AllCollectiblesCollected limit is 0");
			}
			singletonWinnerConditionByType.SetLimit(singletonWinnerConditionByType.Limit - 1);
			if (singletonWinnerConditionByType.Limit == 0)
			{
				MVGameController.Game.WinningConditionManager.RemoveWinnerCondition(singletonWinnerConditionByType.ID);
			}
		}
	}

	private void SetVisible()
	{
		if (!isVisible)
		{
			pickupItem.GreyIn();
			isVisible = true;
		}
		state = CollectibleClientState.Visible;
	}

	private void allWorldObjectTriggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (worldObjectEnableController.EnableState != EnableState.Enable)
		{
			return;
		}
		int num = MVGameController.WOCM.GetWorldObjectClient(e.instigatorWOID).OwnerActorNr;
		bool flag = MVGameController.Game.LocalPlayer.ActorNr == num;
		bool flag2 = num <= 0;
		if ((flag && isVisible) || (!flag && !flag2))
		{
			if ((bool)gameObject.GetComponent<AudioSource>())
			{
				gameObject.GetComponent<AudioSource>().Play();
			}
			particles.Play();
		}
	}

	public virtual void OnPickup(int actorNr)
	{
		if (actorNr == MVGameController.Game.LocalPlayer.ActorNr)
		{
			isVisible = false;
			pickupItem.GreyOut();
			state = CollectibleClientState.PickedUp;
			pickedUpTime = Time.realtimeSinceStartup;
		}
	}

	public override void Reset()
	{
		SetVisible();
	}

	protected override void OnUpdate()
	{
		if (state == CollectibleClientState.Visible || state == CollectibleClientState.Invisible)
		{
			float num = 0.35f + Mathf.Sin(Time.realtimeSinceStartup * 3f) * 0.05f;
			pickupMesh.transform.localScale = new Vector3(num, num, num);
			pickupMesh.transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed * 57.29578f, Space.Self);
		}
		else if (state == CollectibleClientState.PickedUp)
		{
			if (Time.realtimeSinceStartup - pickedUpTime > pickedUpStateDuration)
			{
				state = CollectibleClientState.ReShowing;
				return;
			}
			float num2 = 0f;
			pickupMesh.transform.localScale = new Vector3(num2, num2, num2);
		}
		else if (state == CollectibleClientState.ReShowing)
		{
			float num3 = pickedUpTime + pickedUpStateDuration;
			if (Time.realtimeSinceStartup - num3 > reshowingStateDuration)
			{
				state = CollectibleClientState.Invisible;
				return;
			}
			float num4 = 0.6f * (Time.realtimeSinceStartup - num3);
			pickupMesh.transform.localScale = new Vector3(num4, num4, num4);
		}
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (worldObjectEnableController.EnableState == EnableState.Enable && isVisible)
		{
			MVGameController.Game.TriggerBoxEnter(Id, e.instigatorWOID);
		}
	}
}
