using System.Collections;
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

	private PickupItemObjectScript pickupItem;

	private ObjectParticleEmitterScript particles;

	private CollectibleClientState state;

	private bool isVisible = true;

	private float pickedUpStateDuration = 0.8f;

	private float reshowingStateDuration = 0.5f;

	private float pickedUpTime;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVCollectible(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/CollectibleObject", worldObjects)
	{
		pickupItem = gameObject.GetComponent<PickupItemObjectScript>();
		pickupMesh = pickupItem.pickupObject;
		particles = gameObject.GetComponent<ObjectParticleEmitterScript>();
		TriggerBoxEvents componentInChildren = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if ((Object)(object)componentInChildren != (Object)null)
		{
			componentInChildren.TriggerEnter += triggerBoxEvents_TriggerEnter;
			componentInChildren.TriggerExit += triggerBoxEvents_TriggerExit;
		}
		else
		{
			Debug.LogError((object)("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name));
		}
		AllWorldObjectTriggerBoxEvents componentInChildren2 = gameObject.GetComponentInChildren<AllWorldObjectTriggerBoxEvents>();
		if ((Object)(object)componentInChildren2 != (Object)null)
		{
			componentInChildren2.TriggerEnter += allWorldObjectTriggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError((object)("A AllWorldObjectTriggerBoxEvents object is missing in PickupItem type: " + GetType().Name));
		}
	}

	private void allWorldObjectTriggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		int num = MVGameController.Instance.WOCM.GetWorldObjectClient(e.instigatorWOID).OwnerActorNr;
		bool flag = MVGameController.Instance.Game.LocalPlayer.ActorNr == num;
		bool flag2 = num <= 0;
		if ((flag && isVisible) || (!flag && !flag2))
		{
			if (Object.op_Implicit((Object)(object)gameObject.audio))
			{
				gameObject.audio.Play();
			}
			particles.Play();
		}
	}

	public virtual void OnPickup(int actorNr)
	{
		if (actorNr == MVGameController.Instance.Game.LocalPlayer.ActorNr)
		{
			isVisible = false;
			pickupItem.Take();
			MVGameController.Instance.Game.LocalPlayer.ChangeCollectibleCount(1);
			state = CollectibleClientState.PickedUp;
			pickedUpTime = Time.realtimeSinceStartup;
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		if (!isVisible)
		{
			pickupItem.Respawn();
			isVisible = true;
		}
		state = CollectibleClientState.Visible;
	}

	public override void Reset()
	{
		Initialize();
	}

	protected override void OnUpdate()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if (state == CollectibleClientState.Visible || state == CollectibleClientState.Invisible)
		{
			float num = 0.35f + Mathf.Sin(Time.realtimeSinceStartup * 3f) * 0.05f;
			pickupMesh.transform.localScale = new Vector3(num, num, num);
			pickupMesh.transform.RotateAround(Vector3.up, Time.deltaTime * rotationSpeed);
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
		if (isVisible)
		{
			MVGameController.Instance.Game.TriggerBoxEnter(Id, e.instigatorWOID);
		}
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
	}
}
