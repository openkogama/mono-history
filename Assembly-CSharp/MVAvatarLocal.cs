using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVAvatarLocal : MVAvatar, ILocalObject
{
	private enum AvatarState
	{
		Editing,
		Playing,
		Dead,
		Hidden
	}

	private const float maxFallBelow = 200f;

	private AvatarMotor avatarMotor;

	private MVInteractableBase interactableLocal;

	private UseInteractorHandler useInteractorHandler;

	private MVTriggerHandler triggerHandler;

	private float deadTime;

	private float deadInterval = 2.5f;

	private float respawnTime;

	private float respawnTimeOut = 2f;

	private AvatarModes modes;

	private AvatarMode mode;

	private AvatarState state;

	private PlaymodeCamera playmodeCam;

	private bool showingRespawnDelay;

	private AvatarPickupOwner pickupOwner;

	private PickupGUI pickupGUI;

	private MVRigidBody vehicleRigidBody;

	private AvatarFader avatarFader;

	private string currAnim = string.Empty;

	private bool CanReceivePackages => Time.time - respawnTime > respawnTimeOut && (byte)AvatarRuntimeDataState.Value == 1;

	public float SetTransparency
	{
		set
		{
			if (avatarFader != null)
			{
				avatarFader.SetTransparency(value);
			}
		}
	}

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

	private bool InGunMode
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

	public bool IsDead => state == AvatarState.Dead;

	public bool IsEnteringVehicle => MVGameController.Game.PlayerController.IsEnteringVehicle;

	public bool IsInVehicle => vehicleRigidBody != null;

	public AvatarModes AvatarModes => modes;

	public AvatarMode AvatarMode
	{
		get
		{
			return Mode;
		}
		set
		{
			if (value is WalkMode)
			{
				Mode = value;
				ResetAvatar(toHiddenState: false);
				gameObject.GetComponent<Collider>().enabled = true;
			}
			else if (value is JetPackMode)
			{
				ResetAvatar(toHiddenState: false);
				Mode = value;
				gameObject.GetComponent<Collider>().enabled = false;
			}
		}
	}

	private AvatarMode Mode
	{
		get
		{
			return mode;
		}
		set
		{
			mode = value;
			if (mode == null)
			{
				return;
			}
			RigidBody.Reset();
			mode.Activate();
			if (value == modes.JetPackMode)
			{
				state = AvatarState.Editing;
				MVEquipable component = gameObject.GetComponent<MVEquipable>();
				if (component != null)
				{
					component.Unequip();
				}
				MVTriggerHandler component2 = gameObject.GetComponent<MVTriggerHandler>();
				if (component2 == null)
				{
					Debug.LogError("TriggerHandler on avatar is null");
				}
				else
				{
					component2.enabled = false;
				}
				MVGameController.Game.LocalPlayer.ResetCheckpoint();
			}
			else if (value == modes.WalkMode)
			{
				MVGameController.WOCM.UpdateWorldBounds(SharedCubeFunctions.GetAxisAlignedBoundsRecursively(MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().Transform).Value);
				MVTriggerHandler component3 = gameObject.GetComponent<MVTriggerHandler>();
				if (component3 == null)
				{
					Debug.LogError("TriggerHandler on avatar is null");
				}
				else
				{
					component3.enabled = true;
				}
				state = AvatarState.Playing;
			}
		}
	}

	public event EventHandler Respawned;

	public MVAvatarLocal(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		SetNetworkObject(local: true);
	}

	public Vector3 GetAbsoluteVelocity()
	{
		if (vehicleRigidBody != null)
		{
			return vehicleRigidBody.Velocity;
		}
		return RigidBody.Velocity;
	}

	public override void Initialize()
	{
		base.Initialize();
		avatarFader = new AvatarFader(Body.Transform);
		avatarMotor = gameObject.AddComponent<AvatarMotor>();
		triggerHandler = gameObject.AddComponent<MVTriggerHandler>();
		AvatarInteractable avatarInteractable = gameObject.AddComponent<AvatarInteractable>();
		avatarInteractable.Init(Modifiers, Invulnerable, Health);
		interactableLocal = avatarInteractable;
		AvatarEquipable avatarEquipable = gameObject.AddComponent<AvatarEquipable>();
		avatarEquipable.Init(interactableLocal, CurrentItem);
		avatarMotor.Init(avatarInteractable, CharacterControllerCenterOffset);
		Initialize(avatarPickupOwner, avatarMotor);
		MVGameController.WOCM.AvatarLocal = this;
		if (MVGameController.GameMode != MVGameMode.CharacterEditor)
		{
			useInteractorHandler = gameObject.AddComponent<UseInteractorHandler>();
			useInteractorHandler.Init(gameObject.GetComponent<Collider>());
		}
		MVGameController.Game.PlayerController.Push(this);
		InitializeCamera();
	}

	protected override void AvatarStateChangedHandler(object a)
	{
		base.AvatarStateChangedHandler(a);
		if ((byte)a == 0)
		{
			LayerUtil.SetLayerRecursively(Body.Transform, "Player", "CamRotateTarget");
			MVGameController.Game.CameraController.GetComponent<GrayscaleEffect>().enabled = true;
			MVGameController.Game.CameraController.SecondaryCameraActive = true;
			Body.Transform.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
		}
		else
		{
			Body.Transform.localRotation = Quaternion.AngleAxis(0f, Vector3.up);
			MVGameController.Game.CameraController.GetComponent<GrayscaleEffect>().enabled = false;
			MVGameController.Game.CameraController.SecondaryCameraActive = false;
			LayerUtil.SetLayerRecursively(Body.Transform, "CamRotateTarget", "Player");
		}
	}

	public void SetCharacterController(SmoothCharacterController characterController)
	{
		avatarMotor.OverrideCharacterController(characterController);
	}

	public void LeaveVehicle()
	{
		vehicleRigidBody = null;
		int vehicleID = -1;
		if (!MVGameController.Game.PlayerController.DetachWorldObjectFromVehicle(Id, ref vehicleID))
		{
			return;
		}
		if (vehicleID != -1)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(vehicleID);
			if (worldObjectClient != null && worldObjectClient is MVVehicleBase)
			{
				((MVVehicleBase)worldObjectClient).LeaveLocal();
			}
			else
			{
				Debug.LogError("vehicleWO is null or type is not IVehicle " + vehicleID);
			}
		}
		HandleLeaveVehicle();
		MVGameController.Game.CameraController.SetPlayModeCam();
		RigidBody.Reset();
		RigidBody.enabled = true;
		triggerHandler.enabled = true;
		if (networkObject == null || networkObject is MVNetworkListener)
		{
			Debug.LogError("NetworkObject null or listener on detach. Should be reporter");
		}
		else
		{
			((MVNetworkReporter)networkObject).suspendTransformReporting = false;
		}
	}

	public override void BeforeVehicleEntered()
	{
		triggerHandler.Reset();
	}

	public override void VehicleEntered()
	{
		if (Group.GameObject.GetComponent<MVRigidBody>() != null)
		{
			RigidBody.enabled = false;
			triggerHandler.enabled = false;
		}
		vehicleRigidBody = MVWorldObjectClientManager.GetEnabledMonoBehaviourHighestInHierarchy<MVRigidBody>(gameObject);
	}

	public InteractionInput Update(InteractionInput interactionMap)
	{
		UpdateInvulnerable();
		if (state == AvatarState.Playing)
		{
			pickupOwner.HandleFire(interactionMap.Fire, IsFiring);
		}
		if (RigidBody.enabled)
		{
			Mode.FrameUpdate();
		}
		if (InGunMode)
		{
			if (GameDB.IsClassicGame)
			{
				pickupGUI.Update();
			}
			if ((bool)playmodeCam)
			{
				playmodeCam.lookAtOffset = playmodeCam.shoulderOffset;
			}
		}
		else if ((bool)playmodeCam)
		{
			playmodeCam.lookAtOffset = new Vector3(0f, 0.7f, 0f);
		}
		if (!MVGameController.Game.IsPlaying)
		{
			return interactionMap;
		}
		if (gameObject.transform.position.y < MVGameController.WOCM.WorldBounds.min.y - 200f && !IsDead)
		{
			DieByFalling();
			Debug.Log("Death by falling");
		}
		if (interactionMap.Use && !RigidBody.IsMovementLocked)
		{
			if (IsSeated)
			{
				LeaveVehicle();
			}
			else if (interactableLocal.HandleModifierEffect(AvatarModifierEffect.DisableVehicles, 0f) == 0f)
			{
				useInteractorHandler.Use();
			}
		}
		if (interactionMap.Drop)
		{
			gameObject.GetComponent<MVEquipable>().Equip(AvatarItemType.Hand, null);
		}
		if (!IsSeated && avatarMotor.IsStuck())
		{
			HandleStuck();
		}
		return interactionMap;
	}

	public MovementMap FixedUpdate(MovementMap movementMap)
	{
		modes.WalkMode.InGunMode = InGunMode;
		modes.WalkMode.ForceRotateAvatarToFiringDirection = ForceRotateAvatarToFiringDirection;
		switch (state)
		{
		case AvatarState.Editing:
			FixedUpdateCurrentMode(movementMap);
			break;
		case AvatarState.Playing:
			FixedUpdateCurrentMode(movementMap);
			if ((InGunMode || ForceRotateAvatarToFiringDirection) && RigidBody.enabled && RigidBody.Velocity.sqrMagnitude > 0.001f)
			{
				RotateAvatarToFiringDirection();
			}
			if (!avatarMotor.enabled)
			{
				avatarMotor.UpdateVelocity();
			}
			break;
		case AvatarState.Dead:
			FixedUpdateCurrentMode(movementMap);
			if (Time.time - deadTime > deadInterval)
			{
				Respawn(toHiddenState: true);
			}
			break;
		case AvatarState.Hidden:
			if (LockCursorManager.LockCursor)
			{
				state = AvatarState.Playing;
				ToPlay();
			}
			break;
		}
		return movementMap;
	}

	public void SetAnimation(string animationState)
	{
		if (!(currAnim == animationState))
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)Animation.Value;
			if (currAnim != animationState)
			{
				int serverTimeInMilliSeconds = MVGameController.Game.ServerTimeInMilliSeconds;
				Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
				dictionary2.Add("state", animationState);
				dictionary2.Add("timeStamp", serverTimeInMilliSeconds);
				dictionary = dictionary2;
				Animation.Value = dictionary;
				currAnim = animationState;
			}
		}
	}

	public void Respawn(bool toHiddenState)
	{
		Debug.Log("Respawn " + toHiddenState);
		MVTriggerHandler component = gameObject.GetComponent<MVTriggerHandler>();
		if (component == null)
		{
			Debug.LogError("TriggerHandler on avatar is null");
		}
		else
		{
			component.enabled = true;
		}
		if (showingRespawnDelay)
		{
			return;
		}
		if (!MVGameController.Game.TeamManager.IsTeamActive(MVGameController.Game.TeamManager.GetTeamFromActorNr(OwnerActorNr)))
		{
			List<MVTeam> teamList = MVGameController.Game.TeamManager.GetTeamList();
			MVGameController.Game.SetTeam(teamList[0]);
		}
		ResetAvatar(toHiddenState);
		MVCheckpoint checkpoint = MVGameController.Game.LocalPlayer.GetCheckpoint();
		if (checkpoint != null)
		{
			SetSpawnTransform(checkpoint.WorldPosition, checkpoint.WorldRotation);
		}
		else
		{
			MVLogicObject validSpawnPoint = MVGameController.WOCM.GetValidSpawnPoint();
			if (validSpawnPoint == null)
			{
				Debug.LogError("No spawn-point found on planet!");
			}
			else
			{
				SetSpawnTransform(validSpawnPoint.WorldPosition, validSpawnPoint.WorldRotation);
			}
		}
		if (Respawned != null)
		{
			Respawned(this, EventArgs.Empty);
		}
	}

	public void Suicide()
	{
		if (state != AvatarState.Dead)
		{
			Die();
		}
	}

	private void SetSpawnTransform(Vector3 position, Quaternion rotation)
	{
		GameObject.transform.position = position;
		GameObject.transform.rotation = rotation;
		MVGameController.Game.CameraController.Respawn();
		avatarMotor.Reset();
	}

	protected override void AttachBody(MVBody newBody)
	{
		base.AttachBody(newBody);
		if (!MVGameController.Game.IsPlaying)
		{
			newBody.Visible = false;
		}
		else
		{
			newBody.Visible = true;
		}
		MVGameController.Game.TransferOwnership(newBody.Id, 0, null);
		foreach (MVWorldObjectClient child in newBody.Children)
		{
			MVGameController.Game.TransferOwnership(child.Id, 0, null);
		}
	}

	private void DieByFalling()
	{
		Health.Value = 0f;
		MVGameController.Game.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameController.Game.LocalPlayerActorNumber, MVGameController.Game.LocalPlayerActorNumber, PlayerKilledByType.FallOffWorld));
	}

	private void RotateAvatarToFiringDirection()
	{
		Vector3 lookDirection = avatarPickupOwner.LookDirection;
		lookDirection.y = 0f;
		avatarMotor.OverrideDirection(lookDirection);
	}

	private void InitializeCamera()
	{
		if (MVGameController.GameMode == MVGameMode.Play)
		{
			MVGameController.Game.CameraController.SetPlayModeCam();
		}
		else
		{
			MVGameController.Game.CameraController.SetCamera(CameraType.JetPackCamera);
		}
	}

	private void FixedUpdateCurrentMode(MovementMap movementMap)
	{
		if (RigidBody.enabled)
		{
			Mode.FixedUpdate(movementMap);
		}
	}

	private void Initialize(AvatarPickupOwner pickupOwner, AvatarMotor avatarMotor)
	{
		switch (GameDB.GameType)
		{
		case MVGameType.Classic:
			playmodeCam = MVGameController.Game.CameraController.GetCamera<ThirdPersonCamera>();
			break;
		case MVGameType.Platformer:
			playmodeCam = MVGameController.Game.CameraController.GetCamera<PlatformerCamera>();
			break;
		}
		this.pickupOwner = pickupOwner;
		pickupOwner.onHandleFiring = (MVPickupOwner.OnHandleFiringDelegate)Delegate.Combine(pickupOwner.onHandleFiring, new MVPickupOwner.OnHandleFiringDelegate(OnHandleFiring));
		if (GameDB.IsClassicGame)
		{
			pickupGUI = new PickupGUI(pickupOwner);
		}
		modes = new AvatarModes();
		modes.JetPackMode = new JetPackMode(this);
		modes.WalkMode = new WalkMode(this, avatarMotor);
		if (MVGameController.GameMode == MVGameMode.Edit)
		{
			Mode = modes.JetPackMode;
			state = AvatarState.Editing;
		}
		else
		{
			Mode = modes.WalkMode;
			state = AvatarState.Hidden;
		}
		MVRuntimeDataVariableClampedFloat health = Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			if ((float)obj == 0f)
			{
				Die();
			}
		}));
		HealthBar componentInChildren = gameObject.GetComponentInChildren<HealthBar>();
		UnityEngine.Object.Destroy(componentInChildren.gameObject);
	}

	private void HandleStuck()
	{
		if (state == AvatarState.Playing)
		{
			Die();
			MVGameController.Game.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameController.Game.LocalPlayerActorNumber, MVGameController.Game.LocalPlayerActorNumber, PlayerKilledByType.Crushed));
		}
	}

	private void Die()
	{
		if (state == AvatarState.Playing)
		{
			state = AvatarState.Dead;
			deadTime = Time.time;
			SetAnimation("Dead");
			MVEquipable component = gameObject.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Unequip();
			}
			if (IsSeated)
			{
				LeaveVehicle();
			}
			MVTriggerHandler component2 = gameObject.GetComponent<MVTriggerHandler>();
			if (component2 == null)
			{
				Debug.LogError("TriggerHandler on avatar is null");
			}
			else
			{
				component2.enabled = false;
			}
		}
	}

	private void ResetAvatar(bool toHiddenState)
	{
		showingRespawnDelay = false;
		SetAnimation("Idle");
		RigidBody.Reset();
		Health.Value = 100f;
		if (IsSeated)
		{
			Debug.Log("Left vehicle");
			LeaveVehicle();
		}
		MVEquipable component = GameObject.GetComponent<MVEquipable>();
		if (component != null)
		{
			component.Unequip();
		}
		if (interactableLocal != null)
		{
			interactableLocal.ClearModifiers();
		}
		AvatarState avatarState = AvatarState.Playing;
		if (toHiddenState || state == AvatarState.Hidden)
		{
			avatarState = AvatarState.Hidden;
		}
		if (MVGameController.EditController != null && !MVGameController.Game.IsPlaying)
		{
			avatarState = AvatarState.Editing;
		}
		state = avatarState;
		if (state == AvatarState.Hidden)
		{
			Debug.Log("ToHidden");
			LockCursorManager.LockCursor = false;
		}
		switch (state)
		{
		case AvatarState.Hidden:
			ToHidden();
			break;
		case AvatarState.Playing:
			ToPlay();
			break;
		case AvatarState.Editing:
			ToEdit();
			break;
		case AvatarState.Dead:
			break;
		}
	}

	private void ToEdit()
	{
		AvatarRuntimeDataState.Value = (byte)2;
	}

	private void ToPlay()
	{
		respawnTime = Time.time;
		AvatarRuntimeDataState.Value = (byte)1;
	}

	private void ToHidden()
	{
		AvatarRuntimeDataState.Value = (byte)0;
	}

	private void OnHandleFiring(bool isFiring)
	{
		if (InGunMode && isFiring && RigidBody.enabled && RigidBody.Velocity.sqrMagnitude < 0.01f)
		{
			RotateAvatarToFiringDirection();
		}
	}

	private void UpdateInvulnerable()
	{
		bool flag = !CanReceivePackages;
		if ((bool)Invulnerable.Value != flag)
		{
			Invulnerable.Value = flag;
		}
	}
}
