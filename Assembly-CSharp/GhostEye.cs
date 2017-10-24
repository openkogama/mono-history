using System;
using System.Collections.Generic;
using UnityEngine;

public class GhostEye : MonoBehaviour
{
	private enum GhostEyeState
	{
		RandomEyeRoll,
		LookAtTarget,
		DieRollback,
		SneakySideToSide
	}

	private interface IGhostEyeState
	{
		void Enter(GhostEye ghostEye);

		Quaternion Update(GhostEye ghostEye);

		void Exit();
	}

	private abstract class IdleBase : IGhostEyeState
	{
		protected float rotatationPrSecond = 0.5f;

		protected float wrappedTime;

		protected const float pi2 = (float)Math.PI * 2f;

		protected float direction = 1f;

		protected float radiusPitch;

		protected float radiusYaw;

		public virtual void Enter(GhostEye ghostEye)
		{
			radiusPitch = ghostEye.maxPitch;
			radiusYaw = ghostEye.maxYaw;
			direction = 1f;
			wrappedTime = 0f;
		}

		public virtual Quaternion Update(GhostEye ghostEye)
		{
			return Quaternion.identity;
		}

		public virtual void Exit()
		{
		}

		protected void UpdateWrappedTime(float deltaTime)
		{
			wrappedTime += deltaTime * direction * rotatationPrSecond * ((float)Math.PI * 2f);
			while (wrappedTime >= (float)Math.PI * 2f)
			{
				wrappedTime -= (float)Math.PI * 2f;
			}
			while (wrappedTime < (float)Math.PI * -2f)
			{
				wrappedTime += (float)Math.PI * 2f;
			}
		}

		protected Quaternion GetYawRotation()
		{
			float yaw = GetYaw();
			return Quaternion.AngleAxis(yaw, Vector3.up);
		}

		private float GetYaw()
		{
			return radiusYaw * Mathf.Sin(wrappedTime);
		}
	}

	private class DieRollback : IGhostEyeState
	{
		private static float rollbackPitch = -90f;

		private const float rollbackTime = 1f;

		private float currentRollbackTime;

		private Quaternion rollbackRotation = Quaternion.Euler(rollbackPitch, 0f, 0f);

		public void Enter(GhostEye ghostEye)
		{
			currentRollbackTime = 0f;
		}

		public Quaternion Update(GhostEye ghostEye)
		{
			currentRollbackTime += Time.deltaTime;
			return Quaternion.Slerp(ghostEye.transform.localRotation, rollbackRotation, currentRollbackTime);
		}

		public void Exit()
		{
		}
	}

	private class RandomEyeRoll : IdleBase
	{
		public override Quaternion Update(GhostEye ghostEye)
		{
			return GetEyeRollRotation();
		}

		private Quaternion GetEyeRollRotation()
		{
			UpdateWrappedTime(Time.deltaTime);
			Quaternion pitchRotation = GetPitchRotation();
			Quaternion yawRotation = GetYawRotation();
			return yawRotation * pitchRotation;
		}

		private Quaternion GetPitchRotation()
		{
			float pitch = GetPitch();
			return Quaternion.AngleAxis(pitch, Vector3.right);
		}

		private float GetPitch()
		{
			return radiusPitch * Mathf.Cos(wrappedTime);
		}

		public override void Exit()
		{
		}
	}

	private class LookAtTarget : IGhostEyeState
	{
		private Vector3 target;

		private float maxPitch;

		private float maxYaw;

		public void Enter(GhostEye ghostEye)
		{
			maxPitch = ghostEye.maxPitch;
			maxYaw = ghostEye.maxYaw;
		}

		public Quaternion Update(GhostEye ghostEye)
		{
			if (!TryGetLocalTargetDir(ghostEye, out var localTargetDir))
			{
				return ghostEye.transform.localRotation;
			}
			return Quaternion.LookRotation(localTargetDir, Vector3.up);
		}

