using System;
using System.Collections.Generic;
using UnityEngine;

public class HoverCraftVisualization : VehicleVisualizationBase
{
	public Transform hoverCraftHullRoot;

	public ParticleEmitter ellipsoidParticleEmitter;

	public ParticleSystem fire;

	public VehicleBlinker vehicleBlinker;

	public List<ParticleSystem> thrusters = new List<ParticleSystem>();

	public AudioSource moving;

	private Vector3 HoverOffset = Vector3.zero;

	public float HoverPeriod = 0.8f;

	public float HoverAmplitude = 0.2f;

	public float rotateRollFactor = 7f;

	public float rollSpeed = 9.5f;

	public float rollMax = 30f;

	public float damageParticleFactor = 10f;

	private float angleDiff;

	private Quaternion prevWorldRot = Quaternion.identity;

	private float maxHealth;

	private float prevHealth;

	private VehicleSeatManager vehicleSeatManager;

	private Vector3 localHoverCraftHullRootBasePosition;

	private bool vehicleIsUnoccupied;

	private float unoccupiedTime = Time.time;

	private float vehicleAboutToBeRemovedTime = 3f;

	private Vector3 prevWorldPosition;

	private float smoothMoveSpeed;

	private float smoothPitchFactor;

	public float pitchMax = 30f;

	public float pitchSpeedTime = 10f;

	public float pitchFactor = 20f;

	private float smoothMoveSpeedTime = 5f;

	private float moveSpeed;

	private float smoothAcceleration;

	private float signedAcceleration;

	private Vector3 smoothVelocity = Vector3.zero;

	private float minVolume = 0.03f;

