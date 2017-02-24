using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVFire : MVLogicObject, ILogicWorldObject
{
	private const float damageValue = 100f;

	private const float damageRadius = 2.5f;

	private List<MVWorldObjectClient> woList = new List<MVWorldObjectClient>();

	private FireObject fireObject;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public override Vector3 InputConnectorOffset => new Vector3(-3.488f, 0f, 0f);

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVFire(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVFirePrefab, worldObjects)
	{
		fireObject = (FireObject)component;
		fireObject.AudioSource.pitch = 1f + Random.Range(-0.2f, 0.2f);
		fireObject.TriggerBoxEvents.TriggerEnter += TriggerAreaEnter;
		fireObject.TriggerBoxEvents.TriggerExit += TriggerAreaExit;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(fireObject.VisualObject);
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, OnInputStateUpdate);
		ToggleEmitter(InputSignalReceiver.CurrentlyIsHot);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		ParticleSystem.EmissionModule emission = fireObject.ParticleSystem.emission;
		emission.enabled = false;
		fireObject.enabled = false;
	}

	private void OnInputStateUpdate(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState == LogicInputState.FromColdToHot)
		{
			ToggleEmitter(activeFlag: true);
		}
		if (logicInputState == LogicInputState.FromHotToCold)
		{
			ToggleEmitter(activeFlag: false);
		}
	}

	private void ToggleEmitter(bool activeFlag)
	{
		ParticleSystem.EmissionModule emission = fireObject.ParticleSystem.emission;
		emission.enabled = activeFlag;
		if (activeFlag)
		{
			fireObject.AudioSource.Play();
		}
		else
		{
			fireObject.AudioSource.Stop();
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

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (!InputSignalReceiver.CurrentlyIsHot)
		{
			return;
		}
		for (int i = 0; i < woList.Count; i++)
		{
			MVWorldObjectClient mVWorldObjectClient = woList[i];
			if (mVWorldObjectClient == null || mVWorldObjectClient.GameObject == null)
			{
				woList.RemoveAt(i);
				continue;
			}
			InteractionDataHandlerBase interactionDataHandlerBase = mVWorldObjectClient.InteractionDataHandlerBase;
			if (!(interactionDataHandlerBase == null))
			{
				float num = Vector3.Distance(mVWorldObjectClient.WorldPosition, WorldPosition);
				if (mVWorldObjectClient.Collider != null)
				{
					num = Vector3.Distance(mVWorldObjectClient.Collider.ClosestPointOnBounds(WorldPosition), WorldPosition);
				}
				float value = Time.deltaTime * 100f * (1f - num / 2.5f);
				value = Mathf.Clamp(value, 0f, 100f);
				interactionDataHandlerBase.HandleInteraction(ProximityDamageAndImpulse.Create(value, Vector3.zero, PlayerKilledByType.Fire), interactionIsLocal: true);
			}
		}
	}
}
