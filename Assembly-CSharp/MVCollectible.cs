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

	private float rotationSpeed = 0.6f;

	private CollectibleClientState state;

	private MVCollectibleObject collectibleObject;

	private bool isVisible = true;

	private float pickedUpStateDuration = 0.8f;

	private float reshowingStateDuration = 0.5f;

	private float pickedUpTime;

	private bool initializedInWorld;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVCollectible(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVCollectiblePrefab, worldObjects)
	{
		Create();
	}

	public MVCollectible(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects, ObjectPrefab overridePrefab)
		: base(data, overridePrefab, worldObjects)
	{
		Create();
	}

	private void Create()
	{
		collectibleObject = (MVCollectibleObject)component;
		if (collectibleObject.TriggerBoxEvents != null)
		{
			collectibleObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		if (collectibleObject.AllWorldObjectTriggerBoxEvents != null)
		{
			collectibleObject.AllWorldObjectTriggerBoxEvents.TriggerEnter += allWorldObjectTriggerBoxEvents_TriggerEnter;
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
		AllCollectiblesCollectedClient allCollectiblesCollectedClient = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		if (allCollectiblesCollectedClient == null)
		{
			allCollectiblesCollectedClient = MVGameControllerBase.Game.WinningConditionManager.CreateWinnerCondition<AllCollectiblesCollectedClient>(new object[0]);
		}
		allCollectiblesCollectedClient.SetLimit(allCollectiblesCollectedClient.Limit + 1);
		initializedInWorld = true;
	}

	public override void Destroy()
	{
		base.Destroy();
		if (initializedInWorld)
		{
			AllCollectiblesCollectedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
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
				MVGameControllerBase.Game.WinningConditionManager.RemoveWinnerCondition(singletonWinnerConditionByType.ID);
			}
		}
	}

	private void SetVisible()
	{
		if (!isVisible)
		{
			collectibleObject.PickupItem.GreyIn();
			isVisible = true;
		}
		state = CollectibleClientState.Visible;
	}

	private void allWorldObjectTriggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (collectibleObject.WorldObjectEnableController.EnableState != EnableState.Enable)
		{
			return;
		}
		int num = MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID).OwnerActorNr;
		bool flag = MVGameControllerBase.Game.LocalPlayer.ActorNr == num;
		bool flag2 = num <= 0;
		if ((flag && isVisible) || (!flag && !flag2))
		{
			if ((bool)collectibleObject.AudioSource)
			{
				collectibleObject.AudioSource.Play();
			}
			collectibleObject.Particles.Play();
		}
	}

	public virtual void OnPickup(int actorNr)
	{
		if (actorNr == MVGameControllerBase.Game.LocalPlayer.ActorNr)
		{
			isVisible = false;
			collectibleObject.PickupItem.GreyOut();
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
			collectibleObject.PickupMesh.transform.localScale = new Vector3(num, num, num);
			collectibleObject.PickupMesh.transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed * 57.29578f, Space.Self);
		}
		else if (state == CollectibleClientState.PickedUp)
		{
			if (Time.realtimeSinceStartup - pickedUpTime > pickedUpStateDuration)
			{
				state = CollectibleClientState.ReShowing;
				return;
			}
			float num2 = 0f;
			collectibleObject.PickupMesh.transform.localScale = new Vector3(num2, num2, num2);
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
			collectibleObject.PickupMesh.transform.localScale = new Vector3(num4, num4, num4);
		}
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (collectibleObject.WorldObjectEnableController.EnableState == EnableState.Enable && isVisible)
		{
			MVGameControllerBase.Game.TriggerBoxEnter(Id, e.instigatorWOID);
		}
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}
}