		private Quaternion GetClampedYawRotation(Vector3 localTargetDirection)
		{
			float signedYaw = GetSignedYaw(localTargetDirection);
			float angle = Mathf.Clamp(signedYaw, 0f - maxYaw, maxYaw);
			return Quaternion.AngleAxis(angle, Vector3.up);
		}

		private Quaternion GetClampedPitchRotation(Vector3 localTargetDirection)
		{
			float pitch = GetPitch(localTargetDirection);
			float angle = Mathf.Clamp(pitch, 0f - maxPitch, maxPitch);
			return Quaternion.AngleAxis(angle, Vector3.right);
		}

		private bool TryGetLocalTargetDir(GhostEye ghostEye, out Vector3 localTargetDir)
		{
			localTargetDir = ghostEye.transform.InverseTransformPoint(target);
			if (localTargetDir.sqrMagnitude < 0.1f)
			{
				return false;
			}
			localTargetDir.Normalize();
			return true;
		}

		private float GetPitch(Vector3 localTargetPosition)
		{
			return Vector3.Angle(Vector3.up, localTargetPosition) - 90f;
		}

		private float GetSignedYaw(Vector3 localTargetPosition)
		{
			Vector3 normalized = new Vector3(localTargetPosition.x, 0f, localTargetPosition.z).normalized;
			float num = Vector3.Angle(Vector3.forward, normalized);
			float num2 = Vector3.Dot(normalized, Vector3.right);
			if (num2 < 0f)
			{
				num = 0f - num;
			}
			return num;
		}

		public void Exit()
		{
		}

		public void SetTarget(Vector3 target)
		{
			this.target = target;
		}
	}

	private class SneakySideToSide : IdleBase
	{
		public override Quaternion Update(GhostEye ghostEye)
		{
			return GetSneakySideToSideRotation();
		}

		private Quaternion GetSneakySideToSideRotation()
		{
			UpdateWrappedTime(Time.deltaTime);
			return GetYawRotation();
		}
	}

	private GhostEyeState currentEyeState;

	private readonly Dictionary<GhostEyeState, IGhostEyeState> ghostEyeStates = new Dictionary<GhostEyeState, IGhostEyeState>
	{
		{
			GhostEyeState.RandomEyeRoll,
			new RandomEyeRoll()
		},
		{
			GhostEyeState.LookAtTarget,
			new LookAtTarget()
		},
		{
			GhostEyeState.DieRollback,
			new DieRollback()
		},
		{
			GhostEyeState.SneakySideToSide,
			new SneakySideToSide()
		}
	};

	private const float transitionTime = 10f;

	private float currentTransitionTime;

	public float maxPitch = 20f;

	public float maxYaw = 70f;

	public Transform eyeBall;

	private void Start()
	{
		Spawn();
	}

	private void Update()
	{
		Quaternion newRotation = ghostEyeStates[currentEyeState].Update(this);
		eyeBall.localRotation = TransitionSmooth(newRotation);
		currentTransitionTime += Time.deltaTime;
	}

	public void Spawn()
	{
		SetEyeState(GhostEyeState.RandomEyeRoll);
	}

	private void SetEyeState(GhostEyeState ghostEyeState)
	{
		currentTransitionTime = 0f;
		ghostEyeStates[currentEyeState].Exit();
		ghostEyeStates[ghostEyeState].Enter(this);
		currentEyeState = ghostEyeState;
	}

	public void UpdateLookAtTarget(Vector3 target)
	{
		((LookAtTarget)ghostEyeStates[GhostEyeState.LookAtTarget]).SetTarget(target);
		if (currentEyeState != GhostEyeState.LookAtTarget)
		{
			SetEyeState(GhostEyeState.LookAtTarget);
		}
	}

	public void ClearLookAtTarget()
	{
		if (currentEyeState == GhostEyeState.LookAtTarget)
		{
			SetEyeState(GhostEyeState.RandomEyeRoll);
		}
	}

	private Quaternion TransitionSmooth(Quaternion newRotation)
	{
		if (currentTransitionTime > 10f)
		{
			return newRotation;
		}
		return Quaternion.Slerp(eyeBall.localRotation, newRotation, currentTransitionTime / 10f);
	}
}
