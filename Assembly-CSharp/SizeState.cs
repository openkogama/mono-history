using System;
using System.Collections.Generic;
using UnityEngine;

public class SizeState
{
	private static readonly int layerMask = -5 & ~(1 << LayerMask.NameToLayer("Player")) & ~(1 << LayerMask.NameToLayer("Logic"));

	private readonly MVInteractable interactableLocal;

	private readonly MvCharacterController controllerLocal;

	private float currentSize = 1f;

	private const float scalePercent = 0.1f;

	private static List<Vector3> relativePositions = new List<Vector3>
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(0f, -1f, 0f),
		new Vector3(0f, 0f, 1f),
		new Vector3(1f, 0f, 0f),
		new Vector3(-1f, 0f, 0f),
		new Vector3(0f, 0f, -1f),
		new Vector3(1f, 0f, 1f),
		new Vector3(-1f, 0f, -1f),
		new Vector3(-1f, 0f, 1f),
		new Vector3(1f, 0f, -1f),
		new Vector3(0f, 1f, 1f),
		new Vector3(1f, 1f, 0f),
		new Vector3(-1f, 1f, 0f),
		new Vector3(0f, 1f, -1f),
		new Vector3(-1f, 1f, -1f),
		new Vector3(-1f, 1f, 1f),
		new Vector3(1f, 1f, -1f),
		new Vector3(0f, -1f, 1f),
		new Vector3(1f, -1f, 0f),
		new Vector3(-1f, -1f, 0f),
		new Vector3(0f, -1f, -1f),
		new Vector3(1f, -1f, 1f),
		new Vector3(-1f, -1f, -1f),
		new Vector3(-1f, -1f, 1f),
		new Vector3(1f, -1f, -1f),
		new Vector3(1f, 1f, 1f)
	};

	public float ControllerRadius => controllerLocal.Radius;

	public float ControllerCenterY => controllerLocal.Center.y;

	private float AvatarScale
	{
		get
		{
			if (interactableLocal.HasModifierEffect(AvatarModifierEffect.Scale))
			{
				return interactableLocal.HandleModifierEffect(AvatarModifierEffect.Scale, 1f);
			}
			return MVGameControllerBase.SpawnRoleDataMediatorLocal.Size.Value;
		}
	}

	public event EventHandler EquipSlapGunEvent;

	public event EventHandler<ScaleArgs> CameraScaleEvent;

	public event EventHandler<EventArgs> UnEquipSlapGunEvent;

	public SizeState(MVInteractable interactable, MvCharacterController controller)
	{
		interactableLocal = interactable;
		controllerLocal = controller;
	}

	public void UpdateScale()
	{
		float avatarScale = AvatarScale;
		if (currentSize != avatarScale)
		{
			ScaleChanged();
		}
	}

	public void OnScalingWhileColliding(MVControllerColliderHit hitData)
	{
		float avatarScale = AvatarScale;
		if (currentSize < avatarScale && hitData.slopeNormal != Vector3.up)
		{
			MoveOutOfScalingCollision(hitData);
		}
	}

	public void ScaleChanged()
	{
		float avatarScale = AvatarScale;
		if (currentSize < avatarScale)
		{
			Vector3 position = FindValidMoveLocation(avatarScale);
			controllerLocal.transform.position = position;
			if (interactableLocal.HasModifierEffect(AvatarModifierEffect.Scale))
			{
				EquipSlapGunEvent(this, EventArgs.Empty);
			}
		}
		else if (UnEquipSlapGunEvent != null)
		{
			UnEquipSlapGunEvent(this, EventArgs.Empty);
		}
		controllerLocal.SetScale(avatarScale);
		if (CameraScaleEvent != null)
		{
			CameraScaleEvent(this, new ScaleArgs(avatarScale));
		}
		currentSize = avatarScale;
	}

	private Vector3 FindValidMoveLocation(float scale)
	{
		float num = scale * 0.1f;
		float num2 = currentSize;
		Vector3 position = controllerLocal.transform.position;
		while (num2 < scale)
		{
			num2 += num;
			bool flag = false;
			foreach (Vector3 relativePosition in relativePositions)
			{
				Vector3 vector = relativePosition * num2;
				if (GetIsValidScaledPosition(vector, num2))
				{
					controllerLocal.transform.position += vector;
					position = controllerLocal.transform.position;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Debug.Log("no position found.");
				break;
			}
		}
		return position;
	}

	public bool GetIsValidScaledPosition(Vector3 relativeTestPos, float scale)
	{
		Vector3 vector = controllerLocal.centerBase * scale;
		Vector3 radius = new Vector3(controllerLocal.radiusBase.x, controllerLocal.radiusBase.y, controllerLocal.radiusBase.z) * scale;
		return !MVElipsoidOverlapCheck.ElipsoidOverlapCheckBool(radius, controllerLocal.transform.position + relativeTestPos + vector, Quaternion.identity, layerMask, controllerLocal.IgnoreWoIds);
	}

	public void MoveOutOfScalingCollision(MVControllerColliderHit hitData)
	{
		Vector3 vector = controllerLocal.transform.position + controllerLocal.Center - hitData.positionTouchingHit;
		float num = controllerLocal.Radius / controllerLocal.radiusBase.x;
		Vector3 point = controllerLocal.centerBase - controllerLocal.Center + vector * num;
		float distanceToPoint = new Plane(hitData.slopeNormal, controllerLocal.transform.position + controllerLocal.Center - hitData.positionTouchingHit).GetDistanceToPoint(point);
		Vector3 vector2 = hitData.slopeNormal * distanceToPoint;
		controllerLocal.transform.position = controllerLocal.transform.position + vector2;
	}
}
