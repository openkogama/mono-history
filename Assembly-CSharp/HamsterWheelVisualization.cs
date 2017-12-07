using System;
using System.Collections.Generic;
using UnityEngine;

public class HamsterWheelVisualization : VehicleVisualizationBase
{
	public enum SpeedState
	{
		Idle,
		Moving
	}

	public GameObject wheel;

	[SerializeField]
	private VehicleBlinker vehicleBlinker;

	[SerializeField]
	private Transform hamsterWheelVisualizationRoot;

	private VehicleSeatManager vehicleSeatManager;

	public AvatarBlobShadowController blobShadow;

	private float curHealth;

	private Vector3 prevPosition;

	private Vector3 velocity;

	private SpeedState speedState;

	public AudioSource audioSourceRolling;

	public AudioSource audioSourceWind;

	private bool vehicleIsUnoccupied;

	private float unoccupiedTime;

	private float vehicleAboutToBeRemovedTime = 3f;

	private void Awake()
	{
		unoccupiedTime = Time.time;
	}

	public void Init(VehicleSeatManager vehicleSeatManager, float fullHealth, MVRuntimeDataVariableClampedFloat health, MVRuntimeDataVariable isMovingForward, MVRuntimeDataVariable isMovingBackwards, MVRuntimeDataVariable isGrounded, bool isInSpawner)
	{
		base.isInSpawner = isInSpawner;
		this.vehicleSeatManager = vehicleSeatManager;
		vehicleSeatManager.OnSeatOccupiedChange = (VehicleSeatManager.OnSeatOccupiedChangeDelegate)Delegate.Combine(vehicleSeatManager.OnSeatOccupiedChange, new VehicleSeatManager.OnSeatOccupiedChangeDelegate(OnSeatOccupiedChange));
		curHealth = health.Value;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object healthVal) =>
		{
			OnHealthChange((float)healthVal);
		}));
		isGrounded.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isGrounded.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object val) =>
		{
			OnGroundedChange((bool)val);
		}));
		vehicleBlinker.Init(hamsterWheelVisualizationRoot.gameObject.GetComponentsInChildren<MeshFilter>());
		vehicleBlinker.Visible = true;
		if (isInSpawner)
		{
			enabled = false;
		}
		blobShadow = hamsterWheelVisualizationRoot.GetComponentInChildren<AvatarBlobShadowController>();
		if (blobShadow != null)
		{
			blobShadow.enabled = false;
		}
		prevPosition = wheel.transform.position;
		cullDistance = 100f;
		disableVisualizationDistance = 100f;
	}

	public void OnSeatOccupiedChange()
	{
		if (vehicleSeatManager.OccupiedSeatsCount == 0)
		{
			if (blobShadow != null)
			{
				blobShadow.enabled = false;
			}
		}
		else if (blobShadow != null)
		{
			blobShadow.enabled = true;
		}
	}

	private void Update()
	{
		if (!isInSpawner)
		{
			HandleUnoccupiedVehicle();
		}
		Vector3 axis = Vector3.Cross(velocity.normalized, Vector3.up);
		float magnitude = velocity.magnitude;
		if (!vehicleIsUnoccupied)
		{
			Vector3 vector = new Vector3(velocity.x, 0f, velocity.z);
			MVAvatar owner = vehicleSeatManager.seats[0].Owner;
			int serverTimeInMilliSeconds = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
			if (vector.sqrMagnitude > 0.2f)
			{
				owner.Body.Transform.forward = vector.normalized;
				if (speedState == SpeedState.Idle)
				{
					Dictionary<object, object> dictionary = new Dictionary<object, object>();
					dictionary.Add("state", "Walk");
					dictionary.Add("timeStamp", serverTimeInMilliSeconds);
					Dictionary<object, object> value = dictionary;
					owner.Animation.Value = value;
					speedState = SpeedState.Moving;
				}
			}
			else if (speedState != SpeedState.Idle)
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add("state", "Idle");
				dictionary.Add("timeStamp", serverTimeInMilliSeconds);
				Dictionary<object, object> value2 = dictionary;
				owner.Animation.Value = value2;
				speedState = SpeedState.Idle;
			}
		}
		wheel.transform.Rotate(axis, (0f - magnitude) * Time.deltaTime * 57.29578f, Space.World);
	}

	private void FixedUpdate()
	{
		Vector3 position = wheel.transform.position;
		velocity = (position - prevPosition) / Time.deltaTime;
		float magnitude = velocity.magnitude;
		velocity.y = 0f;
		prevPosition = position;
		float magnitude2 = velocity.magnitude;
		float num = 0.05f;
		if ((magnitude2 > num || magnitude2 < 0f - num) && !audioSourceRolling.isPlaying)
		{
			audioSourceRolling.volume = 1f;
			audioSourceRolling.Play();
		}
		if (magnitude2 <= num && magnitude2 >= 0f - num && audioSourceRolling.isPlaying)
		{
			audioSourceRolling.volume = 0f;
			audioSourceRolling.Stop();
		}
		audioSourceRolling.pitch = Mathf.Clamp(magnitude2 / 35f, 0.2f, 3f);
		if ((magnitude2 > num || magnitude2 < 0f - num) && !audioSourceWind.isPlaying)
		{
			audioSourceWind.volume = 0.08f;
			audioSourceWind.Play();
		}
		if (magnitude2 <= num && magnitude2 >= 0f - num && audioSourceWind.isPlaying)
		{
			audioSourceWind.volume = 0f;
			audioSourceWind.Stop();
		}
		audioSourceWind.pitch = Mathf.Clamp(magnitude / 14f, 0.4f, 5f);
	}

	private void OnEnable()
	{
		vehicleBlinker.enabled = true;
	}

	private void OnDisable()
	{
		if (vehicleBlinker != null)
		{
			vehicleBlinker.enabled = false;
		}
	}

	private void OnHealthChange(float newHealth)
	{
		if (curHealth > newHealth)
		{
			vehicleBlinker.StartBlinking(BlinkType.Damage, 0.3f);
		}
		else if (curHealth < newHealth)
		{
			vehicleBlinker.StartBlinking(BlinkType.Healing, 0.3f);
		}
		curHealth = newHealth;
	}

	private void OnGroundedChange(bool val)
	{
		if (val)
		{
			audioSourceRolling.volume = 1f;
		}
		else
		{
			audioSourceRolling.volume = 0f;
		}
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
}
