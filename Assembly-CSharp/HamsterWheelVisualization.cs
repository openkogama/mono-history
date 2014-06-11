using System;
using System.Collections;
using UnityEngine;

public class HamsterWheelVisualization : VehicleVisualizationBase
{
	public enum SpeedState
	{
		Idle,
		Forward,
		Backwards
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

	private float spinSpeed = 8f;

	private AvatarRotationSlerper slerper;

	public AudioSource audioSourceRolling;

	public AudioSource audioSourceWind;

	public AudioSource audioSourceLanding;

	public AudioSource audioSourceSqueal;

	private bool vehicleIsUnoccupied;

	private float unoccupiedTime = Time.time;

	private float vehicleAboutToBeRemovedTime = 3f;

	public void Init(VehicleSeatManager vehicleSeatManager, float fullHealth, MVRuntimeDataVariableClampedFloat health, MVRuntimeDataVariable isMovingForward, MVRuntimeDataVariable isMovingBackwards, MVRuntimeDataVariable isGrounded, bool isInSpawner)
	{
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		base.isInSpawner = isInSpawner;
		this.vehicleSeatManager = vehicleSeatManager;
		vehicleSeatManager.OnSeatOccupiedChange = (VehicleSeatManager.OnSeatOccupiedChangeDelegate)Delegate.Combine(vehicleSeatManager.OnSeatOccupiedChange, new VehicleSeatManager.OnSeatOccupiedChangeDelegate(OnSeatOccupiedChange));
		curHealth = health.Value;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object healthVal) =>
		{
			OnHealthChange((float)healthVal);
		}));
		isMovingForward.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isMovingForward.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object val) =>
		{
			OnMovingForwardChange((bool)val);
		}));
		isMovingBackwards.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isMovingBackwards.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object val) =>
		{
			OnMovingBackwardsChange((bool)val);
		}));
		isGrounded.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isGrounded.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object val) =>
		{
			OnGroundedChange((bool)val);
		}));
		vehicleBlinker.Init(((Component)hamsterWheelVisualizationRoot).gameObject.GetComponentsInChildren<MeshFilter>());
		vehicleBlinker.Visible = true;
		if (isInSpawner)
		{
			((Behaviour)this).enabled = false;
		}
		blobShadow = ((Component)hamsterWheelVisualizationRoot).GetComponentInChildren<AvatarBlobShadowController>();
		if ((Object)(object)blobShadow != (Object)null)
		{
			((Behaviour)blobShadow).enabled = false;
		}
		prevPosition = wheel.transform.position;
		cullDistance = 100f;
		disableVisualizationDistance = 100f;
	}

	public void OnSeatOccupiedChange()
	{
		if (vehicleSeatManager.OccupiedSeatsCount == 0)
		{
			if ((Object)(object)blobShadow != (Object)null)
			{
				((Behaviour)blobShadow).enabled = false;
			}
		}
		else if ((Object)(object)blobShadow != (Object)null)
		{
			((Behaviour)blobShadow).enabled = true;
		}
	}

	private void Update()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		if (!isInSpawner)
		{
			HandleUnoccupiedVehicle();
		}
		if (slerper != null && !slerper.Update())
		{
			slerper = null;
		}
		Vector3 val = Vector3.Cross(velocity.normalized, Vector3.up);
		float num = velocity.magnitude;
		float num2 = Vector3.Dot(((Component)this).transform.forward, velocity.normalized);
		if (!vehicleIsUnoccupied)
		{
			bool flag = false;
			if (speedState != SpeedState.Idle && ((num < spinSpeed && num > 0f - spinSpeed / 4f) || (speedState == SpeedState.Backwards && num2 > 0f) || (speedState == SpeedState.Forward && num2 < 0f)))
			{
				val = ((Component)this).transform.right;
				if (speedState == SpeedState.Forward)
				{
					num = 0f - spinSpeed;
					flag = true;
				}
				if (speedState == SpeedState.Backwards)
				{
					num = spinSpeed;
					flag = true;
				}
			}
			if (flag)
			{
				if (!audioSourceSqueal.isPlaying)
				{
					audioSourceSqueal.Play();
				}
			}
			else if (audioSourceSqueal.isPlaying)
			{
				audioSourceSqueal.Stop();
			}
		}
		wheel.transform.RotateAround(val, (0f - num) * Time.deltaTime);
		if (speedState == SpeedState.Idle && !vehicleIsUnoccupied)
		{
			vehicleSeatManager.seats[0].Owner.Body.Transform.parent.RotateAround(val, (0f - num) * Time.deltaTime);
		}
	}

	private void FixedUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
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
		((Behaviour)vehicleBlinker).enabled = true;
	}

	private void OnDisable()
	{
		if ((Object)(object)vehicleBlinker != (Object)null)
		{
			((Behaviour)vehicleBlinker).enabled = false;
		}
	}

	private void OnHealthChange(float newHealth)
	{
		if (curHealth > newHealth)
		{
			vehicleBlinker.StartBlinking(BlinkType.Damage, 0.3f);
		}
		curHealth = newHealth;
	}

	private void OnMovingForwardChange(bool value)
	{
		MVAvatar owner = vehicleSeatManager.seats[0].Owner;
		int serverTimeInMilliSeconds = MVGameController.Instance.Game.ServerTimeInMilliSeconds;
		if (value)
		{
			if (owner.Avatar.IsLocal)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("state", "Walk");
				hashtable.Add("timeStamp", serverTimeInMilliSeconds);
				Hashtable value2 = hashtable;
				owner.Animation.Value = value2;
			}
			slerper = new AvatarRotationSlerper(vehicleSeatManager.seats[0].Owner.Body.Transform.parent, ref hamsterWheelVisualizationRoot);
			speedState = SpeedState.Forward;
		}
		else
		{
			if (owner.Avatar.IsLocal)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("state", "Idle");
				hashtable.Add("timeStamp", serverTimeInMilliSeconds);
				Hashtable value3 = hashtable;
				owner.Animation.Value = value3;
			}
			speedState = SpeedState.Idle;
		}
	}

	private void OnMovingBackwardsChange(bool value)
	{
		MVAvatar owner = vehicleSeatManager.seats[0].Owner;
		int serverTimeInMilliSeconds = MVGameController.Instance.Game.ServerTimeInMilliSeconds;
		if (value)
		{
			if (owner.Avatar.IsLocal)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("state", "Walk");
				hashtable.Add("timeStamp", serverTimeInMilliSeconds);
				Hashtable value2 = hashtable;
				owner.Animation.Value = value2;
			}
			slerper = new AvatarRotationSlerper(vehicleSeatManager.seats[0].Owner.Body.Transform.parent, ref hamsterWheelVisualizationRoot);
			speedState = SpeedState.Backwards;
		}
		else
		{
			if (owner.Avatar.IsLocal)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("state", "Idle");
				hashtable.Add("timeStamp", serverTimeInMilliSeconds);
				Hashtable value3 = hashtable;
				owner.Animation.Value = value3;
			}
			speedState = SpeedState.Idle;
		}
	}

	private void OnGroundedChange(bool val)
	{
		if (val)
		{
			audioSourceRolling.volume = 1f;
			audioSourceLanding.Play();
			audioSourceSqueal.volume = 0.06f;
		}
		else
		{
			audioSourceRolling.volume = 0f;
			audioSourceSqueal.volume = 0f;
		}
	}

	private void HandleUnoccupiedVehicle()
	{
		if (!vehicleIsUnoccupied && vehicleSeatManager.OccupiedSeatsCount == 0)
		{
			unoccupiedTime = Time.time;
			vehicleIsUnoccupied = true;
			if (slerper != null)
			{
				slerper = null;
			}
			audioSourceSqueal.Stop();
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
