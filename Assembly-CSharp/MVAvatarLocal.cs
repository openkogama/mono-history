using System;
using System.Collections.Generic;
using Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.Mediator;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePrototypeSettings;
using MV.WorldObject.MetaData;
using UnityEngine;
using UnityEngine.Events;

public class MVAvatarLocal(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects) : MVAvatar(data, PrefabPool.Instance.MVLocalAvatarPrefab, worldObjects), ILocalObject, IBulletImpactVisualizer, ICurrentItemOwner, ISpawnRoleLocal
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
			avatarModes.Add(AvatarRuntimeState.Dead, new DeadMode(avatar));
			avatarModes.Add(AvatarRuntimeState.Revive, new ReviveMode(avatar));
			avatarModes.Add(AvatarRuntimeState.ReviveWait, new ReviveWaitMode(avatar));
			avatarModes.Add(AvatarRuntimeState.Hidden, new LobbyMode(avatar));
			avatarModes.Add(AvatarRuntimeState.TimeAttackFlagDebriefing, new TimeAttackFlagDebriefingMode(avatar));
			avatarModes.Add(AvatarRuntimeState.Wait, new WaitMode(avatar));
			avatarModes.Add(AvatarRuntimeState.Ghost, new GhostMode(avatar));
			currentState = GetStartState();
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
			AvatarRuntimeState fromMode = currentState;
			currentMode.DeActivate(mode);
			currentMode = avatarModes[mode];
			currentState = mode;
			currentMode.Activate(fromMode);
		}

		public void SetToStartMode()
		{
			SetMode(GetStartState());
		}

		private AvatarRuntimeState GetStartState()
		{
			AvatarRuntimeState avatarRuntimeState = AvatarRuntimeState.Playing;
			if (MVGameControllerBase.PlayModeUI.InLobbyState || MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
			{
				return AvatarRuntimeState.Hidden;
			}
			return AvatarRuntimeState.Playing;
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
			SetModeTypes();
		}

		private void SetModeTypes()
		{
			mvAvatar.SpawnRoleModeTypes.Value = modeTypes;
		}
	}

	protected class DeadMode : AvatarMode
	{
		private class AvatarInputControllerDead : IMotorAPI
		{
			private Quaternion rot = Quaternion.identity;

			public Vector3 Direction
			{
				get
				{
					return Vector3.zero;
				}
				set
				{
				}
			}

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

		protected float deadInterval = 4f;

		private AvatarInputControllerDead inputController = new AvatarInputControllerDead();

		public DeadMode(MVAvatarLocal mvAvatar)
			: base(mvAvatar, 2)
		{
			mvAvatar.OnKilled = (Action<int, int, PlayerKilledByType>)Delegate.Combine(mvAvatar.OnKilled, new Action<int, int, PlayerKilledByType>(HandleDeathBriefingPause));
			mvAvatar.OnSuicide = (Action)Delegate.Combine(mvAvatar.OnSuicide, new Action(HandleResetUIPause));
		}

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			MVGameControllerBase.OperationRequests.IncrementStatRequest(IncrementStatRequestType.Died);
			deadTime = Time.time;
			deadInterval = MVGameControllerBase.LocalPlayer.RespawnDuration;
			MVGameControllerBase.LocalPlayer.RespawnTime = Time.time + deadInterval;
			mvAvatar.SetAnimation("Dead");
			mvAvatar.avatarEquipable.Unequip();
			if (mvAvatar.IsSeated)
			{
				mvAvatar.LeaveVehicle(leaveBecauseOfServer: false);
			}
			mvAvatar.triggerHandler.enabled = false;
			MVGameControllerDesktop.LockCursorManager.CursorLock = false;
			MVGameControllerBase.PlayModeUI.InLobbyState = false;
			if (!mvAvatar.InGunMode && MVGameControllerBase.MainCameraManager.CurrentCamera.CameraType == CameraType.FirstPersonCamera)
			{
				mvAvatar.AvatarLocal.CameraController.SetCamera(CameraType.ThirdPerson);
			}
			MVGameControllerBase.Game.GameEventManager.AvatarCommandsBuildMode.OnSetToEditMode += OnEnterEditMode;
		}

		private void HandleDeathBriefingPause(int localPlayerActorNr, int dmgDealerActorNr, PlayerKilledByType damageType)
		{
			HandleDeathBriefingPause();
		}

		private void HandleDeathBriefingPause()
		{
			deadInterval = MVGameControllerBase.LocalPlayer.RespawnDuration;
			MVGameControllerBase.LocalPlayer.RespawnTime = Time.time + deadInterval;
		}

		private void HandleResetUIPause()
		{
			deadInterval = MVGameControllerBase.LocalPlayer.RespawnDuration;
			MVGameControllerBase.LocalPlayer.RespawnTime = Time.time + deadInterval;
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
			if (toMode == AvatarRuntimeState.Playing)
			{
				mvAvatar.OnRespawn();
			}
			MVGameControllerBase.Game.GameEventManager.AvatarCommandsBuildMode.OnSetToEditMode -= OnEnterEditMode;
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
				RevivePlayer();
			}
		}

		private void RevivePlayer()
		{
			mvAvatar.avatarRespawnHandler.Respawn();
		}

		private void OnEnterEditMode()
		{
			MVGameControllerBase.PlayModeUI.InLobbyState = true;
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

	public class GhostMode : AvatarMode
	{
		private bool haveSetTransparency;

		public GhostMode(MVAvatarLocal mvAvatar)
			: base(mvAvatar, 4)
		{
			CreateInputController();
		}

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			mvAvatar.HealParticleSpawnTime = Time.time;
			mvAvatar.Shield.Value = 0f;
			mvAvatar.ResetAvatar();
			mvAvatar.SetToSpawnTransform();
			haveSetTransparency = false;
			mvAvatar.AvatarLocal.CameraController.SetCamera(CameraType.GhostCamera);
			MVGameControllerBase.MainCameraManager.CurrentCamera.Reset();
			MVGameControllerBase.Game.GameEventManager.AvatarCommandsBuildMode.OnSetToEditMode += OnEnterEditMode;
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
			mvAvatar.SetTransparency = 1f;
			MVGameControllerBase.Game.GameEventManager.AvatarCommandsBuildMode.OnSetToEditMode -= OnEnterEditMode;
			ResetCamera();
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
			if (!haveSetTransparency)
			{
				mvAvatar.SetTransparency = 0.5f;
				haveSetTransparency = true;
			}
		}

		private void ResetCamera()
		{
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
			mvAvatar.Visible = true;
			mvAvatar.AvatarLocal.CameraController.SetCamera(CameraType.ThirdPerson);
			MVGameControllerBase.MainCameraManager.CurrentCamera.Reset();
			MVGameControllerBase.MainCameraManager.CurrentCamera.transform.rotation = mvAvatar.transform.rotation;
		}

		private void SendNotification()
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)18, true);
			NotificationController.PushNotification(NotificationType.WaitCountDown, dictionary);
		}

		private void OnEnterEditMode()
		{
			MVGameControllerBase.PlayModeUI.InLobbyState = true;
		}

		private IAvatarInputController CreateInputController()
		{
			if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
			{
				return new AvatarInputController();
			}
			return new AvatarInputController();
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
			bool flag = MVGameControllerBase.GameMode != MVGameMode.Edit || (MVGameControllerBase.GameMode == MVGameMode.Edit && MVGameControllerBase.EditModeUI.IsInPlayInEditMode);
			base.Activate(fromMode);
			LayerUtil.SetLayerRecursively(mvAvatar.Body.Transform, "Player", "CamRotateTarget");
			if (flag)
			{
				MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.AvatarLobbyFocus;
			}
			MVGameControllerBase.PlayModeUI.InLobbyState = true;
			mvAvatar.Body.Transform.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
			mvAvatar.ResetAvatar();
			mvAvatar.SetToSpawnTransform();
			mvAvatar.AvatarLocal.CameraController.PushCamera(CameraType.LobbyState);
			LobbyStateCamera lobbyStateCamera = (LobbyStateCamera)MVGameControllerBase.MainCameraManager.CurrentCamera;
			lobbyStateCamera.SetRotation(mvAvatar.transform.rotation);
			mvAvatar.Body.Visible = true;
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
			mvAvatar.Body.Transform.localRotation = Quaternion.AngleAxis(0f, Vector3.up);
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
			LayerUtil.SetLayerRecursively(mvAvatar.Body.Transform, "CamRotateTarget", "Player");
			mvAvatar.AvatarLocal.CameraController.RemoveCamera(CameraType.LobbyState);
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
		}
	}

	protected class ReviveMode(MVAvatarLocal mvAvatar) : AvatarMode(mvAvatar, 2)
	{
		private class AvatarInputControllerDead : IMotorAPI
		{
			private Quaternion rot = Quaternion.identity;

			public Vector3 Direction
			{
				get
				{
					return Vector3.zero;
				}
				set
				{
				}
			}

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

		protected float reviveInterval = 10f;

		private bool reviveElapsed;

		private bool setDeadCamDelayed;

		private AvatarInputControllerDead inputController = new AvatarInputControllerDead();

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			reviveElapsed = false;
			deadTime = Time.time;
			reviveInterval = MVGameControllerBase.LocalPlayer.ReviveTimeout;
			mvAvatar.SetAnimation("Dead");
			mvAvatar.avatarEquipable.Unequip();
			if (mvAvatar.IsSeated)
			{
				mvAvatar.LeaveVehicle(leaveBecauseOfServer: false);
			}
			mvAvatar.triggerHandler.enabled = false;
			MVGameControllerBase.LocalPlayer.RespawnTime = Time.time + reviveInterval;
			MVGameControllerDesktop.LockCursorManager.CursorLock = false;
			MVGameControllerBase.PlayModeUI.InLobbyState = false;
			setDeadCamDelayed = false;
			if (mvAvatar.InGunMode || MVGameControllerBase.MainCameraManager.CurrentCamera.CameraType == CameraType.FirstPersonCamera)
			{
				mvAvatar.AvatarLocal.CameraController.SetCamera(CameraType.ThirdPerson);
				setDeadCamDelayed = true;
			}
			MVGameControllerBase.Game.GameEventManager.AvatarCommandsBuildMode.OnSetToEditMode += OnEnterEditMode;
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
			MVGameControllerBase.Game.GameEventManager.AvatarCommandsBuildMode.OnSetToEditMode -= OnEnterEditMode;
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
			inputController.Rotation = mvAvatar.avatarMotor.transform.rotation;
			if (mvAvatar.avatarMotor.enabled)
			{
				mvAvatar.avatarMotor.FixedUpdateFunction(inputController);
			}
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
			if (mvAvatar.avatarMotor.enabled)
			{
				mvAvatar.avatarMotor.UpdateFunction();
			}
			if (!reviveElapsed && Time.time - deadTime > reviveInterval)
			{
				NoButtonPressed();
			}
			if (setDeadCamDelayed && Time.time - deadTime > 0.5f)
			{
				mvAvatar.AvatarLocal.CameraController.SetCamera(CameraType.DeadCamera);
				setDeadCamDelayed = false;
			}
		}

		private void NoButtonPressed()
		{
			reviveElapsed = true;
		}

		private void OnEnterEditMode()
		{
			MVGameControllerBase.PlayModeUI.InLobbyState = true;
		}
	}

	protected class ReviveWaitMode : AvatarMode
	{
		public ReviveWaitMode(MVAvatarLocal mvAvatar)
			: base(mvAvatar, 4)
		{
		}

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			mvAvatar.ResetAvatar();
			mvAvatar.AvatarLocal.CameraController.SetCamera(CameraType.GhostCamera);
			mvAvatar.SetAnimation("Idle");
			mvAvatar.SetTransparency = 0.5f;
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
			mvAvatar.SetTransparency = 1f;
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
		}
	}

	public class TimeAttackFlagDebriefingMode : AvatarMode
	{
		private readonly IAvatarInputController avatarInputController;

		private bool isInDebriefing;

		private Transform flagTransform;

		private Quaternion currentDirectionRotation = Quaternion.identity;

		private float lastAngle;

		private float directionInterpolationStartTime;

		private const float walkAroundFlagAngle = 80f;

		private const float walkFromFlagAngle = 100f;

		private const float walkTowardsFlagAngle = 20f;

		private const float walkAroundCircleMinRadius = 1f;

		private const float walkAroundCircleMaxRadius = 1.3f;

		private const float maxFallBelow = 200f;

		private const float interpolationDuration = 0.4f;

		public TimeAttackFlagDebriefingMode(MVAvatarLocal mvAvatar)
			: base(mvAvatar, 1)
		{
			avatarInputController = CreateInputController();
		}

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			avatarInputController.Rotation = mvAvatar.transform.rotation;
			flagTransform = GetClosestTimeAttackFlag();
			mvAvatar.avatarEquipable.Unequip();
			isInDebriefing = MVGameControllerBase.FlagDebriefingControl.IsInFlagDebriefing;
			if (!isInDebriefing)
			{
				mvAvatar.AvatarLocal.CameraController.PushCamera(CameraType.LobbyState);
			}
			else
			{
				CullingApiWrapper.SetDistanceReferencePoint(flagTransform);
				OnEnterTimeAttackFlagDebriefing(MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.TimeAttackFlag, MVGameControllerBase.LocalPlayer.Team, MVGameControllerBase.LocalPlayer.ActorNr));
			}
			FlagDebriefingControl flagDebriefingControl = MVGameControllerBase.FlagDebriefingControl;
			flagDebriefingControl.OnFlagDebriefing = (Action<int>)Delegate.Combine(flagDebriefingControl.OnFlagDebriefing, new Action<int>(OnEnterTimeAttackFlagDebriefing));
			FlagDebriefingControl flagDebriefingControl2 = MVGameControllerBase.FlagDebriefingControl;
			flagDebriefingControl2.OnFlagDebriefingEnd = (Action)Delegate.Combine(flagDebriefingControl2.OnFlagDebriefingEnd, new Action(OnExitTimeAttackFlagDebriefing));
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
			Debug.Log("Time attack flag debriefing mode deactivated: " + toMode);
			Debug.Log("DeActivate " + Time.frameCount);
			mvAvatar.InteractableLocal.AddModifier(AvatarModifierPackageType.SpawnProtection);
			FlagDebriefingControl flagDebriefingControl = MVGameControllerBase.FlagDebriefingControl;
			flagDebriefingControl.OnFlagDebriefing = (Action<int>)Delegate.Remove(flagDebriefingControl.OnFlagDebriefing, new Action<int>(OnEnterTimeAttackFlagDebriefing));
			FlagDebriefingControl flagDebriefingControl2 = MVGameControllerBase.FlagDebriefingControl;
			flagDebriefingControl2.OnFlagDebriefingEnd = (Action)Delegate.Remove(flagDebriefingControl2.OnFlagDebriefingEnd, new Action(OnExitTimeAttackFlagDebriefing));
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
			if (mvAvatar.avatarMotor.enabled)
			{
				mvAvatar.avatarMotor.UpdateFunction();
			}
			if (!isInDebriefing && MVGameControllerBase.MainCameraManager.CurrentCamera.CameraType != CameraType.TimeAttackFlagCountdownCamera)
			{
				mvAvatar.AvatarLocal.CameraController.PushCamera(CameraType.TimeAttackFlagCountdownCamera);
				MVGameControllerBase.MainCameraManager.CurrentCamera.transform.rotation = mvAvatar.transform.rotation;
			}
			if (MVGameControllerBase.MainCameraManager.CurrentCamera.CameraType == CameraType.TimeAttackFlagDebriefingCamera)
			{
				float num = 36f;
				MVGameControllerBase.MainCameraManager.CurrentCamera.transform.Rotate(new Vector3(0f, num * Time.deltaTime, 0f));
			}
			if (!isInDebriefing)
			{
				avatarInputController.Rotation = mvAvatar.transform.rotation;
				avatarInputController.Direction = mvAvatar.transform.forward;
			}
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
			if (mvAvatar.gameObject.transform.position.y < MVGameControllerBase.WOCM.WorldBounds.min.y - 200f)
			{
				DieByFalling();
			}
			if (!mvAvatar.avatarMotor.enabled)
			{
				mvAvatar.avatarMotor.UpdateVelocity();
				return;
			}
			mvAvatar.InteractableLocal.AddModifier(AvatarModifierPackageType.TimeAttackFlagDebriefSlow);
			Vector3 avatarMoveDirection = GetAvatarMoveDirection();
			avatarInputController.HandleInput(avatarMoveDirection, movementMap.Jump, didShoot: false, Vector3.zero, mvAvatar.InGunMode, mvAvatar.ForceRotateAvatarToFiringDirection);
			mvAvatar.avatarMotor.FixedUpdateFunction(avatarInputController);
			if (isInDebriefing && !mvAvatar.Body.Animation.IsPlaying("Jump"))
			{
				mvAvatar.SetAnimation("Idle");
				mvAvatar.SetAnimation("Jump");
				mvAvatar.Body.Animation.Play("Jump");
			}
		}

		private IAvatarInputController CreateInputController()
		{
			if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
			{
				return new AvatarInputController();
			}
			return new AvatarInputController();
		}

		private Transform GetClosestTimeAttackFlag()
		{
			Transform result = null;
			float num = float.MaxValue;
			List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.TimeAttackFlag);
			if (worldObjectsByType.Count == 0)
			{
				throw new Exception("Entered TimeAttackFlagDebriefingMode without there being a timeAttackFlag in the game!");
			}
			Vector3 position = mvAvatar.Position;
			for (int i = 0; i < worldObjectsByType.Count; i++)
			{
				float sqrMagnitude = (position - worldObjectsByType[i].Position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = worldObjectsByType[i].Transform;
				}
			}
			return result;
		}

		private Vector3 GetAvatarMoveDirection()
		{
			if (!isInDebriefing)
			{
				return Vector3.zero;
			}
			Vector3 direction = flagTransform.position - mvAvatar.Position;
			direction.y = 0f;
			float sqrMagnitude = direction.sqrMagnitude;
			if (sqrMagnitude < 1f)
			{
				return RotateDirection(direction, 100f);
			}
			if (sqrMagnitude > 1.6899998f)
			{
				return RotateDirection(direction, 20f);
			}
			return RotateDirection(direction, 80f);
		}

		private Vector3 RotateDirection(Vector3 direction, float angle)
		{
			if (lastAngle != angle)
			{
				directionInterpolationStartTime = Time.time;
			}
			lastAngle = angle;
			Quaternion identity = Quaternion.identity;
			identity.eulerAngles = new Vector3(0f, angle, 0f);
			identity = Quaternion.Inverse(MVGameControllerBase.MainCameraManager.CurrentCamera.transform.rotation) * identity;
			if (!(currentDirectionRotation == Quaternion.identity))
			{
				identity = (currentDirectionRotation = Quaternion.Lerp(currentDirectionRotation, identity, (Time.time - directionInterpolationStartTime) / 0.4f));
			}
			else
			{
				currentDirectionRotation = identity;
			}
			direction = identity * direction;
			return direction;
		}

		private void OnEnterTimeAttackFlagDebriefing(int score)
		{
			if (mvAvatar.IsInVehicle)
			{
				mvAvatar.LeaveVehicle(leaveBecauseOfServer: false);
			}
			mvAvatar.pickupOwner.HandleFire(inputFire: false, mvAvatar.IsFiring);
			mvAvatar.AvatarLocal.CameraController.PushCamera(CameraType.TimeAttackFlagDebriefingCamera);
			isInDebriefing = true;
			mvAvatar.Avatar.AvatarFader.SetTransparency(1f);
		}

		private void OnExitTimeAttackFlagDebriefing()
		{
			isInDebriefing = false;
			avatarInputController.Rotation = mvAvatar.transform.rotation;
			if (MVGameControllerBase.MainCameraManager.CurrentCamera.CameraType == CameraType.TimeAttackFlagDebriefingCamera)
			{
				mvAvatar.AvatarLocal.CameraController.PushCamera(CameraType.LobbyState);
			}
			else
			{
				mvAvatar.AvatarLocal.CameraController.PushCamera(CameraType.ThirdPerson);
			}
			CullingApiWrapper.SetDistanceReferencePoint(MVGameControllerBase.MainCameraManager.MainCamera.transform);
			mvAvatar.ResetAvatar();
			mvAvatar.SetToSpawnTransform();
			MVGameControllerBase.MainCameraManager.CurrentCamera.Reset();
			mvAvatar.Body.Animation.Play("Idle");
		}

		private void DieByFalling()
		{
			mvAvatar.Health.Value = 0f;
			MVGameControllerBase.OperationRequests.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameControllerBase.Game.LocalPlayer.ActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr, PlayerKilledByType.FallOffWorld));
		}
	}

	protected class WaitMode : AvatarMode
	{
		private readonly IAvatarInputController avatarInputController;

		public WaitMode(MVAvatarLocal mvAvatar)
			: base(mvAvatar, 4)
		{
			avatarInputController = CreateInputController();
		}

		public override void Activate(AvatarRuntimeState fromMode)
		{
			base.Activate(fromMode);
			mvAvatar.ResetAvatar();
			mvAvatar.SetToSpawnTransform();
			ResetCamera();
			SendNotification();
			if (fromMode == AvatarRuntimeState.Ghost)
			{
				mvAvatar.SetMode(AvatarRuntimeState.Ghost);
			}
		}

		public override void DeActivate(AvatarRuntimeState toMode)
		{
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
			if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState != MVGameStateType.RoundEnded)
			{
				mvAvatar.SetMode(AvatarRuntimeState.Playing);
			}
		}

		public override void FixedUpdate(IInputToPlayerMovement movementMap)
		{
			mvAvatar.avatarMotor.FixedUpdateFunction(avatarInputController);
		}

		private void ResetCamera()
		{
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
			mvAvatar.Visible = true;
			mvAvatar.AvatarLocal.CameraController.SetCamera(CameraType.ThirdPerson);
			MVGameControllerBase.MainCameraManager.CurrentCamera.Reset();
			MVGameControllerBase.MainCameraManager.CurrentCamera.transform.rotation = mvAvatar.transform.rotation;
		}

		private void SendNotification()
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)18, true);
			NotificationController.PushNotification(NotificationType.WaitCountDown, dictionary);
		}

		private IAvatarInputController CreateInputController()
		{
			if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
			{
				return new AvatarInputController();
			}
			return new AvatarInputController();
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
			Transform transform = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().Transform;
			Bounds? axisAlignedBoundsRecursively = SharedCubeFunctions.GetAxisAlignedBoundsRecursively(transform);
			Bounds bounds = (axisAlignedBoundsRecursively.HasValue ? axisAlignedBoundsRecursively.Value : default(Bounds));
			MVGameControllerBase.WOCM.UpdateWorldBounds(bounds);
			mvAvatar.HealParticleSpawnTime = Time.time;
			mvAvatar.Shield.Value = 0f;
			mvAvatar.triggerHandler.enabled = true;
			mvAvatar.ResetAvatar();
			mvAvatar.Collider.enabled = true;
			avatarInputController.Rotation = mvAvatar.transform.rotation;
			mvAvatar.LimbManager.SetLimbRotatorActivity(shouldBeActive: true);
			if (mvAvatar.Body != null)
			{
				mvAvatar.Body.Visible = true;
			}
			if (MVClientSettings.ReviveEnabled)
			{
				MVGameControllerBase.SpawnRoleDataMediatorLocal.ReviveState.Value.ResetSafePostions();
			}
			if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
			{
				mvAvatar.SetMode(AvatarRuntimeState.Wait);
				return;
			}
			if (fromMode != AvatarRuntimeState.Wait)
			{
				MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
				mvAvatar.Visible = true;
				mvAvatar.AvatarLocal.CameraController.SetCamera(CameraType.ThirdPerson);
				MVGameControllerBase.MainCameraManager.CurrentCamera.Reset();
				MVGameControllerBase.MainCameraManager.CurrentCamera.transform.rotation = mvAvatar.transform.rotation;
			}
			MVGameControllerDesktop.LockCursorManager.CursorLock = true;
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
			if (mvAvatar.InGunMode && MVGameControllerBase.MainCameraManager.CurrentCamera.CameraType == CameraType.ThirdPerson)
			{
				mvAvatar.AvatarLocal.CameraController.SetCamera(CameraType.FirstPersonCamera);
			}
			else if (!mvAvatar.InGunMode && MVGameControllerBase.MainCameraManager.CurrentCamera.CameraType == CameraType.FirstPersonCamera)
			{
				mvAvatar.AvatarLocal.CameraController.SetCamera(CameraType.ThirdPerson);
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
			Vector3 moveDirection = movementMap.Direction;
			if (MVGameControllerBase.FlagDebriefingControl.IsInFlagDebriefing)
			{
				avatarInputController.Rotation = mvAvatar.Transform.rotation;
				moveDirection = Vector3.zero;
			}
			HandleWaterplane();
			avatarInputController.HandleInput(moveDirection, movementMap.Jump, isFiring, mvAvatar.avatarMotor.Velocity, mvAvatar.InGunMode, mvAvatar.ForceRotateAvatarToFiringDirection);
			mvAvatar.avatarMotor.FixedUpdateFunction(avatarInputController);
			if (!mvAvatar.IsInMode(SpawnRoleModeType.Dead))
			{
				SetAnimationState(avatarInputController.Direction);
			}
		}

		private void HandleFocus()
		{
			if (MVGameControllerBase.MainCameraManager.CurrentCamera.CameraType == CameraType.LobbyState && !MVGameControllerBase.PlayModeUI.InLobbyState)
			{
				mvAvatar.AvatarLocal.CameraController.RemoveCamera(CameraType.LobbyState);
			}
			else if (MVGameControllerBase.MainCameraManager.CurrentCamera.CameraType != CameraType.LobbyState && MVGameControllerBase.PlayModeUI.InLobbyState)
			{
				mvAvatar.AvatarLocal.CameraController.PushCamera(CameraType.LobbyState);
				LobbyStateCamera lobbyStateCamera = (LobbyStateCamera)MVGameControllerBase.MainCameraManager.CurrentCamera;
				lobbyStateCamera.SetRotation(MVGameControllerBase.MainCameraManager.transform.rotation);
			}
		}

		private void HandleStuck()
		{
			mvAvatar.Health.Value = 0f;
			mvAvatar.InteractableLocal.DieFromBeingStuck();
		}

		private void DieByFalling()
		{
			mvAvatar.Health.Value = 0f;
			mvAvatar.InteractableLocal.DieFromFalling();
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
			bool flag = mvAvatar.avatarMotor.IsAirJumping();
			if (IsSwimming)
			{
				mvAvatar.SetAnimation("Swim");
			}
			else if (isJumping && !flag)
			{
				mvAvatar.SetAnimation("Jump");
			}
			else if (isJumping && flag)
			{
				mvAvatar.SetAnimation("Idle");
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
			return new AvatarInputController();
		}
	}

	protected SpawnRoleDataReceiver spawnRoleDataReceiver;

	private string currAnim = string.Empty;

	private const float exitVehicleMomentumModifier = 7f;

	private AvatarMotor avatarMotor;

	private AvatarInteractable interactableLocal;

	private UseInteractorHandler useInteractorHandler;

	private MVTriggerHandler triggerHandler;

	private AvatarEquipable avatarEquipable;

	private AvatarPickupOwner pickupOwner;

	private PickupGUI pickupGUI;

	private MVRigidBody vehicleRigidBody;

	private AvatarLocalModes avatarLocalModes;

	private AvatarRespawnHandler avatarRespawnHandler = new AvatarRespawnHandler();

	private Action<int, int, PlayerKilledByType> OnKilled;

	private Action OnSuicide;

	private float previousHealth;

	private float previousShield;

	private float boostedHealthMultiplier = 1f;

	public Action<float, MVPlayer, PlayerKilledByType> OnDamageTaken;

	private bool suspended;

	private int spawnWorldObjectId = -1;

	private Vector3 LookAtPos => transform.position + Vector3.up;

	public AvatarInteractable InteractableLocal => interactableLocal;

	public override Vector3 VelocityAbsolute => (!IsInVehicle) ? RigidBody.Velocity : vehicleRigidBody.Velocity;

	public AvatarPickupOwner PickupOwner => pickupOwner;

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

	public MVRigidBody RigidBody => avatarMotor;

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

	public KogamaSettingWrapperBase Settings => KogamaSettingTools.CreateFromValues(Data, AttributePrototypeSettingsManager.GetRoot(AttributeSettingWoType.Avatar), AttributeSettingsFactory.KogamaSettingValueFactoryAttributeSettings);

	private AvatarLocal AvatarLocal => (AvatarLocal)Avatar;

	public int SpawnId
	{
		private get
		{
			return spawnWorldObjectId;
		}
		set
		{
			spawnWorldObjectId = value;
		}
	}

	public bool IsSpawnRoleActive()
	{
		return spawnRoleDataReceiver != null && spawnRoleDataReceiver.IsActive;
	}

	public float GetColliderRadius()
	{
		return avatarMotor.GetSizeState.ControllerRadius;
	}

	public override void Initialize()
	{
		skillDataManager = new WorldObjectSkillDataManager();
		skillDataManager.Initialize(Settings);
		base.Initialize();
		avatarMotor = gameObject.AddComponent<AvatarMotor>();
		triggerHandler = gameObject.AddComponent<MVTriggerHandler>();
		AvatarInteractable avatarInteractable = gameObject.AddComponent<AvatarInteractable>();
		avatarInteractable.Init(Modifiers, Health, MaxHealth, Shield, skillDataManager);
		interactableLocal = avatarInteractable;
		AvatarInteractable avatarInteractable2 = interactableLocal;
		avatarInteractable2.OnDamageTaken = (Action<float, MVPlayer, PlayerKilledByType>)Delegate.Combine(avatarInteractable2.OnDamageTaken, new Action<float, MVPlayer, PlayerKilledByType>(RelayDamageEvent));
		AvatarInteractable avatarInteractable3 = interactableLocal;
		avatarInteractable3.OnNewSafePosition = (Action<Vector3>)Delegate.Combine(avatarInteractable3.OnNewSafePosition, new Action<Vector3>(RelayNewSafePosition));
		avatarEquipable = gameObject.AddComponent<AvatarEquipable>();
		avatarEquipable.Init(interactableLocal, CurrentItem, skillDataManager);
		avatarMotor.Init(avatarInteractable, CharacterControllerCenterOffset, this, skillDataManager);
		pickupOwner = base.avatarPickupOwner;
		if (MVGameControllerBase.GameMode != MVGameMode.CharacterEditor)
		{
			pickupGUI = gameObject.AddComponent<PickupGUI>();
			pickupGUI.Initialize(Id, pickupOwner);
		}
		AvatarPickupOwner avatarPickupOwner = pickupOwner;
		avatarPickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Combine(avatarPickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(OnEquipItem));
		AvatarPickupOwner avatarPickupOwner2 = pickupOwner;
		avatarPickupOwner2.OnHolsteredChanged = (Action<bool>)Delegate.Combine(avatarPickupOwner2.OnHolsteredChanged, new Action<bool>(OnHolsteredChanged));
		avatarLocalModes = new AvatarLocalModes(this);
		InitializeHealth();
		InitializeShield();
		UpdateMaxHealth();
		if (MVGameControllerBase.GameMode != MVGameMode.CharacterEditor)
		{
			useInteractorHandler = gameObject.AddComponent<UseInteractorHandler>();
			useInteractorHandler.Init(Id, Collider);
		}
		avatarMotor.GetSizeState.EquipSlapGunEvent += avatarEquipable.EquipSlapGun;
		avatarMotor.GetSizeState.CameraScaleEvent += OnCameraScale;
		avatarMotor.GetSizeState.UnEquipSlapGunEvent += OnUnequip;
		avatarInteractable.ModifierPackages.OnUnequipItemEvent += OnUnequip;
		avatarInteractable.ModifierPackages.OnDisableVehiclesEvent += OnDisableVehicles;
		AvatarShieldDecay avatarShieldDecay = gameObject.AddComponent<AvatarShieldDecay>();
		avatarShieldDecay.Init(Shield);
		avatarInteractable.OnShieldReplenished = (Action)Delegate.Combine(avatarInteractable.OnShieldReplenished, new Action(avatarShieldDecay.ResetDecayTimer));
		limbManager = new AvatarLimbManagerLocal();
		limbManager.Initialize(this, Body, avatar.EnabledChangeHandler, LimbRotationRuntimeData);
		avatarRespawnHandler.Initialize(this);
		PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
		RotationChanged = (UnityAction<MVWorldObjectClient, RotationChangedEventArgs>)Delegate.Combine(RotationChanged, new UnityAction<MVWorldObjectClient, RotationChangedEventArgs>(OnRotationChanged));
		ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Combine(ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(OnScaleChanged));
		gameObject.SetActive(value: false);
		MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(OwnerActorNr).NotifyAvatarCreated(Id);
	}

	public void Activate(int idFrom, SpawnRoleDataReceiver spawnRoleDataReceiver, Vector3 position, Quaternion rotation)
	{
		suspended = false;
		SetNetworkObject(local: true);
		SubscribeToExternalEvents();
		this.spawnRoleDataReceiver = spawnRoleDataReceiver;
		if (MVGameControllerBase.LocalPlayer.IsReady && MVGameControllerDesktop.LockCursorManager.CursorLock)
		{
			MVGameControllerBase.PlayModeUI.InLobbyState = false;
		}
		SetToSpawnTransform();
		MVWorldObject worldObject = MVGameControllerBase.WOCM.GetWorldObject(idFrom);
		if (worldObject is MVBuildModeAvatar)
		{
			Position = position;
			Rotation = rotation;
			SetTransform(Position, Rotation);
		}
		SetupSpawnroleReceiver(spawnRoleDataReceiver);
		((AvatarLocal)avatar).CameraController.ActivateCameraController();
		if (Id == idFrom || MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			avatarLocalModes.SetMode(AvatarRuntimeState.Hidden);
		}
		else
		{
			avatarLocalModes.SetMode(AvatarRuntimeState.Playing);
		}
		gameObject.SetActive(value: true);
		MVGameControllerBase.Game.PlayerController.SetAvatarLocalObject(this);
		if (idFrom > 0)
		{
			MVGameControllerBase.MainCameraManager.CurrentCamera.Reset();
			MVGameControllerBase.MainCameraManager.CurrentCamera.transform.rotation = worldObject.Rotation;
		}
		CullingApiWrapper.SetDistanceReferencePoint(transform);
		MVGameControllerBase.Game.LocalPlayer.BoostController.SubscribeToBoostChanged(BoostType.ExtraHealthFloatMultiplier, OnHealthBoostedChanged);
		OnHealthBoostedChanged();
	}

	private void SubscribeToExternalEvents()
	{
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnKillSelf += KillSelf;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnSetRespawnWhenPossible += OnSetRespawnWhenPossible;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnEnterPlaymode += AvatarCommandsPlayModeOnOnSpawn;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnSetToSpawnPoint += AvatarCommandsOnSetToSpawnPoint;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnMoveBodyToSafeSpot += AvatarCommandsOnMoveBodyToSafeSpot;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnSpawnAtSafeSpot += AvatarCommandsOnSpawnAtSafeSpot;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnReadyScreenShot += AvatarCommandsPlayModeOnOnReadyScreenShot;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnWinningConditionIntermediateDebriefing += AvatarCommandsPlayModeOnOnWinningConditionIntermediateDebriefing;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnRemoveFromGame += AvatarCommandsPlayModeOnOnRemoveFromGame;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnSpawnAsGhost += OnSetSpawnAsGhost;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnSetToDeadMode += OnSetToDeadMode;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnRespawn += AvatarCommandsOnRespawn;
		MVGameControllerBase.GameEventManager.GameState.GameStateType.OnChange += GameStateTypeOnOnChange;
		MVGameControllerBase.GameEventManager.OnFirstTimeEvent += GameEventManagerOnOnFirstTimeEvent;
		MVGameControllerBase.GameEventManager.OnXPRewarded += GameEventManagerOnOnXpRewarded;
	}

	private void UnsubscribeFromExternalEvents()
	{
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnKillSelf -= KillSelf;
		MVGameControllerBase.GameEventManager.OnFirstTimeEvent -= GameEventManagerOnOnFirstTimeEvent;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnSetRespawnWhenPossible -= OnSetRespawnWhenPossible;
		MVGameControllerBase.GameEventManager.OnXPRewarded -= GameEventManagerOnOnXpRewarded;
		MVGameControllerBase.GameEventManager.GameState.GameStateType.OnChange -= GameStateTypeOnOnChange;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnReadyScreenShot -= AvatarCommandsPlayModeOnOnReadyScreenShot;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnMoveBodyToSafeSpot -= AvatarCommandsOnMoveBodyToSafeSpot;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnSpawnAtSafeSpot -= AvatarCommandsOnSpawnAtSafeSpot;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnRespawn -= AvatarCommandsOnRespawn;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnEnterPlaymode -= AvatarCommandsPlayModeOnOnSpawn;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnSetToSpawnPoint -= AvatarCommandsOnSetToSpawnPoint;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnWinningConditionIntermediateDebriefing -= AvatarCommandsPlayModeOnOnWinningConditionIntermediateDebriefing;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnRemoveFromGame -= AvatarCommandsPlayModeOnOnRemoveFromGame;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnSpawnAsGhost -= OnSetSpawnAsGhost;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnSetToDeadMode -= OnSetToDeadMode;
		MVLocalPlayer localPlayer = MVGameControllerBase.LocalPlayer;
		localPlayer.OnCheckpointReached = (UnityAction)Delegate.Remove(localPlayer.OnCheckpointReached, new UnityAction(OnCheckpointReachedResetRevive));
		MVGameControllerBase.Game.LocalPlayer.BoostController.UnSubscribeToBoostChanged(BoostType.ExtraHealthFloatMultiplier, OnHealthBoostedChanged);
	}

	public void DeActivate(int idTo, SpawnRoleDataReceiver spawnRoleDataReceiver)
	{
		avatarLocalModes.SetToStartMode();
		avatarEquipable.Unequip();
		interactableLocal.ClearModifiers();
		UnsubscribeFromExternalEvents();
		gameObject.SetActive(value: false);
		this.spawnRoleDataReceiver = null;
		MVGameControllerBase.Game.PlayerController.RemoveAvatarLocalObject();
		MVNetworkObject networkObject = MVGameControllerBase.Game.TransformNetworkManager.GetNetworkObject(Id);
		if (networkObject != null)
		{
			Debug.LogWarning("Removing network object again as this is added multiple times when leaving play mode from a vehicle");
			MVGameControllerBase.Game.TransformNetworkManager.RemoveNetworkObject(id);
		}
	}

	private void SetupSpawnroleReceiver(SpawnRoleDataReceiver spawnRoleDataReceiver)
	{
		float num = ((!skillDataManager.HasSkill("Size")) ? 1f : skillDataManager.GetSkillFloatValue("Size"));
		spawnRoleDataReceiver.size.Value = num;
		Scale = new Vector3(num, num, num);
		Size.Value = num;
		spawnRoleDataReceiver.reviveState.Value = new ReviveState();
		spawnRoleDataReceiver.lastRespawnType.Value = LastRespawnType.None;
		spawnRoleDataReceiver.position.Value = Position;
		spawnRoleDataReceiver.rotation.Value = Rotation;
		spawnRoleDataReceiver.defaultScale.Value = Scale;
		spawnRoleDataReceiver.scale.Value = Scale;
		spawnRoleDataReceiver.woId.Value = Id;
		spawnRoleDataReceiver.maxHealth.Value = MaxHealth.Value;
		spawnRoleDataReceiver.tierRequirement.Value = GetTierRequirement();
		MVLocalPlayer localPlayer = MVGameControllerBase.LocalPlayer;
		localPlayer.OnCheckpointReached = (UnityAction)Delegate.Combine(localPlayer.OnCheckpointReached, new UnityAction(OnCheckpointReachedResetRevive));
	}

	public void Suspend()
	{
		if (suspended)
		{
			Debug.LogError("Already suspended");
			return;
		}
		suspended = true;
		if (IsInVehicle)
		{
			LeaveVehicle(leaveBecauseOfServer: false);
		}
		MVGameControllerBase.Game.TransformNetworkManager.RemoveNetworkObject(Id);
		MVGameControllerBase.Game.RuntimeVariableNetworkManager.SendRuntimeData(this, immediateSend: true);
		MVGameControllerBase.Game.RuntimeVariableNetworkManager.RemoveRuntimeDataVariables(Id);
	}

	public void UnSuspend()
	{
		suspended = false;
		SetNetworkObject(local: true);
	}

	public void SetMode(AvatarRuntimeState localMode)
	{
		avatarLocalModes.SetMode(localMode);
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
		AvatarLocal.CameraController.SetCamera(CameraType.ThirdPerson);
		RigidBody.Reset();
		RigidBody.enabled = true;
		triggerHandler.enabled = true;
		RigidBody.AddImpulse(impulse);
		OnLeaveVehicle();
		spawnRoleDataReceiver.isInVehicle.Value = false;
		pickupGUI.AvatarLeftVehicle();
		MVGameControllerBase.Game.TransformNetworkManager.AddReporter(id, new MVNetworkReporter(this));
	}

	public override void BeforeVehicleEntered()
	{
		triggerHandler.Reset();
	}

	public override void OnEnterVehicle()
	{
		base.OnEnterVehicle();
		if (Group.GameObject.GetComponent<MVRigidBody>() != null)
		{
			RigidBody.enabled = false;
			triggerHandler.enabled = false;
		}
		vehicleRigidBody = MVWorldObjectClientManager.GetEnabledMonoBehaviourHighestInHierarchy<MVRigidBody>(gameObject);
		spawnRoleDataReceiver.isInVehicle.Value = true;
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

	public Dictionary<object, object> GetCurrentItemState()
	{
		return (Dictionary<object, object>)CurrentItem.Value;
	}

	public void SetCurrentItemState(Dictionary<object, object> aNewState)
	{
		CurrentItem.Value = aNewState;
	}

	public void VisualizeBulletImpact(VoxelHit voxelHit, Ray lineOfFire, int shooterActorNumber, float damage = 100f)
	{
		MVPlayer player = null;
		if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(shooterActorNumber, out player) && !player.IsOnSameTeam(this) && !IsInMode(SpawnRoleModeType.Dead) && !avatar.HasModifierEffect(AvatarModifierEffect.Invulnerable))
		{
			avatar.VisualizeBulletImpact(voxelHit, lineOfFire, shooterActorNumber, damage);
		}
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

	protected override void OnSeatedChanged(bool isSeated)
	{
		spawnRoleDataReceiver.isSeated.Value = isSeated;
	}

	protected override void AvatarStateChangedHandler(object a)
	{
		base.AvatarStateChangedHandler(a);
		Debug.Log("avatar: " + GameObject.name + " - a: " + (SpawnRoleModeType)a/*cast due to constrained. prefix*/);
		spawnRoleDataReceiver.spawnRoleMode.Value = (SpawnRoleModeType)a;
	}

	protected override void AttachBody(MVBody newBody)
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

	private void AvatarCommandsPlayModeOnOnRemoveFromGame()
	{
		SetMode(AvatarRuntimeState.Hidden);
	}

	private void AvatarCommandsPlayModeOnOnWinningConditionIntermediateDebriefing(WinningConditionType winningConditionType)
	{
		if (winningConditionType != WinningConditionType.TimeAttackFlag)
		{
			throw new Exception("Not implemented");
		}
		SetMode(AvatarRuntimeState.TimeAttackFlagDebriefing);
	}

	private void AvatarCommandsOnSetToSpawnPoint()
	{
		SetToSpawnTransform();
	}

	private void AvatarCommandsPlayModeOnOnSpawn()
	{
		SetMode(AvatarRuntimeState.Playing);
	}

	private void AvatarCommandsOnRespawn()
	{
		avatarRespawnHandler.Respawn();
	}

	private void AvatarCommandsOnSetToSpawnMode()
	{
		SetToSpawnTransform();
	}

	private void AvatarCommandsPlayModeOnOnReadyScreenShot()
	{
		avatar.AvatarFader.SetTransparency(1f);
	}

	private void GameStateTypeOnOnChange(MVGameStateType gameStateType)
	{
		if (gameStateType == MVGameStateType.RoundEnded && avatarLocalModes.CurrentState != AvatarRuntimeState.Hidden)
		{
			SetMode(AvatarRuntimeState.Wait);
		}
	}

	private void GameEventManagerOnOnXpRewarded(int obj)
	{
		AvatarLocal.OnXpProgressing(obj);
	}

	private void OnSetRespawnWhenPossible()
	{
		avatarRespawnHandler.ShouldRespawnAsGhost = false;
	}

	private void OnSetSpawnAsGhost()
	{
		avatarRespawnHandler.ShouldRespawnAsGhost = true;
		avatarRespawnHandler.Respawn();
	}

	private void OnSetToDeadMode()
	{
		SetMode(AvatarRuntimeState.Dead);
	}

	private void GameEventManagerOnOnFirstTimeEvent(FirstTimeEvent firstTimeEvent)
	{
		if (firstTimeEvent == FirstTimeEvent.PM_LobbyState)
		{
			FirstTimeAvatarJumpAnimator firstTimeAvatarJumpAnimator = gameObject.AddComponent<FirstTimeAvatarJumpAnimator>();
			firstTimeAvatarJumpAnimator.Initialize(this);
		}
	}

	private void OnHolsteredChanged(bool obj)
	{
		HandleBlinkerVisibility();
		spawnRoleDataReceiver.pickupItemIsInHand.Value = pickupOwner.PickupItemIsInHand;
	}

	private void OnUnequip(object sender, EventArgs e)
	{
		AvatarEquipable avatarEquipable = gameObject.GetComponent<MVEquipable>() as AvatarEquipable;
		if (!(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Scale, 1f) > 1f) || !avatarEquipable.GetIsEquipped(AvatarItemType.SlapGun))
		{
			avatarEquipable.Unequip();
		}
	}

	private void KillSelf()
	{
		if (IsInMode(SpawnRoleModeType.Playing) && !MVGameControllerBase.FlagDebriefingControl.IsInFlagDebriefing)
		{
			Suicide();
		}
	}

	private void SetToSpawnTransform()
	{
		MVGameControllerBase.MainCameraManager.CancelTransitionCam();
		Transform spawnTransform = GetSpawnTransform();
		SetTransform(spawnTransform.position, spawnTransform.rotation);
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
		Health.Value = MaxHealth.Value;
		if (IsSeated)
		{
			LeaveVehicle(leaveBecauseOfServer: false);
		}
		avatarEquipable.Unequip();
		interactableLocal.ClearModifiers();
		avatarMotor.Reset();
	}

	private int GetBoostedHealth(int unBoostedMaxHealth)
	{
		return Mathf.FloorToInt((float)unBoostedMaxHealth * boostedHealthMultiplier);
	}

	private void OnHealthBoostedChanged()
	{
		boostedHealthMultiplier = 1f;
		if (MVGameControllerBase.Game.LocalPlayer.BoostController.TryGetActiveBoost(BoostType.ExtraHealthFloatMultiplier, out var boost))
		{
			boostedHealthMultiplier = 1f + (float)(int)boost.Value / 100f;
			UpdateMaxHealth();
			Health.Value = MaxHealth.Value;
		}
		else
		{
			float num = (float)MaxHealth.Value / Health.Value;
			UpdateMaxHealth();
			Health.Value = (float)MaxHealth.Value / num;
		}
	}

	private void UpdateMaxHealth()
	{
		int unBoostedMaxHealth = ((!skillDataManager.HasSkill("MaxHealth")) ? 100 : skillDataManager.GetSkillIntValue("MaxHealth"));
		int boostedHealth = GetBoostedHealth(unBoostedMaxHealth);
		MaxHealth.Value = boostedHealth;
		if (spawnRoleDataReceiver != null)
		{
			spawnRoleDataReceiver.maxHealth.Value = MaxHealth.Value;
		}
	}

	private void Die()
	{
		Debug.Log("Die");
		if (IsInMode(SpawnRoleModeType.Playing))
		{
			Debug.Log("Goto dead state");
			Shield.Value = 0f;
			bool flag = MVClientSettings.ReviveEnabled && spawnRoleDataReceiver.reviveState.Value.CanSafelySpawn && avatarMotor.GetSizeState.GetIsValidScaledPosition(spawnRoleDataReceiver.reviveState.Value.SafeGroundedData.Position, 1f);
			if (!flag)
			{
				spawnRoleDataReceiver.reviveState.Value.ResetSafePostions();
			}
			if (flag)
			{
				avatarLocalModes.SetMode(AvatarRuntimeState.Revive);
			}
			else
			{
				avatarLocalModes.SetMode(AvatarRuntimeState.Dead);
			}
		}
	}

	private void OnRespawn()
	{
		interactableLocal.AddModifier(AvatarModifierPackageType.SpawnProtection);
		SetToSpawnTransform();
	}

	private void SetTransform(Vector3 position, Quaternion rotation)
	{
		WorldObjectClient.WorldPosition = position;
		WorldObjectClient.WorldRotation = rotation;
		GameObject.transform.position = position;
		GameObject.transform.rotation = rotation;
		avatarMotor.Reset();
	}

	private void InitializeShield()
	{
		previousShield = Shield.Value;
		MVRuntimeDataVariableClampedFloat mVRuntimeDataVariableClampedFloat = Shield;
		mVRuntimeDataVariableClampedFloat.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(mVRuntimeDataVariableClampedFloat.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object shield) =>
		{
			TrySpawningHealParticles(previousShield, Shield.Value);
			previousShield = Shield.Value;
			spawnRoleDataReceiver.shield.Value = (float)shield;
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
		MainCameraManager.GetSettings(MVGameControllerBase.Game.GameType).ScaleCameraValues(args.scale);
	}

	private void InitializeHealth()
	{
		previousHealth = Health.Value;
		MVRuntimeDataVariable<float> health = Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			if ((float)obj <= 0f)
			{
				Die();
			}
			else
			{
				TrySpawningHealParticles(previousHealth, Health.Value);
			}
			previousHealth = Health.Value;
			spawnRoleDataReceiver.health.Value = (float)obj;
		}));
	}

	private void Suicide()
	{
		if (IsInMode(SpawnRoleModeType.Dead))
		{
			return;
		}
		if (interactableLocal.LastDamageSource == null || interactableLocal.LastDamageSource.Outdated)
		{
			Die();
			if (OnSuicide != null)
			{
				OnSuicide();
				spawnRoleDataReceiver.NotifySuicide();
			}
		}
		else
		{
			interactableLocal.DieFromRespawn(interactableLocal.LastDamageSource.shooter, interactableLocal.LastDamageSource.damageType);
		}
	}

	private Vector3 CalculateVehicleExitMomentum(Vector3 velocity)
	{
		velocity /= Time.deltaTime;
		velocity /= 2f;
		velocity.y += velocity.magnitude / 7f;
		return velocity;
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
				OnKilled(actorNr, num, damageType);
				spawnRoleDataReceiver.NotifyKilled(actorNr, num, damageType);
			}
		}
	}

	private void RelayNewSafePosition(Vector3 lastSafePosition)
	{
		Transform transform = MVGameControllerBase.MainCameraManager.CurrentCamera.transform;
		SafeSpotData safeGroundedData = new SafeSpotData(lastSafePosition, spawnRoleDataReceiver.rotation.Value, transform.position, transform.rotation);
		spawnRoleDataReceiver.reviveState.Value.SafeGroundedData = safeGroundedData;
	}

	private void AvatarCommandsOnMoveBodyToSafeSpot(int safeSpotIndex)
	{
		Debug.Log("Move to safe spot");
		spawnRoleDataReceiver.lastRespawnType.Value = LastRespawnType.Revive;
		spawnRoleDataReceiver.reviveState.Value.SetSafeGroundedDataIndex(safeSpotIndex);
		SafeSpotData safeGroundedDataAtSelectedIndex = spawnRoleDataReceiver.reviveState.Value.GetSafeGroundedDataAtSelectedIndex();
		Vector3 eulerAngles = safeGroundedDataAtSelectedIndex.Rotation.eulerAngles;
		SetTransform(rotation: Quaternion.Euler(new Vector3(y: safeGroundedDataAtSelectedIndex.CameraRotation.eulerAngles.y, x: eulerAngles.x, z: eulerAngles.z)), position: safeGroundedDataAtSelectedIndex.Position);
		MVGameControllerBase.MainCameraManager.CurrentCamera.transform.rotation = safeGroundedDataAtSelectedIndex.CameraRotation;
		SetMode(AvatarRuntimeState.ReviveWait);
	}

	private void AvatarCommandsOnSpawnAtSafeSpot(int safeSpotIndex)
	{
		Debug.Log("SetMode to playing from SafeSpot");
		AvatarCommandsOnMoveBodyToSafeSpot(safeSpotIndex);
		avatarRespawnHandler.ShouldRespawnAsGhost = false;
		avatarRespawnHandler.Respawn();
	}

	private void OnEquipItem(PickupItem equippeditem)
	{
		spawnRoleDataReceiver.isInGunMode.Value = pickupOwner.InGunMode;
		spawnRoleDataReceiver.pickupItemIsInHand.Value = pickupOwner.PickupItemIsInHand;
	}

	private Transform GetSpawnTransform()
	{
		MVCheckpoint checkpoint = MVGameControllerBase.Game.LocalPlayer.GetCheckpoint();
		if (checkpoint != null)
		{
			spawnRoleDataReceiver.lastRespawnType.Value = LastRespawnType.Checkpoint;
			return checkpoint.Transform;
		}
		spawnRoleDataReceiver.lastRespawnType.Value = LastRespawnType.Spawnpoint;
		if (SpawnId != -1)
		{
			if (MVGameControllerBase.WOCM.TryGetWorldObject(SpawnId, out var worldObject))
			{
				return ((MVWorldObjectClient)worldObject).Transform;
			}
			Debug.LogWarning("Spawn role creator was destroyed. This might be ok.");
		}
		MVWorldObjectClient validSpawnPoint = MVGameControllerBase.WOCM.GetValidSpawnPoint();
		if (validSpawnPoint == null)
		{
			Debug.LogError("No spawn-point found on planet!");
			return null;
		}
		return validSpawnPoint.Transform;
	}

	private void OnCheckpointReachedResetRevive()
	{
		if (MVClientSettings.ReviveEnabled)
		{
			spawnRoleDataReceiver.reviveState.Value.ResetSafePostions();
		}
	}

	private bool IsInTempTier()
	{
		if (!GamePassesManager.GamePassesActive)
		{
			return false;
		}
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		return (int)previewGamePassTier > (int)gamePassTier;
	}

	private GamePassTier GetTierRequirement()
	{
		if (MVGameControllerBase.WOCM.TryGetWorldObject(SpawnId, out var worldObject) && worldObject is MVAvatarSpawnRoleCreator)
		{
			return ((MVAvatarSpawnRoleCreator)worldObject).Tier;
		}
		return GamePassTier.Tier0;
	}

	private void OnPositionChanged(MVWorldObjectClient wo, PositionChangedEventArgs positionChangedEventArgs)
	{
		spawnRoleDataReceiver.position.Value = positionChangedEventArgs.NewPos;
	}

	private void OnRotationChanged(MVWorldObjectClient wo, RotationChangedEventArgs rotationChangedEventArgs)
	{
		spawnRoleDataReceiver.rotation.Value = rotationChangedEventArgs.NewRotation;
	}

	private void OnScaleChanged(MVWorldObjectClient wo, ScaleChangedEventArgs scaleChangedEventArgs)
	{
		spawnRoleDataReceiver.scale.Value = scaleChangedEventArgs.NewScale;
	}

	protected override void OnCurrentPickupChange(object newPickupDataData)
	{
		base.OnCurrentPickupChange(newPickupDataData);
		HandleBlinkerVisibility();
	}

	private void HandleBlinkerVisibility()
	{
		if (CurrentPickup.IsInFirstPersonMode)
		{
			Body.DisableBodyBlinker();
		}
		else
		{
			Body.EnableBodyBlinker();
		}
	}
}
