using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVFire : MVLogicObject, ILogicWorldObject
{
	private const float damageValue = 100f;

	private const float originalDamageRadius = 2.5f;

	private const float originalVisualObjectScale = 5f;

	private const float originalParticleSize = 4f;

	private const float fireHitBoxYOffset = 0.04f;

	private const float originalIntensity = 4f;

	private List<MVWorldObjectClient> woList = new List<MVWorldObjectClient>();

	private FireObject fireObject;

	private SphereVolumeIndicator rangeVis;

	private float damageRadius = 2.5f;

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
		fireObject.FireCollider.enabled = MVGameControllerBase.IEditModeUI == null;
		if (MVGameControllerBase.IEditModeUI != null)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Combine(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		SetupCulling(fireObject.VisualObject);
		rangeVis = UnityEngine.Object.Instantiate(PrefabPool.Instance.RangeVisualizationObject);
		rangeVis.transform.parent = fireObject.transform;
		rangeVis.transform.localPosition = Vector3.zero;
		rangeVis.Radius = damageRadius;
		rangeVis.Initialize(Id);
		SetFireToData();
		float num = damageRadius / 2.5f * 5f;
		SetFireHitBoxYOffset(num * 0.04f);
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
				float num2 = CalculateDamageModifier();
				float num3 = Time.deltaTime * 100f * (1f - num / damageRadius);
				num3 = Mathf.Clamp(num2 * num3, 0f, 100f);
				interactionDataHandlerBase.HandleInteraction(ProximityDamageAndImpulse.Create(num3, Vector3.zero, PlayerKilledByType.Fire), interactionIsLocal: true);
			}
		}
	}

	private float CalculateDamageModifier()
	{
		float num = 1f;
		float startSize = fireObject.ParticleSystem.startSize;
		if (startSize <= 4f)
		{
			return startSize / 12f;
		}
		return startSize / 13f;
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
			fireObject.ParticleSystem.startColor = new Color(array[0], array[1], array[2]);
		}
		float oldDamageRadius = CalculateOldDamageRadius();
		float num = CalculateOldScale(oldDamageRadius);
		SetFireHitBoxYOffset((0f - num) * 0.04f);
		if (Data.ContainsKey("I"))
		{
			fireObject.ParticleSystem.startSize = (float)Data["I"];
		}
		float startSize = fireObject.ParticleSystem.startSize;
		UpdateDamageRadius(startSize);
		float num2 = CalculateScale();
		SetFireHitBoxYOffset(num2 * 0.04f);
		UpdateScale(num2);
		RenewCullingSize();
		UpdateSoundVolume(startSize);
		if (startSize < 4f)
		{
			SetCandleAnimation();
		}
		else
		{
			SetOriginalAnimation();
		}
	}

	private float CalculateOldDamageRadius()
	{
		return fireObject.ParticleSystem.startSize * 0.625f / 2f;
	}

	private float CalculateOldScale(float oldDamageRadius)
	{
		return oldDamageRadius / 2.5f * 5f;
	}

	private void UpdateDamageRadius(float intensity)
	{
		damageRadius = intensity * 0.625f;
		damageRadius /= 2f;
		rangeVis.Radius = damageRadius;
	}

	private float CalculateScale()
	{
		return damageRadius / 2.5f * 5f;
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
		ParticleSystem.MinMaxCurve rate = emission.rate;
		rate.constantMax = 10f;
		emission.rate = rate;
		fireObject.ParticleSystem.startLifetime = 0.4f;
		fireObject.ParticleSystem.startSpeed = 0.5f;
		ParticleSystem.ShapeModule shape = fireObject.ParticleSystem.shape;
		shape.randomDirection = false;
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
		ParticleSystem.MinMaxCurve rate = emission.rate;
		rate.constantMax = 22f;
		emission.rate = rate;
		fireObject.ParticleSystem.startLifetime = 0.6f;
		fireObject.ParticleSystem.startSpeed = 1.6f;
		ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = fireObject.ParticleSystem.sizeOverLifetime;
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(0f, 0.4f);
		animationCurve.AddKey(0.6f, 1f);
		animationCurve.AddKey(1f, 0.1f);
		ParticleSystem.MinMaxCurve size = new ParticleSystem.MinMaxCurve(1f, animationCurve);
		sizeOverLifetime.size = size;
		ParticleSystem.ShapeModule shape = fireObject.ParticleSystem.shape;
		shape.randomDirection = true;
	}

	private void RenewCullingSize()
	{
		cullingSubscriberBase.Destroy();
		cullingSubscriberBase = new CullingSubscriberBase(damageRadius, WorldPosition, OnStateChanged);
	}

	public override void Destroy()
	{
		base.Destroy();
		if (MVGameControllerBase.IEditModeUI != null)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Remove(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
	}

	private void SetFireHitBoxYOffset(float offset)
	{
		if (offset < 0.04f && offset > 0f)
		{
			offset *= -1f;
		}
		Vector3 vector = fireObject.TriggerBoxEvents.transform.position;
		vector.y += offset;
		Vector3 localPosition = rangeVis.transform.localPosition;
		localPosition.y += offset;
		rangeVis.transform.localPosition = localPosition;
		fireObject.TriggerBoxEvents.transform.position = vector;
	}
}
