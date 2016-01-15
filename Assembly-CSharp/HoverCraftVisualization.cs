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

	public float HoverPeriod = 0.8f;

	public float HoverAmplitude = 0.2f;

	public float rotateRollFactor = 7f;

	public float rollSpeed = 9.5f;

	public float rollMax = 30f;

	public float pitchMax = 30f;

	public float pitchSpeedTime = 10f;

	public float pitchFactor = 20f;

	public float damageParticleFactor = 10f;

	private Vector3 HoverOffset = Vector3.zero;

	private float angleDiff;

	private Quaternion prevWorldRot = Quaternion.identity;

	private float maxHealth;

	private float prevHealth;

	private bool vehicleIsUnoccupied;

	private float unoccupiedTime;

	private float vehicleAboutToBeRemovedTime = 3f;

	private Vector3 prevWorldPosition;

	private float minVolume = 0.03f;

	private float smoothMoveSpeed;

	private float smoothPitchFactor;

	private float smoothMoveSpeedTime = 5f;

	private Vector3 smoothVelocity = Vector3.zero;

	private float moveSpeed;

	private float smoothAcceleration;

	private float signedAcceleration;

	private VehicleSeatManager vehicleSeatManager;

	private Vector3 localHoverCraftHullRootBasePosition;

	private ParticleSystem fireSystem;

	private void Awake()
	{
		unoccupiedTime = Time.time;
	}

	public void Init(Transform hoverCraftHull, VehicleSeatManager vehicleSeatManager, float maxHealth, MVRuntimeDataVariableClampedFloat health, bool isInSpawner)
	{
		localHoverCraftHullRootBasePosition = hoverCraftHullRoot.transform.localPosition;
		base.isInSpawner = isInSpawner;
		this.vehicleSeatManager = vehicleSeatManager;
		Vector3 localPosition = hoverCraftHull.localPosition;
		Quaternion localRotation = hoverCraftHull.localRotation;
		hoverCraftHull.parent = hoverCraftHullRoot;
		hoverCraftHull.localPosition = localPosition;
		hoverCraftHull.localRotation = localRotation;
		prevWorldPosition = transform.position;
		prevWorldPosition.y = 0f;
		prevWorldRot = transform.rotation;
		this.maxHealth = maxHealth;
		prevHealth = health.Value;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object healthVal) =>
		{
			OnHealthChange((float)healthVal);
		}));
		vehicleBlinker.Init(hoverCraftHull.gameObject.GetComponentsInChildren<MeshFilter>());
		vehicleBlinker.Visible = true;
		if (isInSpawner)
		{
			enabled = false;
		}
		if (fire != null)
		{
			fireSystem = fire.GetComponent<ParticleSystem>();
		}
	}

	private void OnEnable()
	{
		vehicleBlinker.enabled = true;
		foreach (ParticleSystem thruster in thrusters)
		{
			thruster.enableEmission = true;
			thruster.gameObject.SetActive(value: true);
		}
		smoothMoveSpeed = 0f;
		smoothPitchFactor = 0f;
		prevWorldPosition = transform.position;
	}

	private void OnDisable()
	{
		if (vehicleBlinker != null)
		{
			vehicleBlinker.enabled = false;
		}
		foreach (ParticleSystem thruster in thrusters)
		{
			if (!(thruster == null))
			{
				thruster.enableEmission = false;
				thruster.Clear();
				thruster.gameObject.SetActive(value: false);
			}
		}
		if (fireSystem != null)
		{
			fireSystem.Clear();
		}
	}

	private void OnHealthChange(float newHealth)
	{
		if (newHealth < maxHealth && !ellipsoidParticleEmitter.emit)
		{
			ellipsoidParticleEmitter.emit = true;
			fireSystem.enableEmission = true;
		}
		if (newHealth == maxHealth && ellipsoidParticleEmitter.emit)
		{
			ellipsoidParticleEmitter.emit = false;
			fireSystem.enableEmission = false;
			return;
		}
		if (prevHealth > newHealth)
		{
			vehicleBlinker.StartBlinking(BlinkType.Damage, 0.3f);
		}
		float num = (1f - newHealth / maxHealth) * damageParticleFactor;
		ellipsoidParticleEmitter.minSize = num;
		ellipsoidParticleEmitter.maxSize = num;
		prevHealth = newHealth;
		fireSystem.startSize = num * 0.3f;
	}

	private void Update()
	{
		AnimateHullInertia();
		CalculateMovementValues();
		AnimateHullSpeed();
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

	private void PassiveAnim()
	{
		hoverCraftHullRoot.transform.localPosition = HoverOffset + Vector3.up * Mathf.Sin(Time.realtimeSinceStartup / HoverPeriod) * HoverAmplitude + localHoverCraftHullRootBasePosition;
	}

	private void AnimateHullInertia()
	{
		Vector3 vector = prevWorldRot * Vector3.forward;
		Vector3 vector2 = transform.rotation * Vector3.forward;
		angleDiff = Mathf.SmoothStep(angleDiff, MathFunctions.SignedAngle(vector.normalized, vector2.normalized, Vector3.up) / Time.deltaTime * rotateRollFactor, Time.deltaTime * rollSpeed);
		if (Mathf.Abs(angleDiff) < 0.0001f)
		{
			angleDiff = 0f;
		}
		prevWorldRot = transform.rotation;
		hoverCraftHullRoot.localRotation = Quaternion.AngleAxis(Mathf.Clamp(0f - angleDiff, 0f - rollMax, rollMax), Vector3.forward);
	}

	private void CalculateMovementValues()
	{
		Vector3 position = transform.position;
		position.y = 0f;
		Vector3 b = (position - prevWorldPosition) / Time.deltaTime;
		moveSpeed = b.magnitude;
		float num = smoothMoveSpeed;
		smoothMoveSpeed = Mathf.SmoothStep(smoothMoveSpeed, moveSpeed, Time.deltaTime * smoothMoveSpeedTime);
		smoothVelocity = Vector3.Lerp(smoothVelocity, b, Time.deltaTime * smoothMoveSpeedTime);
		signedAcceleration = (smoothMoveSpeed - num) / Time.deltaTime;
		prevWorldPosition = position;
	}

	private void AnimateHullSpeed()
	{
		float num = Vector3.Dot((transform.rotation * Vector3.forward).normalized, smoothVelocity);
		num = ((!((double)num > 0.0)) ? (-1f) : 1f);
		smoothAcceleration = Mathf.SmoothStep(smoothAcceleration, signedAcceleration * num, Time.deltaTime * smoothMoveSpeedTime);
		smoothPitchFactor = Mathf.SmoothStep(smoothPitchFactor, smoothAcceleration * pitchFactor, Time.deltaTime * pitchSpeedTime);
		hoverCraftHullRoot.localRotation *= Quaternion.AngleAxis(Mathf.Clamp(smoothPitchFactor, 0f - pitchMax, pitchMax), Vector3.right);
	}

	private void HandleSound()
	{
		moving.volume = Mathf.Clamp(smoothMoveSpeed / 70f, minVolume, 1f);
	}
}
