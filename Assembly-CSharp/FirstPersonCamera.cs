using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.Rendering;

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

	[Header("Dependencies")]
	[SerializeField]
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

	public override float FieldOfView => 70f;

	protected abstract void UpdateCameraRotation();

	public override void Reset()
	{
		cameraOffset.y = cameraHeight;
		maxLookAngleDownward = Mathf.Clamp(maxLookAngleDownward, 0.1f, 89f);
		maxLookAngleUpward = Mathf.Clamp(maxLookAngleUpward, 0.1f, 89f);
	}

	private void Initialize(MVCameraController cameraController)
	{
		localAvatar = MVGameControllerBase.WOCM.AvatarLocal;
		targetRotation.x = cameraController.transform.rotation.eulerAngles.x;
		targetRotation.y = cameraController.transform.rotation.eulerAngles.y;
		transform.localRotation = cameraController.transform.localRotation;
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
		UpdateCamera(cameraController, cameraController.ProtectedTransform);
	}

	public override void Resume(MVCameraController cameraController)
	{
		if (localAvatar.CurrentPickup.FirstPersonCapable)
		{
			base.Resume(cameraController);
			Initialize(cameraController);
			ActivateFirstPerson();
			UpdateCamera(cameraController, cameraController.ProtectedTransform);
		}
		else
		{
			MVGameControllerBase.CameraController.RemoveCamera(CameraType);
		}
	}

	public override void Exit(MVCameraController camController)
	{
		base.Exit(camController);
		DeactivateFirstPerson();
		EnableFading(camController);
	}

	public override void Suspend(MVCameraController camController)
	{
		base.Suspend(camController);
		DeactivateFirstPerson();
		EnableFading(camController);
	}

	private void EnableFading(MVCameraController camController)
	{
		AvatarCameraFade component = camController.gameObject.GetComponent<AvatarCameraFade>();
		if (component != null)
		{
			component.enabled = true;
		}
	}

	private void ActivateFirstPerson()
	{
		MoveItemToFirstpersonView(localAvatar.CurrentPickup);
		HideBody(shouldHideBody: true);
		HideBlinking(shouldHideBlinking: true);
		if (haveHiddenVehicle)
		{
			HideVehicle();
		}
		AvatarPickupOwner pickupOwner = localAvatar.PickupOwner;
		pickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Combine(pickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(MoveItemToFirstpersonView));
		MVAvatarLocal mVAvatarLocal = localAvatar;
		mVAvatarLocal.OnDamageTaken = (Action<float, MVPlayer, PlayerKilledByType>)Delegate.Combine(mVAvatarLocal.OnDamageTaken, new Action<float, MVPlayer, PlayerKilledByType>(healingIndicator.ShowHealing));
		MVAvatarLocal mVAvatarLocal2 = localAvatar;
		mVAvatarLocal2.OnDamageTaken = (Action<float, MVPlayer, PlayerKilledByType>)Delegate.Combine(mVAvatarLocal2.OnDamageTaken, new Action<float, MVPlayer, PlayerKilledByType>(damageIndicator.ShowDamage));
		damageIndicator.enabled = true;
		modifierIndicator.enabled = true;
	}

	private void DeactivateFirstPerson()
	{
		localAvatar.CurrentPickup.LeaveFirstPersonView();
		HideBody(shouldHideBody: false);
		HideBlinking(shouldHideBlinking: false);
		if (haveHiddenVehicle)
		{
			ShowVehicle();
		}
		AvatarPickupOwner pickupOwner = localAvatar.PickupOwner;
		pickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Remove(pickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(MoveItemToFirstpersonView));
		MVAvatarLocal mVAvatarLocal = localAvatar;
		mVAvatarLocal.OnDamageTaken = (Action<float, MVPlayer, PlayerKilledByType>)Delegate.Remove(mVAvatarLocal.OnDamageTaken, new Action<float, MVPlayer, PlayerKilledByType>(healingIndicator.ShowHealing));
		MVAvatarLocal mVAvatarLocal2 = localAvatar;
		mVAvatarLocal2.OnDamageTaken = (Action<float, MVPlayer, PlayerKilledByType>)Delegate.Remove(mVAvatarLocal2.OnDamageTaken, new Action<float, MVPlayer, PlayerKilledByType>(damageIndicator.ShowDamage));
		damageIndicator.ResetIndicators();
		damageIndicator.enabled = false;
		modifierIndicator.ResetIndicators();
		modifierIndicator.enabled = false;
		MVGameControllerBase.WOCM.AvatarLocal.Avatar.AvatarFader.SetTransparency(0f);
		MVGameControllerBase.WOCM.AvatarLocal.Avatar.AvatarFader.enabled = true;
	}

	private void MoveItemToFirstpersonView(PickupItem item)
	{
		if (item.FirstPersonCapable)
		{
			item.EnterFirstPersonView(this);
			weaponBob.Initialize(localAvatar.CurrentPickup.transform);
		}
	}

	private void HideBody(bool shouldHideBody)
	{
		Component[] componentsInChildren = localAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.Torso).GetComponentsInChildren<MeshRenderer>();
		if (shouldHideBody)
		{
			for (short num = 0; num < componentsInChildren.Length; num++)
			{
				MeshRenderer meshRenderer = (MeshRenderer)componentsInChildren[num];
				meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			}
		}
		else
		{
			for (short num2 = 0; num2 < componentsInChildren.Length; num2++)
			{
				MeshRenderer meshRenderer2 = (MeshRenderer)componentsInChildren[num2];
				meshRenderer2.shadowCastingMode = ShadowCastingMode.On;
			}
		}
	}

	private void HideBlinking(bool shouldHideBlinking)
	{
		localAvatar.Body.ToggleBlinking(!shouldHideBlinking);
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

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		if (MVGameControllerBase.WOCM.AvatarLocal.InGunMode)
		{
			UpdateAvatar();
			UpdateCameraPosition();
			UpdateCameraRotation();
			weaponBob.Update();
			UpdateImpactSimulation(targetTransform);
			base.UpdateCamera(camController, targetTransform);
		}
		else
		{
			MVGameControllerBase.CameraController.SetCamera(CameraType.ThirdPerson);
			MVGameControllerBase.CameraController.StartTransitionCam(0.3f);
		}
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
			Transform transform = MVGameControllerBase.WOCM.AvatarLocal.Transform;
			transform.localRotation = Quaternion.Euler(0f, base.transform.localRotation.eulerAngles.y, 0f);
		}
		else if (!haveHiddenVehicle)
		{
			HideVehicle();
			haveHiddenVehicle = true;
		}
	}
}
