using System.Collections.Generic;
using UnityEngine;

public class MVSmoke : MVLogicObject, ILogicWorldObject
{
	private ParticleSystem particleSystem;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.Smoke;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVSmoke(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSmokePrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
		interactionFlags |= InteractionFlags.HasSettings;
		particleSystem = Object.Instantiate(PrefabPool.Instance.ParticleFluffySmoke, gameObject.transform.position, Quaternion.identity) as ParticleSystem;
		particleSystem.transform.parent = gameObject.transform;
		particleSystem.Stop();
		ToggleEmitter(toggle: false);
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gameObject);
		SetSmokeProperties();
		SetupSmokeCulling(particleSystem.startLifetime, gameObject);
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, OnInputStateUpdate);
		ToggleEmitter(InputSignalReceiver.CurrentlyIsHot);
		particleSystem.Play();
	}

	protected CullingSubscriberBase SetupSmokeCulling(float radius, GameObject lodGameObject)
	{
		cullingSubscriberBase.Destroy();
		cullingSubscriberBase = new CullingSubscriberBase(radius, WorldPosition, OnStateChanged);
		return cullingSubscriberBase;
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

	public override void OnDataUpdate()
	{
		SetSmokeProperties();
		SetupSmokeCulling(particleSystem.startLifetime, gameObject);
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	private void SetSmokeProperties()
	{
		if (Data.ContainsKey("color"))
		{
			float[] array = (float[])Data["color"];
			particleSystem.startColor = new Color(array[0], array[1], array[2], array[3]);
		}
		if (Data.ContainsKey("length"))
		{
			float startLifetime = (float)Data["length"];
			particleSystem.startLifetime = startLifetime;
		}
		if (Data.ContainsKey("wind"))
		{
			float value = (float)Data["wind"];
			particleSystem.transform.rotation = transform.rotation;
			ParticleSystem.ForceOverLifetimeModule forceOverLifetime = particleSystem.forceOverLifetime;
			AnimationCurve animationCurve = new AnimationCurve();
			animationCurve.AddKey(0.4f, 0f);
			animationCurve.AddKey(0.5f, value);
			forceOverLifetime.x = new ParticleSystem.MinMaxCurve(1f, animationCurve, animationCurve);
		}
	}
}
