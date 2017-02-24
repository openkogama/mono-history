using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public abstract class FirstPersonCamera : MVCameraBase
{
	[SerializeField]
	[Header("Configuration")]
	private float cameraHeight = 2f;

	[SerializeField]
	protected float maxLookAngleDownward = 60f;

	[SerializeField]
	protected float maxLookAngleUpward = 60f;

	[HideInInspector]
	[SerializeField]
	private Vector3 cameraOffset = new Vector3(0f, 2f, 0f);

	[SerializeField]
	protected float pitchSensitivity = 0.5f;

	[SerializeField]
	protected float yawSensitivity = 0.5f;

	[SerializeField]
	private FirstPersonWeaponBob weaponBob;

	[SerializeField]
	[Header("Dependencies")]
	private DamageIndicator damageIndicator;

	[SerializeField]
	private ModifierIndicator modifierIndicator;

	[SerializeField]
	private HealingIndicator healingIndicator;

	[SerializeField]
	protected TargetRotation smoothRotation;

	protected Vector2 targetRotation;

	private MVAvatarLocal localAvatar;

	private List<MeshRenderer> vehiclesHiddenMeshRenderers = new List<MeshRenderer>(32);

	private bool haveHiddenVehicle;

	public override CameraType CameraType => CameraType.FirstPersonCamera;

	protected abstract void UpdateCameraRotation();

	private void OnValidate()
	{
		cameraOffset.y = cameraHeight;
		maxLookAngleDownward = Mathf.Clamp(maxLookAngleDownward, 0.1f, 89f);
		maxLookAngleUpward = Mathf.Clamp(maxLookAngleUpward, 0.1f, 89f);
		if (damageIndicator != null)
		{
			damageIndicator.enabled = false;
		}
	}

	private void Initialize(MVCameraController cameraController)
	{
		localAvatar = MVGameControllerBase.WOCM.AvatarLocal;
		targetRotation.x = cameraController.transform.rotation.eulerAngles.x;
		targetRotation.y = cameraController.transform.rotation.eulerAngles.y;
		modifierIndicator.Initialize(localAvatar);
		MVGameControllerBase.CameraController.StartTransitionCam(0.3f);
		MVGameControllerBase.WOCM.AvatarLocal.SetTransparency = 1f;
		cameraController.AvatarCameraFade.enabled = false;
		haveHiddenVehicle = false;
	}

	public override void Enter(MVCameraController cameraController)
	{
		base.Enter(cameraController);
		Initialize(cameraController);
		ActivateFirstPerson();
	}

	public override void Resume(MVCameraController cameraController)
	{
		base.Resume(cameraController);
		ActivateFirstPerson();
	}

	public override void Exit(MVCameraController camController)
	{
		base.Exit(camController);
		DeactivateFirstPerson();
	}

	public override void Suspend(MVCameraController camController)
	{
		base.Suspend(camController);
		DeactivateFirstPerson();
	}

	private void ActivateFirstPerson()
	{
		HideBody(b: true);
		if (haveHiddenVehicle)
		{
			HideVehicle();
		}
		MoveItemToFirstpersonView(localAvatar.CurrentPickup);
		AvatarPickupOwner pickupOwner = localAvatar.PickupOwner;
		pickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Combine(pickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(MoveItemToFirstpersonView));
		MVAvatarLocal mVAvatarLocal = localAvatar;
		mVAvatarLocal.OnDamageTaken = (Action<float, MVPlayer, PlayerKilledByType>)Delegate.Combine(mVAvatarLocal.OnDamageTaken, new Action<float, MVPlayer, PlayerKilledByType>(damageIndicator.ShowDamage));
		MVAvatarLocal mVAvatarLocal2 = localAvatar;
		mVAvatarLocal2.OnDamageTaken = (Action<float, MVPlayer, PlayerKilledByType>)Delegate.Combine(mVAvatarLocal2.OnDamageTaken, new Action<float, MVPlayer, PlayerKilledByType>(healingIndicator.ShowHealing));
		damageIndicator.enabled = true;
		modifierIndicator.enabled = true;
	}

	private void DeactivateFirstPerson()
	{
		HideBody(b: false);
		if (haveHiddenVehicle)
		{
			ShowVehicle();
		}
		localAvatar.CurrentPickup.LeaveFirstPersonView();
		AvatarPickupOwner pickupOwner = localAvatar.PickupOwner;
		pickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Remove(pickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(MoveItemToFirstpersonView));
		MVAvatarLocal mVAvatarLocal = localAvatar;
		mVAvatarLocal.OnDamageTaken = (Action<float, MVPlayer, PlayerKilledByType>)Delegate.Remove(mVAvatarLocal.OnDamageTaken, new Action<float, MVPlayer, PlayerKilledByType>(damageIndicator.ShowDamage));
		MVAvatarLocal mVAvatarLocal2 = localAvatar;
		mVAvatarLocal2.OnDamageTaken = (Action<float, MVPlayer, PlayerKilledByType>)Delegate.Remove(mVAvatarLocal2.OnDamageTaken, new Action<float, MVPlayer, PlayerKilledByType>(healingIndicator.ShowHealing));
		damageIndicator.enabled = false;
		damageIndicator.ResetIndicators();
		modifierIndicator.enabled = false;
		modifierIndicator.ResetIndicators();
	}

	private void MoveItemToFirstpersonView(PickupItem item)
	{
		if (item.ActivateGunModeOnEquip)
		{
			item.EnterFirstPersonView(this);
			weaponBob.Initialize(localAvatar.CurrentPickup.transform);
		}
	}

	private void HideBody(bool b)
	{
		localAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.Torso).gameObject.SetActive(!b);
	}

	private void HideVehicle()
	{
		int woIDWithLocalOwnerHighestInHierarchy = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(localAvatar.Id);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDWithLocalOwnerHighestInHierarchy);
		worldObjectClient.GameObject.GetComponentsInChildren(includeInactive: false, vehiclesHiddenMeshRenderers);
		for (int i = 0; i < vehiclesHiddenMeshRenderers.Count; i++)
		{
			vehiclesHiddenMeshRenderers[i].enabled = false;
		}
	}

	private void ShowVehicle()
	{
		for (int i = 0; i < vehiclesHiddenMeshRenderers.Count; i++)
		{
			vehiclesHiddenMeshRenderers[i].enabled = true;
		}
	}

	protected void HighlightFriendsInSight()
	{
		Ray ray = new Ray(transform.position, transform.forward);
		int layerMask = 1 << LayerMask.NameToLayer("Player");
		HashSet<int> hashSet = new HashSet<int>();
		hashSet.Add(MVGameControllerBase.WOCM.AvatarLocal.Id);
		HashSet<int> ignoreWoIds = hashSet;
		if (!CollisionDetection.MVHit(ray, out var voxelHit, 1000f, ignoreWoIds, layerMask))
		{
			return;
		}
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
		if (MVGameControllerBase.Game.TeamManager.IsOnSameTeam(worldObjectClient.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr))
		{
			Avatar component = worldObjectClient.GameObject.GetComponent<Avatar>();
			if (component != null)
			{
				component.mvAvatar.Body.Highlight();
			}
		}
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		HighlightFriendsInSight();
		if (MVGameControllerBase.WOCM.AvatarLocal.InGunMode)
		{
			UpdateCameraPosition();
			UpdateCameraRotation();
			UpdateAvatar();
			base.UpdateCamera(camController, targetTransform);
			weaponBob.Update();
		}
		else
		{
			MVGameControllerBase.CameraController.SetCamera(CameraType.ThirdPerson);
			MVGameControllerBase.CameraController.StartTransitionCam(0.3f);
		}
		UpdateImpactSimulation(targetTransform);
	}

	private void UpdateCameraPosition()
	{
		transform.position = localAvatar.Body.Transform.position + cameraOffset;
	}

	private void UpdateAvatar()
	{
		if (!MVGameControllerBase.WOCM.AvatarLocal.IsInVehicle)
		{
			haveHiddenVehicle = false;
			Transform transform = MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform;
			transform.localRotation = Quaternion.Euler(0f, base.transform.localRotation.eulerAngles.y, 0f);
		}
		else if (!haveHiddenVehicle)
		{
			HideVehicle();
			haveHiddenVehicle = true;
		}
	}
}
