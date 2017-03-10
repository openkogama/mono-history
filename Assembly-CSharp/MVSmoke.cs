using System.Collections.Generic;
using UnityEngine;

public class MVSmoke : MVLogicObject, ILogicWorldObject
{
	private ParticleSystem particleSystem;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVSmoke(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSmokePrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
		particleSystem = Object.Instantiate(PrefabPool.Instance.ParticleFluffySmoke, gameObject.transform.position, Quaternion.identity) as ParticleSystem;
		particleSystem.transform.parent = gameObject.transform;
		ToggleEmitter(toggle: false);
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gameObject);
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, OnInputStateUpdate);
		ToggleEmitter(InputSignalReceiver.CurrentlyIsHot);
	}

	private void OnInputStateUpdate(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState == LogicInputState.FromColdToHot)
		{
			ToggleEmitter(toggle: true);
		}
		if (logicInputState == LogicInputState.FromHotToCold)
		{
			ToggleEmitter(toggle: false);
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		particleSystem.Clear();
	}

	private void ToggleEmitter(bool toggle)
	{
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.enabled = toggle;
	}
}
