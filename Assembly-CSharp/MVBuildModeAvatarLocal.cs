using System;
using System.Collections.Generic;
using Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.Mediator;
using UnityEngine;
using UnityEngine.Events;

public class MVBuildModeAvatarLocal : MVBuildModeAvatar, ILocalObject, ISpawnRoleLocal
{
	public enum AvatarBuildModes : byte
	{
		None,
		Edit
	}

	public abstract class AvatarBuildModeBase
	{
		protected readonly MVBuildModeAvatarLocal buildModeAvatar;

		public abstract AvatarBuildModes AvatarBuildModeRuntimeState { get; }

		protected AvatarBuildModeBase(MVBuildModeAvatarLocal buildModeAvatar)
		{
			this.buildModeAvatar = buildModeAvatar;
		}

		public abstract void Activate(AvatarBuildModes fromMode);

		public abstract void DeActivate(AvatarBuildModes toMode);

		public abstract void FixedUpdate(IInputToPlayerMovement movementMap);

		public abstract void FrameUpdate(InputToInGameAction interactionMap);
	}

	public class EditMode : AvatarBuildModeBase
	{
		public class CERoamUUISetupData
		{
			public readonly Vector3 centerPos;

			public readonly Vector3 lookAtPosition;

			public CERoamUUISetupData(Vector3 centerPos, Vector3 lookAtPosition)
			{
				this.centerPos = centerPos;
				this.lookAtPosition = lookAtPosition;
			}
		}

		public class EditCubesSetupData
		{
			public readonly int focusWoId;

			public EditCubesSetupData(int focusWoId)
			{
				this.focusWoId = focusWoId;
			}
		}

		public class CEEditBodyUUIData
		{
			public readonly int focusWoId;

			public CEEditBodyUUIData(int focusWoId)
			{
				this.focusWoId = focusWoId;
			}
		}

		public class ESEditCubeTutorialData
		{
			public readonly int focusWoId;

			public ESEditCubeTutorialData(int focusWoId)
			{
				this.focusWoId = focusWoId;
			}
		}

		private AvatarBuildModes _avatarBuildModeRuntimeState = AvatarBuildModes.Edit;

		private readonly float maxSpeed = 1.75f;

		private readonly float speedModifier = 5f;

		private Vector3 jetPackTargetDeltaPos;

		private float targetSpeed;

		private float speed;

		private float speedSmoothingTime = 10f;

		private bool moveConstraintSet;

		private Vector3 moveConstraintCenter;

		private float moveConstraintRadius;

		private const float moveSlowDownPoint = 0.75f;

		private DoubleTapMovementChecker doubleTap = new DoubleTapMovementChecker();

		private Camera mainCamera;

		private float keyVelocity;

		private float keyAcceleration = 20f;

		private float keyDamping = 10f;

		private readonly float heightAdjustSpeed = 5f;

		public override AvatarBuildModes AvatarBuildModeRuntimeState => _avatarBuildModeRuntimeState;

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

		public EditMode(MVBuildModeAvatarLocal buildModeAvatar)
			: base(buildModeAvatar)
		{
			mainCamera = Camera.main;
			YMovementSpeedScale = 1f;
			XZMovementSpeedScale = 1f;
		}

		public override void Activate(AvatarBuildModes fromMode)
		{
			buildModeAvatar.Visible = false;
			SetCamera(CameraType.EditorCamera);
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.OnEnterBuildStateEvent += AvatarCommandsBuildModeOnEnterBuildStateEvent;
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.OnExitBuildStateEvent += AvatarCommandsBuildModeOnExitBuildStateEvent;
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.OnSetSpawn += AvatarCommandsBuildModeOnOnSetSpawn;
		}

		private void SetCamera(CameraType cameraType)
		{
			buildModeAvatar.SetCamera(cameraType);
		}

		private void AvatarCommandsBuildModeOnOnSetSpawn(Vector3 position, Quaternion rotation)
		{
			buildModeAvatar.WorldPosition = position;
			buildModeAvatar.WorldRotation = rotation;
		}

		public override void DeActivate(AvatarBuildModes toMode)
		{
			buildModeAvatar.Visible = true;
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.OnEnterBuildStateEvent -= AvatarCommandsBuildModeOnEnterBuildStateEvent;
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.OnExitBuildStateEvent -= AvatarCommandsBuildModeOnExitBuildStateEvent;
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.OnSetSpawn -= AvatarCommandsBuildModeOnOnSetSpawn;
		}

