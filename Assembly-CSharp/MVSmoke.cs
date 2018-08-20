using System.Collections.Generic;
using UnityEngine;

public class MVSmoke : MVLogicObject, ILogicWorldObject
{
	private ParticleSystem particleSystem;

	private float lengthCullingScale = 1.5f;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.Smoke;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVSmoke(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSmokePrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
		interactionFlags |= InteractionFlags.HasSettings;
		particleSystem = Object.Instantiate(PrefabPool.Instance.ParticleFluffySmoke, gameObject.transform.position, Quaternion.identity);
		particleSystem.transform.parent = gameObject.transform;
		particleSystem.Stop();
		ToggleEmitter(toggle: false);
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gameObject);
		SetupSmokeCulling(particleSystem.main.startLifetimeMultiplier, gameObject);
		SetSmokeProperties();
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, OnInputStateUpdate);
		ToggleEmitter(InputSignalReceiver.CurrentlyIsHot);
		particleSystem.Play();
	}

	protected CullingSubscriberBase SetupSmokeCulling(float radius, GameObject lodGameObject)
	{
		if (cullingSubscriberBase != null)
		{
			cullingSubscriberBase.Destroy();
		}
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
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	private void SetSmokeProperties()
	{
		float startLifetimeMultiplier = particleSystem.main.startLifetimeMultiplier;
		if (Data.ContainsKey("color"))
		{
			float[] array = (float[])Data["color"];
			ParticleSystem.MainModule main = particleSystem.main;
			main.startColor = new Color(array[0], array[1], array[2], array[3]);
		}
		float num = 0f;
		if (Data.ContainsKey("length"))
		{
			if (Data.ContainsKey("wind"))
			{
				num = (float)Data["wind"];
			}
			startLifetimeMultiplier = (float)Data["length"];
			SetupSmokeCulling(startLifetimeMultiplier * lengthCullingScale, gameObject);
			ParticleSystem.MainModule main2 = particleSystem.main;
			main2.startLifetimeMultiplier = startLifetimeMultiplier / (1f + num);
			particleSystem.transform.rotation = transform.rotation;
			ParticleSystem.ForceOverLifetimeModule forceOverLifetime = particleSystem.forceOverLifetime;
			AnimationCurve animationCurve = new AnimationCurve();
			animationCurve.AddKey(0f, 0f);
			animationCurve.AddKey(0.05f, num);
			forceOverLifetime.x = new ParticleSystem.MinMaxCurve(1f, animationCurve, animationCurve);
		}
	}
}
