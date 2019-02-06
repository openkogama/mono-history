using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVFire : MVLogicObject, ILogicWorldObject
{
	private List<MVWorldObjectClient> woList = new List<MVWorldObjectClient>();

	private FireObject fireObject;

	private SphereVolumeIndicator rangeVis;

	private const float damageValue = 100f;

	private const float originalDamageRadius = 2.5f;

	private float damageRadius = 2.5f;

	private const float originalVisualObjectScale = 5f;

	private const float originalParticleSize = 4f;

	private const float fireHitBoxYOffset = 0.04f;

	private const float originalIntensity = 4f;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.Fire;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public override Vector3 InputConnectorOffset => new Vector3(-1f, 0f, 0f);

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVFire(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVFirePrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
		InteractionFlags |= InteractionFlags.HasSettings;
		fireObject = (FireObject)component;
		fireObject.AudioSource.pitch = 1f + UnityEngine.Random.Range(-0.2f, 0.2f);
		fireObject.TriggerBoxEvents.TriggerEnter += TriggerAreaEnter;
		fireObject.TriggerBoxEvents.TriggerExit += TriggerAreaExit;
	}

	public override void Initialize()
	{
		base.Initialize();
		this.fireObject.FireCollider.enabled = MVGameControllerBase.EditModeUI == null;
		if (MVGameControllerBase.EditModeUI != null)
		{
			IEditModeUI editModeUI = MVGameControllerBase.EditModeUI;
			editModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Combine(editModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		SetupCulling(this.fireObject.VisualObject);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			rangeVis = UnityEngine.Object.Instantiate(PrefabPool.Instance.RangeVisualizationObject);
			rangeVis.transform.parent = this.fireObject.transform;
			rangeVis.transform.localPosition = Vector3.zero;
			rangeVis.SetRadius(damageRadius);
		}
		SetFireToData();
		float num = damageRadius / 2.5f * 5f;
		SetFireHitBoxYOffset(num * 0.04f);
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, OnInputStateUpdate);
		ToggleEmitter(InputSignalReceiver.CurrentlyIsHot);
		FireObject fireObject = this.fireObject;
		fireObject.OnFireObjectCreated = (Action)Delegate.Combine(fireObject.OnFireObjectCreated, new Action(OnFireObjectPlaced));
	}

	private void OnFireObjectPlaced()
	{
		FireObject fireObject = this.fireObject;
		fireObject.OnFireObjectCreated = (Action)Delegate.Remove(fireObject.OnFireObjectCreated, new Action(OnFireObjectPlaced));
		Debug.Log("is active now after subscribing");
		ToggleEmitter(InputSignalReceiver.CurrentlyIsHot);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		ParticleSystem.EmissionModule emission = fireObject.ParticleSystem.emission;
		emission.enabled = false;
		fireObject.enabled = false;
	}

	private void OnEditModeChange(EditModeChangeArgs arg)
	{
		fireObject.FireCollider.enabled = false;
		if (arg.playInEditor)
		{
			fireObject.FireCollider.enabled = true;
		}
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
		if (activeFlag && !fireObject.AudioSource.isPlaying)
		{
			fireObject.AudioSource.Play();
		}
		else if (!activeFlag && fireObject.AudioSource.isPlaying)
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
		if (InputSignalReceiver == null || !InputSignalReceiver.CurrentlyIsHot)
		{
			return;
		}
		for (int num = woList.Count - 1; num >= 0; num--)
		{
			MVWorldObjectClient mVWorldObjectClient = woList[num];
			if (mVWorldObjectClient == null || mVWorldObjectClient.GameObject == null)
			{
				woList.RemoveAt(num);
			}
			else
			{
				InteractionDataHandlerBase interactionDataHandlerBase = mVWorldObjectClient.InteractionDataHandlerBase;
				if (!(interactionDataHandlerBase == null))
				{
					float num2 = Vector3.Distance(mVWorldObjectClient.WorldPosition, WorldPosition);
					if (mVWorldObjectClient.Collider != null)
					{
						num2 = Vector3.Distance(mVWorldObjectClient.Collider.ClosestPointOnBounds(WorldPosition), WorldPosition);
					}
					float num3 = CalculateDamageModifier();
					float num4 = Time.deltaTime * 100f * (1f - num2 / damageRadius);
					num4 = Mathf.Clamp(num3 * num4, 0f, 100f);
					interactionDataHandlerBase.HandleInteraction(ProximityDamageAndImpulse.Create(num4, Vector3.zero, PlayerKilledByType.Fire), interactionIsLocal: true);
				}
			}
		}
	}

	private float CalculateDamageModifier()
	{
		float num = 1f;
		float startSizeMultiplier = fireObject.ParticleSystem.main.startSizeMultiplier;
		if (startSizeMultiplier <= 4f)
		{
			return startSizeMultiplier / 12f;
		}
		return startSizeMultiplier / 13f;
	}

	public override void OnDataUpdate()
	{
		SetFireToData();
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	private void SetFireToData()
	{
		if (Data.ContainsKey("C"))
		{
			float[] array = (float[])Data["C"];
			ParticleSystem.MainModule main = fireObject.ParticleSystem.main;
			main.startColor = new Color(array[0], array[1], array[2]);
		}
		float num = CalculateDamageRadius(fireObject.ParticleSystem.main.startSizeMultiplier);
		float num2 = CalculateScale(num);
		SetFireHitBoxYOffset((0f - num2) * 0.04f);
		if (Data.ContainsKey("I"))
		{
			ParticleSystem.MainModule main2 = fireObject.ParticleSystem.main;
			main2.startSizeMultiplier = (float)Data["I"];
		}
		float startSizeMultiplier = fireObject.ParticleSystem.main.startSizeMultiplier;
		UpdateDamageRadius(startSizeMultiplier);
		float num3 = CalculateScale(damageRadius);
		SetFireHitBoxYOffset(num3 * 0.04f);
		UpdateScale(num3);
		RenewCullingSize();
		UpdateSoundVolume(startSizeMultiplier);
		if (startSizeMultiplier < 4f)
		{
			SetCandleAnimation();
		}
		else
		{
			SetOriginalAnimation();
		}
	}

	private float CalculateDamageRadius(float intensity)
	{
		return intensity * 0.625f / 2f;
	}

	private float CalculateScale(float damageRadius)
	{
		return damageRadius / 2.5f * 5f;
	}

	private void UpdateDamageRadius(float intensity)
	{
		damageRadius = CalculateDamageRadius(intensity);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			rangeVis.SetRadius(damageRadius);
		}
	}

	private void UpdateScale(float scale)
	{
		Vector3 localScale = fireObject.TriggerBoxEvents.transform.localScale;
		localScale.x = scale;
		localScale.y = scale;
		localScale.z = scale;
		fireObject.TriggerBoxEvents.transform.localScale = localScale;
	}

	private void UpdateSoundVolume(float intensity)
	{
		fireObject.AudioSource.volume = fireObject.SoundIntensityScale.Evaluate(intensity);
	}

	private void SetCandleAnimation()
	{
		ParticleSystem.EmissionModule emission = fireObject.ParticleSystem.emission;
		emission.rateOverTimeMultiplier = 10f;
		ParticleSystem.MainModule main = fireObject.ParticleSystem.main;
		main.startLifetimeMultiplier = 0.4f;
		main.startSpeedMultiplier = 0.5f;
		ParticleSystem.ShapeModule shape = fireObject.ParticleSystem.shape;
		shape.randomDirectionAmount = 0f;
		ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = fireObject.ParticleSystem.sizeOverLifetime;
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(0f, 0.5f);
		animationCurve.AddKey(0.6f, 0.5f);
		animationCurve.AddKey(1f, 0.1f);
		ParticleSystem.MinMaxCurve size = new ParticleSystem.MinMaxCurve(1f, animationCurve);
		sizeOverLifetime.size = size;
	}

	private void SetOriginalAnimation()
	{
		ParticleSystem.EmissionModule emission = fireObject.ParticleSystem.emission;
		emission.rateOverTimeMultiplier = 22f;
		ParticleSystem.MainModule main = fireObject.ParticleSystem.main;
		main.startLifetimeMultiplier = 0.6f;
		main.startSpeedMultiplier = 1.6f;
		ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = fireObject.ParticleSystem.sizeOverLifetime;
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(0f, 0.4f);
		animationCurve.AddKey(0.6f, 1f);
		animationCurve.AddKey(1f, 0.1f);
		ParticleSystem.MinMaxCurve size = new ParticleSystem.MinMaxCurve(1f, animationCurve);
		sizeOverLifetime.size = size;
		ParticleSystem.ShapeModule shape = fireObject.ParticleSystem.shape;
		shape.randomDirectionAmount = 1f;
	}

	private void RenewCullingSize()
	{
		cullingSubscriberBase.Destroy();
		cullingSubscriberBase = new CullingSubscriberBase(damageRadius, WorldPosition, OnStateChanged);
	}

	public override void Destroy()
	{
		base.Destroy();
		if (this.fireObject != null)
		{
			FireObject fireObject = this.fireObject;
			fireObject.OnFireObjectCreated = (Action)Delegate.Remove(fireObject.OnFireObjectCreated, new Action(OnFireObjectPlaced));
		}
		if (MVGameControllerBase.EditModeUI != null)
		{
			IEditModeUI editModeUI = MVGameControllerBase.EditModeUI;
			editModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Remove(editModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
	}

	private void SetFireHitBoxYOffset(float offset)
	{
		Vector3 translation = new Vector3(0f, offset, 0f);
		fireObject.TriggerBoxEvents.transform.Translate(translation, Space.World);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			rangeVis.transform.Translate(translation, Space.Self);
		}
	}
}
