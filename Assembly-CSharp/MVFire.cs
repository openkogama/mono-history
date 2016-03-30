using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVFire : MVLogicObject
{
	private const float damageValue = 100f;

	private const float damageRadius = 2.5f;

	private List<MVWorldObjectClient> woList = new List<MVWorldObjectClient>();

	private AudioSource aSource;

	private TriggerBoxEvents triggerBoxEvents;

	private ParticleSystem particleSystem;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVFire(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVFirePrefab, worldObjects)
	{
		aSource = gameObject.GetComponent<AudioSource>();
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		particleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
		aSource.pitch = 1f + Random.Range(-0.2f, 0.2f);
		triggerBoxEvents.TriggerEnter += TriggerAreaEnter;
		triggerBoxEvents.TriggerExit += TriggerAreaExit;
	}

	protected override void OnUpdate()
	{
		if (!InputState && InputLinkRefs.Count != 0)
		{
			return;
		}
		for (int i = 0; i < woList.Count; i++)
		{
			MVWorldObjectClient mVWorldObjectClient = woList[i];
			InteractionDataHandlerBase interactionDataHandlerBase = mVWorldObjectClient.InteractionDataHandlerBase;
			if (!(interactionDataHandlerBase == null))
			{
				float num = Vector3.Distance(mVWorldObjectClient.WorldPosition, WorldPosition);
				if (mVWorldObjectClient.Collider != null)
				{
					num = Vector3.Distance(mVWorldObjectClient.Collider.ClosestPointOnBounds(WorldPosition), WorldPosition);
				}
				float damage = Time.deltaTime * 100f * (1f - num / 2.5f);
				interactionDataHandlerBase.HandleInteraction(ProximityDamageAndImpulse.Create(damage, Vector3.zero, PlayerKilledByType.Fire), interactionIsLocal: true);
			}
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		OnInputLinkChanged();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.enabled = false;
	}

	public override void OnInputLinkChanged()
	{
		if (InputLinkRefs.Count == 0)
		{
			ToggleEmitter(activeFlag: true);
		}
		else
		{
			OnInputStateChanged();
		}
	}

	public override void OnInputStateChanged()
	{
		ToggleEmitter(InputState);
	}

	private void ToggleEmitter(bool activeFlag)
	{
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.enabled = activeFlag;
		if (activeFlag)
		{
			aSource.Play();
		}
		else
		{
			aSource.Stop();
		}
	}

	private void TriggerAreaEnter(object sender, TriggerEventArgs e)
	{
		woList.Add(MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID));
	}

	private void TriggerAreaExit(object sender, TriggerEventArgs e)
	{
		woList.Remove(MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID));
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));
	}
}
