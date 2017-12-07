using System;
using UnityEngine;

public class AvatarEmoteRecogniser
{
	private const float maxRotationMovement = 45f;

	public Action OnStartEvent;

	private bool isActive;

	private float angleSensitivity = 15f;

	private float previousAngle;

	private short eventRecognitioner;

	private short recognitionsBeforeEvent;

	private float resetInterval = 2f;

	private float resetCooldown;

	private short startModulusOffset;

	public void Initlialize(AvatarLimbManager limbManager, float angleSensitivity, float resetInterval, short recognitionsBeforeEvent, bool shouldRecognisePositiveAngleFirst, bool isActive)
	{
		limbManager.OnAvatarRotate = (Action)Delegate.Combine(limbManager.OnAvatarRotate, new Action(ResetRecognition));
		this.angleSensitivity = angleSensitivity;
		this.resetInterval = resetInterval;
		resetCooldown = resetInterval;
		this.recognitionsBeforeEvent = recognitionsBeforeEvent;
		if (shouldRecognisePositiveAngleFirst)
		{
			startModulusOffset = 0;
		}
		else
		{
			startModulusOffset = 1;
		}
		this.isActive = isActive;
	}

	public void HandleNewAngle(float angle)
	{
		if (!isActive)
		{
			ResetRecognition();
		}
		if (eventRecognitioner % 2 == startModulusOffset)
		{
			HandleRecognition(angle, previousAngle, checkPositiveRotation: true);
		}
		else
		{
			HandleRecognition(angle, previousAngle, checkPositiveRotation: false);
		}
		if (IsEventRecognitionDone() && OnStartEvent != null)
		{
			OnStartEvent();
		}
	}

	private void HandleRecognition(float newAngle, float previousAngle, bool checkPositiveRotation)
	{
		if (IsRotationPositive(newAngle, previousAngle) == checkPositiveRotation)
		{
			if (Mathf.Abs(newAngle - previousAngle) > angleSensitivity)
			{
				eventRecognitioner++;
			}
		}
		else
		{
			this.previousAngle = newAngle;
		}
	}

	private bool IsRotationPositive(float newAngle, float previousAngle)
	{
		if (previousAngle > 315f && newAngle < 45f)
		{
			return true;
		}
		if (previousAngle < 45f && newAngle > 315f)
		{
			return false;
		}
		if (previousAngle < newAngle)
		{
			return true;
		}
		return false;
	}

	private bool IsEventRecognitionDone()
	{
		if (eventRecognitioner >= recognitionsBeforeEvent)
		{
			eventRecognitioner = 0;
			resetCooldown = resetInterval;
			return true;
		}
		return false;
	}

	public void Update()
	{
		if (eventRecognitioner == 0)
		{
			ResetRecognition();
		}
		resetCooldown -= Time.deltaTime;
		if (resetCooldown <= 0f)
		{
			ResetRecognition();
		}
	}

	private void ResetRecognition()
	{
		eventRecognitioner = 0;
		resetCooldown = resetInterval;
	}

	public void SetIsActive(bool shouldBeActive)
	{
		if (shouldBeActive)
		{
			Activate();
		}
		else
		{
			Deactivate();
		}
	}

	public void Deactivate()
	{
		isActive = false;
	}

	public void Activate()
	{
		isActive = true;
	}
}
