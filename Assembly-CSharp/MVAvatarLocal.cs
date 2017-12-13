using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVAvatarLocal : MVAvatar, ILocalObject, ICurrentItemOwner, IBulletImpactVisualizer
{
	private class AvatarLocalModes
	{
		private readonly Dictionary<AvatarRuntimeState, AvatarMode> avatarModes = new Dictionary<AvatarRuntimeState, AvatarMode>();

		private AvatarMode currentMode;

		private AvatarRuntimeState currentState;

		public AvatarMode CurrentMode => currentMode;

		public AvatarRuntimeState CurrentState => currentState;

		public AvatarLocalModes(MVAvatarLocal avatar)
		{
			avatarModes.Add(AvatarRuntimeState.Playing, new WalkMode(avatar));
			avatarModes.Add(AvatarRuntimeState.Edit2D, new EditorAvatarMode2D(avatar));
			avatarModes.Add(AvatarRuntimeState.Edit, new JetPackMode(avatar));
			avatarModes.Add(AvatarRuntimeState.Dead, new DeadMode(avatar));
			avatarModes.Add(AvatarRuntimeState.Hidden, new LobbyMode(avatar));
			avatarModes.Add(AvatarRuntimeState.Godzilla, new GodzillaMode(avatar));
			avatarModes.Add(AvatarRuntimeState.GodzillaDead, new GodzillaDeadMode(avatar));
			currentState = AvatarRuntimeState.Hidden;
			currentMode = avatarModes[currentState];
		}

		public void FrameUpdate(InputToInGameAction interactionMap)
		{
			currentMode.FrameUpdate(interactionMap);
		}

		public void FixedUpdate(IInputToPlayerMovement movementMap)
		{
			currentMode.FixedUpdate(movementMap);
		}

		public void SetMode(AvatarRuntimeState mode)
		{
			currentMode.DeActivate(mode);
			currentMode = avatarModes[mode];
			currentMode.Activate(currentState);
			currentState = mode;
		}
	}

	public abstract class AvatarMode
	{
		protected readonly MVAvatarLocal mvAvatar;

		public int modeTypes;

		protected AvatarMode(MVAvatarLocal mvAvatar, int modeTypes)
		{
			this.mvAvatar = mvAvatar;
			this.modeTypes = modeTypes;
		}

		public abstract void DeActivate(AvatarRuntimeState toMode);

		public abstract void FixedUpdate(IInputToPlayerMovement movementMap);

		public abstract void FrameUpdate(InputToInGameAction interactionMap);

		public virtual void Activate(AvatarRuntimeState fromMode)
		{
			mvAvatar.AvatarModeTypeFlags = modeTypes;
		}
	}

	protected class DeadMode(MVAvatarLocal mvAvatar) : AvatarMode(mvAvatar, 2)
	{
		private class AvatarInputControllerDead : IMotorAPI
		{
			private Quaternion rot = Quaternion.identity;

			public Vector3 Direction => Vector3.zero;

			public Quaternion Rotation
			{
				get
				{
					return rot;
				}
				set
				{
					rot = value;
				}
			}

			public bool Jump => false;
		}

		protected float deadTime;

		protected float deadInterval = 2.5f;

		private AvatarInputControllerDead inputController = new AvatarInputControllerDead();

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			deadTime = Time.time;
			mvAvatar.SetAnimation("Dead");
			mvAvatar.avatarEquipable.Unequip();
			if (mvAvatar.IsSeated)
			{
				mvAvatar.LeaveVehicle(leaveBecauseOfServer: false);
			}
			mvAvatar.triggerHandler.enabled = false;
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
			inputController.Rotation = mvAvatar.avatarMotor.transform.rotation;
			mvAvatar.avatarMotor.FixedUpdateFunction(inputController);
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
			if (mvAvatar.avatarMotor.enabled)
			{
				mvAvatar.avatarMotor.UpdateFunction();
			}
			if (Time.time - deadTime > deadInterval)
			{
				mvAvatar.SetMode(AvatarRuntimeState.Hidden);
			}
		}
	}

	public abstract class EditAvatarModeBase : AvatarMode
	{
		protected EditAvatarModeBase(MVAvatarLocal mvAvatar, int modeTypes)
			: base(mvAvatar, modeTypes)
		{
		}

		protected void SetToEditMode()
		{
			mvAvatar.avatarEquipable.Equip(AvatarItemType.LaserPointer, AvatarEquipableType.Weapon, null);
			mvAvatar.triggerHandler.enabled = false;
			MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
		}
	}

	public class EditorAvatarMode2D(MVAvatarLocal mvAvatar) : EditAvatarModeBase(mvAvatar, 0)
	{
		private const float distanceModifierDivider = -15f;

		private const float distanceMinModifier = 1f;

		private const float distanceMaxModifier = 10f;

		private float resetZ;

		private float speed = 8f;

		private float fastSpeed = 35f;

		private float speedSmoothingTime = 10f;

		private float moveBehindOnInsert = -0.3f;

		private float closestDistanceInBehindDrawPlaneMode = -0.1f;

		private bool behindDrawPlaneModeEnabled;

		private float zSpawnOffset = -30f;

		private float keyVelocity;

		private float keyAcceleration = 10f;

		private float keyDamping = 10f;

		private readonly float heightAdjustSpeed = 5f;

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			mvAvatar.SetAnimation("Idle");
			if (mvAvatar.Body == null)
			{
				Debug.LogWarning("mvAvatar.Body not present");
				return;
			}
			mvAvatar.Body.Visible = false;
			MVGameControllerBase.CameraController.SetCamera(CameraType.EditorCamera2D);
			mvAvatar.ResetAvatar();
			SetToEditMode();
			mvAvatar.Collider.enabled = false;
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
			if (toMode == AvatarRuntimeState.Playing)
			{
				float z = GetSpawnTransform().position.z;
				Vector3 position = mvAvatar.gameObject.transform.position;
				resetZ = position.z;
				position.z = z;
				mvAvatar.gameObject.transform.position = position;
				MVGameControllerBase.CameraController.StartTransitionCam(0.5f);
			}
		}

		public void SetToSpawn()
		{
			mvAvatar.SetToSpawnTransform();
			Vector3 position = mvAvatar.Transform.position;
			position.z += zSpawnOffset;
			mvAvatar.SetTransform(position, mvAvatar.transform.rotation);
		}

		protected void SetToZPos(float z)
		{
			Vector3 position = mvAvatar.gameObject.transform.position;
			position.z = z;
			mvAvatar.SetTransform(position, mvAvatar.transform.rotation);
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
		}

		public void ResetToZPos()
		{
			SetToZPos(resetZ);
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
			Vector3 inputDirection = GetInputDirection();
			float b = speed;
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveFast))
			{
				b = fastSpeed;
			}
			speed = Mathf.Lerp(speed, b, speedSmoothingTime * Time.deltaTime);
			Vector3 vector = speed * inputDirection * Time.deltaTime;
			float distanceToPlaneModifier = 1f;
			if (mvAvatar.WorldPosition.z < 0f)
			{
				distanceToPlaneModifier = Mathf.Clamp(mvAvatar.WorldPosition.z / -15f, 1f, 10f);
			}
			mvAvatar.Transform.position += vector + GetElevationVelocity(distanceToPlaneModifier);
			if (behindDrawPlaneModeEnabled && mvAvatar.WorldPosition.z > closestDistanceInBehindDrawPlaneMode)
			{
				Vector3 position = mvAvatar.Transform.position;
				position.z = closestDistanceInBehindDrawPlaneMode;
				mvAvatar.Transform.position = position;
			}
		}

		protected Vector3 GetElevationVelocity(float distanceToPlaneModifier)
		{
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveUp))
			{
				keyVelocity += keyAcceleration * Time.deltaTime;
			}
			else if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveDown))
			{
				keyVelocity -= keyAcceleration * Time.deltaTime;
			}
			else if (MVInputWrapper.GetAxis("Mouse ScrollWheel") != 0f)
			{
				if (MVInputWrapper.GetAxis("Mouse ScrollWheel") > 0f)
				{
					keyVelocity = 3f;
				}
				else
				{
					keyVelocity = -3f;
				}
			}
			else
			{
				float num = Mathf.Min(1f, keyDamping * Time.deltaTime);
				keyVelocity = (1f - num) * keyVelocity;
			}
			float num2 = keyVelocity * distanceToPlaneModifier * Time.deltaTime;
			return num2 * Vector3.up * heightAdjustSpeed;
		}

		private static Vector3 GetInputDirection()
		{
			Vector3 zero = Vector3.zero;
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveForward))
			{
				zero += Vector3.forward;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveBackwards))
			{
				zero += Vector3.back;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveLeft))
			{
				zero += Vector3.left;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveRight))
			{
				zero += Vector3.right;
			}
			return zero.normalized;
		}

		public void EnableBehindDrawPlaneMode()
		{
			if (mvAvatar.WorldPosition.z > closestDistanceInBehindDrawPlaneMode)
			{
				SetToZPos(moveBehindOnInsert);
			}
			behindDrawPlaneModeEnabled = true;
		}

		public void DisableBehindDrawPlaneMode()
		{
			behindDrawPlaneModeEnabled = false;
		}
	}

	public class JetPackMode : EditAvatarModeBase
	{
		private const float moveSlowDownPoint = 0.75f;

		private readonly float maxSpeed = 1.75f;

		private readonly float speedModifier = 5f;

		private Vector3 jetPackTargetDeltaPos;

		private float targetSpeed;

		private float speed;

		private float speedSmoothingTime = 10f;

		private bool moveConstraintSet;

		private Vector3 moveConstraintCenter;

		private float moveConstraintRadius;

		private float keyVelocity;

		private float keyAcceleration = 20f;

		private float keyDamping = 10f;

		private readonly float heightAdjustSpeed = 5f;

		private float YMovementSpeedScale { get; set; }

		private float XZMovementSpeedScale { get; set; }

		public bool MovementConstrained
		{
			get
			{
				return moveConstraintSet;
			}
			set
			{
				moveConstraintSet = value;
			}
		}

		public JetPackMode(MVAvatarLocal mvAvatar)
			: base(mvAvatar, 0)
		{
			YMovementSpeedScale = 1f;
			XZMovementSpeedScale = 1f;
		}

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			mvAvatar.SetAnimation("Idle");
			if (mvAvatar.Body == null)
			{
				Debug.LogWarning("mvAvatar.Body not present");
				return;
			}
			mvAvatar.Body.Visible = false;
			mvAvatar.ResetAvatar();
			SetToEditMode();
			MVGameControllerBase.CameraController.SetCamera(CameraType.EditorCamera);
			mvAvatar.Collider.enabled = false;
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
		}

		public void SetToSpawn()
		{
			mvAvatar.SetToSpawnTransform();
			MVGameControllerBase.CameraController.CurCamera.Reset();
		}

		public void ModifySpeed(float xz, float y)
		{
			XZMovementSpeedScale = xz;
			YMovementSpeedScale = y;
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
			Move(GetMovementVelocity());
			MoveCharacter(GetElevationVelocity() * YMovementSpeedScale);
			UpdateRotationToCamera();
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
		}

		public void SetMoveConstraint(Vector3 center, float radius)
		{
			MovementConstrained = true;
			moveConstraintCenter = center;
			moveConstraintRadius = radius;
		}

		private void UpdateRotationToCamera()
		{
			Vector3 eulerAngles = mvAvatar.GameObject.transform.eulerAngles;
			eulerAngles.y = MVGameControllerBase.CameraController.transform.eulerAngles.y;
			mvAvatar.GameObject.transform.eulerAngles = eulerAngles;
		}

		private void MoveCharacter(Vector3 moveDelta)
		{
			if (moveConstraintSet)
			{
				Vector3 position = mvAvatar.GameObject.transform.position;
				Vector3 b = position + moveDelta;
				Vector3 normalized = (moveConstraintCenter - position).normalized;
				Vector3 normalized2 = moveDelta.normalized;
				if (!(0.5f < Vector3.Dot(normalized2, normalized)))
				{
					float num = Vector3.Distance(moveConstraintCenter, b);
					float num2 = 0.75f * moveConstraintRadius;
					if (num2 < num)
					{
						float num3 = num - num2;
						float num4 = 0.25f * moveConstraintRadius;
						float num5 = 0f;
						if (num3 < num4)
						{
							num5 = 1f - num3 / num4;
						}
						moveDelta *= num5 * num5;
					}
				}
			}
			mvAvatar.GameObject.transform.position += moveDelta;
		}

		protected Vector3 GetElevationVelocity()
		{
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveUp))
			{
				keyVelocity += keyAcceleration * Time.deltaTime;
			}
			else if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveDown))
			{
				keyVelocity -= keyAcceleration * Time.deltaTime;
			}
			else if (MVInputWrapper.GetAxis("Mouse ScrollWheel") != 0f)
			{
				if (MVInputWrapper.GetAxis("Mouse ScrollWheel") > 0f)
				{
					keyVelocity = 3f;
				}
				else
				{
					keyVelocity = -3f;
				}
			}
			else
			{
				float num = Mathf.Min(1f, keyDamping * Time.deltaTime);
				keyVelocity = (1f - num) * keyVelocity;
			}
			float num2 = keyVelocity * Time.deltaTime;
			return num2 * Vector3.up * heightAdjustSpeed;
		}

		private void Move(Vector3 velocity)
		{
			Vector3 vector = velocity * Time.deltaTime;
			MoveCharacter(vector);
			Vector3 forward = vector;
			forward.y = 0f;
			if ((double)forward.sqrMagnitude > 0.001)
			{
				mvAvatar.GameObject.transform.rotation = Quaternion.LookRotation(forward);
			}
		}

		private Vector3 GetDirection()
		{
			Transform transform = Camera.main.transform;
			Vector3 result = transform.rotation * GetInputDirection();
			if (!MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveFast))
			{
				result.y = 0f;
			}
			result.Normalize();
			return result;
		}

		private static Vector3 GetInputDirection()
		{
			Vector3 zero = Vector3.zero;
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveForward))
			{
				zero += Vector3.forward;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveBackwards))
			{
				zero += Vector3.back;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveLeft))
			{
				zero += Vector3.left;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveRight))
			{
				zero += Vector3.right;
			}
			return zero.normalized;
		}

		private Vector3 GetMovementVelocity()
		{
			Vector3 direction = GetDirection();
			targetSpeed = direction.magnitude * maxSpeed * ((!MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveFast)) ? 1f : speedModifier);
			speed = Mathf.Lerp(speed, targetSpeed, speedSmoothingTime * Time.deltaTime);
			return direction * speed * speedModifier * XZMovementSpeedScale;
		}
	}

	protected class LobbyMode : AvatarMode
	{
		public LobbyMode(MVAvatarLocal mvAvatar)
			: base(mvAvatar, 4)
		{
		}

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			LayerUtil.SetLayerRecursively(mvAvatar.Body.Transform, "Player", "CamRotateTarget");
			MVGameControllerBase.CameraController.BlueModeEnabled = true;
			MVGameControllerBase.IPlayModeUI.InLobbyState = true;
			mvAvatar.Body.Transform.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
			if (mvAvatar.Respawned != null)
			{
				mvAvatar.Respawned(this, EventArgs.Empty);
			}
			mvAvatar.ResetAvatar();
			mvAvatar.SetToSpawnTransform();
			MVGameControllerBase.CameraController.PushCamera(CameraType.LobbyState);
			LobbyStateCamera lobbyStateCamera = (LobbyStateCamera)MVGameControllerBase.CameraController.CurCamera;
			lobbyStateCamera.SetRotation(mvAvatar.transform.rotation);
			mvAvatar.Body.Visible = true;
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
			mvAvatar.Body.Transform.localRotation = Quaternion.AngleAxis(0f, Vector3.up);
			MVGameControllerBase.CameraController.BlueModeEnabled = false;
			LayerUtil.SetLayerRecursively(mvAvatar.Body.Transform, "CamRotateTarget", "Player");
			MVGameControllerBase.CameraController.RemoveCamera(CameraType.LobbyState);
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
		}
	}

	protected class WalkMode : AvatarMode
	{
		private const float maxFallBelow = 200f;

		private const float respawnTimeOut = 2f;

		private readonly IAvatarInputController avatarInputController;

		private bool isFiring;

		private bool isJumping;

		private readonly AvatarSound avatarSound;

		private readonly float swimStartProximity = 0.6f;

		private float prevWaterProximity;

		private bool IsSwimming => swimStartProximity <= prevWaterProximity;

		public WalkMode(MVAvatarLocal mvAvatar)
			: base(mvAvatar, 1)
		{
			AvatarPickupOwner pickupOwner = mvAvatar.pickupOwner;
			pickupOwner.onHandleFiring = (MVPickupOwner.OnHandleFiringDelegate)Delegate.Combine(pickupOwner.onHandleFiring, new MVPickupOwner.OnHandleFiringDelegate(OnHandleFiring));
			avatarInputController = CreateInputController();
			avatarSound = mvAvatar.GameObject.GetComponent<AvatarSound>();
			AvatarMotor avatarMotor = mvAvatar.avatarMotor;
			avatarMotor.OnWallJump = (AvatarMotor.OnWallJumpDelegate)Delegate.Combine(avatarMotor.OnWallJump, new AvatarMotor.OnWallJumpDelegate(avatarSound.HandleWallJump));
			AvatarMotor avatarMotor2 = mvAvatar.avatarMotor;
			avatarMotor2.OnActiveBounce = (AvatarMotor.OnActiveBounceDelegate)Delegate.Combine(avatarMotor2.OnActiveBounce, new AvatarMotor.OnActiveBounceDelegate(avatarSound.HandleActiveBounce));
		}

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			switch (fromMode)
			{
			case AvatarRuntimeState.Hidden:
			case AvatarRuntimeState.Dead:
			case AvatarRuntimeState.GodzillaDead:
				OnRespawn();
				break;
			}
			MVGameControllerBase.CameraController.BlueModeEnabled = false;
			MVGameControllerBase.CameraController.SetPlayModeCam();
			MVGameControllerBase.CameraController.CurCamera.Reset();
			MVGameControllerBase.WOCM.UpdateWorldBounds(SharedCubeFunctions.GetAxisAlignedBoundsRecursively(MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().Transform).Value);
			mvAvatar.triggerHandler.enabled = true;
			mvAvatar.ResetAvatar();
			mvAvatar.Collider.enabled = true;
			avatarInputController.Rotation = mvAvatar.transform.rotation;
			mvAvatar.LimbManager.SetLimbRotatorActivity(shouldBeActive: true);
			if (mvAvatar.Body != null)
			{
				mvAvatar.Body.Visible = true;
			}
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
			mvAvatar.LimbManager.SetLimbRotatorActivity(shouldBeActive: false);
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
			HandleFocus();
			if (mvAvatar.avatarMotor.enabled)
			{
				mvAvatar.avatarMotor.UpdateFunction();
			}
			if (mvAvatar.gameObject.transform.position.y < MVGameControllerBase.WOCM.WorldBounds.min.y - 200f)
			{
				DieByFalling();
			}
			if (interactionMap.Use && !mvAvatar.RigidBody.IsMovementLocked)
			{
				if (mvAvatar.IsSeated)
				{
					mvAvatar.LeaveVehicle(leaveBecauseOfServer: false);
				}
				else
				{
					mvAvatar.useInteractorHandler.Use();
				}
			}
			if (mvAvatar.pickupOwner.CurrentItem != null)
			{
				HandlePickupUpdate(interactionMap);
			}
			if (!mvAvatar.IsSeated && mvAvatar.avatarMotor.IsStuck())
			{
				HandleStuck();
			}
		}

		private void HandlePickupUpdate(InputToInGameAction interactionMap)
		{
			bool isHolstered = mvAvatar.pickupOwner.CurrentItem.IsHolstered;
			mvAvatar.pickupOwner.SetLineOfFireLocal();
			if (!isHolstered)
			{
				mvAvatar.pickupOwner.HandleFire(interactionMap.Fire, mvAvatar.IsFiring);
			}
			mvAvatar.pickupOwner.HandlePointing(interactionMap.Fire);
			if (mvAvatar.pickupOwner.CurrentItem.Type == AvatarItemType.Hand || (mvAvatar.IsSeated && !IsInJetpack()))
			{
				return;
			}
			if (mvAvatar.pickupOwner.CurrentItem.CanHolster)
			{
				bool flag = !isHolstered && interactionMap.Holster;
				bool flag2 = isHolstered && interactionMap.Holster;
				if (flag)
				{
					mvAvatar.avatarEquipable.Holster();
				}
				else if (flag2)
				{
					mvAvatar.avatarEquipable.Unholster();
				}
			}
			if (interactionMap.Drop)
			{
				mvAvatar.avatarEquipable.Unequip();
			}
		}

		private bool IsInJetpack()
		{
			int woIDWithLocalOwnerHighestInHierarchy = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(mvAvatar.Id);
			if (woIDWithLocalOwnerHighestInHierarchy == -1)
			{
				return false;
			}
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDWithLocalOwnerHighestInHierarchy);
			if (!(worldObjectClient is MVJetPack))
			{
				return false;
			}
			return true;
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
			if (!mvAvatar.avatarMotor.enabled)
			{
				mvAvatar.avatarMotor.UpdateVelocity();
				return;
			}
			HandleWaterplane();
			avatarInputController.HandleInput(movementMap.Direction, movementMap.Jump, isFiring, mvAvatar.avatarMotor.Velocity, mvAvatar.InGunMode, mvAvatar.ForceRotateAvatarToFiringDirection);
			mvAvatar.avatarMotor.FixedUpdateFunction(avatarInputController);
			if (!mvAvatar.IsDead)
			{
				SetAnimationState(avatarInputController.Direction);
			}
		}

		private static void HandleFocus()
		{
			if (MVGameControllerBase.CameraController.CurCamera.CameraType == CameraType.LobbyState && !MVGameControllerBase.IPlayModeUI.InLobbyState)
			{
				MVGameControllerBase.CameraController.RemoveCamera(CameraType.LobbyState);
			}
			else if (MVGameControllerBase.CameraController.CurCamera.CameraType != CameraType.LobbyState && MVGameControllerBase.IPlayModeUI.InLobbyState)
			{
				MVGameControllerBase.CameraController.PushCamera(CameraType.LobbyState);
				LobbyStateCamera lobbyStateCamera = (LobbyStateCamera)MVGameControllerBase.CameraController.CurCamera;
				lobbyStateCamera.SetRotation(MVGameControllerBase.CameraController.transform.rotation);
			}
		}

		private void HandleStuck()
		{
			mvAvatar.Die();
			MVGameControllerBase.OperationRequests.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameControllerBase.Game.LocalPlayer.ActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr, PlayerKilledByType.Crushed));
		}

		private void DieByFalling()
		{
			mvAvatar.Health.Value = 0f;
			MVGameControllerBase.OperationRequests.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameControllerBase.Game.LocalPlayer.ActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr, PlayerKilledByType.FallOffWorld));
		}

		private void OnHandleFiring(bool isFiring)
		{
			this.isFiring = isFiring;
		}

		private void HandleWaterplane()
		{
			WaterPlaneManager waterPlaneManager = MVGameControllerBase.WaterPlaneManager;
			float num = waterPlaneManager.ComputeAvatarWaterProximity(mvAvatar.GameObject.transform.position);
			if (prevWaterProximity < swimStartProximity && swimStartProximity <= num)
			{
				mvAvatar.SetAnimation("Swim");
			}
			else if (swimStartProximity < prevWaterProximity && num < swimStartProximity)
			{
				mvAvatar.SetAnimation("Walk");
			}
			prevWaterProximity = num;
		}

		private void SetAnimationState(Vector3 moveDirection)
		{
			isJumping = mvAvatar.avatarMotor.IsJumping();
			if (IsSwimming)
			{
				mvAvatar.SetAnimation("Swim");
			}
			else if (isJumping)
			{
				mvAvatar.SetAnimation("Jump");
			}
			else if (moveDirection.sqrMagnitude > 0f)
			{
				mvAvatar.SetAnimation("Walk");
			}
			else
			{
				mvAvatar.SetAnimation("Idle");
			}
		}

		private IAvatarInputController CreateInputController()
		{
			if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
			{
				return new AvatarInputController();
			}
			if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
			{
				return new AvatarInputController();
			}
			if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
			{
				return new AvatarInputController2DPlayMode();
			}
			throw new Exception("Implement input controller");
		}

		public void OnRespawn()
		{
			mvAvatar.interactableLocal.AddModifier(AvatarModifierPackageType.SpawnProtection);
		}
	}

	protected class GodzillaDeadMode : DeadMode
	{
		private const float modRemovalDelay = 0.25f;

		private bool hasRestoredFromGodzillamode;

		private FirstPersonDeathCamera camera;

		public GodzillaDeadMode(MVAvatarLocal mvAvatar)
			: base(mvAvatar)
		{
		}

		public override void Activate(AvatarRuntimeState fromMode)
		{
			mvAvatar.AvatarModeTypeFlags = modeTypes;
			deadTime = Time.time;
			hasRestoredFromGodzillamode = false;
			mvAvatar.SetAnimation("Dead");
			camera = UnityEngine.Object.Instantiate(PrefabPool.Instance.FirstPersonDeathCamera);
			MVGameControllerBase.CameraController.PushCamera(camera);
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
			base.DeActivate(toMode);
			MVGameControllerBase.CameraController.RemoveCamera(camera);
			UnityEngine.Object.Destroy(camera.gameObject);
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
			if (!hasRestoredFromGodzillamode && (Time.time - deadTime) / deadInterval > 0.25f)
			{
				mvAvatar.interactableLocal.RemoveModifier(AvatarModifierPackageType.GodzillaS);
				mvAvatar.interactableLocal.RemoveModifier(AvatarModifierPackageType.GodzillaM);
				mvAvatar.interactableLocal.RemoveModifier(AvatarModifierPackageType.GodzillaL);
				mvAvatar.interactableLocal.RemoveModifier(AvatarModifierPackageType.GodzillaXL);
				mvAvatar.avatarEquipable.Unequip();
			}
			base.FrameUpdate(interactionMap);
		}
	}

	public class GodzillaMode : AvatarMode
	{
		private const float levitationHeight = 0.2f;

		public static readonly string screenName = TM._("Colossus");

		private MVCameraBase camera;

		private GodzillaTrigger triggerRef;

		private AvatarModifierPackageType activeModifierPackageType;

		public GodzillaMode(MVAvatarLocal mvAvatar)
			: base(mvAvatar, 1)
		{
		}

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			if (mvAvatar.IsSeated)
			{
				mvAvatar.LeaveVehicle(leaveBecauseOfServer: false);
			}
			mvAvatar.triggerHandler.enabled = false;
			mvAvatar.Collider.enabled = true;
			mvAvatar.RigidBody.enabled = false;
			mvAvatar.SetAnimation("Idle");
			mvAvatar.Body.Visible = true;
			mvAvatar.interactableLocal.ClearModifiers();
			ActivateGodzillaCamera();
		}

		public void ActivateModifier(AvatarModifierPackageType godzillaType)
		{
			if (activeModifierPackageType != AvatarModifierPackageType.None)
			{
				mvAvatar.interactableLocal.AddModifier(godzillaType);
				activeModifierPackageType = godzillaType;
				mvAvatar.avatarEquipable.Unequip();
			}
			activeModifierPackageType = godzillaType;
			mvAvatar.interactableLocal.AddModifier(godzillaType);
			mvAvatar.interactableLocal.AddModifier(AvatarModifierPackageType.GodzillaGrowthInvulnerability);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("avatarModifierPackageType", (byte)activeModifierPackageType);
			Dictionary<object, object> itemData = dictionary;
			mvAvatar.avatarEquipable.Equip(AvatarItemType.GodzillaLaser, AvatarEquipableType.Weapon, itemData);
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
			DeactivateGodzillaCamera();
			mvAvatar.RigidBody.enabled = true;
			mvAvatar.avatarMotor.Reset();
			if (triggerRef != null)
			{
				triggerRef.Exit(mvAvatar.id);
			}
			if (toMode != AvatarRuntimeState.GodzillaDead)
			{
				mvAvatar.interactableLocal.RemoveModifier(activeModifierPackageType);
				mvAvatar.avatarEquipable.Unequip();
			}
			activeModifierPackageType = AvatarModifierPackageType.None;
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
			if (triggerRef == null)
			{
				mvAvatar.SetMode(AvatarRuntimeState.Playing);
				return;
			}
			float y = 0.2f * mvAvatar.Scale.y;
			mvAvatar.Position = triggerRef.Position + new Vector3(0f, y, 0f);
			if (!interactionMap.IgnorePickupOwner)
			{
				mvAvatar.pickupOwner.HandleFire(interactionMap.Fire, mvAvatar.IsFiring);
			}
		}

		public void SetGodzillaTriggerRef(GodzillaTrigger trigger)
		{
			triggerRef = trigger;
		}

		private void ActivateGodzillaCamera()
		{
			MVGameControllerBase.CameraController.BlueModeEnabled = false;
			switch (MVGameControllerBase.Game.GameType)
			{
			case MVGameType.Classic:
				camera = UnityEngine.Object.Instantiate(PrefabPool.Instance.GodzillaCameraDesktop);
				break;
			case MVGameType.Platformer:
				camera = UnityEngine.Object.Instantiate(PrefabPool.Instance.GodzillaCamera2D);
				((GodzillaCamera2D)camera).SetScale(GodzillaModifier.constants[(GodzillaModifier.GodzillaModifierPackageType)activeModifierPackageType].sizeModifier);
				break;
			default:
				Debug.LogError("Unknown game mode.");
				break;
			}
			MVGameControllerBase.CameraController.PushCamera(camera);
		}

		private void DeactivateGodzillaCamera()
		{
			MVGameControllerBase.CameraController.CancelTransitionCam();
			MVGameControllerBase.CameraController.RemoveCamera(camera);
			UnityEngine.Object.Destroy(camera.gameObject);
		}
	}

	private const float exitVehicleMomentumModifier = 7f;

	private string currAnim = string.Empty;

	private AvatarMotor avatarMotor;

	private AvatarInteractable interactableLocal;

	private UseInteractorHandler useInteractorHandler;

	private MVTriggerHandler triggerHandler;

	private AvatarEquipable avatarEquipable;

	private AvatarPickupOwner pickupOwner;

	private PickupGUI pickupGUI;

	private MVRigidBody vehicleRigidBody;

	private AvatarLocalModes avatarLocalModes;

	public Action<string> OnKilled;

	private float previousHealth;

	private float previousShield;

	public Action<float, MVPlayer, PlayerKilledByType> OnDamageTaken;

	public AvatarInteractable InteractableLocal => interactableLocal;

	public AvatarPickupOwner PickupOwner => pickupOwner;

	private AvatarRuntimeState CurrentState => avatarLocalModes.CurrentState;

	public Vector3 LookAtPos => transform.position + Vector3.up;

	public bool IsSeated
	{
		get
		{
			if (!RunTimeData.ContainsObscuredKey("seat"))
			{
				Debug.LogError("MVAvatarLocal does not contain key seat");
				return false;
			}
			return (int)(ObscuredInt)RunTimeData.GetObscuredType("seat") != -1;
		}
	}

	public bool InGunMode
	{
		get
		{
			if (pickupOwner == null)
			{
				return false;
			}
			return pickupOwner.InGunMode;
		}
	}

	public bool ForceRotateAvatarToFiringDirection { private get; set; }

	public ILaserPointer LaserPointer => avatarPickupOwner.LaserPointer;

	public MVRigidBody RigidBody => avatarMotor;

	public MVTriggerHandler TriggerHandler => triggerHandler;

	public bool IsDead => IsInMode(AvatarModeTypes.Dead);

	public bool IsEnteringVehicle => MVGameControllerBase.Game.PlayerController.IsEnteringVehicle;

	public bool IsInVehicle => vehicleRigidBody != null;

	public override Vector3 VelocityRelative
	{
		get
		{
			if (avatarMotor == null)
			{
				return Vector3.zero;
			}
			return avatarMotor.Velocity;
		}
	}

	public override Vector3 VelocityAbsolute => (!IsInVehicle) ? RigidBody.Velocity : vehicleRigidBody.Velocity;

	public AvatarMode CurrentMode => avatarLocalModes.CurrentMode;

	public event EventHandler Respawned;

	public MVAvatarLocal(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		SetNetworkObject(local: true);
	}

	private void RelayDamageEvent(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (OnDamageTaken != null)
		{
			OnDamageTaken(amount, damageDealer, damageType);
		}
		if (Health.Value <= 0f)
		{
			int num = damageDealer?.ActorNr ?? MVGameControllerBase.Game.LocalPlayer.ActorNr;
			int actorNr = MVGameControllerBase.Game.LocalPlayer.ActorNr;
			if (OnKilled != null)
			{
				bool shotSelf = actorNr == num;
				string obj = string.Format(KillNotification.GetKillText(damageType, shotSelf), MVGameControllerBase.Game.MVPlayerContainer[actorNr].Username, MVGameControllerBase.Game.MVPlayerContainer[num].Username);
				OnKilled(obj);
			}
		}
	}

	public float GetColliderRadius()
	{
		return avatarMotor.GetSizeState.ControllerRadius;
	}

	public override void Initialize()
	{
		base.Initialize();
		avatarMotor = gameObject.AddComponent<AvatarMotor>();
		triggerHandler = gameObject.AddComponent<MVTriggerHandler>();
		AvatarInteractable avatarInteractable = gameObject.AddComponent<AvatarInteractable>();
		avatarInteractable.Init(Modifiers, Health, Shield);
		interactableLocal = avatarInteractable;
		AvatarInteractable avatarInteractable2 = interactableLocal;
		avatarInteractable2.OnDamageTaken = (Action<float, MVPlayer, PlayerKilledByType>)Delegate.Combine(avatarInteractable2.OnDamageTaken, new Action<float, MVPlayer, PlayerKilledByType>(RelayDamageEvent));
		avatarEquipable = gameObject.AddComponent<AvatarEquipable>();
		avatarEquipable.Init(interactableLocal, CurrentItem);
		avatarMotor.Init(avatarInteractable, CharacterControllerCenterOffset);
		pickupOwner = avatarPickupOwner;
		if (MVGameControllerBase.GameMode != MVGameMode.CharacterEditor)
		{
			pickupGUI = gameObject.AddComponent<PickupGUI>();
			pickupGUI.Initialize(pickupOwner);
		}
		avatarLocalModes = new AvatarLocalModes(this);
		InitializeHealth();
		InitializeShield();
		avatar.DeactivateBars();
		MVGameControllerBase.WOCM.AvatarLocal = this;
		InitializeAvatarState(MVGameControllerBase.GameMode, MVGameControllerBase.Game.GameType);
		if (MVGameControllerBase.GameMode != MVGameMode.CharacterEditor)
		{
			useInteractorHandler = gameObject.AddComponent<UseInteractorHandler>();
			useInteractorHandler.Init(Collider);
		}
		MVGameControllerBase.Game.PlayerController.Push(this);
		avatarMotor.GetSizeState.EquipSlapGunEvent += avatarEquipable.EquipSlapGun;
		avatarMotor.GetSizeState.CameraScaleEvent += OnCameraScale;
		avatarMotor.GetSizeState.UnEquipSlapGunEvent += OnUnequip;
		avatarInteractable.ModifierPackages.OnUnequipItemEvent += OnUnequip;
		avatarInteractable.ModifierPackages.OnDisableVehiclesEvent += OnDisableVehicles;
		AvatarShieldDecay avatarShieldDecay = gameObject.AddComponent<AvatarShieldDecay>();
		avatarShieldDecay.Init(Shield);
		avatarInteractable.OnShieldReplenished = (Action)Delegate.Combine(avatarInteractable.OnShieldReplenished, new Action(avatarShieldDecay.ResetDecayTimer));
		CullingApiWrapper.SetDistanceReferencePoint(transform);
		limbManager = new AvatarLimbManagerLocal();
		limbManager.Initialize(avatarPickupOwner, this);
		avatarlateUpdateManager.Initialize(Body, limbManager);
	}

	public void ResetMode()
	{
		SetMode(CurrentState);
	}

	public void SetMode(AvatarRuntimeState localMode)
	{
		healParticleSpawnTime = Time.time;
		Shield.Value = 0f;
		avatarLocalModes.SetMode(localMode);
	}

	public void OnUnequip(object sender, EventArgs e)
	{
		AvatarEquipable avatarEquipable = gameObject.GetComponent<MVEquipable>() as AvatarEquipable;
		if (!(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Scale, 1f) > 1f) || !avatarEquipable.GetIsEquipped(AvatarItemType.SlapGun))
		{
			avatarEquipable.Unequip();
		}
	}

	public void SetCharacterController(SmoothCharacterController characterController)
	{
		avatarMotor.OverrideCharacterController(characterController);
	}

	public void LeaveVehicle(bool leaveBecauseOfServer)
	{
		Vector3 impulse = RigidBody.Velocity;
		vehicleRigidBody = null;
		int vehicleID = -1;
		if (!MVGameControllerBase.Game.PlayerController.DetachWorldObjectFromVehicle(Id, ref vehicleID, leaveBecauseOfServer))
		{
			return;
		}
		if (vehicleID != -1)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(vehicleID);
			if (worldObjectClient != null && worldObjectClient is MVVehicleBase)
			{
				((MVVehicleBase)worldObjectClient).LeaveLocal();
				MVRigidBody mVRigidBody = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
				if (mVRigidBody != null)
				{
					impulse = CalculateVehicleExitMomentum(mVRigidBody.Velocity);
				}
			}
			else
			{
				Debug.LogError("vehicleWO is null or type is not IVehicle " + vehicleID);
			}
		}
		HandleLeaveVehicle();
		MVGameControllerBase.CameraController.SetPlayModeCam();
		RigidBody.Reset();
		RigidBody.enabled = true;
		triggerHandler.enabled = true;
		RigidBody.AddImpulse(impulse);
		MVGameControllerBase.Game.TransformNetworkManager.AddReporter(id, new MVNetworkReporter(this));
	}

	private Vector3 CalculateVehicleExitMomentum(Vector3 velocity)
	{
		velocity /= Time.deltaTime;
		velocity /= 2f;
		velocity.y += velocity.magnitude / 7f;
		return velocity;
	}

	public override void BeforeVehicleEntered()
	{
		triggerHandler.Reset();
	}

	public override void OnEnterVehicle()
	{
		if (Group.GameObject.GetComponent<MVRigidBody>() != null)
		{
			RigidBody.enabled = false;
			triggerHandler.enabled = false;
		}
		vehicleRigidBody = MVWorldObjectClientManager.GetEnabledMonoBehaviourHighestInHierarchy<MVRigidBody>(gameObject);
	}

	public InputToInGameAction Update(InputToInGameAction interactionMap)
	{
		avatarLocalModes.FrameUpdate(interactionMap);
		return interactionMap;
	}

	public IInputToPlayerMovement FixedUpdate(IInputToPlayerMovement movementMap)
	{
		avatarLocalModes.FixedUpdate(movementMap);
		return movementMap;
	}

	public void SetAnimation(string animationState)
	{
		if (!(currAnim == animationState))
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)Animation.Value;
			if (currAnim != animationState)
			{
				int serverTimeInMilliSeconds = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
				Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
				dictionary2.Add("state", animationState);
				dictionary2.Add("timeStamp", serverTimeInMilliSeconds);
				dictionary = dictionary2;
				Animation.Value = dictionary;
				currAnim = animationState;
			}
		}
	}

	public void Respawn()
	{
		if (IsInMode(AvatarModeTypes.Playing))
		{
			MVGameControllerBase.WOCM.AvatarLocal.Suicide();
		}
		else if (CurrentState == AvatarRuntimeState.Edit)
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetToSpawnTransform();
		}
		else if (CurrentState == AvatarRuntimeState.Edit2D)
		{
			((EditorAvatarMode2D)avatarLocalModes.CurrentMode).SetToSpawn();
		}
	}

	public void SetToSpawnTransform()
	{
		Transform spawnTransform = GetSpawnTransform();
		SetTransform(spawnTransform.position, spawnTransform.rotation);
	}

	private void Suicide()
	{
		if (!IsInMode(AvatarModeTypes.Dead))
		{
			if (interactableLocal.LastDamageSource == null || interactableLocal.LastDamageSource.Outdated)
			{
				Die();
			}
			else
			{
				interactableLocal.TakeDamage(100f, interactableLocal.LastDamageSource.shooter, interactableLocal.LastDamageSource.damageType);
			}
		}
	}

	private static Transform GetSpawnTransform()
	{
		MVCheckpoint checkpoint = MVGameControllerBase.Game.LocalPlayer.GetCheckpoint();
		if (checkpoint != null)
		{
			return checkpoint.Transform;
		}
		MVLogicObject validSpawnPoint = MVGameControllerBase.WOCM.GetValidSpawnPoint();
		if (validSpawnPoint == null)
		{
			Debug.LogError("No spawn-point found on planet!");
			return null;
		}
		return validSpawnPoint.Transform;
	}

	private void InitializeAvatarState(MVGameMode gameMode, MVGameType gameType)
	{
		switch (gameMode)
		{
		case MVGameMode.Edit:
			switch (gameType)
			{
			case MVGameType.Platformer:
				avatarLocalModes.SetMode(AvatarRuntimeState.Edit2D);
				((EditorAvatarMode2D)avatarLocalModes.CurrentMode).SetToSpawn();
				break;
			case MVGameType.Classic:
				avatarLocalModes.SetMode(AvatarRuntimeState.Edit);
				((JetPackMode)avatarLocalModes.CurrentMode).SetToSpawn();
				break;
			}
			break;
		case MVGameMode.CharacterEditor:
			avatarLocalModes.SetMode(AvatarRuntimeState.Edit);
			break;
		case MVGameMode.Play:
			avatarLocalModes.SetMode(AvatarRuntimeState.Hidden);
			break;
		}
	}

	private void InitializeHealth()
	{
		previousHealth = Health.Value;
		MVRuntimeDataVariableClampedFloat health = Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			if ((float)obj == 0f)
			{
				Die();
			}
			else
			{
				TrySpawningHealParticles(previousHealth, Health.Value);
			}
			previousHealth = Health.Value;
		}));
	}

	private void InitializeShield()
	{
		previousShield = Shield.Value;
		MVRuntimeDataVariableClampedFloat mVRuntimeDataVariableClampedFloat = Shield;
		mVRuntimeDataVariableClampedFloat.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(mVRuntimeDataVariableClampedFloat.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object shield) =>
		{
			TrySpawningHealParticles(previousShield, Shield.Value);
			previousShield = Shield.Value;
		}));
	}

	private void OnDisableVehicles(object sender, EventArgs args)
	{
		if (IsSeated)
		{
			LeaveVehicle(leaveBecauseOfServer: false);
		}
	}

	private void OnCameraScale(object sender, ScaleArgs args)
	{
		MVCameraController.GetSettings(MVGameControllerBase.Game.GameType).ScaleCameraValues(args.scale);
	}

	public override void AttachBody(MVBody newBody)
	{
		base.AttachBody(newBody);
		if (!MVGameControllerBase.Game.IsPlaying)
		{
			newBody.Visible = false;
		}
		else
		{
			newBody.Visible = true;
		}
		MVGameControllerBase.OperationRequests.TransferOwnership(newBody.Id, 0, null);
		foreach (MVWorldObjectClient child in newBody.Children)
		{
			MVGameControllerBase.OperationRequests.TransferOwnership(child.Id, 0, null);
		}
	}

	private void Die()
	{
		if (IsInMode(AvatarModeTypes.Playing))
		{
			Shield.Value = 0f;
			AvatarRuntimeState currentState = CurrentState;
			if (currentState == AvatarRuntimeState.Godzilla)
			{
				avatarLocalModes.SetMode(AvatarRuntimeState.GodzillaDead);
			}
			else
			{
				avatarLocalModes.SetMode(AvatarRuntimeState.Dead);
			}
		}
	}

	private void ResetAvatar()
	{
		triggerHandler.enabled = true;
		int actorNr = MVGameControllerBase.Game.LocalPlayer.ActorNr;
		MVTeam teamFromActorNr = MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(actorNr);
		if (teamFromActorNr != MVTeam.None && !MVGameControllerBase.Game.TeamManager.IsTeamActive(teamFromActorNr))
		{
			List<MVTeam> teamList = MVGameControllerBase.Game.TeamManager.GetTeamList();
			MVGameControllerBase.OperationRequests.SetTeam(teamList[0]);
		}
		SetAnimation("Idle");
		Health.Value = 100f;
		if (IsSeated)
		{
			LeaveVehicle(leaveBecauseOfServer: false);
		}
		avatarEquipable.Unequip();
		interactableLocal.ClearModifiers();
		avatarMotor.Reset();
	}

	private void SetTransform(Vector3 position, Quaternion rotation)
	{
		GameObject.transform.position = position;
		GameObject.transform.rotation = rotation;
		avatarMotor.Reset();
	}

	public void VisualizeBulletImpact(VoxelHit voxelHit, Ray lineOfFire, int shooterActorNumber, float damage = 100f)
	{
		MVPlayer player = null;
		if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(shooterActorNumber, out player) && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(this, player.Avatar) && !IsInMode(AvatarModeTypes.Dead) && !avatar.HasModifierEffect(AvatarModifierEffect.Invulnerable))
		{
			avatar.VisualizeBulletImpact(voxelHit, lineOfFire, shooterActorNumber, damage);
		}
	}

	public Dictionary<object, object> GetCurrentItemState()
	{
		return (Dictionary<object, object>)CurrentItem.Value;
	}

	public void SetCurrentItemState(Dictionary<object, object> aNewState)
	{
		CurrentItem.Value = aNewState;
	}
}