	public HoverCraftVisualization()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Init(Transform hoverCraftHull, VehicleSeatManager vehicleSeatManager, float maxHealth, MVRuntimeDataVariableClampedFloat health, bool isInSpawner)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		localHoverCraftHullRootBasePosition = ((Component)hoverCraftHullRoot).transform.localPosition;
		base.isInSpawner = isInSpawner;
		this.vehicleSeatManager = vehicleSeatManager;
		Vector3 localPosition = hoverCraftHull.localPosition;
		Quaternion localRotation = hoverCraftHull.localRotation;
		hoverCraftHull.parent = hoverCraftHullRoot;
		hoverCraftHull.localPosition = localPosition;
		hoverCraftHull.localRotation = localRotation;
		prevWorldPosition = ((Component)this).transform.position;
		prevWorldPosition.y = 0f;
		prevWorldRot = ((Component)this).transform.rotation;
		this.maxHealth = maxHealth;
		prevHealth = health.Value;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object healthVal) =>
		{
			OnHealthChange((float)healthVal);
		}));
		vehicleBlinker.Init(((Component)hoverCraftHull).gameObject.GetComponentsInChildren<MeshFilter>());
		vehicleBlinker.Visible = true;
		if (isInSpawner)
		{
			((Behaviour)this).enabled = false;
		}
	}

	private void OnEnable()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)vehicleBlinker).enabled = true;
		foreach (ParticleSystem thruster in thrusters)
		{
			thruster.enableEmission = true;
			((Component)thruster).gameObject.active = true;
		}
		smoothMoveSpeed = 0f;
		smoothPitchFactor = 0f;
		prevWorldPosition = ((Component)this).transform.position;
	}

	private void OnDisable()
	{
		if ((Object)(object)vehicleBlinker != (Object)null)
		{
			((Behaviour)vehicleBlinker).enabled = false;
		}
		foreach (ParticleSystem thruster in thrusters)
		{
			if (!((Object)(object)thruster == (Object)null))
			{
				thruster.enableEmission = false;
				thruster.Clear();
				((Component)thruster).gameObject.active = false;
			}
		}
		if ((Object)(object)fire != (Object)null)
		{
			((Component)fire).particleSystem.Clear();
		}
	}

	private void OnHealthChange(float newHealth)
	{
		if (newHealth < maxHealth && !ellipsoidParticleEmitter.emit)
		{
			ellipsoidParticleEmitter.emit = true;
			((Component)fire).particleSystem.enableEmission = true;
		}
		if (newHealth == maxHealth && ellipsoidParticleEmitter.emit)
		{
			ellipsoidParticleEmitter.emit = false;
			((Component)fire).particleSystem.enableEmission = false;
			return;
		}
		if (prevHealth > newHealth)
		{
			vehicleBlinker.StartBlinking(BlinkType.Damage, 0.3f);
		}
		float num = (1f - newHealth / maxHealth) * damageParticleFactor;
		ellipsoidParticleEmitter.minSize = num;
		ellipsoidParticleEmitter.maxSize = num;
		((Component)fire).particleSystem.startSize = num * 0.3f;
		prevHealth = newHealth;
	}

	private void Update()
	{
		PassiveAnim();
		if (!isInSpawner)
		{
			HandleUnoccupiedVehicle();
		}
		HandleSound();
	}

	private void HandleUnoccupiedVehicle()
	{
		if (!vehicleIsUnoccupied && vehicleSeatManager.OccupiedSeatsCount == 0)
		{
			unoccupiedTime = Time.time;
			vehicleIsUnoccupied = true;
		}
		if (vehicleIsUnoccupied && vehicleSeatManager.OccupiedSeatsCount > 0)
		{
			vehicleIsUnoccupied = false;
		}
		if (vehicleIsUnoccupied && 30f - (Time.time - unoccupiedTime) < vehicleAboutToBeRemovedTime)
		{
			vehicleBlinker.StartBlinking(BlinkType.AboutToExpire, 0.3f);
		}
	}

	private void FixedUpdate()
	{
		AnimateHullInertia();
		CalculateMovementValues();
		AnimateHullSpeed();
	}

	private void PassiveAnim()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		((Component)hoverCraftHullRoot).transform.localPosition = HoverOffset + Vector3.up * Mathf.Sin(Time.realtimeSinceStartup / HoverPeriod) * HoverAmplitude + localHoverCraftHullRootBasePosition;
	}

	private void AnimateHullInertia()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = prevWorldRot * Vector3.forward;
		Vector3 val2 = ((Component)this).transform.rotation * Vector3.forward;
		angleDiff = Mathf.SmoothStep(angleDiff, MathFunctions.SignedAngle(val.normalized, val2.normalized, Vector3.up) / Time.deltaTime * rotateRollFactor, Time.deltaTime * rollSpeed);
		if (Mathf.Abs(angleDiff) < 0.0001f)
		{
			angleDiff = 0f;
		}
		prevWorldRot = ((Component)this).transform.rotation;
		hoverCraftHullRoot.localRotation = Quaternion.AngleAxis(Mathf.Clamp(0f - angleDiff, 0f - rollMax, rollMax), Vector3.forward);
	}

	private void CalculateMovementValues()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		position.y = 0f;
		Vector3 val = (position - prevWorldPosition) / Time.deltaTime;
		moveSpeed = val.magnitude;
		float num = smoothMoveSpeed;
		smoothMoveSpeed = Mathf.SmoothStep(smoothMoveSpeed, moveSpeed, Time.deltaTime * smoothMoveSpeedTime);
		smoothVelocity = Vector3.Lerp(smoothVelocity, val, Time.deltaTime * smoothMoveSpeedTime);
		signedAcceleration = (smoothMoveSpeed - num) / Time.deltaTime;
		prevWorldPosition = position;
	}

	private void AnimateHullSpeed()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.rotation * Vector3.forward;
		float num = Vector3.Dot(val.normalized, smoothVelocity);
		num = ((!((double)num > 0.0)) ? (-1f) : 1f);
		smoothAcceleration = Mathf.SmoothStep(smoothAcceleration, signedAcceleration * num, Time.deltaTime * smoothMoveSpeedTime);
		smoothPitchFactor = Mathf.SmoothStep(smoothPitchFactor, smoothAcceleration * pitchFactor, Time.deltaTime * pitchSpeedTime);
		Transform val2 = hoverCraftHullRoot;
		val2.localRotation *= Quaternion.AngleAxis(Mathf.Clamp(smoothPitchFactor, 0f - pitchMax, pitchMax), Vector3.right);
	}

	private void HandleSound()
	{
		moving.volume = Mathf.Clamp(smoothMoveSpeed / 70f, minVolume, 1f);
	}
}
