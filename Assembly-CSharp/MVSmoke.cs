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
		particleSystem = Object.Instantiate(PrefabPool.Instance.ParticleFluffySmoke, gameObject.transform.position, Quaternion.identity) as ParticleSystem;
		particleSystem.transform.parent = gameObject.transform;
		particleSystem.Stop();
		ToggleEmitter(toggle: false);
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gameObject);
		SetupSmokeCulling(particleSystem.startLifetime, gameObject);
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
		float startLifetime = particleSystem.startLifetime;
		if (Data.ContainsKey("color"))
		{
			float[] array = (float[])Data["color"];
			particleSystem.startColor = new Color(array[0], array[1], array[2], array[3]);
		}
		float num = 0f;
		if (Data.ContainsKey("length"))
		{
			if (Data.ContainsKey("wind"))
			{
				num = (float)Data["wind"];
			}
			startLifetime = (float)Data["length"];
			SetupSmokeCulling(startLifetime * lengthCullingScale, gameObject);
			particleSystem.startLifetime = startLifetime / (1f + num);
			particleSystem.transform.rotation = transform.rotation;
			ParticleSystem.ForceOverLifetimeModule forceOverLifetime = particleSystem.forceOverLifetime;
			AnimationCurve animationCurve = new AnimationCurve();
			animationCurve.AddKey(0f, 0f);
			animationCurve.AddKey(0.05f, num);
			forceOverLifetime.x = new ParticleSystem.MinMaxCurve(1f, animationCurve, animationCurve);
		}
	}
}
