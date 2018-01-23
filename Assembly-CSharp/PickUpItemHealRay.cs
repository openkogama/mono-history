using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public class PickUpItemHealRay : PickupItem
{
	private struct RayCastData
	{
		public enum RayCastStatus
		{
			StopRay,
			StartRay,
			ContinueRay
		}

		public RayCastStatus Status;

		public VoxelHit HitVoxel;

		public Vector3 Direction;
	}

	private const float updateWaitTime = 0.05f;

	private const float hitGroundStartPositionMidifier = 0.6f;

	private const string stuckObjectIDString = "S";

	private float passedLerpTime;

	private Quaternion lerpStartRotation;

	[SerializeField]
	private ParticleSystem rayParticles;

	private ParticleSystem hitParticles;

	[SerializeField]
	private float maxRayRange = 20f;

	[SerializeField]
	private float maxLockOnRange = 25f;

	[SerializeField]
	private float rayMinimumChargeTime = 0.5f;

	[SerializeField]
	private AudioSource audioSource;

	private float rayStartTime;

	private GameObject stuckObject;

	private bool isLockedOn;

	private bool isShooting;

	private ObscuredFloat currentAmmoLeft = 0f;

	private float elapsedUpdateWaitTime;

	private Vector3 hitOffset;

	[SerializeField]
	[Tooltip("How many seconds the healrays ammo lasts.")]
	private ObscuredFloat maxAmmoTime = 100f;

	private LayerMask layers = -5 & ~(1 << LayerUtil.GetLayerNumber(LayerFlags.Logic));

	[SerializeField]
	private Material ZIgnoreMaterial;

	[SerializeField]
	private Transform localMuzzePoint;

	[SerializeField]
	private ParticleSystem localRayParticles;

	private Transform remoteMuzzlePoint;

	private ParticleSystem remoteParticleSystem;

	private Material normalRayMaterial;

	private ParticleSystemRenderer particleRenderer;

	public override AvatarItemType Type => AvatarItemType.HealRay;

	public override int Quantity => Mathf.RoundToInt((float)currentAmmoLeft / (float)maxAmmoTime * 100f);

	private void Awake()
	{
		currentAmmoLeft = maxAmmoTime;
		lerpStartRotation = rayParticles.transform.rotation;
		hitOffset = Vector3.zero;
	}

	private void OnDestroy()
	{
		if (hitParticles != null)
		{
			hitParticles.Stop();
		}
	}

	public override void OnEquip()
	{
		base.OnEquip();
		hitParticles = OneShotPooledParticleSystem.Instantiate(PoolEnums.HealRaySparks, transform.position, Quaternion.LookRotation(owner.LookDirection));
		hitParticles.Stop();
		if (owner.IsLocal)
		{
			particleRenderer = rayParticles.GetComponent<ParticleSystemRenderer>();
			normalRayMaterial = particleRenderer.material;
			particleRenderer.material = ZIgnoreMaterial;
			remoteMuzzlePoint = muzzlePoint;
			remoteParticleSystem = rayParticles;
			muzzlePoint = localMuzzePoint;
			rayParticles = localRayParticles;
		}
	}

	public override void OnUnequip()
	{
		base.OnUnequip();
		hitParticles.Stop();
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		currentAmmoLeft = maxAmmoTime;
	}

	private bool IsStillChargingRay()
	{
		return isShooting || rayStartTime + rayMinimumChargeTime >= Time.time;
	}

	private void DoHealing()
	{
		if (IsStillChargingRay() && (float)currentAmmoLeft > 0f)
		{
			Vector3 direction;
			if (stuckObject != null)
			{
				Vector3 vector = CalculateStuckPosition();
				direction = vector - owner.LookOrigin;
			}
			else
			{
				direction = owner.LookDirection;
			}
			RayCastData result = UpdateRaycastLocally(direction);
			HandleRaycastResultLocally(result);
		}
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		ChangeUsedParticleSystem();
		ParticleSystem.EmissionModule emission = rayParticles.emission;
		emission.enabled = true;
		if (IsStillChargingRay())
		{
			isShooting = true;
			rayStartTime = Time.time;
			return;
		}
		rayStartTime = Time.time;
		isShooting = true;
		if (!owner.IsLocal)
		{
			rayParticles.transform.rotation = Quaternion.LookRotation(CalculateParticlesRotation());
		}
	}

	private void ChangeUsedParticleSystem()
	{
		_ = owner.IsLocal;
		if (owner.IsLocal && owner is VehiclePickupOwner)
		{
			particleRenderer.material = normalRayMaterial;
			muzzlePoint = remoteMuzzlePoint;
			rayParticles = remoteParticleSystem;
		}
	}

	private Vector3 CalculateParticlesRotation()
	{
		Vector3 vector = owner.LookOrigin + owner.LookDirection * 30f;
		return (vector - muzzlePoint.position).normalized;
	}

	private void DoAmmoDepletion()
	{
		if (IsStillChargingRay())
		{
			currentAmmoLeft = (float)currentAmmoLeft - Time.deltaTime;
			muzzlePoint.transform.forward = owner.LookDirection;
			DoFloating();
		}
		if (Quantity <= 0)
		{
			MVEquipable component = owner.WorldObjectOwner.GameObject.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Unequip();
			}
		}
	}

	private void DoFloating()
	{
		MVRigidBody component = owner.WorldObjectOwner.GameObject.GetComponent<MVRigidBody>();
		if (!component.Grounded)
		{
			float y = component.Velocity.y;
			if (y < 0f)
			{
				float a = owner.LookDirection.y * (float)MVPhysics.Gravity * 0.95f * Time.deltaTime * 40f;
				float num = Mathf.Min(a, y);
				component.AddImpulse(new Vector3(0f, 0f - num, 0f), suspendImpactDamage: true);
			}
		}
	}

	private void Update()
	{
		UpdateRaysVisualRepresentation();
		if (!isShooting)
		{
			return;
		}
		if (!audioSource.isPlaying)
		{
			audioSource.Play();
		}
		if (!rayParticles.isPlaying)
		{
			rayParticles.Play();
		}
		if (owner.IsLocal)
		{
			elapsedUpdateWaitTime += Time.deltaTime;
			if (elapsedUpdateWaitTime >= 0.05f)
			{
				elapsedUpdateWaitTime = 0f;
				DoHealing();
			}
			DoAmmoDepletion();
			return;
		}
		if (!isLockedOn)
		{
			LerpRaysVisualRepresentation();
		}
		Vector3 direction;
		if (stuckObject != null)
		{
			Vector3 vector = CalculateStuckPosition();
			direction = vector - owner.LookOrigin;
		}
		else
		{
			direction = rayParticles.transform.forward;
		}
		UpdateRaycastRemotely(direction);
	}

	private void UpdateRaysVisualRepresentation()
	{
		if (stuckObject != null)
		{
			Vector3 worldPosition = CalculateStuckPosition();
			rayParticles.transform.LookAt(worldPosition);
			Color startColor = new Color(0f, 139f / 255f, 139f / 255f);
			rayParticles.startColor = startColor;
			audioSource.pitch = 2f;
			rayParticles.startSize = 0.4f;
			return;
		}
		Color startColor2 = new Color(58f / 255f, 1f, 133f / 255f);
		rayParticles.startColor = startColor2;
		audioSource.pitch = 1f;
		rayParticles.startSize = 0.3f;
		if (owner.IsLocal && isShooting)
		{
			rayParticles.transform.LookAt(muzzlePoint.position + CalculateParticlesRotation());
		}
	}

	private void LerpRaysVisualRepresentation()
	{
		Quaternion quaternion = Quaternion.LookRotation(owner.LookDirection);
		if (rayParticles.transform.rotation != quaternion)
		{
			if (lerpStartRotation != quaternion)
			{
				lerpStartRotation = quaternion;
				passedLerpTime = 0f;
			}
			passedLerpTime += Time.deltaTime;
			rayParticles.transform.rotation = Quaternion.Lerp(quaternion, rayParticles.transform.rotation, 0.2f / passedLerpTime);
		}
		else
		{
			passedLerpTime = 0f;
		}
	}

	public override void TriggerEnd()
	{
		isLockedOn = false;
		ParticleSystem.EmissionModule emission = rayParticles.emission;
		emission.enabled = false;
		isShooting = false;
		audioSource.Stop();
		stuckObject = null;
		hitParticles.Stop();
		if (owner.IsLocal)
		{
			UpdateItemState(-1);
		}
	}

	private RayCastData UpdateRaycastLocally(Vector3 direction)
	{
		RayCastData result = default;
		if (!IsDirectionValid(direction))
		{
			result.Status = RayCastData.RayCastStatus.StopRay;
			return result;
		}
		direction = direction.normalized;
		result.Direction = direction;
		List<VoxelHit> list = DoRaycast(direction);
		if (list.Count > 0)
		{
			result.HitVoxel = CalculateClosestVoxelHit(list);
			if (isLockedOn)
			{
				result.Status = RayCastData.RayCastStatus.ContinueRay;
			}
			else
			{
				result.Status = RayCastData.RayCastStatus.StartRay;
			}
		}
		else
		{
			result.Status = RayCastData.RayCastStatus.StopRay;
		}
		return result;
	}

	private void UpdateRaycastRemotely(Vector3 direction)
	{
		if (stuckObject == null)
		{
			List<VoxelHit> list = DoRaycast(direction);
			if (list.Count > 0)
			{
				float distance = CalculateClosestVoxelHit(list).distance;
				Vector3 hitPosition = CalculateHitPosition(owner.LookOrigin, direction, distance);
				OnHitParticleUpdate(hitPosition, direction, distance);
			}
			else
			{
				rayParticles.startLifetime = GetMaxRange() / rayParticles.startSpeed;
				hitParticles.Stop();
			}
		}
		else
		{
			float magnitude = (stuckObject.transform.position - muzzlePoint.position).magnitude;
			OnHitParticleUpdate(stuckObject.transform.position, direction, magnitude);
		}
	}

	private void HandleRaycastResultLocally(RayCastData result)
	{
		switch (result.Status)
		{
		case RayCastData.RayCastStatus.StartRay:
		{
			int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(result.HitVoxel.woId);
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
			HandleOnStuck(worldObjectClient, result);
			UpdateRayParticleEffect(result.HitVoxel, result.Direction);
			UpdateRayHealingLogic(result.HitVoxel);
			break;
		}
		case RayCastData.RayCastStatus.ContinueRay:
			UpdateRayParticleEffect(result.HitVoxel, result.Direction);
			UpdateRayHealingLogic(result.HitVoxel);
			break;
		case RayCastData.RayCastStatus.StopRay:
			rayParticles.startLifetime = GetMaxRange() / rayParticles.startSpeed;
			hitParticles.Stop();
			HandleNoHit();
			break;
		}
	}

	private void UpdateRayParticleEffect(VoxelHit hitVoxel, Vector3 direction)
	{
		float distance = hitVoxel.distance;
		Vector3 fireFromPosition = owner.LookOrigin;
		if (!isLockedOn)
		{
			fireFromPosition = muzzlePoint.position;
			fireFromPosition -= direction * 0.6f;
		}
		Vector3 vector = CalculateHitPosition(fireFromPosition, direction, distance);
		float magnitude = (vector - muzzlePoint.position).magnitude;
		vector = HandleVehicleHitPosition(vector, direction, distance);
		OnHitParticleUpdate(vector, direction, magnitude);
	}

	private Vector3 HandleVehicleHitPosition(Vector3 hitPosition, Vector3 direction, float hitDistance)
	{
		if (!isLockedOn && owner is VehiclePickupOwner)
		{
			hitPosition = CalculateHitPosition(owner.LookOrigin, direction, hitDistance);
		}
		return hitPosition;
	}

	private void UpdateRayHealingLogic(VoxelHit hitVoxel)
	{
		MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(hitVoxel.transform);
		InteractionDataHandlerBase interactionHandler = GetInteractionHandler(mVObject);
		interactionHandler = HandleChildObjectHit(hitVoxel, interactionHandler);
		TryHealTarget(interactionHandler);
	}

	private InteractionDataHandlerBase HandleChildObjectHit(VoxelHit hitVoxel, InteractionDataHandlerBase interactionHandler)
	{
		if (interactionHandler == null)
		{
			int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(hitVoxel.woId);
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
			return GetInteractionHandler(worldObjectClient);
		}
		return interactionHandler;
	}

	private InteractionDataHandlerBase GetInteractionHandler(MVWorldObjectClient worldObject)
	{
		return worldObject?.InteractionDataHandlerBase;
	}

	private void TryHealTarget(InteractionDataHandlerBase interactionHandler)
	{
		if (interactionHandler == null)
		{
			HandleNoHit();
		}
		else
		{
			interactionHandler.HandleInteraction(HealRayHitPackage.Create(), interactionIsLocal: false);
		}
	}

	private bool IsDirectionValid(Vector3 direction)
	{
		if (stuckObject != null)
		{
			direction.Normalize();
			float num = Vector3.Angle(owner.LookDirection, direction);
			float num2 = (stuckObject.transform.position - owner.transform.position).magnitude;
			if (num2 < 1f)
			{
				num2 = 1f;
			}
			float num3 = 1f - num2 / GetMaxRange();
			float num4 = 50f;
			float num5 = num4 * num3 + num4;
			if (num > num5 || num < 0f - num5)
			{
				stuckObject = null;
				direction = owner.LookDirection;
				if (isLockedOn)
				{
					TriggerEnd();
					return false;
				}
			}
		}
		return true;
	}

	private List<VoxelHit> DoRaycast(Vector3 direction)
	{
		Ray ray = new Ray(owner.LookOrigin, direction);
		return CollisionDetection.MVHitAll(ray, GetMaxRange(), owner.IgnoreWOIDs, layers);
	}

	private VoxelHit CalculateClosestVoxelHit(List<VoxelHit> hitVoxels)
	{
		VoxelHit result = hitVoxels[0];
		float num = hitVoxels[0].distance;
		for (int i = 1; i < hitVoxels.Count; i++)
		{
			float distance = hitVoxels[i].distance;
			if (distance < num)
			{
				num = distance;
				result = hitVoxels[i];
			}
		}
		return result;
	}

	private Vector3 CalculateHitPosition(Vector3 fireFromPosition, Vector3 direction, float distance)
	{
		return fireFromPosition + direction * distance;
	}

	private void OnHitParticleUpdate(Vector3 hitPosition, Vector3 firingDirection, float distance)
	{
		UpdateHitParticles(firingDirection, hitPosition);
		SetRayParticleDistance(distance);
	}

	private void UpdateHitParticles(Vector3 firingDirection, Vector3 hitPosition)
	{
		hitParticles.transform.position = hitPosition;
		hitParticles.transform.rotation = Quaternion.LookRotation(-firingDirection);
		hitParticles.Play();
	}

	private void SetRayParticleDistance(float distance)
	{
		rayParticles.startLifetime = distance / rayParticles.startSpeed;
	}

	private void HandleNoHit()
	{
		if (isLockedOn)
		{
			TriggerEnd();
		}
	}

	private void HandleOnStuck(MVWorldObjectClient hitObject, RayCastData result)
	{
		if (!IsObjectStuckable(hitObject))
		{
			HandleNoHit();
		}
		else if (!isLockedOn)
		{
			ShowHitEffect();
			UpdateStuckObject(hitObject);
			CalculateHitOffset(result.HitVoxel, result.Direction);
			UpdateItemState(hitObject.Id);
			isLockedOn = true;
		}
	}

	private bool IsObjectStuckable(MVWorldObjectClient hitObject)
	{
		if (hitObject == null)
		{
			return false;
		}
		InteractionDataHandlerBase interactionDataHandlerBase = hitObject.InteractionDataHandlerBase;
		if (interactionDataHandlerBase == null)
		{
			return false;
		}
		return true;
	}

	private void CalculateHitOffset(VoxelHit hitVoxel, Vector3 direction)
	{
		float distance = hitVoxel.distance;
		Vector3 lookOrigin = owner.LookOrigin;
		Vector3 vector = CalculateHitPosition(lookOrigin, direction, distance);
		hitOffset = vector - stuckObject.transform.position;
	}

	private void ShowHitEffect()
	{
		MVGameControllerBase.CameraController.PlayPlingSound();
		MVGameControllerBase.IPlayModeUI.GetCrossHair().ShowHasHitEffect();
	}

	private void UpdateStuckObject(MVWorldObjectClient hitObject)
	{
		stuckObject = hitObject.GameObject;
		if (hitObject is IHealRayAttachementObject)
		{
			stuckObject = ((IHealRayAttachementObject)hitObject).GetHealRayAttachmentObject();
		}
	}

	private void UpdateItemState(int stuckObjectId)
	{
		if (owner.WorldObjectOwner is ICurrentItemOwner)
		{
			Dictionary<object, object> currentItemState = ((ICurrentItemOwner)owner.WorldObjectOwner).GetCurrentItemState();
			ChangeValueInState(currentItemState, "S", stuckObjectId);
			SyncState(currentItemState);
		}
		else
		{
			Debug.LogWarning("HealRay holder " + owner.WorldObjectOwner.GameObject.name + " is not a ICurrentItemOwner. This might cause a so that HealRays lock-on is not syncrhonized correctly.");
		}
	}

	private Vector3 CalculateStuckPosition()
	{
		Vector3 position = stuckObject.transform.position;
		return position + hitOffset;
	}

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		if (owner.IsLocal || !newState.ContainsKey("S"))
		{
			return;
		}
		int num = (int)newState["S"];
		if (num == -1)
		{
			TriggerEnd();
		}
		else
		{
			if (!MVGameControllerBase.WOCM.TryGetWorldObject(num, out var worldObject))
			{
				return;
			}
			MVWorldObjectClient mVWorldObjectClient = (MVWorldObjectClient)worldObject;
			stuckObject = null;
			if (mVWorldObjectClient == null)
			{
				return;
			}
			InteractionDataHandlerBase interactionDataHandlerBase = mVWorldObjectClient.InteractionDataHandlerBase;
			if (interactionDataHandlerBase != null)
			{
				stuckObject = mVWorldObjectClient.GameObject;
				isLockedOn = true;
				if (mVWorldObjectClient is IHealRayAttachementObject)
				{
					stuckObject = ((IHealRayAttachementObject)mVWorldObjectClient).GetHealRayAttachmentObject();
				}
			}
		}
	}

	private void SyncState(Dictionary<object, object> newState)
	{
		if (owner.IsLocal && owner.CurrentItem == this)
		{
			float num = currentAmmoLeft;
			float num2 = elapsedUpdateWaitTime;
			if (owner.WorldObjectOwner is ICurrentItemOwner)
			{
				((ICurrentItemOwner)owner.WorldObjectOwner).SetCurrentItemState(newState);
			}
			currentAmmoLeft = num;
			elapsedUpdateWaitTime = num2;
		}
	}

	private void ChangeValueInState(Dictionary<object, object> newState, string key, int value)
	{
		if (!newState.ContainsKey(key))
		{
			newState.Add(key, value);
		}
		else
		{
			newState[key] = value;
		}
	}

	private void ChangeValueInState(Dictionary<object, object> newState, string key, float value)
	{
		if (!newState.ContainsKey(key))
		{
			newState.Add(key, value);
		}
		else
		{
			newState[key] = value;
		}
	}

	private float GetMaxRange()
	{
		if (isLockedOn)
		{
			return maxLockOnRange;
		}
		return maxRayRange;
	}
}