		public void ModifySpeed(float xz, float y)
		{
			XZMovementSpeedScale = xz;
			YMovementSpeedScale = y;
		}

		public override void FrameUpdate(InputToInGameAction interactionMap)
		{
			doubleTap.FrameUpdate();
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
			Vector3 eulerAngles = buildModeAvatar.GameObject.transform.eulerAngles;
			eulerAngles.y = MVGameControllerBase.MainCameraManager.transform.eulerAngles.y;
			buildModeAvatar.WorldRotation = Quaternion.Euler(eulerAngles);
		}

		protected void SetToEditMode()
		{
			MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
		}

		private void MoveCharacter(Vector3 moveDelta)
		{
			if (moveConstraintSet)
			{
				Vector3 position = buildModeAvatar.GameObject.transform.position;
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
			buildModeAvatar.WorldPosition += moveDelta;
		}

		protected Vector3 GetElevationVelocity()
		{
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveUp) && !MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveDown))
			{
				keyVelocity += keyAcceleration * Time.deltaTime;
			}
			else if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveDown))
			{
				keyVelocity -= keyAcceleration * Time.deltaTime;
			}
			else if (MVInputWrapper.GetAxisWithoutSensitivity("Mouse ScrollWheel") != 0f)
			{
				if (MVInputWrapper.GetAxisWithoutSensitivity("Mouse ScrollWheel") > 0f)
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
				buildModeAvatar.GameObject.transform.rotation = Quaternion.LookRotation(forward);
			}
		}

		private Vector3 GetDirection(bool freeFlight)
		{
			Transform transform = mainCamera.transform;
			Vector3 result = transform.rotation * GetInputDirection();
			if (!freeFlight)
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
			bool booleanControl = MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelectAlt);
			Vector3 direction = GetDirection(booleanControl);
			targetSpeed = direction.magnitude * maxSpeed;
			if (MVInputWrapper.GetBooleanControl(KogamaControls.EditMoveFast) || doubleTap.DoubleTap)
			{
				targetSpeed *= speedModifier;
			}
			speed = Mathf.Lerp(speed, targetSpeed, speedSmoothingTime * Time.deltaTime);
			return direction * speed * speedModifier * XZMovementSpeedScale;
		}

		private void AvatarCommandsBuildModeOnEnterBuildStateEvent(EditorEvent editorEvent, object data)
		{
			switch (editorEvent)
			{
			case EditorEvent.CERoamUUI:
				CERoamUUIEnterSetup((CERoamUUISetupData)data);
				break;
			case EditorEvent.CEEditBodyUUI:
				CEEditBodyUUIEnterSetup((CEEditBodyUUIData)data);
				break;
			case EditorEvent.ESEditCubeTutorial:
				ESEditCubeTutorialSetup((ESEditCubeTutorialData)data);
				break;
			case EditorEvent.ESLeaveCubeTutorial:
				ESLeaveCubeTutorialSetup();
				break;
			case EditorEvent.EditCubes:
				EditCubesDataEnterSetup((EditCubesSetupData)data);
				break;
			}
		}

		private void ESLeaveCubeTutorialSetup()
		{
			buildModeAvatar.SetToSpawn();
			FocusOnPosition();
		}

		private void FocusOnPosition()
		{
			((JetPackCamera)MVGameControllerBase.MainCameraManager.CurrentCamera).FocusOnPosition(MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.Position + Vector3.up + Vector3.up + buildModeAvatar.Transform.rotation * Vector3.forward, 0f);
		}

		private void ESEditCubeTutorialSetup(ESEditCubeTutorialData data)
		{
			SetMoveConstraint(new Vector3(0f, 0f, 0f), 25f);
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(data.focusWoId);
			ModifySpeed(Mathf.Min(1f, 1f * worldObjectClient.Scale.x), Mathf.Min(1f, 1f * worldObjectClient.Scale.x));
		}

		private void AvatarCommandsBuildModeOnExitBuildStateEvent(EditorEvent editorEvent, object data)
		{
			switch (editorEvent)
			{
			case EditorEvent.CERoamUUI:
				CERoamUUIExitSetup();
				break;
			case EditorEvent.CEEditBodyUUI:
				CEEditBodyUUIExitSetup();
				break;
			case EditorEvent.ESEditCubeTutorial:
				ESEditCubeTutorialExitSetup();
				break;
			case EditorEvent.EditCubes:
				EditCubesExitSetup();
				break;
			}
		}

		private void EditCubesExitSetup()
		{
			ModifySpeed(1f, 1f);
		}

		private void ESEditCubeTutorialExitSetup()
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.ChangeState(LaserPointerState.Idle);
			MovementConstrained = false;
			ModifySpeed(1f, 1f);
			buildModeAvatar.SetToSpawn();
		}

		private void CERoamUUIExitSetup()
		{
			ModifySpeed(1f, 1f);
		}

		private void CEEditBodyUUIExitSetup()
		{
			MVGameControllerBase.MainCameraManager.BlueModeEnabled = false;
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.ChangeState(LaserPointerState.Idle);
			ModifySpeed(1f, 1f);
		}

		private void CEEditBodyUUIEnterSetup(CEEditBodyUUIData data)
		{
			SetCamera(CameraType.AvatarEditModeCamera);
			MVGameControllerBase.MainCameraManager.CurrentCamera.FocusOnObject(MVGameControllerBase.WOCM.GetWorldObjectClient(data.focusWoId));
			SharedCubeFunctions.SetLayerRecursively(buildModeAvatar.Transform, select: false);
			MVGameControllerBase.MainCameraManager.BlueModeEnabled = true;
			ModifySpeed(0.25f, 0.25f);
		}

		private void CERoamUUIEnterSetup(CERoamUUISetupData data)
		{
			MVGameControllerBase.MainCameraManager.BlueModeEnabled = false;
			SetMoveConstraint(data.centerPos, 10f);
			ModifySpeed(0.8f, 0.25f);
			SetCamera(CameraType.AvatarEditModeCamera);
			((AvatarEditModeCamera)MVGameControllerBase.MainCameraManager.CurrentCamera).ResetDistanceAndDirectionToAvatar(data.lookAtPosition);
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.ChangeState(LaserPointerState.Idle);
		}

		private void EditCubesDataEnterSetup(EditCubesSetupData data)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(data.focusWoId);
			ModifySpeed(Mathf.Min(1f, 2f * worldObjectClient.Scale.x), Mathf.Min(1f, 2f * worldObjectClient.Scale.x));
		}
	}

	public class BuildModeAvatarLocalModes
	{
		private AvatarBuildModeBase currentMode;

		private MVBuildModeAvatarLocal avatar;

		public BuildModeAvatarLocalModes(MVBuildModeAvatarLocal avatar)
		{
			this.avatar = avatar;
		}

		public void FrameUpdate(InputToInGameAction interactionMap)
		{
			currentMode.FrameUpdate(interactionMap);
		}

		public void FixedUpdate(IInputToPlayerMovement movementMap)
		{
			currentMode.FixedUpdate(movementMap);
		}

		public void SetMode(AvatarBuildModes mode)
		{
			AvatarBuildModes avatarBuildModes = AvatarBuildModes.None;
			if (currentMode != null)
			{
				currentMode.DeActivate(mode);
				avatarBuildModes = currentMode.AvatarBuildModeRuntimeState;
			}
			Debug.Log(".............prevAvatarBuildModes: " + avatarBuildModes);
			Debug.Log(".............nextAvatarBuildModes: " + mode);
			currentMode = BuildModeFactory(mode);
			currentMode.Activate(avatarBuildModes);
		}

		private AvatarBuildModeBase BuildModeFactory(AvatarBuildModes avatarBuildModeRuntimeState)
		{
			if (avatarBuildModeRuntimeState == AvatarBuildModes.Edit)
			{
				return new EditMode(avatar);
			}
			throw new Exception("Not implemented");
		}
	}

	protected SpawnRoleDataReceiver spawnRoleDataReceiver;

	private AvatarLocalBuildMode avatarScriptObject;

	private BuildModeAvatarLocalModes buildModeAvatarLocalModes;

	private LaserPointer laserPoint;

	public MVBuildModeAvatarLocal(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVAvatarLocalBuildModePrefab, worldObjects)
	{
		avatarScriptObject = gameObject.GetComponent<AvatarLocalBuildMode>();
		SetNetworkObject(local: true);
	}

	public override void Initialize()
	{
		avatarScriptObject.Initialize(this);
		buildModeAvatarLocalModes = new BuildModeAvatarLocalModes(this);
		buildModeAvatarLocalModes.SetMode(AvatarBuildModes.Edit);
		body.GameObject.SetActive(value: false);
		laserPoint = InitLaser(isLocal: true);
		laserPoint.SubscribeToCommands();
		PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
		ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Combine(ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(OnScaleChanged));
		base.Initialize();
		limbManager = new AvatarLimbManagerLocal();
		limbManager.Initialize(this, body, avatarScriptObject.EnabledChangeHandler, limbRotationRuntimeData);
		MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(OwnerActorNr).NotifyAvatarCreated(Id);
	}

	public void Activate(int idFrom, SpawnRoleDataReceiver spawnRoleDataReceiver, Vector3 position, Quaternion rotation)
	{
		this.spawnRoleDataReceiver = spawnRoleDataReceiver;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnKillSelf += SetToSpawn;
		laserPoint.gameObject.SetActive(value: true);
		MVGameControllerBase.Game.PlayerController.SetAvatarLocalObject(this);
		avatarScriptObject.Activate();
		avatarScriptObject.AvatarCamerasDesktopBuildMode.SetCamera(CameraType.EditorCamera);
		MVGameControllerBase.MainCameraManager.FieldOfView = MVGameControllerBase.MainCameraManager.CurrentCamera.FieldOfView;
		Position = position;
		Rotation = rotation;
		if (idFrom > 0)
		{
			spawnRoleDataReceiver.position.Value = Position;
		}
		spawnRoleDataReceiver.woId.Value = Id;
		CullingApiWrapper.SetDistanceReferencePoint(transform);
		ChatCommandManager.UpdateChatCommandCallback(ChatCommand.HideAllUI, (Action)Delegate.Combine(ChatCommandManager.GetChatCommandCallback(ChatCommand.HideAllUI), new Action(HideEditCube)));
	}

	public void DeActivate(int idTo, SpawnRoleDataReceiver spawnRoleDataReceiver)
	{
		this.spawnRoleDataReceiver = null;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.OnKillSelf -= SetToSpawn;
		laserPoint.gameObject.SetActive(value: false);
		ChatCommandManager.UpdateChatCommandCallback(ChatCommand.HideAllUI, (Action)Delegate.Remove(ChatCommandManager.GetChatCommandCallback(ChatCommand.HideAllUI), new Action(HideEditCube)));
	}

	public InputToInGameAction Update(InputToInGameAction movementMap)
	{
		buildModeAvatarLocalModes.FrameUpdate(movementMap);
		return movementMap;
	}

	public IInputToPlayerMovement FixedUpdate(IInputToPlayerMovement movementMap)
	{
		buildModeAvatarLocalModes.FixedUpdate(movementMap);
		return movementMap;
	}

	private void SetCamera(CameraType cameraType)
	{
		avatarScriptObject.AvatarCamerasDesktopBuildMode.SetCamera(cameraType);
	}

	private void SetToSpawn()
	{
		MVWorldObjectClient validSpawnPoint = MVGameControllerBase.WOCM.GetValidSpawnPoint();
		WorldPosition = validSpawnPoint.WorldPosition;
		WorldRotation = validSpawnPoint.WorldRotation;
		MVGameControllerBase.MainCameraManager.CurrentCamera.Reset();
	}

	private void HideEditCube()
	{
		laserPoint.gameObject.SetActive(value: false);
	}

	private void OnPositionChanged(MVWorldObjectClient wo, PositionChangedEventArgs positionChangedEventArgs)
	{
		spawnRoleDataReceiver.position.Value = positionChangedEventArgs.NewPos;
	}

	private void OnScaleChanged(MVWorldObjectClient wo, ScaleChangedEventArgs scaleChangedEventArgs)
	{
		spawnRoleDataReceiver.scale.Value = scaleChangedEventArgs.NewScale;
	}

	protected override Vector3 GetLookDirection()
	{
		return MVGameControllerBase.MainCameraManager.FireDirection.normalized;
	}
}
